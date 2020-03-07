using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	[PublicAPI]
	public class SimpleTypeRestrictionModel : ISimpleTypeSpecificationModel
	{
		[CanBeNull]
		public SimpleTypeConstraint Constraint { get; set; }
	}
}