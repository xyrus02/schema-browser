using System;
using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx.Runtime;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;
using XyrusWorx.Windows.ViewModels;

namespace XyrusWorx.SchemaBrowser.Windows.ViewModels
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class PropertyViewModel : ViewModel<PropertyModel>, IPropertyContainerViewModel
    {
        private readonly IServiceLocator mServices;
        private readonly Func<object, bool> mIsLast;
        private bool mIsExpanded;

        public PropertyViewModel(IServiceLocator services, HashSet<XName> typeStack, PropertyModel model, Func<object, bool> isLast)
        {
            mServices = services;
            Model = model;
            mIsLast = isLast;
            
            ComplexType = model.DataType is ComplexTypeModel ct && !typeStack.Contains(ct.TypeName) ? new ComplexTypeViewModel(services, typeStack, ct, isLast, false) : null;
            typeStack.Add(model.DataType.TypeName);
        }

        public bool IsLast => mIsLast(this);
        public bool IsEmpty => false;

        public string DisplayName => Model.PropertyName;
        public string DataTypeName => Model.DataType.DisplayName;
        
        public ComplexTypeViewModel ComplexType { get; }

        public string Quantifier
        {
            get
            {
                if (Model.MinOccurs == Model.MaxOccurs)
                {
                    return Model.MinOccurs.ToString();
                }

                if (Model.MaxOccurs == uint.MaxValue)
                {
                    return $"{Model.MinOccurs}..*";
                }

                return $"{Model.MinOccurs}..{Model.MaxOccurs}";
            }
        }

        public bool IsComplexType => ComplexType != null;
        public bool IsAbstractType => !Model.DataType.IsAbstract;
        
        public bool IsExpanded
        {
            get => mIsExpanded;
            set
            {
                if (value == mIsExpanded) return;
                mIsExpanded = value;
                OnPropertyChanged();
            }
        }
        
        IPropertyContainerViewModel[] IPropertyContainerViewModel.Children { get; } = new IPropertyContainerViewModel[0];
    }
}