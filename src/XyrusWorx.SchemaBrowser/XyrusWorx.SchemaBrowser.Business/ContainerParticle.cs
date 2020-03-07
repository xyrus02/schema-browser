using JetBrains.Annotations;
using XyrusWorx.Diagnostics;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
	[PublicAPI]
	public class ContainerParticle : XsdParticle<ComplexTypeModel>
	{
		protected override void ProcessOverride(ProcessorContext context, ComplexTypeModel model)
		{
			var source = context.Peek();
			var localName = source.Name.LocalName;
			var reference = source.Attribute("ref")?.Value;
			
			if (!string.IsNullOrWhiteSpace(reference))
			{
				source = context.Index.Lookup(localName, context.Index.GetQualifiedName(reference, source))?.Element;
				
				if (source == null)
				{
					context.Log.WriteWarning($"The {localName} particle \"{reference}\" was referenced by \"{model.TypeName}\" but not found in the index. This could be because a schema import is missing or there is a more general schema file.");
					return;
				}
			}

			foreach (var child in source.Elements())
			{
				if (child.Name.NamespaceName != XmlIndex.XsdNamespace)
				{
					continue;
				}

				using (context.For(child))
				{
					switch (child.Name.LocalName)
					{
						case "any":
						case "anyAttribute":
							// ignored
							break;
						case "annotation":
							context.Read<AnnotationParticle>(model);
							break;
						case "sequence":
						case "all":
						case "group":
						case "attributeGroup":
							context.Read<ContainerParticle>(model);
							break;
						case "choice":
							// todo
							context.Log.WriteWarning($"The complex type \"{model.TypeName}\" uses the unsupported schema element \"choice\".");
							break;
						case "simpleContent":
							// todo
							context.Read<SimpleContentParticle>(model);
							break;
						case "complexContent":
							context.Read<HierarchyParticle>(model);
							break;
						case "element":
						case "attribute":
							context.Read<PropertyParticle>(model);
							break;
					
					}
				}
			}
		}
	}
}