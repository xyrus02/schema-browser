namespace XyrusWorx.SchemaBrowser.Windows.ViewModels
{
    public interface IPropertyContainerViewModel
    {
        string DisplayName { get; }
        bool IsEmpty { get; }
        
        IPropertyContainerViewModel[] Children { get; }
    }
}