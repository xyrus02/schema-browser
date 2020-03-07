using JetBrains.Annotations;

namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
	[PublicAPI]
	public class AnnotatedValue : IAnnotableModel
	{
		public AnnotatedValue(string value = null)
		{
			Value = value;
		}
		
		public string Value { get; }
		
		public string Annotation { get; set; }
	}
}