namespace XyrusWorx.SchemaBrowser.Business.ObjectModel
{
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