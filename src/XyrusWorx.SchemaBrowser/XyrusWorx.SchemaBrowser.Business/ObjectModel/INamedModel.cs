using System.Xml.Linq;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel 
{
	[PublicAPI]
	public interface INamedModel 
	{
		XName TypeName { get; set; }
	}
}