using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
    }
}
