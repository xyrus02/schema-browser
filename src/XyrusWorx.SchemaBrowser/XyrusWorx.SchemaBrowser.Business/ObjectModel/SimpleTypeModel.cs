using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel 
{
	[PublicAPI]
	public class SimpleTypeModel : PropertyTypeModel, IAnnotableModel, INamedModel, IXsdTypeDescription
	{
		public override XName TypeName { get; set; }

		public ISimpleTypeSpecificationModel Specification { get; set; }

		[NotNull]
		public List<XName> BaseTypeNames { get; } = new List<XName>();
		
		public override string DisplayName => TypeName.LocalName;
		
		public bool IsAbstract { get; set; }
		
		public override bool IsSimpleContent => true;

		protected override IEnumerable<IXsdPropertyDescription> Children()
		{
			yield break;
		}
	}
}