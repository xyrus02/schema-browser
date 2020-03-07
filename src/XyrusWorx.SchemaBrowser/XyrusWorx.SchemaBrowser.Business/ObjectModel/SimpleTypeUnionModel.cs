using System.Collections.Generic;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	[PublicAPI]
	public class SimpleTypeUnionModel : ISimpleTypeSpecificationModel
	{
		[NotNull]
		public List<SimpleTypeModel> Components { get; } = new List<SimpleTypeModel>();
	}
}