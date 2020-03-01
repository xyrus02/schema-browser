using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	public class SimpleTypeListModel : ISimpleTypeSpecificationModel
	{
		[CanBeNull]
		public SimpleTypeModel ElementType { get; set; }
	}
}