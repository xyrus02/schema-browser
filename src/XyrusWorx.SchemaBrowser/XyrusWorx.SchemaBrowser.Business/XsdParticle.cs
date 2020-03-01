using System;
using JetBrains.Annotations;
using XyrusWorx;
using XyrusWorx.Diagnostics;

namespace XyrusWorx.SchemaBrowser.Business 
{
	public abstract class XsdParticle<T> : IXsdParticle
	{
		public void Process([NotNull] ProcessorContext context, [NotNull] T model)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (context.Log.Verbosity <= LogVerbosity.Debug)
			{
				context.Log.WriteDebug($"[{context.Stack(x => x.Count)}]\tPUSH\t{GetType().Name}\t{GetCompositeStackHead(context)}");
			}
			
			ProcessOverride(context, model);
			
			if (context.Log.Verbosity <= LogVerbosity.Debug)
			{
				context.Log.WriteDebug($"[{context.Stack(x => x.Count)}]\tPOP \t{GetType().Name}");
			}
		}

		private string GetCompositeStackHead(ProcessorContext context)
		{
			var token = context.Peek(); //context.Stack(x => x.ToArray());
			var id = 
				token.Attribute("name")?.Value.NormalizeNull() 
				?? token.Attribute("ref")?.Value.NormalizeNull()
				?? token.Attribute("base")?.Value.NormalizeNull();
			
			return string.IsNullOrWhiteSpace(id) ? token.Name.LocalName : $"{token.Name.LocalName}[{id}]";
		}

		protected abstract void ProcessOverride([NotNull] ProcessorContext context, [NotNull] T model);
		void IXsdParticle.Process(ProcessorContext context, object model) => Process(context, (T)model);
	}
}