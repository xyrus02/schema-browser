using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel 
{
	[PublicAPI]
	public abstract class PropertyTypeModel : TargetableModel, IEnumerable<IXsdPropertyGroupDescription>
	{
		public IEnumerator<IXsdPropertyGroupDescription> GetEnumerator() => Children().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Children().GetEnumerator();

		public string Annotation { get; set; }
		
		[NotNull]
		public abstract XName TypeName { get; set; }

		[CanBeNull]
		public string ContentTypeDisplayName => ContentTypeName?.LocalName;
		
		[CanBeNull]
		public XName ContentTypeName { get; set; }
		
		public abstract bool IsSimpleContent { get; }
		
		[NotNull]
		protected abstract IEnumerable<IXsdPropertyGroupDescription> Children();
	}
}