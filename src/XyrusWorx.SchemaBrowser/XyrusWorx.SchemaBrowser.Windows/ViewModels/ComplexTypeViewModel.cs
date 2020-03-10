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
    public sealed class ComplexTypeViewModel : ViewModel<ComplexTypeModel>, IHierarchyViewModel
    {
        private readonly IServiceLocator mServices;
        private readonly Func<object, bool> mIsLast;

        public ComplexTypeViewModel(IServiceLocator services, HashSet<XName> typeStack, ComplexTypeModel model, Func<object, bool> isLast, bool isRoot)
        {
            mServices = services;
            Model = model;
            mIsLast = isLast;

            typeStack.Add(model.TypeName);
            Children = model.PropertyGroups.Select(x => (IHierarchyViewModel)new PropertyGroupViewModel(mServices, typeStack, x, item => Children.Last() == item)).ToArray();
        }
        
        public bool IsLast => mIsLast(this);
        public bool IsEmpty => !Children.Any();
        public bool IsRoot { get; }

        public string DisplayName => Model.DisplayName;
        public string Namespace => Model.TypeName.NamespaceName;

        public bool HasAnnotation => !string.IsNullOrWhiteSpace(Annotation);
        public string Annotation => Model.Annotation;
        
        public IHierarchyViewModel[] Children { get; }
        IHierarchyViewModel[] IHierarchyViewModel.ComplexChildren { get; } = new IHierarchyViewModel[0];
    }
}