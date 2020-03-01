using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	public abstract class SimpleTypeConstraint
	{
		private readonly SimpleTypeModel mConstrainedType;

		protected SimpleTypeConstraint([NotNull] SimpleTypeModel constrainedType)
		{
			mConstrainedType = constrainedType ?? throw new ArgumentNullException(nameof(constrainedType));
		}

		[NotNull]
		public IEnumerable<AnnotatedValue> GetConstraintDescription()
		{
			if (Enumerable.Any(Enumeration))
			{
				foreach (var item in Enumeration)
				{
					yield return item;
				}

				yield break;
			}

			yield return new AnnotatedValue(GetConstraintDescriptionOverride());
		}
		
		[NotNull]
		public List<AnnotatedValue> Enumeration { get; } = new List<AnnotatedValue>();

		[CanBeNull]
		internal static SimpleTypeConstraint Create([NotNull] SimpleTypeModel forType, [NotNull] IStringResolver outputLanguage)
		{
			if (forType == null)
			{
				throw new ArgumentNullException(nameof(forType));
			}

			if (outputLanguage == null)
			{
				throw new ArgumentNullException(nameof(outputLanguage));
			}

			var rootType = GetRootTypeName(forType);
			
			switch (rootType?.LocalName.ToLowerInvariant())
			{
				case "duration":
				case "datetime":
				case "time":
				case "date":
				case "gyearmonth":
				case "gyear":
				case "gmonthday":
				case "gday":
				case "gmonth":
				case "integer":
				case "nonpositiveinteger":
				case "nonnegativeinteger":
				case "positiveinteger":
				case "negativeinteger":
				case "long":
				case "int":
				case "short":
				case "byte":
				case "unsignedlong":
				case "unsignedint":
				case "unsignedshort":
				case "unsignedbyte":
				case "float":
				case "double":
				case "decimal":
					return new ValueConstraint(forType, outputLanguage);

				case "hexbinary":
				case "base64binary":
				case "anyuri":
				case "qname":
				case "token":
				case "language":
				case "name":
				case "ncname":
				case "nmtoken":
				case "nmtokens":
				case "id":
				case "idref":
				case "idrefs":
				case "entity":
				case "entities":
				case "string":
				case "normalizedString":
				case "boolean":
					return new TextConstraint(forType, outputLanguage);

				default:
					return null;
			}
		}

		internal XName GetRootTypeName() => GetRootTypeName(mConstrainedType);
		internal static string GetSystemTypeClassification(XName qualifiedName, [NotNull] IStringResolver outputLanguage)
		{
			if (outputLanguage == null)
			{
				throw new ArgumentNullException(nameof(outputLanguage));
			}

			switch (qualifiedName?.LocalName.ToLowerInvariant())
			{
				case "integer":
				case "nonpositiveinteger":
				case "nonnegativeinteger":
				case "positiveinteger":
				case "negativeinteger":
				case "long":
				case "int":
				case "short":
				case "byte":
				case "unsignedlong":
				case "unsignedint":
				case "unsignedshort":
				case "unsignedbyte":
					return outputLanguage.Resolve(@"ValueClassInteger");

				case "qname":
				case "token":
				case "language":
				case "name":
				case "ncname":
				case "nmtoken":
				case "nmtokens":
				case "id":
				case "idref":
				case "idrefs":
				case "entity":
				case "entities":
				case "string":
				case "normalizedString":
					return outputLanguage.Resolve(@"ValueClassString");

				case "boolean":
					return outputLanguage.Resolve(@"ValueClassBoolean");

				case "float":
				case "double":
				case "decimal":
					return outputLanguage.Resolve(@"ValueClassDecimal");

				case "duration":
					return outputLanguage.Resolve(@"ValueClassDuration");
				case "datetime":
					return outputLanguage.Resolve(@"ValueClassDateTime");
				case "time":
					return outputLanguage.Resolve(@"ValueClassTime");
				case "date":
					return outputLanguage.Resolve(@"ValueClassDate");
				case "gyearmonth":
					return outputLanguage.Resolve(@"ValueClassYearMonth");
				case "gyear":
					return outputLanguage.Resolve(@"ValueClassYear");
				case "gmonthday":
					return outputLanguage.Resolve(@"ValueClassMonthDay");
				case "gday":
					return outputLanguage.Resolve(@"ValueClassDay");
				case "gmonth":
					return outputLanguage.Resolve(@"ValueClassMonth");
				case "hexbinary":
					return outputLanguage.Resolve(@"ValueClassBinaryHex");
				case "base64binary":
					return outputLanguage.Resolve(@"ValueClassBinaryBase64");
				case "anyuri":
					return outputLanguage.Resolve(@"ValueClassUri");

				default:
					return outputLanguage.Resolve(@"ValueClassOther");
			}
		}
		
		[NotNull]
		protected abstract string GetConstraintDescriptionOverride();
		
		private static XName GetRootTypeName(SimpleTypeModel simpleTypeModel)
		{
			if (simpleTypeModel.TypeName.NamespaceName == XmlIndex.XsdNamespace)
			{
				return simpleTypeModel.TypeName;
			}
			
			return simpleTypeModel.BaseTypeNames.FirstOrDefault(x => x.NamespaceName == XmlIndex.XsdNamespace);
		}
	}
}