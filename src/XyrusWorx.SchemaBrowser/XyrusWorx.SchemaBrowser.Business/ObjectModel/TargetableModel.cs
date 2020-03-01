using System;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel 
{
	public abstract class TargetableModel
	{
		public Guid Uuid { get; } = Guid.NewGuid();
		
		public abstract string DisplayName { get; }
	}
}