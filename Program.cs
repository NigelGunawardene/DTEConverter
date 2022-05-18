using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Humanizer;
using Tesseract;

namespace DTEConverter
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DTEContext());
        }
    }

    internal partial class DTEContext : ApplicationContext
    {
        private static string tesseractFiles = "C:\\tessFiles\\tesseract";
        private static string tesseractEnglish = "eng";
        private static string tesseractDutch = "nld";
        private static Mode appMode = Mode.PHRASE;

        private NotifyIcon _notifyIcon;
        private List<string> engPhrasesList = new List<string>();
        private List<string> dutchPhraseList = new List<string>();


        public DTEContext()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<string> resourceNames = new List<string>(assembly.GetManifestResourceNames());

            #region Create Menu Items
            var translateImage = new ToolStripMenuItem("Select Picture", null, TranslateImage);
            var translateSnip = new ToolStripMenuItem("Snip", null, TranslateSnip);
            var translateSnipSingleWordMode = new ToolStripMenuItem("Snip - words", null, TranslateSnipSingleWordMode);
            var exitMenuItem = new ToolStripMenuItem("Exit", null, OnExitClick);

            #endregion

            #region Create Menu and add Items
            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add(translateImage);
            contextMenuStrip.Items.Add(translateSnip);
            contextMenuStrip.Items.Add(translateSnipSingleWordMode);
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

        private void TranslateImage(object sender, EventArgs e)
        {
            ClearPhraseLists();
            appMode = Mode.PHRASE;
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var img = new Bitmap(fileDialog.FileName);
                ExecuteReadAndTranslateFunctions(fileDialog.FileName);
            }
        }

        private void TranslateSnip(object sender, EventArgs e)
        {
            ClearPhraseLists();
            appMode = Mode.PHRASE;
            var snip = SnippingTool.Snip();
            if (snip != null)
            {
                var filePath = "C:\\tessFiles\\snip.Jpg";
                try
                {
                    snip.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ExecuteReadAndTranslateFunctions(filePath);
                    snip.Dispose();
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void TranslateSnipSingleWordMode(object sender, EventArgs e)
        {
            ClearPhraseLists();
            appMode = Mode.WORD;
            var snip = SnippingTool.Snip();
            if (snip != null)
            {
                var filePath = "C:\\tessFiles\\snip.Jpg";
                try
                {
                    snip.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ExecuteReadAndTranslateFunctions(filePath);
                    snip.Dispose();
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void ExecuteReadAndTranslateFunctions(string filePath)
        {
            TesseractReader(filePath);
            TranslateDutchListToEnglish();
            ShowTranslatedText();
        }


        // References - 
        // https://github.com/charlesw/tesseract
        // https://github.com/charlesw/tesseract-samples
        private void TesseractReader(string filePath)
        {
            try
            {
                using (TesseractEngine ocr = new TesseractEngine(tesseractFiles, tesseractDutch, EngineMode.Default))
                {
                    Bitmap image = new Bitmap(filePath);
                    var result = ocr.Process(image);

                    var meanConfidence = result.GetMeanConfidence();
                    var resultText = result.GetText().Humanize();
                    if (appMode == Mode.PHRASE)
                    {
                        dutchPhraseList.Add(resultText);
                    }
                    else
                    {
                        //use this if we want to split the sentence.
                        resultText.Split(null).ToList().ForEach(word =>
                        {
                            dutchPhraseList.Add(word);
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        //private string TranslateDutchToEnglish(string sentence)
        //{
        //    try
        //    {
        //        var toLanguage = "en";//English
        //        var fromLanguage = "nl";//Dutch
        //        var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={System.Web.HttpUtility.UrlEncode(sentence)}";
        //        var webClient = new WebClient
        //        {
        //            Encoding = System.Text.Encoding.UTF8
        //        };
        //        var result = webClient.DownloadString(url);
        //        result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
        //        return result;
        //    }
        //    catch
        //    {
        //        return "An error occured with translation, please make sure you have an internet connection";
        //    }
        //}

        private void TranslateDutchListToEnglish()
        {
            try
            {
                var toLanguage = "en";//English
                var fromLanguage = "nl";//Dutch
                foreach (var word in dutchPhraseList)
                {
                    var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={System.Web.HttpUtility.UrlEncode(word)}";
                    var webClient = new WebClient
                    {
                        Encoding = System.Text.Encoding.UTF8
                    };
                    var result = webClient.DownloadString(url);
                    result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                    engPhrasesList.Add(result);
                }
            }
            catch
            {
                Console.WriteLine("An error occured with translation, please make sure you have an internet connection");
            }
        }


        private void ShowTranslatedText()
        {
            TranslatedTextForm translatedTextForm = new TranslatedTextForm(dutchPhraseList, engPhrasesList);
            translatedTextForm.Show();
        }

        private void ClearPhraseLists()
        {
            engPhrasesList.Clear();
            dutchPhraseList.Clear();
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            Application.Exit();
        }

    }

    internal enum Mode
    {
        PHRASE,
        WORD
    }
}
