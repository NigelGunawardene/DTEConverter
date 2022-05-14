using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTEConverter
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MyAppContext());
        }
    }

    internal class MyAppContext : ApplicationContext
    {
        private NotifyIcon _notifyIcon;

        public MyAppContext()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<string> resourceNames = new List<string>(assembly.GetManifestResourceNames());

            #region Create Menu Items
            var exitMenuItem = new ToolStripMenuItem("Exit", null, OnExitClick);
            #endregion

            #region Create Menu and add Items
            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add(exitMenuItem);
            #endregion


            #region Notify Icon

            var resourcePath = GetType().Assembly.GetManifestResourceNames().First(name => name.EndsWith("dte.ico"));
            Stream iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);


            var icon = new Icon(iconStream);
            _notifyIcon = new NotifyIcon
            {
                Icon = icon,
                Visible = true,
                ContextMenuStrip = contextMenuStrip
            };
            #endregion
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}
