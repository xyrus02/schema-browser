using System.Collections.Generic;
using System.Xml.Linq;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	public interface IXsdTypeDescription : IEnumerable<IXsdPropertyDescription>, IXsdModel
	{
		XName TypeName { get; }
		XName ContentTypeName { get; }
		
		string Annotation { get; }
		string DisplayName { get; }
		string ContentTypeDisplayName { get; }
		
		bool IsAbstract { get; }
		bool IsSimpleContent { get; }
	}
}