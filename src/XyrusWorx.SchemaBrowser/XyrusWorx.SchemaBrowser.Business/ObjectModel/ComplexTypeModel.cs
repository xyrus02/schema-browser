using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx;
using XyrusWorx.Collections;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel 
{
	public class ComplexTypeModel : PropertyTypeModel, IAnnotableModel, INamedModel, IXsdTypeDescription
	{
		public override XName TypeName { get; set; }
		
		[NotNull]
		public List<XName> BaseTypeNames { get; } = new List<XName>();
		
		[NotNull]
		public Dictionary<StringKey, PropertyModel> Properties { get; } = new Dictionary<StringKey, PropertyModel>();
		
		public override string DisplayName => TypeName.LocalName;
		
		public bool IsAbstract { get; set; }
		
		public bool IsComplexTypeWithSimpleContent { get; set; }
		
		public override bool IsSimpleContent => IsComplexTypeWithSimpleContent;

		public void InheritFrom(ComplexTypeModel baseModel)
		{
			foreach (var property in baseModel.Properties)
			{
				Properties.AddIfMissing(property.Key, property.Value);
			}
		}

		protected override IEnumerable<IXsdPropertyDescription> Children() => Properties.Values;
	}
}