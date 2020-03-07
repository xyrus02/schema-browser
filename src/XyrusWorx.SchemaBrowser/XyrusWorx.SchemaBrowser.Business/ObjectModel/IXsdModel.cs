using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	[PublicAPI]
	public interface IXsdModel
	{
		bool IsActive { get; set; }
	}
}