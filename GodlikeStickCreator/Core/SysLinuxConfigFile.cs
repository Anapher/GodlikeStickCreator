using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using GodlikeStickCreator.Core.Config;
using GodlikeStickCreator.Core.System;
using GodlikeStickCreator.Utilities;

namespace GodlikeStickCreator.Core
{
    public class SysLinuxConfigFile
    {
        private readonly BootStickConfig _bootStickConfig;
        private readonly string _filename;
        private string _content;

        private SysLinuxConfigFile(string filename, BootStickConfig bootStickConfig)
        {
            _filename = filename;
            _bootStickConfig = bootStickConfig;
        }

        private string GetDefaultHeader()
        {
            return
                $@"UI vesamenu.c32
TIMEOUT 300
MENU RESOLUTION 640 480
MENU TITLE {_bootStickConfig.ScreenTitle}
MENU BACKGROUND background.png
MENU TABMSG  {_bootStickConfig
                    .ScreenMessage}
MENU CLEAR

MENU WIDTH 72
MENU MARGIN 10
MENU VSHIFT 3
MENU HSHIFT 6
MENU ROWS 15
MENU TABMSGROW 20
MENU TIMEOUTROW 22

MENU COLOR title        1;36;44 {ColorToHex
                        (_bootStickConfig.TitleForegroundColor)} {ColorToHex(_bootStickConfig.TitleBackgroundColor)} none
MENU COLOR hotsel       30;47   #2980b9 #DDDDDDDD
MENU COLOR sel          30;47   {ColorToHex
                            (_bootStickConfig.SelectedForegroundColor)} {ColorToHex(
                                _bootStickConfig.SelectedBackgroundColor)}
MENU COLOR border       30;44	{ColorToHex(
                                    _bootStickConfig.BorderForegroundColor)} {ColorToHex(
                                        _bootStickConfig.BorderBackgroundColor)} std
MENU COLOR scrollbar    30;44   {ColorToHex
                                            (_bootStickConfig.ScrollBarForegroundColor)} {ColorToHex(
                                                _bootStickConfig.ScrollBarBackgroundColor)} none
MENU COLOR tabmsg       31;40   {ColorToHex
                                                    (_bootStickConfig.TabMsgForegroundColor)} #00000000 std
MENU COLOR unsel	    37;44   {ColorToHex
                                                        (_bootStickConfig.UnselectedForegroundColor)} {ColorToHex(
                                                            _bootStickConfig.UnselectedBackgroundColor)} std";
        }

        private void CreateNew()
        {
            var configString =
                $@"# <Header>
{GetDefaultHeader()}
# </Header>

LABEL Boot from first Hard Drive
MENU LABEL Continue to Boot from ^First HD (default)
KERNEL chain.c32
APPEND hd1
MENU DEFAULT";
            File.WriteAllText(_filename, configString);
            Load(configString);
        }

        private string ColorToHex(Color c)
        {
            // ReSharper disable once UseStringInterpolation
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B);
        }

        private void Load(string content)
        {
            var contentHeader =
                Regex.Match(content, @"# <Header>\s*(?<header>(.*?))\s*# </Header>", RegexOptions.Singleline).Groups[
                    "header"].Value;
            var defaultHeader = GetDefaultHeader();
            if (contentHeader != defaultHeader)
                content = content.Replace(contentHeader, defaultHeader);

            _content = content;
        }

        public void AppendCategoryIfNotExists(Category category)
        {
            if (!_content.Contains($"MENU BEGIN {category}"))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"MENU BEGIN {category}");
                stringBuilder.AppendLine($"MENU TITLE {EnumUtilities.GetDescription(category)}");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"\t# {category}:nextEntry");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("\tMENU SEPARATOR");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"\tLABEL exit_{category}");
                stringBuilder.AppendLine($"\tMENU LABEL {_bootStickConfig.ReturnToMainMenuText}");
                stringBuilder.AppendLine("\tMENU EXIT");
                stringBuilder.AppendLine("MENU END");

                _content += "\r\n\r\n" + stringBuilder;
            }
        }

        public bool ContainsSystem(SystemInfo systemInfo)
        {
            return _content.Contains($"# {systemInfo.Name}");
        }

        public string GetSystemDirectory(SystemInfo systemInfo)
        {
            return
                Regex.Match(_content, $@"# <{systemInfo.Name.Replace(" ", null)} directory=""(?<directoryName>(.*?))"">")
                    .Groups["directoryName"].Value;
        }

        public void AddSystem(SystemInfo systemInfo, MenuItemInfo menuItemInfo, string directoryName)
        {
            AppendCategoryIfNotExists(systemInfo.Category);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"\t# <{systemInfo.Name.Replace(" ", null)} directory=\"{directoryName}\">");
            if (!menuItemInfo.Raw)
            {
                stringBuilder.AppendLine($"\t\tLABEL {systemInfo.Name}");
                stringBuilder.AppendLine($"\t\tMENU LABEL {systemInfo.Name}");
                stringBuilder.AppendLine("\t\tMENU INDENT 1");
            }

            foreach (var line in menuItemInfo.Text.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries))
                stringBuilder.AppendLine("\t\t" + line);

            stringBuilder.AppendLine($"\t# </{systemInfo.Name.Replace(" ", null)}>");
            stringBuilder.AppendLine();

            var nextItemEntry = $"\t# {systemInfo.Category}:nextEntry";
            stringBuilder.Append(nextItemEntry);

            _content = _content.Replace(nextItemEntry, stringBuilder.ToString());
        }

        public void RemoveSystem(SystemInfo systemInfo)
        {
            var openTag = $"\t# <{systemInfo.Name.Replace(" ", null)}>";
            var closeTag = $"\t# </{systemInfo.Name.Replace(" ", null)}>";

            var beginIndex = _content.IndexOf(openTag, StringComparison.InvariantCulture);
            var endIndex = _content.IndexOf(closeTag, StringComparison.InvariantCulture) + closeTag.Length;
            _content = _content.Remove(beginIndex, endIndex - beginIndex);
        }

        public void Save()
        {
            File.WriteAllText(_filename, _content);
        }

        public static SysLinuxConfigFile Create(string filename, BootStickConfig bootStickConfig)
        {
            var configFile = new SysLinuxConfigFile(filename, bootStickConfig);
            configFile.CreateNew();
            return configFile;
        }

        public static SysLinuxConfigFile OpenFile(string filename, BootStickConfig bootStickConfig)
        {
            var configFile = new SysLinuxConfigFile(filename, bootStickConfig);
            configFile.Load(File.ReadAllText(filename));
            return configFile;
        }
    }

    public class MenuItemInfo
    {
        public MenuItemInfo(string text)
        {
            Text = text;
        }

        public MenuItemInfo(string text, bool raw)
        {
            Text = text;
            Raw = raw;
        }

        public string Text { get; }
        public bool Raw { get; set; }
    }
}