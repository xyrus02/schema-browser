namespace XyrusWorx.SchemaBrowser.Windows.ViewModels
{
    public interface IPropertyContainerViewModel
    {
        string DisplayName { get; }
        IPropertyContainerViewModel[] Children { get; }
    }
}