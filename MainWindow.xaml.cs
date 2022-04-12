using ExamTickets.Pages;
using System.Windows;
using System.Windows.Controls;

namespace ExamTickets
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new QuestionsPage());
        }

        private void QuestionsButtonClick(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new QuestionsPage());
        }

        private void TicketsButtonClick(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new TicketsPage());
        }
    }
}
