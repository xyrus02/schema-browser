using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	public class SimpleTypeRestrictionModel : ISimpleTypeSpecificationModel
	{
		[CanBeNull]
		public SimpleTypeConstraint Constraint { get; set; }
	}
}