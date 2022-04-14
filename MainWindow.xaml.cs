using ExamTickets.Pages;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ExamTickets
{
    public partial class MainWindow : Window
    {
        private static readonly Random random = new Random();
        List<string> paragraphs = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new QuestionsPage(paragraphs));

            Array backgrounds = (Array)Resources["backgrounds"];
            Background = (Brush)backgrounds.GetValue(random.Next(backgrounds.Length));  
        }
    

        private void QuestionsButtonClick(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new QuestionsPage(paragraphs));
        }

        private void TicketsButtonClick(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new TicketsPage(paragraphs));
        }
    }
}
