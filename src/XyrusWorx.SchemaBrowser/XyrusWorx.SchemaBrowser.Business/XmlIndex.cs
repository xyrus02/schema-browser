using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx.Collections;
using XyrusWorx.Diagnostics;

namespace XyrusWorx.SchemaBrowser.Business
{
	[PublicAPI]
	public class XmlIndex
	{
		internal const string XsdNamespace = @"http://www.w3.org/2001/XMLSchema";
		
		private readonly Dictionary<StringKey, Dictionary<XName, XmlIndexEntry>> mElements;
		private readonly Dictionary<StringKey, HashSet<Action<XmlIndexEntry>>> mCallbacks;
		private readonly HashSet<XName> mRootTypeCandidates;
		private readonly HashSet<string> mProcessedPaths;
		private readonly ILogWriter mLog;

		public XmlIndex(ILogWriter log = null)
		{
			mLog = log;
			mElements = new Dictionary<StringKey, Dictionary<XName, XmlIndexEntry>>();
			mCallbacks = new Dictionary<StringKey, HashSet<Action<XmlIndexEntry>>>();
			mRootTypeCandidates = new HashSet<XName>();
			mProcessedPaths = new HashSet<string>();
		}

		[CanBeNull]
		public XElement RootElement { get; private set; }

		[CanBeNull]
		public XmlIndexEntry GlobalLookup(XName qualifiedName) => mElements.FirstOrDefault(x => x.Value.ContainsKey(qualifiedName)).Value?[qualifiedName];
		
		[CanBeNull]
		public XmlIndexEntry Lookup(StringKey schemaElement, XName qualifiedName) => mElements.GetValueByKeyOrDefault(new StringKey(schemaElement).Normalize())?.GetValueByKeyOrDefault(qualifiedName);

		[CanBeNull]
		public IEnumerable<XmlIndexEntry> GetEntries(StringKey schemaElement) => mElements.GetValueByKeyOrDefault(new StringKey(schemaElement).Normalize())?.Values;

		[CanBeNull]
		public XName GetQualifiedName(string key, XElement context)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return null;
			}

			string name, namespaceName;
			
			if (key.Contains(":"))
			{
				var tokens = key.Split(new[] {':'}, 2, StringSplitOptions.None);
				if (string.IsNullOrWhiteSpace(tokens.ElementAtOrDefault(1)))
				{
					name = key;
					namespaceName = "";
				}
				else if (string.IsNullOrWhiteSpace(tokens.ElementAtOrDefault(0)))
				{
					name = tokens.ElementAt(1);
					namespaceName = "";
				}
				else
				{
					name = tokens.ElementAt(1);
					namespaceName = context.GetNamespaceOfPrefix(tokens.ElementAt(0))?.NamespaceName;
				}
			}
			else
			{
				name = key;
				namespaceName = context.Document?.Root?.Attribute("targetNamespace")?.Value;
			}

			return XName.Get(name, namespaceName ?? "");

		}

		public bool IsRootTypeCandidate(XName qualifiedName) => mRootTypeCandidates.Contains(qualifiedName);

		public Action ResetCallback { get; set; }
		
		public void AddCallback(StringKey schemaElement, Action<XmlIndexEntry> callback)
		{
			if (schemaElement.IsEmpty || callback == null)
			{
				return;
			}
			
			var listName = schemaElement.Normalize();
			var listInstance = mCallbacks.GetValueByKeyOrDefault(listName);
			var listEntry = callback;

			if (listInstance == null)
			{
				mCallbacks.Add(listName, listInstance = new HashSet<Action<XmlIndexEntry>>());
			}

			listInstance.Add(listEntry);
		}
		public void RemoveCallback(StringKey schemaElement, Action<XmlIndexEntry> callback)
		{
			if (schemaElement.IsEmpty || callback == null)
			{
				return;
			}
			
			mCallbacks.GetValueByKeyOrDefault(schemaElement.Normalize())?.Remove(callback);
		}

		public void ClearContext()
		{
			ResetCallback?.Invoke();
			
			mRootTypeCandidates.Clear();
			mElements.Clear();
		}
		
		[NotNull]
		public IResult Process(string path)
		{
			try
			{
				RootElement = XDocument.Load(path).Root;
				
				if (RootElement == null)
				{
					return Result.CreateError("The XML schema failed to parse. The root element could not be determined.");
				}

				if (RootElement.Name.NamespaceName != XsdNamespace || RootElement.Name.LocalName != "schema")
				{
					return Result.CreateError("The file provided is not a valid XML schema.");
				}
			}
			catch (Exception exception)
			{
				return Result.CreateError($"The XML schema failed to parse. {exception.GetBaseException().Message}");
			}

			using (var streamReader = new StreamReader(path))
			{
				return ProcessCore(streamReader, path, 0);
			}
		}

		private static (string baseDir, string path) SplitPath(string pathOrUri)
		{
			if (string.IsNullOrWhiteSpace(pathOrUri))
			{
				return ("", "");
			}

			var isUri = pathOrUri.Contains("://");
			if (isUri)
			{
				var match = Regex.Match(pathOrUri, "(.*)/(.*)");
				return !match.Success ? ("", pathOrUri) : (match.Groups[1].Value, match.Groups[2].Value);
			}

			var isRooted = Path.IsPathRooted(pathOrUri);
			if (!isRooted)
			{
				pathOrUri = Path.GetFullPath(pathOrUri);
			}

			return (Path.GetDirectoryName(pathOrUri), Path.GetFileName(pathOrUri));
		}
		private static string JoinPath(string pathOrUri, string appendix)
		{
			if (string.IsNullOrWhiteSpace(pathOrUri))
			{
				return appendix;
			}

			return pathOrUri.Contains("://") ? $"{pathOrUri.TrimEnd('/')}/{appendix.TrimStart('/')}" : Path.Combine(pathOrUri, appendix);
		}

		private IResult ProcessCore(TextReader reader, string pathOrUri, int level)
		{
			try
			{
				var (baseDir, _) = SplitPath(pathOrUri);
				
				mLog?.WriteVerbose($"Processing schema file: \"{pathOrUri}\"...");

				var document = XDocument.Load(reader);
				var root = document.Root;
				
				if (root == null)
				{
					return Result.CreateError("The XML schema failed to parse. The root element could not be determined.");
				}

				foreach (var element in root.Elements())
				{
					if (element.Name.NamespaceName != XsdNamespace)
					{
						continue;
					}
					
					if (new[]{"import", "include"}.Contains(element.Name.LocalName))
					{
						var referencePath = element.Attribute("schemaLocation")?.Value;
						if (referencePath == null || string.IsNullOrWhiteSpace(referencePath))
						{
							continue;
						}

						if (!referencePath.Contains("://") && !Path.IsPathRooted(referencePath))
						{
							referencePath = JoinPath(baseDir, referencePath);
						}
						
						if (referencePath.Contains("://"))
						{
							byte[] schemaData;

							using (var wc = new System.Net.WebClient())
							{
								schemaData = wc.DownloadData(referencePath);
							}

							using (var streamReader = new StreamReader(new MemoryStream(schemaData)))
							{
								var urlReferenceResult = ProcessReference(streamReader, referencePath, level);
								if (urlReferenceResult.HasError)
								{
									return urlReferenceResult;
								}
							}

							continue;
						}

						var referenceResult = ProcessReference(referencePath, pathOrUri, level);
						if (referenceResult.HasError)
						{
							return referenceResult;
						}

						continue;
					}

					if (element.Name.LocalName == "element")
					{
						var typeRef = element.Attribute("type")?.Value;
						if (typeRef == null || string.IsNullOrWhiteSpace(typeRef))
						{
							var extensionElements = element.Descendants(XName.Get("extension", XsdNamespace));
							var childElements = element.Descendants(XName.Get("element", XsdNamespace));

							foreach (var childElement in extensionElements)
							{
								var elementRefTarget = GetQualifiedName(childElement.Attribute("base")?.Value, element);
								if (elementRefTarget == null)
								{
									continue;
								}

								mRootTypeCandidates.Add(elementRefTarget);
							}
							
							foreach (var childElement in childElements)
							{
								var elementRefTarget = GetQualifiedName(childElement.Attribute("type")?.Value, element);
								if (elementRefTarget == null)
								{
									continue;
								}

								mRootTypeCandidates.Add(elementRefTarget);
							}
						}
						else
						{
							var typeRefTarget = GetQualifiedName(typeRef, element);
							if (typeRefTarget == null)
							{
								continue;
							}

							mRootTypeCandidates.Add(typeRefTarget);
						}
					}
					
					InsertElement(element);
				}

				return Result.Success;
			}
			catch (Exception exception)
			{
				return Result.CreateError($"The XML schema failed to parse. {exception.GetBaseException().Message}");
			}
		}
		private IResult ProcessReference(TextReader reader, string uri, int level)
		{
			return !mProcessedPaths.Add(uri.ToLowerInvariant()) ? Result.Success : ProcessCore(reader, uri, level + 1);
		}
		private IResult ProcessReference(string path, string referencingFile, int level)
		{
			try
			{
				path = Path.GetFullPath(path);
			}
			catch (NotSupportedException)
			{
				Debug.Assert(false, path);
			}

			if (!mProcessedPaths.Add(path.ToLowerInvariant()))
			{
				return Result.Success;
			}

			if (!File.Exists(path))
			{
				return Result.CreateError($"The  file \"{referencingFile}\" contains a reference which can't be resolved. The file \"{path}\" doesn't exist or is inaccessible.");
			}

			using (var streamReader = new StreamReader(path))
			{
				return ProcessCore(streamReader, path, level + 1);
			}
		}
		
		private void InsertElement(XElement element)
		{
			var key = element.Attribute("name")?.Value;
			var name = GetQualifiedName(key, element);

			if (name == null)
			{
				return;
			}

			var listName = new StringKey(element.Name.LocalName).Normalize();
			var listInstance = mElements.GetValueByKeyOrDefault(listName);
			var listEntry = new XmlIndexEntry(name, element);

			if (listInstance == null)
			{
				mElements.Add(listName, listInstance = new Dictionary<XName, XmlIndexEntry>());
			}

			mCallbacks.GetValueByKeyOrDefault(listName)?.Foreach(x => x.Invoke(listEntry));
			listInstance.AddOrUpdate(name, listEntry);
		}
	}
}