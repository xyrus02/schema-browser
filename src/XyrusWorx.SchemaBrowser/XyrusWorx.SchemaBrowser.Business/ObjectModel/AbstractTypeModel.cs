using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	[PublicAPI]
	public class AbstractTypeModel
	{
		public AbstractTypeModel([NotNull] ComplexTypeModel type, IEnumerable<ComplexTypeModel> implementations = null)
		{
			TypeModel = type ?? throw new ArgumentNullException(nameof(type));
			Implementations = new ReadOnlyCollection<ComplexTypeModel>(ImplementationList = new List<ComplexTypeModel>(implementations ?? new ComplexTypeModel[0]));
		}

		[NotNull]
		public ComplexTypeModel TypeModel { get; }

		[NotNull]
		public IReadOnlyList<ComplexTypeModel> Implementations { get; }
		
		[NotNull]
		internal List<ComplexTypeModel> ImplementationList { get; }
	}
}
