using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Word = Microsoft.Office.Interop.Word;

namespace ExamTickets.Pages
{
    public partial class TicketsPage : Page
    {
        List<string> paragraphs;
        public TicketsPage(List<string> paragraph)
        {
            InitializeComponent();
            paragraphs = paragraph;
        }

        private void GenerateButtonClick(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (titleSubject.Text == "")
                errors.AppendLine("  - предмет");
            if (semester.Text == "")
                errors.AppendLine("  - семестр");
            if (countTickets.Text == "")
                errors.AppendLine("  - количество билетов");
            if (countQuestions.Text == "")
                errors.AppendLine("  - количество вопросов в билете");

            if (CheckValue(semester.Text) == false)
                errors.AppendLine("\n\nсеместр - целое однозначное число");
            if (CheckValue(countTickets.Text) == false)
                errors.AppendLine("количество билетов - целое число (макс. 2х-значное)");
            if (CheckValue(countQuestions.Text) == false)
                errors.AppendLine("количество вопросов в билете - целое число (макс. 2х-значное)");

            if (errors.Length > 0)
            {
                MessageBox.Show("Введите\n" + errors.ToString());
                return;
            }

            try
            {
                int count = Convert.ToInt32(countTickets.Text) * Convert.ToInt32(countQuestions.Text);
                var random = new Random();
                var randomList = Enumerable.Range(0, count).Select(p => random.Next(1, paragraphs.Count())).ToList();
                List<string> result = new List<string>();

                for (int i = 0; i < count; i++)
                {
                    result.Add(paragraphs.ElementAt(randomList[i]));
                }

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
            string b = @"C:\Users\User\Desktop";

            for (int i = 0; i < 3; i+=2)
            {
                WordHelper.NewDoc(b, @"C:\Users\User\Desktop\ExamTickets\Resource\Sample.dotx", result[i], result[i+1]);
                i+=2;
                WordHelper.NewDoc(b, @"C:\Users\User\Desktop\ExamTickets\Resource\Sample.dotx", result[i], result[i+1]);
            }
            //List<string> a = new List<string>();

            //a.Add(b + @"\Документ4");
            
            //WordHelper.JoinDocs("Doc", a);
            WordHelper.CloseApp();
        }

     


        public static bool CheckValue(string value)
        {
            var minMaxChar = new Regex(@"^(?=.{1,2}$)");
            var number = new Regex(@"[2-5]+");
            var upperChar = new Regex(@"[A-Z]");
            var lowerChar = new Regex(@"[a-z]");
            var symbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (lowerChar.IsMatch(value))
                return false;

            else if (upperChar.IsMatch(value))
                return false;

            else if (!minMaxChar.IsMatch(value))
                return false;

            else if (!number.IsMatch(value))
                return false;

            else if (symbols.IsMatch(value))
                return false;

            return true;
        }
       
    }

    public static class WordHelper
    {
        private static dynamic _wdApp;
        private static dynamic _wdDoc;

        public static void CreateApp()
        {
            _wdApp = Activator.CreateInstance(Type.GetTypeFromProgID("Word.Application"));
            _wdApp.Visible = true;
        }
       
        /// <param name="fileName">Полное имя файла нового документа</param>
        /// <param name="template">Путь к шаблону, на основе которого будет создаваться документ</param>
        /// <param name="firstName">Имя</param>
        /// <param name="middleName">Отчество</param>
        /// <param name="lastName">Фамилия</param>
        public static void NewDoc(string fileName, string template, string question1, string question2)
        {
           
            _wdDoc = _wdApp.Documents.Add(template);
            _wdDoc.sleep(100);
            _wdDoc.Bookmarks["question1"].Range.Text = question1;
            _wdDoc.Bookmarks["question2"].Range.Text = question2;
           
            _wdDoc.SaveAs(fileName);
            _wdDoc.Close();
        }

        /// <param name="fileName">Имя результирующего документа</param>
        /// <param name="docs">Полные пути к файлам, которые нужно включить в результирующий документ</param>
        public static void JoinDocs(string fileName, IEnumerable<string> docs)
        {
            _wdDoc = _wdApp.Documents.Add();
            foreach (var doc in docs)
            {
                _wdDoc.Paragraphs.Last.Range.InsertFile($"\"{doc}\"");
                _wdDoc.Paragraphs.Last.Range.InsertBreak(7);//wdPageBreak
            }
            _wdDoc.SaveAs(fileName);
            _wdDoc.Close();
        }

        public static void CloseApp()
        {
            if (_wdApp != null)
            {
                _wdApp.Quit(false);
                _wdApp = null;
            }
        }
    }
}


