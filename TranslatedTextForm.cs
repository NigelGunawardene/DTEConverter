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
        private static int previousValue = 0;
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
        public extern static int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);


        private void DutchTextBox_VScroll(object sender, EventArgs e)
        {
            int nPos = GetScrollPos(DutchTextBox.Handle, (int)ScrollBarType.SbVert);
            if (previousValue != nPos)
            {
                previousValue = nPos;
                nPos <<= 16;
                int wParam = (int)ScrollBarCommands.SB_THUMBPOSITION | (int)nPos;
                SendMessage(EnglishTextBox.Handle, (int)Messaging.WM_VSCROLL, new IntPtr(wParam), new IntPtr(0));
            }
        }

        private void EnglishTextBox_VScroll(object sender, EventArgs e)
        {
            int nPos = GetScrollPos(EnglishTextBox.Handle, (int)ScrollBarType.SbVert);
            nPos <<= 16;
            int wParam = (int)ScrollBarCommands.SB_THUMBPOSITION | (int)nPos;
            SendMessage(DutchTextBox.Handle, (int)Messaging.WM_VSCROLL, new IntPtr(wParam), new IntPtr(0));
        }
    }


    public enum ScrollBarType : int
    {
        SbHorz = 0,
        SbVert = 1,
        SbCtl = 2,
        SbBoth = 3
    }

    public enum Messaging : int
    {
        WM_VSCROLL = 0x0115
    }

    public enum ScrollBarCommands : int
    {
        SB_THUMBPOSITION = 4
    }
}
