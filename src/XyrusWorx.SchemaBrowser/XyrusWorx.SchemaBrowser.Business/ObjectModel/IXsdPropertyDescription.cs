﻿namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	public interface IXsdPropertyDescription : IXsdModel
	{
		string PropertyName { get; }
		string Annotation { get; }

		IXsdTypeDescription DataType { get; }

		uint MinOccurs { get; }
		uint MaxOccurs { get; }

		bool IsAttribute { get; }
		bool IsNillable { get; }
		bool IsContent { get; }
		bool IsMandatory { get; }
	}
}