using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
    [PublicAPI]
    public enum PropertyGroupType
    {
        Virtual,
        Sequence,
        OneOf,
        AllOf
    }
}