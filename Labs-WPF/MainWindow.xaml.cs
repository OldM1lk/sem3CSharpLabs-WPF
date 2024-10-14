using System.Windows;

namespace Labs_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void dichotomyMethodBtn_Click(object sender, RoutedEventArgs e)
        {
            DichotomyWindow dichotomyWindow = new DichotomyWindow();
            dichotomyWindow.Show();
        }
    }
}