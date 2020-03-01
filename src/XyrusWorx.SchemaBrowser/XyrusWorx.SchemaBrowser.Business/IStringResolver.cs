using System;
using JetBrains.Annotations;
using XyrusWorx;

namespace XyrusWorx.SchemaBrowser.Business
{
	public interface IStringResolver 
	{
		[NotNull]
		IFormatProvider FormatProvider { get; }
		
		[CanBeNull]
		string Resolve(StringKey key);
	}

}