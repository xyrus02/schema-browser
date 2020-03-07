using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel 
{
	[PublicAPI]
	public class ComplexTypeModel : PropertyTypeModel, IAnnotableModel, INamedModel, IXsdTypeDescription
	{
		public override XName TypeName { get; set; }
		
		[NotNull]
		public List<XName> BaseTypeNames { get; } = new List<XName>();
		
		[NotNull]
		public List<PropertyGroupModel> PropertyGroups { get; } = new List<PropertyGroupModel>();
		
		public override string DisplayName => TypeName.LocalName;
		
		public bool IsAbstract { get; set; }
		
		public bool IsComplexTypeWithSimpleContent { get; set; }
		
		public override bool IsSimpleContent => IsComplexTypeWithSimpleContent;

		public void InheritFrom(ComplexTypeModel baseModel)
		{
			foreach (var property in baseModel.PropertyGroups.Where(x => x.PropertyGroups.Any() || x.Properties.Any()))
			{
				PropertyGroups.Add(property);
			}
		}

		protected override IEnumerable<IXsdPropertyGroupDescription> Children() => PropertyGroups;
	}
}