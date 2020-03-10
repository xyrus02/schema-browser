using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using XyrusWorx.Windows.ViewModels;

namespace XyrusWorx.SchemaBrowser.Windows.ViewModels
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class HierarchyRootViewModel : CollectionViewModel<IHierarchyViewModel>
    {
        public HierarchyRootViewModel()
        {
            var oc = new ObservableCollection<IHierarchyViewModel>();
            oc.CollectionChanged += (o, e) => NotifyChange(nameof(HasItems));
            
            Items = oc;
            Selection = new SelectionViewModel<IHierarchyViewModel>(this);
        }
        
        public SelectionViewModel<IHierarchyViewModel> Selection { get; }
		
        public override IList<IHierarchyViewModel> Items { get; }
    }
}