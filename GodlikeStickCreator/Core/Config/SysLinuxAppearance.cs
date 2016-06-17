using System.Windows.Media;
using System.Xml.Serialization;
using GodlikeStickCreator.Utilities;

namespace GodlikeStickCreator.Core.Config
{
    public class SysLinuxAppearance
    {
        public SysLinuxAppearance()
        {
            ScreenTitle = "Godlike Stick";
            ScreenMessage = "What is Dead May Never Die";
            ScreenBackground = string.Empty;
            ReturnToMainMenuText = "Return to the main menu.";
            TitleForegroundColor = Color.FromRgb(190, 190, 190);
            TitleBackgroundColor = Color.FromArgb(0, 0, 0, 0);
            SelectedBackgroundColor = Color.FromRgb(209, 209, 209);
            SelectedForegroundColor = Color.FromRgb(0, 0, 0);
            BorderForegroundColor = Color.FromArgb(180, 190, 190, 190);
            BorderBackgroundColor = Color.FromArgb(0, 0, 0, 0);
            TabMsgForegroundColor = Color.FromRgb(190, 190, 190);
            UnselectedForegroundColor = Color.FromRgb(190, 190, 190);
            UnselectedBackgroundColor = Color.FromArgb(0, 0, 0, 0);
        }

        public string ScreenTitle { get; set; }
        public string ScreenMessage { get; set; }
        public string ScreenBackground { get; set; }
        public string ReturnToMainMenuText { get; set; }

        [XmlIgnore]
        public Color TitleForegroundColor { get; set; }

        [XmlIgnore]
        public Color TitleBackgroundColor { get; set; }

        [XmlIgnore]
        public Color SelectedBackgroundColor { get; set; }

        [XmlIgnore]
        public Color SelectedForegroundColor { get; set; }

        [XmlIgnore]
        public Color BorderBackgroundColor { get; set; }

        [XmlIgnore]
        public Color BorderForegroundColor { get; set; }

        [XmlIgnore]
        public Color TabMsgForegroundColor { get; set; }

        [XmlIgnore]
        public Color UnselectedBackgroundColor { get; set; }

        [XmlIgnore]
        public Color UnselectedForegroundColor { get; set; }

        [XmlIgnore]
        public Color ScrollBarBackgroundColor { get; set; }

        [XmlIgnore]
        public Color ScrollBarForegroundColor { get; set; }

        [XmlElement("TitleForegroundColor")]
        public int TitleForegroundColorAsArgb
        {
            get { return TitleForegroundColor.ToInt(); }
            set { TitleForegroundColor = value.ToColor(); }
        }

        [XmlElement("TitleBackgroundColor")]
        public int TitleBackgroundColorAsArgb
        {
            get { return TitleBackgroundColor.ToInt(); }
            set { TitleBackgroundColor = value.ToColor(); }
        }

        [XmlElement("SelectedForegroundColor")]
        public int SelectedForegroundColorAsArgb
        {
            get { return SelectedForegroundColor.ToInt(); }
            set { SelectedForegroundColor = value.ToColor(); }
        }

        [XmlElement("SelectedBackgroundColor")]
        public int SelectedBackgroundColorAsArgb
        {
            get { return SelectedBackgroundColor.ToInt(); }
            set { SelectedBackgroundColor = value.ToColor(); }
        }

        [XmlElement("BorderForegroundColor")]
        public int BorderForegroundColorAsArgb
        {
            get { return BorderForegroundColor.ToInt(); }
            set { BorderForegroundColor = value.ToColor(); }
        }

        [XmlElement("BorderBackgroundColor")]
        public int BorderBackgroundColorAsArgb
        {
            get { return BorderBackgroundColor.ToInt(); }
            set { BorderBackgroundColor = value.ToColor(); }
        }

        [XmlElement("TabMsgForegroundColor")]
        public int TabMsgForegroundColorAsArgb
        {
            get { return TabMsgForegroundColor.ToInt(); }
            set { TabMsgForegroundColor = value.ToColor(); }
        }

        [XmlElement("UnselectedForegroundColor")]
        public int UnselectedForegroundColorAsArgb
        {
            get { return UnselectedForegroundColor.ToInt(); }
            set { UnselectedForegroundColor = value.ToColor(); }
        }

        [XmlElement("UnselectedBackgroundColor")]
        public int UnselectedBackgroundColorAsArgb
        {
            get { return UnselectedBackgroundColor.ToInt(); }
            set { UnselectedBackgroundColor = value.ToColor(); }
        }

        [XmlElement("ScrollBarForegroundColor")]
        public int ScrollBarForegroundColorAsArgb
        {
            get { return ScrollBarForegroundColor.ToInt(); }
            set { ScrollBarForegroundColor = value.ToColor(); }
        }

        [XmlElement("UnselectedBackgroundColor")]
        public int ScrollBarBackgroundColorAsArgb
        {
            get { return ScrollBarBackgroundColor.ToInt(); }
            set { ScrollBarBackgroundColor = value.ToColor(); }
        }
    }
}