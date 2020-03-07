using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
    [PublicAPI]
    public interface IXsdAbstractPropertyDescription
    {
        uint MinOccurs { get; }
        uint MaxOccurs { get; }
        bool IsMandatory { get; }
    }
}