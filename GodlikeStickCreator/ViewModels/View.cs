using System;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.ViewModelBase;

namespace GodlikeStickCreator.ViewModels
{
    public abstract class View : PropertyChangedBase
    {
        private bool _canGoForward = true;

        protected View(UsbStickSettings usbStickSettings)
        {
            UsbStickSettings = usbStickSettings;
        }

        public UsbStickSettings UsbStickSettings { get; }

        public bool CanGoForward
        {
            get { return _canGoForward; }
            set
            {
                if (SetProperty(value, ref _canGoForward))
                    GoForwardChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler GoForwardChanged;
    }
}