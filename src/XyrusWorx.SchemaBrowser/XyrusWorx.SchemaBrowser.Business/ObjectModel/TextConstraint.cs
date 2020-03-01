using System;
using System.Text;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
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

		// todo localize
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
				if (MinLength == MaxLength)
				{
					sb.Append($" {mOutputLanguage.Format(@"TextConstraintFixedLength", MinLength)}");
				}
				else
				{
					sb.Append($" {mOutputLanguage.Format(@"TextConstraintLengthBetween", MinLength, MaxLength)}");
				}
			}
			else if (MinLength == null && MaxLength == null)
			{
				// nothing
			}
			else if (MinLength == null)
			{
				sb.Append($" {mOutputLanguage.Format(@"TextConstraintMaximumLength", MaxLength)}");
			}
			else if (MaxLength == null)
			{
				sb.Append($" {mOutputLanguage.Format(@"TextConstraintMinimumLength", MinLength)}");
			}

			if (!string.IsNullOrWhiteSpace(Pattern))
			{
				sb.Append($" {mOutputLanguage.Format(@"TextConstraintPattern", Pattern)}");
			}

			return sb.ToString();
		}
	}
}