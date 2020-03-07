using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	[PublicAPI]
	public interface IXsdPropertyDescription : IXsdModel, IXsdAbstractPropertyDescription
	{
		string PropertyName { get; }
		string Annotation { get; }

		IXsdTypeDescription DataType { get; }

		bool IsAttribute { get; }
		bool IsNillable { get; }
		bool IsContent { get; }
	}
}