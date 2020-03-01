using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XyrusWorx.Collections;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
	public class SimpleTypeRestrictionParticle : XsdParticle<SimpleTypeModel>
	{
		protected override void ProcessOverride(ProcessorContext context, SimpleTypeModel model)
		{
			var constraint = SimpleTypeConstraint.Create(model, context.GetOutputLanguageResolver());
			var restriction = new SimpleTypeRestrictionModel
			{
				Constraint = constraint
			};
			
			model.Specification = restriction;

			var source = context.Peek();
			var enumerationValues = source.Elements(XName.Get("enumeration", XmlIndex.XsdNamespace)).AsArray();

			if (enumerationValues.Any())
			{
				restriction.Constraint = new TextConstraint(model, context.GetOutputLanguageResolver());
				ProcessEnumeration(context, restriction.Constraint, enumerationValues);
				return;
			}

			switch (constraint)
			{
				case TextConstraint textConstraint:
					ProcessTextConstraint(context, textConstraint);
					break;
				case ValueConstraint valueConstraint:
					ProcessValueConstraint(context, valueConstraint);
					break;
				default:
					restriction.Constraint = null;
					break;
			}
		}

		private static void ProcessEnumeration(ProcessorContext context, SimpleTypeConstraint constraint, IEnumerable<XElement> enumerationValues)
		{
			foreach (var enumerationValue in enumerationValues)
			{
				var value = new AnnotatedValue(enumerationValue.Attribute("value")?.Value);
				var annotation = enumerationValue.Element(XName.Get("annotation", XmlIndex.XsdNamespace));
				
				if (annotation != null)
				{
					using (context.For(annotation))
					{
						context.Read<AnnotationParticle>(value);
					}
				}
			
				constraint.Enumeration.Add(value);
			}
		}
		private static void ProcessValueConstraint(ProcessorContext context, ValueConstraint valueConstraint)
		{
			var source = context.Peek();

			var minConstraint = source.Element(XName.Get("minInclusive", XmlIndex.XsdNamespace)) ??
			                    source.Element(XName.Get("minExclusive", XmlIndex.XsdNamespace));
			
			var maxConstraint = source.Element(XName.Get("maxInclusive", XmlIndex.XsdNamespace)) ??
			                    source.Element(XName.Get("maxExclusive", XmlIndex.XsdNamespace));

			valueConstraint.MinValue = minConstraint?.Attribute("value")?.Value;
			valueConstraint.MinIsInclusive = minConstraint?.Name.LocalName == "minInclusive";
			
			valueConstraint.MaxValue = maxConstraint?.Attribute("value")?.Value;
			valueConstraint.MaxIsInclusive = maxConstraint?.Name.LocalName == "maxInclusive";

			valueConstraint.Pattern = source.Element(XName.Get("pattern", XmlIndex.XsdNamespace))?.Attribute("value")?.Value;
		}
		private static void ProcessTextConstraint(ProcessorContext context, TextConstraint textConstraint)
		{
			var source = context.Peek();

			textConstraint.FixedLength = (source.Element(XName.Get("length", XmlIndex.XsdNamespace))?.Attribute("value")?.Value).TryDeserialize<int>(int.TryParse);
			
			textConstraint.MinLength = (source.Element(XName.Get("minLength", XmlIndex.XsdNamespace))?.Attribute("value")?.Value).TryDeserialize<int>(int.TryParse);
			textConstraint.MaxLength = (source.Element(XName.Get("maxLength", XmlIndex.XsdNamespace))?.Attribute("value")?.Value).TryDeserialize<int>(int.TryParse);
			
			textConstraint.Pattern = source.Element(XName.Get("pattern", XmlIndex.XsdNamespace))?.Attribute("value")?.Value;
		}
	}
}