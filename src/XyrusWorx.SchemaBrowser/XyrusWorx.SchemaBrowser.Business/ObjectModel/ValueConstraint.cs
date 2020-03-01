using System;
using System.Text;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	public class ValueConstraint : SimpleTypeConstraint
	{
		private readonly IStringResolver mOutputLanguage;

		public ValueConstraint(SimpleTypeModel constrainedType, [NotNull] IStringResolver outputLanguage) : base(constrainedType)
		{
			mOutputLanguage = outputLanguage ?? throw new ArgumentNullException(nameof(outputLanguage));
		}
		
		public string MinValue { get; set; }
		public bool MinIsInclusive { get; set; }
		public string MaxValue { get; set; }
		public bool MaxIsInclusive { get; set; }
		public string Pattern { get; set; }

		// todo localize
		protected override string GetConstraintDescriptionOverride()
		{
			var sb = new StringBuilder();
			var rn = GetRootTypeName();
			var quantifiers = new[]
			{
				mOutputLanguage.Resolve(@"QuantificationLessThanNumber"), 
				mOutputLanguage.Resolve(@"QuantificationExactNumber"), 
				mOutputLanguage.Resolve(@"QuantificationMoreThanNumber")
			};
			
			switch (rn?.LocalName.ToLowerInvariant())
			{
				case "gmonthday":
				case "gyear":
				case "gyearmonth":
				case "date":
					quantifiers = new[]
					{
						mOutputLanguage.Resolve(@"QuantificationLessThanDate"), 
						mOutputLanguage.Resolve(@"QuantificationExactDate"), 
						mOutputLanguage.Resolve(@"QuantificationMoreThanDate")
					};
					break;
				case "datetime":
					quantifiers =new[]
					{
						mOutputLanguage.Resolve(@"QuantificationLessThanDateTime"), 
						mOutputLanguage.Resolve(@"QuantificationExactDateTime"),
						mOutputLanguage.Resolve(@"QuantificationMoreThanDateTime")
					};
					break;
				case "time":
					quantifiers =new[]
					{
						mOutputLanguage.Resolve(@"QuantificationLessThanTime"), 
						mOutputLanguage.Resolve(@"QuantificationExactTime"), 
						mOutputLanguage.Resolve(@"QuantificationMoreThanTime")
					};
					break;
			}
			
			sb.Append(GetSystemTypeClassification(rn, mOutputLanguage));
			
			if (!string.IsNullOrWhiteSpace(MinValue) && !string.IsNullOrWhiteSpace(MaxValue))
			{
				sb.Append($" {(MinIsInclusive ? mOutputLanguage.Resolve(@"QuantificationAtLeast") : quantifiers[0])} {MinValue}");
				sb.Append($" {mOutputLanguage.Resolve(@"Conjunction")}");
				sb.Append($" {(MaxIsInclusive ? mOutputLanguage.Resolve(@"QuantificationAtMost") : quantifiers[2])} {MaxValue}");
			}
			else if (string.IsNullOrWhiteSpace(MinValue) && string.IsNullOrWhiteSpace(MaxValue))
			{
				// nothing
			}
			else if (string.IsNullOrWhiteSpace(MinValue))
			{
				sb.Append($" {(MaxIsInclusive ? mOutputLanguage.Resolve(@"QuantificationAtMost") : quantifiers[2])} {MaxValue}");
			}
			else  if (string.IsNullOrWhiteSpace(MaxValue))
			{
				sb.Append($" {(MinIsInclusive ? mOutputLanguage.Resolve(@"QuantificationAtLeast") : quantifiers[0])} {MinValue}");
			}

			if (!string.IsNullOrWhiteSpace(Pattern))
			{
				sb.Append($" {mOutputLanguage.Format(@"TextConstraintPattern", Pattern)}");
			}

			return sb.ToString();
		}
	}
}