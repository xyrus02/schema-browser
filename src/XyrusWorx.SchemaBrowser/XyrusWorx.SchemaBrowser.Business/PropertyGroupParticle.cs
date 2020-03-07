using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
    [PublicAPI]
    public class PropertyGroupParticle : XsdParticle<ComplexTypeModel>
    {
        protected sealed override void ProcessOverride(ProcessorContext context, ComplexTypeModel model)
        {
            var source = context.Peek();
            var groupType = source.Name.LocalName switch
            {
                "sequence" => PropertyGroupType.Sequence,
                "all" => PropertyGroupType.AllOf,
                "choice" => PropertyGroupType.OneOf,
                "attributeGroup" => PropertyGroupType.AllOf,
                _ => PropertyGroupType.Virtual
            };
			
			
            var propertyGroup = new PropertyGroupModel(model, groupType);
            var digitRegex = new Regex("\\d+");
			
            propertyGroup.MinOccurs = source.Attribute("minOccurs")?.Value.TryTransform(x => digitRegex.IsMatch(x) ? x.TryDeserialize<uint>() : 0) ?? 1;
            propertyGroup.MaxOccurs = source.Attribute("maxOccurs")?.Value.TryTransform(x => digitRegex.IsMatch(x) ? x.TryDeserialize<uint>() : uint.MaxValue) ?? 1;

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
                        case "element":
                        case "attribute":
                            context.Read<PropertyParticle>(propertyGroup);
                            break;
                    }
                }
            }

            if (propertyGroup.Properties.Any())
            {
                model.PropertyGroups.Add(propertyGroup);
            }
        }
    }
}