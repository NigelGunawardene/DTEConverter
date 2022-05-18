using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTEConverter
{
    public partial class TranslatedTextForm : Form
    {

        private string oneLineSpace = "\n";
        private string twoLineSpaces = "\n\n";
        public TranslatedTextForm(List<string> dutchPhrases, List<string> engPhrases)
        {
            InitializeComponent();
            foreach (var phrase in dutchPhrases)
            {
                DutchTextBox.Text += phrase;
                DutchTextBox.Text += oneLineSpace;
            }

            foreach (var phrase in engPhrases)
            {
                EnglishTextBox.Text += phrase;
                EnglishTextBox.Text += oneLineSpace;
            }
        }

        private void TranslatedTextForm_Load(object sender, EventArgs e)
        {

        }

        private void CloseButton_Click(object sender, EventArgs e) //TranslatedTextBox
        {
            this.Close();
        }


        [DllImport("User32.dll")]
        public extern static int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("User32.dll")]
        public extern static int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        private void DutchTextBox_vScroll(Message msg)
        {
            msg.HWnd = EnglishTextBox.Handle;
            EnglishTextBox.PubWndProc(ref msg);
        }

        private void EnglishTextBox_vScroll(Message msg)
        {
            msg.HWnd = DutchTextBox.Handle;
            DutchTextBox.PubWndProc(ref msg);
        }
    }





    //public class SynchronizedScrollRichTextBox : System.Windows.Forms.RichTextBox
    //{
    //    public event vScrollEventHandler vScroll;
    //    public delegate void vScrollEventHandler(System.Windows.Forms.Message message);

    //    public const int WM_VSCROLL = 0x115;

    //    protected override void WndProc(ref System.Windows.Forms.Message msg)
    //    {
    //        if (msg.Msg == WM_VSCROLL)
    //        {
    //            if (vScroll != null)
    //            {
    //                vScroll(msg);
    //            }
    //        }
    //        base.WndProc(ref msg);
    //    }

    //    public void PubWndProc(ref System.Windows.Forms.Message msg)
    //    {
    //        base.WndProc(ref msg);
    //    }
    //}
}
