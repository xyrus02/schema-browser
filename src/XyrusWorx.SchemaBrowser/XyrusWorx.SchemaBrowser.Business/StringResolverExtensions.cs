namespace XyrusWorx.SchemaBrowser.Business
{
	public static class StringResolverExtensions
	{
		public static string Format(this IStringResolver instance, StringKey key, params object[] values)
		{
			var formatString = instance?.Resolve(key);
			if (string.IsNullOrWhiteSpace(formatString))
			{
				return formatString;
			}

			var result = string.Format(instance.FormatProvider, formatString, values ?? new object[0]);

			return result;
		}
	}
}