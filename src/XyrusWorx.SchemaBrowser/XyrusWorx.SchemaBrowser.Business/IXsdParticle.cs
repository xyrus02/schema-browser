using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business 
{
	public interface IXsdParticle
	{
		void Process([NotNull] ProcessorContext context, [NotNull] object model);
	}
}