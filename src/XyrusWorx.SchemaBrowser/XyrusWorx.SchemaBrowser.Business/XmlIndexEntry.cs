using System;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business
{
	public class XmlIndexEntry
	{
		public XmlIndexEntry(XName name, [NotNull] XElement element, int includeDepth = int.MaxValue)
		{
			Name = name;
			Element = element ?? throw new ArgumentNullException(nameof(element));
			IncludeDepth = includeDepth;
		}

		public XName Name { get; }
		public XElement Element { get; }
		public int IncludeDepth { get; }
	}
}