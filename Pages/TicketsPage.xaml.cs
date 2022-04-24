using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace ExamTickets.Pages
{
    public partial class TicketsPage : System.Windows.Controls.Page
    {
        List<string> paragraphs;
        List<string> result = new List<string>();
        const int countQuestion = 2;
        public TicketsPage(List<string> paragraph)
        {
            InitializeComponent();
            paragraphs = paragraph;
        }

        private void GenerateButtonClick(object sender, RoutedEventArgs e)
        {
            Check();

            try
            {
                int count = Convert.ToInt32(countTickets.Text) * countQuestion;
                var random = new Random();
                var randomList = Enumerable.Range(0, count).Select(p => random.Next(1, paragraphs.Count())).ToList();

                for (int i = 0; i < count; i++)
                    result.Add(paragraphs.ElementAt(randomList[i]));
                
                CreateWord(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void CreateWord(List<string> result)
        {
            WordHelper.CreateApp();
            int numberTicket = 1;
            List<string> paths = new List<string>();
            string path = "";
            string pathTemplate = Path.Combine(Environment.CurrentDirectory, "Samples.dotx");

            for (int i = 0; i < (Convert.ToInt32(countTickets.Text) * 2); i += 2)
            {
                path = Path.Combine(Environment.CurrentDirectory, "Doc" + numberTicket + ".docx");
                WordHelper.NewDoc(path, pathTemplate, result[i], result[i + 1], numberTicket, titleSubject.Text, semester.Text);
                numberTicket++;
                paths.Add(path);      
            }

            WordHelper.JoinDocs(pathFile.Text + "/Document", paths);
            MessageBox.Show("Файл Document создан!");
            WordHelper.CloseApp(paths);
        }

        private void Check ()
        {
            StringBuilder errors = new StringBuilder();

            if (titleSubject.Text == "")
                errors.AppendLine("  - предмет");
            if (semester.Text == "")
                errors.AppendLine("  - семестр");
            if (countTickets.Text == "")
                errors.AppendLine("  - количество билетов");
            if (pathFile.Text == "")
                errors.AppendLine("  - путь к файлу");


            if (CheckValue(semester.Text) == false)
                errors.AppendLine("\n\nсеместр - целое число");
            if (CheckValue(countTickets.Text) == false)
                errors.AppendLine("количество билетов - целое число");

            if (errors.Length > 0)
            {
                MessageBox.Show("Введите\n" + errors.ToString());
                return;
            }
        }
        public static bool CheckValue(string value)
        {
            var upperChar = new Regex(@"[A-Z]");
            var lowerChar = new Regex(@"[a-z]");
            var symbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (lowerChar.IsMatch(value))
                return false;

            else if (upperChar.IsMatch(value))
                return false;

            else if (symbols.IsMatch(value))
                return false;

            return true;
        }
    }

    public static class WordHelper
    {
        private static dynamic app;
        private static dynamic doc;

        public static void CreateApp()
        {
            app = Activator.CreateInstance(Type.GetTypeFromProgID("Word.Application"));
        }

        public static void NewDoc(string fileName, string template, string question1, string question2, int numberTicket, string subject, string semester)
        {
            doc = app.Documents.Add(template);

            doc.Bookmarks["question1"].Range.Text = question1;
            doc.Bookmarks["question2"].Range.Text = question2;
            doc.Bookmarks["numberTicket"].Range.Text = numberTicket;
            doc.Bookmarks["subject"].Range.Text = subject;
            doc.Bookmarks["semester"].Range.Text = semester;

            doc.SaveAs(fileName);
            doc.Close();
        }

        public static void JoinDocs(string fileName, IEnumerable<string> docs)
        {
            doc = app.Documents.Add();
            foreach (var doc in docs)
            {
                WordHelper.doc.Paragraphs.Last.Range.InsertFile($"\"{doc}\"");
                WordHelper.doc.Paragraphs.Last.Range.InsertBreak(1);
            }
            doc.SaveAs(fileName);
            doc.Close();
        }

        public static void CloseApp(IEnumerable<string> docs)
        {
            if (app != null)
            {
                app.Quit(false);

                foreach (var doc in docs)
                {
                    File.Delete(doc);
                }
                app = null;
            }
        }
    }
}