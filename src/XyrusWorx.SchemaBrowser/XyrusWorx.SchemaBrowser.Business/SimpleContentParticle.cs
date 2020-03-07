using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
	[PublicAPI]
	public class SimpleContentParticle : XsdParticle<ComplexTypeModel>
	{
		protected override void ProcessOverride(ProcessorContext context, ComplexTypeModel model)
		{
			var source = context.Peek();
			
			var childElement =
				source.Element(XName.Get("extension", XmlIndex.XsdNamespace)) ??
				source.Element(XName.Get("restriction", XmlIndex.XsdNamespace));

			XName baseName = null;

			if (childElement != null)
			{
				var baseTypeName = childElement.Attribute("base")?.Value;
				if (!string.IsNullOrWhiteSpace(baseTypeName))
				{
					var qualifiedName = context.Index.GetQualifiedName(baseTypeName, source);
					if (qualifiedName != null)
					{
						baseName = qualifiedName;
					}
				}
				
				foreach (var grandChild in childElement.Elements())
				{
					if (grandChild.Name.NamespaceName != XmlIndex.XsdNamespace)
					{
						continue;
					}

					using (context.For(grandChild))
					{
						switch (grandChild.Name.LocalName)
						{
							case "any":
							case "anyAttribute":
								// ignored
								break;
							case "attribute":
								context.Read<PropertyParticle>(model);
								break;
						}
					}
				}
			}
			
			var property = new PropertyModel(context.GetOutputLanguageResolver().Resolve(@"ContentPlaceholder") ?? "(content)");
			
			var virtualTypeName = $"{model.TypeName.LocalName}_Content";
			var virtualTypeCounter = 0;
			var virtualTypeQualifiedName = XName.Get($"{virtualTypeName}{(virtualTypeCounter == 0 ? "" : $"_{virtualTypeCounter}")}", source.Name.NamespaceName);

			while (context.Has<ComplexTypeModel>(virtualTypeQualifiedName) || context.Has<SimpleTypeModel>(virtualTypeName))
			{
				virtualTypeCounter++;
				virtualTypeQualifiedName = XName.Get($"{virtualTypeName}{(virtualTypeCounter == 0 ? "" : $"_{virtualTypeCounter}")}", source.Name.NamespaceName);
			}

			property.DataType = context.Read<SimpleTypeParticle, SimpleTypeModel>(virtualTypeQualifiedName).TryTransform(x =>
				{
					x.ContentTypeName = baseName;
					return x;
				});
			
			property.MinOccurs = 1;
			property.MaxOccurs = 1;
			property.IsContent = true;
			
			var propertyGroup = new PropertyGroupModel(model, PropertyGroupType.Virtual);
			
			propertyGroup.Properties.Add(new StringKey(), property);
			
			propertyGroup.MinOccurs = 1;
			propertyGroup.MaxOccurs = 1;

			model.PropertyGroups.Add(propertyGroup);
			model.IsComplexTypeWithSimpleContent = true;
		}
	}
}