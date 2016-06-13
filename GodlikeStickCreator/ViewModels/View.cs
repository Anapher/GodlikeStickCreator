using System;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.ViewModelBase;

namespace GodlikeStickCreator.ViewModels
{
    public abstract class View : PropertyChangedBase, IDisposable
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

        public virtual void Dispose()
        {
        }

        public event EventHandler GoForwardChanged;
    }
}