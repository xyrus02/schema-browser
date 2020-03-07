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
    public sealed class PropertyGroupViewModel : ViewModel<PropertyGroupModel>, IPropertyContainerViewModel
    {
        private readonly IServiceLocator mServices;
        private readonly Func<object, bool> mIsLast;

        public PropertyGroupViewModel(IServiceLocator services, HashSet<XName> typeStack, PropertyGroupModel model, Func<object, bool> isLast)
        {
            mServices = services;
            Model = model;
            mIsLast = isLast;

            Children = 
                new IPropertyContainerViewModel[0]
                    .Concat(model.PropertyGroups.Select(x => (IPropertyContainerViewModel)new PropertyGroupViewModel(mServices, typeStack, x, item => Children.Last() == item)))
                    .Concat(model.Properties.Values.Where(x => !typeStack.Contains(x.DataType.TypeName)).Select(x => (IPropertyContainerViewModel)new PropertyViewModel(mServices, typeStack, x, item => Children.Last() == item)))
                    .ToArray();
        }

        public bool IsLast => mIsLast(this);

        public string DisplayName => Model.GroupType switch
        {
            PropertyGroupType.Sequence => "in this order",
            PropertyGroupType.AllOf => "in any order",
            PropertyGroupType.OneOf => "one of",
            _ => "unspecified"
        };

        public bool IsVirtual => Model.GroupType == PropertyGroupType.Virtual;
        
        public IPropertyContainerViewModel[] Children { get; }
    }
}