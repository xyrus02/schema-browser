﻿using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel 
{
	[PublicAPI]
	public class PropertyModel : IAnnotableModel, IEnumerable<IXsdPropertyGroupDescription>, IXsdPropertyDescription
	{
		public PropertyModel([NotNull] string propertyName)
		{
			PropertyName = propertyName.NormalizeNull() ?? throw new ArgumentNullException(nameof(propertyName));
		}
		
		[NotNull]
		public string PropertyName { get; }
		
		public IXsdTypeDescription DataType { get; set; }
		
		public uint MinOccurs { get; set; }
		
		public uint MaxOccurs { get; set; }
		
		public bool IsAttribute { get; set; }
		
		public bool IsNillable { get; set; }
		
		public bool IsContent { get; set; }

		public bool IsMandatory => !IsNillable && MinOccurs > 0;
		
		public string Annotation { get; set; }

		IEnumerator<IXsdPropertyGroupDescription> IEnumerable<IXsdPropertyGroupDescription>.GetEnumerator() => DataType.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => DataType.GetEnumerator();
	}
}