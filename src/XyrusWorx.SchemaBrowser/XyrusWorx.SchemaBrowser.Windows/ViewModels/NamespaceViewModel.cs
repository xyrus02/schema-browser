using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using XyrusWorx.Windows.ViewModels;

namespace XyrusWorx.SchemaBrowser.Windows.ViewModels
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class NamespaceViewModel : ViewModel, IHierarchyViewModel
    {
        public NamespaceViewModel(string name, IEnumerable<ComplexTypeViewModel> types)
        {
            DisplayName = name;
            Children = types.OfType<IHierarchyViewModel>().ToArray();
            IsEmpty = !Children.Any();
        }
        
        public string DisplayName { get; }
        public bool IsEmpty { get; }
        
        public IHierarchyViewModel[] Children { get; }
        public IHierarchyViewModel[] ComplexChildren => Children;
    }
}