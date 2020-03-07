using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
    [PublicAPI]
    public class PropertyGroupParticle : XsdParticle<ComplexTypeModel>
    {
        protected sealed override void ProcessOverride(ProcessorContext context, ComplexTypeModel model)
        {
            var propertyGroup = FillGroup(context, CreateGroup( context.Peek(), model));
            if (propertyGroup.Properties.Any())
            {
                model.PropertyGroups.Add(propertyGroup);
            }
        }

        private PropertyGroupModel CreateGroup(XElement element, IXsdTypeDescription owner)
        {
            var groupType = MapType(element.Name);

            var propertyGroup = new PropertyGroupModel(owner, groupType);
            var digitRegex = new Regex("\\d+");
			
            propertyGroup.MinOccurs = element.Attribute("minOccurs")?.Value.TryTransform(x => digitRegex.IsMatch(x) ? x.TryDeserialize<uint>() : 0) ?? 1;
            propertyGroup.MaxOccurs = element.Attribute("maxOccurs")?.Value.TryTransform(x => digitRegex.IsMatch(x) ? x.TryDeserialize<uint>() : uint.MaxValue) ?? 1;

            return propertyGroup;
        }
        private PropertyGroupModel FillGroup(ProcessorContext context, PropertyGroupModel model)
        {
            var source = context.Peek();
            
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
                        case "sequence":
                        case "all":
                        case "group":
                        case "attributeGroup":
                        case "choice":
                            var childGroup = FillGroup(context, CreateGroup(context.Peek(), model.Owner));
                            if (childGroup.Properties.Any())
                            {
                                model.PropertyGroups.Add(childGroup);
                            }
                            break;
                        case "element":
                        case "attribute":
                            context.Read<PropertyParticle>(model);
                            break;
                    }
                }
            }

            return model;
        }

        private PropertyGroupType MapType(XName name) => name.LocalName switch
        {
            "sequence" => PropertyGroupType.Sequence,
            "all" => PropertyGroupType.AllOf,
            "choice" => PropertyGroupType.OneOf,
            "attributeGroup" => PropertyGroupType.AllOf,
            _ => PropertyGroupType.Virtual
        };
    }
}