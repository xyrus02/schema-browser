﻿using JetBrains.Annotations;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
	[PublicAPI]
	public class SimpleTypeListParticle : XsdParticle<SimpleTypeModel>
	{
		protected override void ProcessOverride(ProcessorContext context, SimpleTypeModel model)
		{
			model.Specification = new SimpleTypeListModel
			{
				ElementType = SimpleTypeParticle.GetReferencedTypeFromToken(context, model, context.Peek().Attribute("itemType")?.Value)
			};
		}
	}
}