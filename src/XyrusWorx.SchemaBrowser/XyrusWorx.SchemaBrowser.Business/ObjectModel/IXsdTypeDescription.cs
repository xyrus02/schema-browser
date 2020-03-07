using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	[PublicAPI]
	public interface IXsdTypeDescription : IEnumerable<IXsdPropertyGroupDescription>, IXsdModel
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