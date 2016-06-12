using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GodlikeStickCreator.Core.Applications
{
    public class ApplicationInfo : INotifyPropertyChanged
    {
        private bool _add;
        public string Name { get; set; }
        public string Description { get; set; }
        public Lazy<string> DownloadUrl { get; set; }
        public ApplicationCategory ApplicationCategory { get; set; }

        public bool Add
        {
            get { return _add; }
            set
            {
                if (_add != value)
                {
                    _add = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum ApplicationCategory
    {
        [Description("Decompiler")]
        Decompiler,
        [Description("Editors")]
        Editors,
        [Description("File Tools")]
        FileTools,
        [Description("Information Tools")]
        InformationTools,
        [Description("Multimedia")]
        Multimedia,
        [Description("Network")]
        Network,
        [Description("Programming")]
        Programming,
        [Description("System Tools")]
        SystemTools
    }
}