using System.Collections.Generic;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	public class SimpleTypeUnionModel : ISimpleTypeSpecificationModel
	{
		[NotNull]
		public List<SimpleTypeModel> Components { get; } = new List<SimpleTypeModel>();
	}
}