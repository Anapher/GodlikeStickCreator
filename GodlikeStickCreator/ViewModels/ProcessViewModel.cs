using GodlikeStickCreator.Controls;
using GodlikeStickCreator.Core;

namespace GodlikeStickCreator.ViewModels
{
    public class ProcessViewModel : View
    {
        private double _currentProgress;
        private string _message;

        public ProcessViewModel(UsbStickSettings usbStickSettings) : base(usbStickSettings)
        {
            Logger = new Logger();
        }

        public double CurrentProgress
        {
            get { return _currentProgress; }
            set { SetProperty(value, ref _currentProgress); }
        }

        public string Message
        {
            get { return _message; }
            set { SetProperty(value, ref _message); }
        }

        public Logger Logger { get; }
    }
}