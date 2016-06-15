using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GodlikeStickCreator.ViewModels;

namespace GodlikeStickCreator.Views
{
    public class ViewManager : IValueConverter
    {
        private static ReadOnlyDictionary<Type, Type> _viewsViewModels;
        private readonly Dictionary<Type, FrameworkElement> _cachedViews;

        public ViewManager()
        {
            _cachedViews = new Dictionary<Type, FrameworkElement>();

            if (_viewsViewModels == null)
                _viewsViewModels = new ReadOnlyDictionary<Type, Type>(new Dictionary<Type, Type>
                {
                    {typeof (SelectDriveViewModel), typeof (SelectDriveView)},
                    {typeof (SystemsViewModel), typeof (SystemsView)},
                    {typeof (ApplicationsViewModel), typeof (ApplicationsView)},
                    {typeof (ModifyAppearanceViewModel), typeof (ModifyAppearanceView)},
                    {typeof (ProcessViewModel), typeof (ProcessView)},
                    {typeof (SucceededView), typeof (SucceededViewModel)}
                });
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var type = value.GetType();

            if (_viewsViewModels.ContainsKey(type))
                return GetView(type, value, _viewsViewModels[type]);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private FrameworkElement GetView(Type viewModelType, object viewModel, Type viewType)
        {
            if (_cachedViews.ContainsKey(viewModelType))
            {
                var result = _cachedViews[viewModelType];
                result.DataContext = viewModel;
                return result;
            }

            var view = (FrameworkElement) Activator.CreateInstance(viewType);
            view.DataContext = viewModel;

            _cachedViews.Add(viewModelType, view);
            return view;
        }
    }
}