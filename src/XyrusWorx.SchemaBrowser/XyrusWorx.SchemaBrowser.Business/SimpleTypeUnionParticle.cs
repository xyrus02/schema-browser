using System.Text.RegularExpressions;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
	public class SimpleTypeUnionParticle : XsdParticle<SimpleTypeModel>
	{
		protected override void ProcessOverride(ProcessorContext context, SimpleTypeModel model)
		{
			var source = context.Peek();
			var union = new SimpleTypeUnionModel();

			foreach (var token in Regex.Split(source.Attribute("memberTypes")?.Value ?? "", @"\s", RegexOptions.Singleline))
			{
				var referencedType = SimpleTypeParticle.GetReferencedTypeFromToken(context, model, token);
				if (referencedType == null)
				{
					continue;
				}
				
				union.Components.Add(referencedType);
			}

			model.Specification = union;
		}
	}
}