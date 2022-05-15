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
using IronOcr;
using NetSpell.SpellChecker;
using NetSpell.SpellChecker.Dictionary;

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
            Application.Run(new DTEContext());
        }
    }

    internal partial class DTEContext : ApplicationContext
    {
        private NotifyIcon _notifyIcon;
        private List<string> engPhrasesList = new List<string>();
        private List<string> dutchPhraseList = new List<string>();


        public DTEContext()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<string> resourceNames = new List<string>(assembly.GetManifestResourceNames());

            #region Create Menu Items
            var translateScreenshot = new ToolStripMenuItem("Screenshot - Don't use", null, TranslateScreenshot);
            var translateImage = new ToolStripMenuItem("Select Picture", null, TranslateImage);
            var translateSnip = new ToolStripMenuItem("Snip", null, TranslateSnip);
            var exitMenuItem = new ToolStripMenuItem("Exit", null, OnExitClick);

            #endregion

            #region Create Menu and add Items
            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add(translateScreenshot);
            contextMenuStrip.Items.Add(translateImage);
            contextMenuStrip.Items.Add(translateSnip);
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


        // not fully implemented
        private void TranslateScreenshot(object sender, EventArgs e)
        {
            ClearPhraseLists();
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;

            // Create a bitmap of the appropriate size to receive the full-screen screenshot.
            using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
            {
                // Draw the screenshot into our bitmap.
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(screenLeft, screenTop, 0, 0, bitmap.Size);
                }

                //Save the screenshot as a Jpg image
                var filePath = "C:\\temp\\a.Jpg";
                try
                {
                    bitmap.Save(filePath, ImageFormat.Jpeg);
                    CreateBasicReader(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        // not fully implemented
        private void TranslateImage(object sender, EventArgs e)
        {
            ClearPhraseLists();
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //var img = new Bitmap(fileDialog.FileName);
                var dutchWord = CreateAdvancedReader(fileDialog.FileName);
                var engWord = TranslateDutchToEnglish(dutchWord);
                Console.WriteLine(engWord);
            }
        }

        private void TranslateSnip(object sender, EventArgs e)
        {
            ClearPhraseLists();
            var snip = SnippingTool.Snip();
            if (snip != null)
            {
                var filePath = "C:\\temp\\snip.Jpg";
                try
                {
                    snip.Save(filePath, ImageFormat.Jpeg);
                    var dutchPhrase = CreateAdvancedReader(filePath);
                    var engPhrase = TranslateDutchToEnglish(dutchPhrase);


                    dutchPhraseList.Add(dutchPhrase);
                    engPhrasesList.Add(engPhrase);


                    ShowTranslatedText(dutchPhraseList, engPhrasesList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        private string CreateBasicReader(string filePath)
        {
            AutoOcr OCR = new AutoOcr();
            var textFromImage = OCR.Read(filePath);
            var humanizeText = textFromImage.Text.Humanize();
            Console.WriteLine(textFromImage.Text);
            Console.WriteLine(humanizeText);
            return textFromImage.Text;
        }

        private string CreateAdvancedReader(string filePath)
        {
            AdvancedOcr ocr = new AdvancedOcr()
            {
                CleanBackgroundNoise = false,
                EnhanceContrast = false,
                EnhanceResolution = true,
                Language = OcrLanguage.Dutch,
                Strategy = IronOcr.AdvancedOcr.OcrStrategy.Advanced,
                ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                DetectWhiteTextOnDarkBackgrounds = true,
                InputImageType = AdvancedOcr.InputTypes.AutoDetect,
                RotateAndStraighten = true,
                ReadBarCodes = false,
                ColorDepth = 4
            };
            var textFromImage = ocr.Read(filePath);
            var humanizeText = textFromImage.Text.Humanize();
            return humanizeText;
        }




        // this is probably needed as I can use humanize for this
        private string[] SplitUpSentence(string sentence)
        {
            string[] tokens = sentence.Split(',');
            return tokens;
        }

        private string CreateTesseractReader(string filePath)
        {
            IronTesseract ocr = new IronTesseract();
            ocr.Language = OcrLanguage.English;
            ocr.Configuration.TesseractVersion = TesseractVersion.Tesseract5;
            using (var Input = new OcrInput())
            {
                Input.AddImage(filePath);
                var textFromImage = ocr.Read(Input);
                Console.WriteLine(textFromImage.Text);
                return textFromImage.Text;
            }
            //var textFromImage = ocr.Read(filePath);
            //Console.Write(textFromImage.Text);
        }

        private void EnglishWordCheck(string wordToCheck)
        {
            WordDictionary oDict = new WordDictionary();
            oDict.DictionaryFile = "en-US.dic";
            oDict.Initialize();
            Spelling oSpell = new Spelling();

            oSpell.Dictionary = oDict;
            if (!oSpell.TestWord(wordToCheck))
            {
                //Word does not exist in dictionary
            }
        }

        private string TranslateDutchToEnglish(string sentence)
        {
            try
            {
                var toLanguage = "en";//English
                var fromLanguage = "nl";//Dutch
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={System.Web.HttpUtility.UrlEncode(sentence)}";
                var webClient = new WebClient
                {
                    Encoding = System.Text.Encoding.UTF8
                };
                var result = webClient.DownloadString(url);
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                return result;
            }
            catch
            {
                return "An error occured with translation, please make sure you have an internet connection";
            }
        }


        private void ShowTranslatedText(List<string> dutchPhrases, List<string> engPhrases)
        {
            TranslatedTextForm translatedTextForm = new TranslatedTextForm(dutchPhrases, engPhrases);
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
}
