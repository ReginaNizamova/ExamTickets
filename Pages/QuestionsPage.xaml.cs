using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ExamTickets.Pages
{
    public partial class QuestionsPage : Page
    {
        public QuestionsPage()
        {
            InitializeComponent();
        }

        private void AddFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.docx)|*.docx";


            if (openFileDialog.ShowDialog() == true)
            {
                WordprocessingDocument wordDoc = WordprocessingDocument.Open(openFileDialog.FileName, true);
                Body body = wordDoc.MainDocumentPart.Document.Body;

                var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var file = Path.Combine(directory, "ExamTicket.txt");
                List<string> paragraphs = new List<string>();

                foreach (var childElement in body.ChildElements)
                {
                    var text = childElement.InnerText;
                    File.AppendAllText(file, '\n' + text);
                    
                    if (text != "")
                    {
                        paragraphs.Add(text);
                    }
                }

                questionText.Text = File.ReadAllText(file);
                File.WriteAllText(file, string.Empty);
            }
        }
    }
}
