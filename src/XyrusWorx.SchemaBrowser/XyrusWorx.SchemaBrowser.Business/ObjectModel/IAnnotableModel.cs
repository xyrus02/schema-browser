using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel 
{
	[PublicAPI]
	public interface IAnnotableModel 
	{
		string Annotation { get; set; }
	}
}