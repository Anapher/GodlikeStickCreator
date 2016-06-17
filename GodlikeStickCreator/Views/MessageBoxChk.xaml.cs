using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using GodlikeStickCreator.Native;
using GodlikeStickCreator.Utilities;

namespace GodlikeStickCreator.Views
{
    /// <summary>
    ///     Interaction logic for MessageBoxChk.xaml
    /// </summary>
    public partial class MessageBoxChk
    {
        private MessageBoxChk(Window owner)
        {
            InitializeComponent();
            if (owner != null)
            {
                Owner = owner;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            SourceInitialized += OnSourceInitialized;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Loaded -= OnLoaded;
            switch (DefaultResult)
            {
                case MessageBoxResult.OK:
                    OkButton.Focus();
                    break;
                case MessageBoxResult.Cancel:
                    CancelButton.Focus();
                    break;
                case MessageBoxResult.Yes:
                    YesButton.Focus();
                    break;
                case MessageBoxResult.No:
                    NoButton.Focus();
                    break;
            }
        }

        private void OnSourceInitialized(object sender, EventArgs eventArgs)
        {
            SourceInitialized -= OnSourceInitialized;
            this.RemoveIcon();
        }

        public string Message { get; private set; }
        public string CheckBoxText { get; private set; }
        public bool CheckBoxResult { get; private set; }
        public MessageBoxButton MessageBoxButtons { get; private set; }
        public MessageBoxResult DefaultResult { get; private set; }
        public BitmapSource Image { get; private set; }
        public MessageBoxResult MessageBoxResult { get; private set; }

        public static MessageBoxResult Show(Window owner, string message, string title, string checkBoxText, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, MessageBoxResult defaultResult, ref bool isChecked)
        {
            var messageBox = new MessageBoxChk(owner);
            messageBox.Title = title;
            messageBox.Message = message;
            messageBox.CheckBoxText = checkBoxText;
            messageBox.CheckBoxResult = isChecked;
            messageBox.MessageBoxButtons = messageBoxButton;
            messageBox.Image = GetMessageBoxImage(messageBoxImage);
            messageBox.DefaultResult = defaultResult;
            messageBox.ShowDialog();

            isChecked = messageBox.CheckBoxResult;
            return messageBox.MessageBoxResult;
        }

        private static BitmapSource GetMessageBoxImage(MessageBoxImage messageBoxImage)
        {
            SHSTOCKICONID iconId;
            switch (messageBoxImage)
            {
                case MessageBoxImage.None:
                    return null;
                case MessageBoxImage.Error:
                    iconId = SHSTOCKICONID.SIID_ERROR;
                    break;
                case MessageBoxImage.Question:
                    iconId = SHSTOCKICONID.SIID_HELP;
                    break;
                case MessageBoxImage.Exclamation:
                    iconId = SHSTOCKICONID.SIID_WARNING;
                    break;
                case MessageBoxImage.Asterisk:
                    iconId = SHSTOCKICONID.SIID_INFO;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageBoxImage), messageBoxImage, null);
            }

            var sii = new SHSTOCKICONINFO {cbSize = (uint) Marshal.SizeOf(typeof (SHSTOCKICONINFO))};

            Marshal.ThrowExceptionForHR(NativeMethods.SHGetStockIconInfo(iconId, SHGSI.SHGSI_ICON, ref sii));

            Icon icon = System.Drawing.Icon.FromHandle(sii.hIcon);
            var bs = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            NativeMethods.DestroyIcon(sii.hIcon);
            return bs;
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult = MessageBoxResult.OK;
            DialogResult = true;
        }

        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult = MessageBoxResult.Yes;
            DialogResult = true;
        }

        private void NoButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult = MessageBoxResult.No;
            DialogResult = true;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult = MessageBoxResult.Cancel;
            DialogResult = true;
        }
    }
}