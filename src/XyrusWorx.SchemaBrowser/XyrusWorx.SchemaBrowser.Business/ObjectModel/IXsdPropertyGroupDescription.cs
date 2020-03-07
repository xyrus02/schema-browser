using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
    [PublicAPI]
    public interface IXsdPropertyGroupDescription : IXsdModel
    {
        IXsdTypeDescription Owner { get; }
        PropertyGroupType GroupType { get; }
		
        uint MinOccurs { get; }
        uint MaxOccurs { get; }
		
        bool IsMandatory { get; }
    }
}