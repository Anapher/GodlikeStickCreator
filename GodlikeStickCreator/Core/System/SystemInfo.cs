using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace GodlikeStickCreator.Core.System
{
    public class SystemInfo : INotifyPropertyChanged
    {
        private bool _isAdded;

        public string Name { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public Category Category { get; set; }
        public InstallMethod InstallMethod { get; set; }
        public string DownloadUrl { get; set; }
        public BitmapImage Thumbnail { get; set; }
        public SpecialSnowflake SpecialSnowflake { get; set; }
        public string IsoFileMatch { get; set; }
        public string Filename { get; set; }

        public bool IsAdded
        {
            get { return _isAdded; }
            set
            {
                if (_isAdded != value)
                {
                    _isAdded = value;
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

    public enum Category
    {
        [Description("Linux Distributions")] LinuxDistributions,
        [Description("System Tools")] SystemTools,
        [Description("Antivirus Tools")] AntiVirus,
        [Description("Multimedia")] Multimedia,
        [Description("Other")] Other
    }
}