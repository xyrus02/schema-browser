using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using XyrusWorx.Windows.ViewModels;

namespace XyrusWorx.SchemaBrowser.Windows.ViewModels
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SchemaCollectionViewModel : CollectionViewModel<ComplexTypeViewModel>
    {
        public SchemaCollectionViewModel()
        {
            var oc = new ObservableCollection<ComplexTypeViewModel>();
            oc.CollectionChanged += (o, e) => NotifyChange(nameof(HasItems));
            
            Items = oc;
            Selection = new SelectionViewModel<ComplexTypeViewModel>(this);
        }
        
        public SelectionViewModel<ComplexTypeViewModel> Selection { get; }
		
        public override IList<ComplexTypeViewModel> Items { get; }
    }
}