using System.Xml.Linq;
using XyrusWorx.Diagnostics;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
	public class HierarchyParticle : XsdParticle<ComplexTypeModel>
	{
		protected override void ProcessOverride(ProcessorContext context, ComplexTypeModel model)
		{
			var source = context.Peek();
			var childElement =
				source.Element(XName.Get("extension", XmlIndex.XsdNamespace)) ??
				source.Element(XName.Get("restriction", XmlIndex.XsdNamespace));

			if (childElement == null)
			{
				return;
			}
			
			var baseTypeName = childElement.Attribute("base")?.Value;
			if (!string.IsNullOrWhiteSpace(baseTypeName))
			{
				var qualifiedName = context.Index.GetQualifiedName(baseTypeName, source);
				if (qualifiedName != null)
				{
					model.BaseTypeNames.Add(qualifiedName);
				}
				
				var baseType = context.Index.GlobalLookup(qualifiedName)?.Element;
				if (qualifiedName == null || baseType == null)
				{
					context.Log.WriteWarning($"The complex type \"{model.TypeName}\" references type \"{qualifiedName}\" but the target type was not found in the index. This could be because a schema import is missing or there is a more general schema file.");
					return;
				}

				using (context.For(baseType))
				{
					var baseModel = context.Read<ComplexTypeParticle, ComplexTypeModel>(qualifiedName);
					// todo: should there be a distinction between "restriction" and "extension"? I can't see any obvious difference for this use case
					model.InheritFrom(baseModel);
				}
			}

			using (context.For(childElement))
			{
				context.Read<ContainerParticle>(model);
			}
		}
	}
}