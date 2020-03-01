using System.Xml.Linq;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel 
{
	public interface INamedModel 
	{
		XName TypeName { get; set; }
	}
}