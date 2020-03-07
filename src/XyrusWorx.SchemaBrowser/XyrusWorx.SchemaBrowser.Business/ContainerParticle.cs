using System.Linq;
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
			
			var virtualGroup = new PropertyGroupModel(model, PropertyGroupType.Virtual);

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
						case "choice":
							context.Read<PropertyGroupParticle>(model);
							break;
						case "simpleContent":
							context.Read<SimpleContentParticle>(model);
							break;
						case "complexContent":
							context.Read<HierarchyParticle>(model);
							break;
						case "element":
						case "attribute":
							context.Read<PropertyParticle>(virtualGroup);
							break;
					
					}
				}
			}

			if (virtualGroup.Properties.Any())
			{
				model.PropertyGroups.Insert(0, virtualGroup);
			}
		}
	}
}