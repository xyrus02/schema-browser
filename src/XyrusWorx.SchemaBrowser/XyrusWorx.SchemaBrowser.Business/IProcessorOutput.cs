using System.Collections.Generic;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business 
{
	public interface IProcessorOutput 
	{
		IStringResolver OutputLanguage { get; }
		
		IReadOnlyList<ComplexTypeModel> RootModels { get; }
		IReadOnlyList<SimpleTypeModel> SimpleTypes { get; }
		IReadOnlyList<AbstractTypeModel> AbstractTypes { get; }
	}
}