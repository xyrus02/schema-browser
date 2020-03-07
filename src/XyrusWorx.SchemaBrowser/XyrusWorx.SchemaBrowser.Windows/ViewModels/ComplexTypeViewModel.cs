using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx.Runtime;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;
using XyrusWorx.Windows.ViewModels;

namespace XyrusWorx.SchemaBrowser.Windows.ViewModels
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class ComplexTypeViewModel : ViewModel<ComplexTypeModel>, IPropertyContainerViewModel
    {
        private readonly IServiceLocator mServices;
        private readonly Func<object, bool> mIsLast;

        public ComplexTypeViewModel(IServiceLocator services, HashSet<XName> typeStack, ComplexTypeModel model, Func<object, bool> isLast)
        {
            mServices = services;
            Model = model;
            mIsLast = isLast;

            typeStack.Add(model.TypeName);
            Children = model.PropertyGroups.Select(x => (IPropertyContainerViewModel)new PropertyGroupViewModel(mServices, typeStack, x, item => Children.Last() == item)).ToArray();
        }
        
        public bool IsLast => mIsLast(this);

        public string DisplayName => Model.DisplayName;
        public string Namespace => Model.TypeName.NamespaceName;

        public bool HasAnnotation => !string.IsNullOrWhiteSpace(Annotation);
        public string Annotation => Model.Annotation;
        
        public IPropertyContainerViewModel[] Children { get; }
    }
}