using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using Application = System.Windows.Application;

namespace GodlikeStickCreator.Controls
{
    /// <summary>
    ///     Interaction logic for LogControl.xaml
    /// </summary>
    public partial class LogControl
    {
        public static readonly DependencyProperty LoggerProperty = DependencyProperty.Register(
            "Logger", typeof (Logger), typeof (LogControl),
            new PropertyMetadata(default(Logger), PropertyChangedCallback));

        private Paragraph _paragraph;

        public LogControl()
        {
            InitializeComponent();
            _paragraph = new Paragraph();
            MainRichTextBox.Document.Blocks.Add(_paragraph);
        }

        public Logger Logger
        {
            get { return (Logger) GetValue(LoggerProperty); }
            set { SetValue(LoggerProperty, value); }
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as LogControl;
            control?.LoggerChanged((Logger) dependencyPropertyChangedEventArgs.NewValue,
                (Logger) dependencyPropertyChangedEventArgs.OldValue);
        }

        private void LoggerChanged(Logger logger, Logger oldLogger)
        {
            if (oldLogger != null)
                oldLogger.NewLogMessage -= BuildLoggerOnNewLogMessage;
            logger.NewLogMessage += BuildLoggerOnNewLogMessage;
            _paragraph.Inlines.Clear();
        }

        private void BuildLoggerOnNewLogMessage(object sender, NewLogMessageEventArgs e)
        {
            Dispatcher.BeginInvoke(
                (MethodInvoker)
                    delegate
                    {
                        Brush foreground;
                        string prefix;

                        switch (e.LogType)
                        {
                            case LogType.Status:
                                foreground = (Brush) Application.Current.Resources["BlackBrush"];
                                prefix = (string) Application.Current.Resources["Status"];
                                break;
                            case LogType.Warning:
                                foreground = new SolidColorBrush(Color.FromArgb(255, 231, 76, 60));
                                prefix = (string) Application.Current.Resources["Warning"];
                                break;
                            case LogType.Error:
                                foreground = new SolidColorBrush(Color.FromArgb(255, 192, 57, 43));
                                prefix = (string) Application.Current.Resources["Error"];
                                break;
                            case LogType.Success:
                                foreground = new SolidColorBrush(Color.FromArgb(255, 39, 174, 96));
                                prefix = (string) Application.Current.Resources["Success"];
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        _paragraph.Inlines.Add(
                            new Run($"[{prefix.ToUpper()}] \t" + e.Content + "\r\n")
                            {
                                Foreground = foreground
                            });

                        MainRichTextBox.ScrollToEnd();
                    });
        }
    }
}