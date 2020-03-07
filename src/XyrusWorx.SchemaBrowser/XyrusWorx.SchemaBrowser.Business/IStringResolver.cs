using System;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business
{
	[PublicAPI]
	public interface IStringResolver 
	{
		[NotNull]
		IFormatProvider FormatProvider { get; }
		
		[CanBeNull]
		string Resolve(StringKey key);
	}

}