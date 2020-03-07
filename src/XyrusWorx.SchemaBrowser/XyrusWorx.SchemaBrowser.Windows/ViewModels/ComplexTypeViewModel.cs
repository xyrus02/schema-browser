using JetBrains.Annotations;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;
using XyrusWorx.Windows.ViewModels;

namespace XyrusWorx.SchemaBrowser.Windows.ViewModels
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class ComplexTypeViewModel : ViewModel<ComplexTypeModel>
    {
        public ComplexTypeViewModel(ComplexTypeModel model)
        {
            Model = model;
        }

        public string DisplayName => Model.DisplayName;
        public string Namespace => Model.TypeName.NamespaceName;

        public bool HasAnnotation => !string.IsNullOrWhiteSpace(Annotation);
        public string Annotation => Model.Annotation;
    }
}