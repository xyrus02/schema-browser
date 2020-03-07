using System;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel 
{
	[PublicAPI]
	public abstract class TargetableModel
	{
		public Guid Uuid { get; } = Guid.NewGuid();
		
		public abstract string DisplayName { get; }
	}
}