using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
    [PublicAPI]
    public class PropertyGroupModel : IEnumerable<IXsdPropertyDescription>, IEnumerable<IXsdPropertyGroupDescription>, IXsdPropertyGroupDescription
    {
        public PropertyGroupModel(IXsdTypeDescription owner, PropertyGroupType type)
        {
            Owner = owner;
            GroupType = type;
        }

        public IXsdTypeDescription Owner { get; }
		
        public PropertyGroupType GroupType { get; }
		
        [NotNull]
        public Dictionary<StringKey, PropertyModel> Properties { get; } = new Dictionary<StringKey, PropertyModel>();
		
        [NotNull]
        public List<PropertyGroupModel> PropertyGroups { get; } = new List<PropertyGroupModel>();
		
        public uint MinOccurs { get; set; }
		
        public uint MaxOccurs { get; set; }
		
        public bool IsMandatory => MinOccurs > 0;
		
        IEnumerator<IXsdPropertyGroupDescription> IEnumerable<IXsdPropertyGroupDescription>.GetEnumerator() => PropertyGroups.GetEnumerator();
        IEnumerator<IXsdPropertyDescription> IEnumerable<IXsdPropertyDescription>.GetEnumerator() => Properties.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Properties.Values.GetEnumerator();
    }
}