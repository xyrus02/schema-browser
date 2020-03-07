using JetBrains.Annotations;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;
using XyrusWorx.Diagnostics;

namespace XyrusWorx.SchemaBrowser.Business 
{
	[PublicAPI]
	public class ComplexTypeParticle : XsdParticle<ComplexTypeModel>
	{
		protected override void ProcessOverride(ProcessorContext context, ComplexTypeModel model)
		{
			context.Log.WriteVerbose("Descending into \"{0}\"", model.TypeName);
			
			model.IsAbstract = (context.Peek().Attribute("abstract")?.Value).TryDeserialize<bool>();
			context.Read<ContainerParticle>(model);
		}
	}
}