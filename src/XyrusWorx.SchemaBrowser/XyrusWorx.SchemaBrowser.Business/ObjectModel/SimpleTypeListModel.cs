using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	[PublicAPI]
	public class SimpleTypeListModel : ISimpleTypeSpecificationModel
	{
		[CanBeNull]
		public SimpleTypeModel ElementType { get; set; }
	}
}