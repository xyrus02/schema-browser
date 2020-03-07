using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
    [PublicAPI]
    public interface IXsdPropertyGroupDescription : IXsdModel, IXsdAbstractPropertyDescription
    {
        IXsdTypeDescription Owner { get; }
        PropertyGroupType GroupType { get; }
    }
}