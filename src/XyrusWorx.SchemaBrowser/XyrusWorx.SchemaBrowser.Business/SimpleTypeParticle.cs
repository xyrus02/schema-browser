using XyrusWorx.Diagnostics;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
	public class SimpleTypeParticle : XsdParticle<SimpleTypeModel>
	{
		protected override void ProcessOverride(ProcessorContext context, SimpleTypeModel model)
		{
			var source = context.Peek();
			
			model.IsAbstract = (source.Attribute("abstract")?.Value).TryDeserialize<bool>();
			
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
							context.Read<AnnotationParticle>(model);
							break;

						case "restriction":
							using (context.For(source))
							{
								context.Read<SimpleTypeHierarchyParticle>(model);
							}

							context.Read<SimpleTypeRestrictionParticle>(model);
							break;
						
						case "union":
							context.Read<SimpleTypeUnionParticle>(model);
							break;
						
						case "list":
							context.Read<SimpleTypeListParticle>(model);
							break;
					}
				}
			}
		}
		
		internal static SimpleTypeModel GetReferencedTypeFromToken(ProcessorContext context, SimpleTypeModel model, string token)
		{
			var qualifiedName = context.Index.GetQualifiedName(token, context.Peek());
			var referencedType = context.Index.GlobalLookup(qualifiedName);

			if (referencedType == null)
			{
				if (qualifiedName?.NamespaceName == XmlIndex.XsdNamespace)
				{
					var systemType = context.Init<SimpleTypeModel>(qualifiedName);
					if (systemType.Specification == null)
					{
						systemType.Specification = new SystemTypeSpecificationModel(systemType, context.GetOutputLanguageResolver());
					}

					return systemType;
				}

				context.Log.WriteWarning($"The simple type \"{model.TypeName}\" references type \"{qualifiedName}\" but the target type was not found in the index. This could be because a schema import is missing or there is a more general schema file.");
				return null;
			}

			return context.Read<SimpleTypeParticle, SimpleTypeModel>(qualifiedName);
		}
	}
}