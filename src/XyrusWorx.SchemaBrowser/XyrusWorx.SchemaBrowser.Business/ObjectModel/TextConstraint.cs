using System;
using System.Text;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	[PublicAPI]
	public class TextConstraint : SimpleTypeConstraint
	{
		private readonly IStringResolver mOutputLanguage;

		public TextConstraint(SimpleTypeModel constrainedType, [NotNull] IStringResolver outputLanguage) : base(constrainedType)
		{
			mOutputLanguage = outputLanguage ?? throw new ArgumentNullException(nameof(outputLanguage));
		}
		
		public int? FixedLength { get; set; }
		public int? MinLength { get; set; }
		public int? MaxLength { get; set; }
		public string Pattern { get; set; }
		
		protected override string GetConstraintDescriptionOverride()
		{
			var sb = new StringBuilder();

			sb.Append(GetSystemTypeClassification(GetRootTypeName(), mOutputLanguage));
			
			if (FixedLength != null)
			{
				sb.Append($" {mOutputLanguage.Format(@"TextConstraintFixedLength", FixedLength)}");
			}
			else if (MinLength != null && MaxLength != null)
			{
				sb.Append(MinLength == MaxLength
					? $" {mOutputLanguage.Format(@"TextConstraintFixedLength", MinLength)}"
					: $" {mOutputLanguage.Format(@"TextConstraintLengthBetween", MinLength, MaxLength)}");
			}
			else switch (MinLength)
			{
				case null when MaxLength == null:
					// nothing
					break;
				
				case null:
					sb.Append($" {mOutputLanguage.Format(@"TextConstraintMaximumLength", MaxLength)}");
					break;
				
				default:
				{
					if (MaxLength == null)
					{
						sb.Append($" {mOutputLanguage.Format(@"TextConstraintMinimumLength", MinLength)}");
					}

					break;
				}
			}

			if (!string.IsNullOrWhiteSpace(Pattern))
			{
				sb.Append($" {mOutputLanguage.Format(@"TextConstraintPattern", Pattern)}");
			}

			return sb.ToString();
		}
	}
}