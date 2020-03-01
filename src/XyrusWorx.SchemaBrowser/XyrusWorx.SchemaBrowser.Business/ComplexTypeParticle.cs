using XyrusWorx.SchemaBrowser.Business.ObjectModel;
using XyrusWorx;
using XyrusWorx.Diagnostics;

namespace XyrusWorx.SchemaBrowser.Business 
{
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