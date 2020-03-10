namespace XyrusWorx.SchemaBrowser.Windows.ViewModels
{
    public interface IHierarchyViewModel
    {
        string DisplayName { get; }
        bool IsEmpty { get; }
        
        IHierarchyViewModel[] Children { get; }
        IHierarchyViewModel[] ComplexChildren { get; }
    }
}