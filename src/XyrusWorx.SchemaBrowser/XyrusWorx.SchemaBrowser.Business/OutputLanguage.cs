using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx.Collections;

namespace XyrusWorx.SchemaBrowser.Business
{
	class OutputLanguage : IStringResolver
	{
		private const string mNamespace = @"http://schemas.xyrus-worx.org/2020/localization";
		
		private readonly XDocument mXml;
		private readonly Dictionary<StringKey, string> mStrings;
		
		private static OutputLanguage mInvariant;

		[NotNull]
		public static OutputLanguage GetInvariant() => mInvariant ?? (mInvariant = new OutputLanguage());

		private OutputLanguage() : this(CultureInfo.GetCultureInfoByIetfLanguageTag("en-US")) {}
		public OutputLanguage([NotNull] CultureInfo cultureInfo)
		{
			if (cultureInfo == null)
			{
				throw new ArgumentNullException(nameof(cultureInfo));
			}
			
			var re = new Regex(@"\.([a-z]{2}(?:\-[A-Z]{2})?)\.xml$");
			var stream = (Stream)null;
			
			foreach (var item in typeof(LocalizationService).Assembly.GetManifestResourceNames())
			{
				var m = re.Match(item);
				if (m.Success && m.Groups[1].Value == cultureInfo.IetfLanguageTag)
				{
					stream = typeof(LocalizationService).Assembly.GetManifestResourceStream(item);
				}
			}

			if (stream != null)
			{
				using (stream)
				{
					mXml = XDocument.Load(stream);
				}				
			}
			else
			{
				Debug.Assert(false, "Failed to load language file: " + cultureInfo.IetfLanguageTag);
			}
			
			mStrings = new Dictionary<StringKey, string>();
			FormatProvider = cultureInfo;
		}

		public IFormatProvider FormatProvider { get; }

		public string Resolve(StringKey key)
		{
			if (key.IsEmpty)
			{
				return null;
			}
			
			if (mStrings.ContainsKey(key))
			{
				return mStrings[key];
			}

			var matching =
				from element in mXml.Root?.Elements(XName.Get("String", mNamespace)) ?? new XElement[0]
				where string.Equals(element.Attribute("id")?.Value?.Trim(), key.RawData?.Trim(), StringComparison.Ordinal)
				select element;

			var str = matching.FirstOrDefault()?.Value.NormalizeNull();
			mStrings.AddOrUpdate(key, str);
			return str;
		}
	}
}