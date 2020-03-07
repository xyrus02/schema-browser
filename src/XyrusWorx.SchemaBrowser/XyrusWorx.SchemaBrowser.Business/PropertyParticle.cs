using System.Text.RegularExpressions;
using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx.Collections;
using XyrusWorx.Diagnostics;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
	[PublicAPI]
	public class PropertyParticle : XsdParticle<PropertyGroupModel>
	{
		protected override void ProcessOverride(ProcessorContext context, PropertyGroupModel model)
		{
			var source = context.Peek();
			var propertyName = source.Attribute("name")?.Value;
			
			if (propertyName == null || string.IsNullOrWhiteSpace(propertyName))
			{
				// should not happen as the index uses the name attribute and excludes CTs without one
				var propertyRef = source.Attribute("ref")?.Value;
				var qualifiedName = context.Index.GetQualifiedName(propertyRef, source);

				if (qualifiedName == null)
				{
					// ignore property with missing ref
					return;
				}

				source = context.Index.Lookup(source.Name.LocalName, qualifiedName)?.Element;
				propertyName = source?.Attribute("name")?.Value;

				if (source == null || propertyName == null || string.IsNullOrWhiteSpace(propertyName))
				{
					// ignore property with unresolvable ref
					return;
				}
			}

			var property = new PropertyModel(propertyName);
			var digitRegex = new Regex("\\d+");

			property.IsNillable = source.Attribute("nillable")?.Value.TryDeserialize<bool>() ?? false;
			property.MinOccurs = source.Attribute("minOccurs")?.Value.TryTransform(x => digitRegex.IsMatch(x) ? x.TryDeserialize<uint>() : 0) ?? 1;
			property.MaxOccurs = source.Attribute("maxOccurs")?.Value.TryTransform(x => digitRegex.IsMatch(x) ? x.TryDeserialize<uint>() : uint.MaxValue) ?? 1;
			property.IsAttribute = source.Name.LocalName == "attribute";

			model.Properties.AddOrUpdate(propertyName, property);

			foreach (var child in source.Elements())
			{
				if (child.Name.NamespaceName != XmlIndex.XsdNamespace)
				{
					continue;
				}

				using (context.For(child))
				{
					switch (child.Name.LocalName)
					{
						case "annotation":
							context.Read<AnnotationParticle>(property);
							break;
						case "complexType":
						case "simpleType":

							var virtualTypeName = $"{model.Owner.TypeName.LocalName}_{propertyName}";
							var virtualTypeCounter = 0;
							var virtualTypeQualifiedName = XName.Get($"{virtualTypeName}{(virtualTypeCounter == 0 ? "" : $"_{virtualTypeCounter}")}", source.Name.NamespaceName);

							while (context.Has<ComplexTypeModel>(virtualTypeQualifiedName) || context.Has<SimpleTypeModel>(virtualTypeName))
							{
								virtualTypeCounter++;
								virtualTypeQualifiedName = XName.Get($"{virtualTypeName}{(virtualTypeCounter == 0 ? "" : $"_{virtualTypeCounter}")}", source.Name.NamespaceName);
							}

							if (child.Name.LocalName == "complexType")
							{
								property.DataType = context.Read<ComplexTypeParticle, ComplexTypeModel>(virtualTypeQualifiedName);
							}
							else
							{
								property.DataType = context.Read<SimpleTypeParticle, SimpleTypeModel>(virtualTypeQualifiedName);
							}

							break;
					}
				}
				
			}

			if (property.DataType != null)
			{
				return;
			}

			var typeRef = source.Attribute("type")?.Value;
			if (string.IsNullOrWhiteSpace(typeRef))
			{
				return;
			}

			var typeQualifiedName = context.Index.GetQualifiedName(typeRef, source);
			var type = context.Index.GlobalLookup(typeQualifiedName)?.Element;
			
			if (typeQualifiedName == null || type == null)
			{
				context.Log.WriteWarning($"The property \"{propertyName}\" on complex type \"{model.Owner.TypeName}\" references type \"{typeQualifiedName}\" but the target type was not found in the index. This could be because a schema import is missing or there is a more general schema file.");
				return;
			}

			using (context.For(type))
			{
				switch (type.Name.LocalName) {
					case "complexType":
						property.DataType = context.Read<ComplexTypeParticle, ComplexTypeModel>(typeQualifiedName);
						break;
					case "simpleType":
						property.DataType = context.Read<SimpleTypeParticle, SimpleTypeModel>(typeQualifiedName);
						break;
					default:
						context.Log.WriteWarning($"The property \"{propertyName}\" on complex type \"{model.Owner.TypeName}\" references token \"{typeRef}\" which is not a type definition.");
						break;
				}
			}
		}
	}
}