using System;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	[PublicAPI]
	public class SystemTypeSpecificationModel : ISimpleTypeSpecificationModel
	{
		public SystemTypeSpecificationModel([NotNull] SimpleTypeModel typeInstance, [NotNull] IStringResolver stringResolver)
		{
			if (stringResolver == null)
			{
				throw new ArgumentNullException(nameof(stringResolver));
			}

			typeInstance = typeInstance ?? throw new ArgumentNullException(nameof(typeInstance));
			Constraint = SimpleTypeConstraint.Create(typeInstance, stringResolver) ?? new TextConstraint(typeInstance, stringResolver);
		}
		
		[NotNull]
		public SimpleTypeConstraint Constraint { get; }
	}
}