using System;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business
{
	[PublicAPI]
	public class XmlIndexEntry
	{
		public XmlIndexEntry(XName name, [NotNull] XElement element)
		{
			Name = name;
			Element = element ?? throw new ArgumentNullException(nameof(element));
		}

		public XName Name { get; }
		public XElement Element { get; }
	}
}