using System.Linq;
using System.Xml.Linq;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
	public class AnnotationParticle : XsdParticle<IAnnotableModel>
	{
		protected override void ProcessOverride(ProcessorContext context, IAnnotableModel model)
		{
			var element = context.Peek();
			var documentationElement = (element

				// first with empty language
				.Elements(XName.Get("documentation", XmlIndex.XsdNamespace))
				.FirstOrDefault(
					documentation => string.IsNullOrWhiteSpace(
						documentation
							.Attributes()
							.FirstOrDefault(a => a.Name.LocalName == "lang")?.Value)) ?? element

				// or simply first
				.Elements(XName.Get("documentation", XmlIndex.XsdNamespace))
				.FirstOrDefault());

			if (documentationElement != null)
			{
				model.Annotation = documentationElement.Value;
			}
		}
	}
}