using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	public class AbstractTypeModel
	{
		private readonly ComplexTypeModel mType;

		public AbstractTypeModel([NotNull] ComplexTypeModel type, IEnumerable<ComplexTypeModel> implementations = null)
		{
			mType = type ?? throw new ArgumentNullException(nameof(type));
			Implementations = new ReadOnlyCollection<ComplexTypeModel>(ImplementationList = new List<ComplexTypeModel>(implementations ?? new ComplexTypeModel[0]));
		}

		[NotNull]
		public ComplexTypeModel TypeModel => mType;
		
		[NotNull]
		public IReadOnlyList<ComplexTypeModel> Implementations { get; }
		
		[NotNull]
		internal List<ComplexTypeModel> ImplementationList { get; }
	}
}
