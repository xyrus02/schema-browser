using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using JetBrains.Annotations;
using XyrusWorx.Diagnostics;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel 
{
	public class Processor
	{
		private readonly ProcessorContext mContext;
		private ComplexTypeModel mOutputModel;

		public Processor([NotNull] XmlIndex index, [NotNull] LocalizationService localizationService, ILogWriter log = null)
		{
			mContext = new ProcessorContext(index, localizationService, log);
		}

		[NotNull]
		public ProcessorContext Context => mContext;

		[CanBeNull]
		public ComplexTypeModel GetOutput() => mOutputModel;

		[NotNull]
		public IEnumerable<T> Get<T>() => mContext.All<T>();

		[NotNull]
		public Processor Process(string key, XElement context)
		{
			var name = mContext.Index.GetQualifiedName(key, context);
			return Process(name);
		}
		
		[NotNull]
		public Processor Process(XName qualifiedName)
		{
			mOutputModel = null;
			
			var typeElement = mContext.Index.Lookup("complexType", qualifiedName);
			if (typeElement == null)
			{
				mContext.Log.WriteError($"The complex type \"{qualifiedName}\" was not found in the index. Did you select the correct schema file?");
				return this;
			}

			mOutputModel = Process(typeElement);
			return this;
		}

		private ComplexTypeModel Process(XmlIndexEntry entry)
		{
			var typeQualifiedName = mContext.Index.GetQualifiedName(entry.Element.Attribute("name")?.Value, entry.Element);
			if (typeQualifiedName == null)
			{
				// should not happen as the index uses the name attribute and excludes CTs without one
				throw new XmlSchemaException("Missing 'name' attribute for complex type.");
			}

			using (mContext.For(entry.Element))
			{
				return mContext.Read<ComplexTypeParticle, ComplexTypeModel>(typeQualifiedName);
			}
		}
	}
}