using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using XyrusWorx;
using XyrusWorx.Collections;

namespace XyrusWorx.SchemaBrowser.Business
{
	public class LocalizationService
	{
		private readonly Dictionary<StringKey, IStringResolver> mOutputLanguages = new Dictionary<StringKey, IStringResolver>();

		[NotNull]
		public IEnumerable<CultureInfo> GetSupportedOutputCultures()
		{
			yield return CultureInfo.InvariantCulture;

			var re = new Regex(@"\.([a-z]{2}(?:\-[A-Z]{2})?)\.xml$");
			
			foreach (var item in typeof(LocalizationService).Assembly.GetManifestResourceNames())
			{
				var m = re.Match(item);
				if (m.Success)
				{
					yield return CultureInfo.GetCultureInfoByIetfLanguageTag(m.Groups[1].Value);
				}
			}
		}
		
		[NotNull]
		public IStringResolver GetOutputLanguage([NotNull] string ietfTag) 
			=> GetOutputLanguage(CultureInfo.GetCultureInfoByIetfLanguageTag(ietfTag));
		
		[NotNull]
		public IStringResolver GetOutputLanguage(CultureInfo cultureInfo)
		{
			if (Equals(cultureInfo, CultureInfo.InvariantCulture))
			{
				return OutputLanguage.GetInvariant();
			}
			
			if (mOutputLanguages.ContainsKey(cultureInfo.IetfLanguageTag))
			{
				return mOutputLanguages[cultureInfo.IetfLanguageTag];
			}
			
			var language = GetOutputLanguageOverride(cultureInfo);
			mOutputLanguages.AddOrUpdate(cultureInfo.IetfLanguageTag, language);
			return language;
		}

		[NotNull]
		protected virtual IStringResolver GetOutputLanguageOverride([NotNull] CultureInfo cultureInfo) => new OutputLanguage(cultureInfo);
	}

}
