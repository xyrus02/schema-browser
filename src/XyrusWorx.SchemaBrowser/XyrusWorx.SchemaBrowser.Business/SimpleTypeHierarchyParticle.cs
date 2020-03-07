using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
	[PublicAPI]
	public class SimpleTypeHierarchyParticle : XsdParticle<SimpleTypeModel>
	{
		protected override void ProcessOverride(ProcessorContext context, SimpleTypeModel model)
		{
			var token = context.Peek().Element(XName.Get("restriction", XmlIndex.XsdNamespace))?.Attribute("base")?.Value;
			if (string.IsNullOrWhiteSpace(token))
			{
				return;
			}
			
			var qualifiedName = context.Index.GetQualifiedName(token, context.Peek());
			var referencedType = context.Index.GlobalLookup(qualifiedName)?.Element;

			if (referencedType == null)
			{
				if (qualifiedName?.NamespaceName != XmlIndex.XsdNamespace)
				{
					return;
				}

				var systemType = context.Init<SimpleTypeModel>(qualifiedName);
				if (systemType.Specification == null)
				{
					systemType.Specification = new SystemTypeSpecificationModel(systemType, context.GetOutputLanguageResolver());
				}
					
				model.BaseTypeNames.Add(systemType.TypeName);

				return;
			}

			using (context.For(referencedType))
			{
				var baseTypeModel = context.Read<SimpleTypeParticle, SimpleTypeModel>(qualifiedName);
				model.BaseTypeNames.Add(baseTypeModel.TypeName);
				model.ContentTypeName = baseTypeModel.TypeName;
			}
		}
	}
}