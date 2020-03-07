using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business 
{
	[PublicAPI]
	public interface IXsdParticle
	{
		void Process([NotNull] ProcessorContext context, [NotNull] object model);
	}
}