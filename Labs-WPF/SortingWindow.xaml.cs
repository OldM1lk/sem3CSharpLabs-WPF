using Microsoft.Win32;
using System.Windows;

namespace Labs_WPF
{
    /// <summary>
    /// Логика взаимодействия для SortingWindow.xaml
    /// </summary>
    public partial class SortingWindow : Window
    {
        public SortingWindow()
        {
            InitializeComponent();
        }

        private void bogoSortCB_Click(object sender, RoutedEventArgs e)
        {
            if (bogoSortCB.IsChecked == true)
            {
                bogoSortIterationsTB.IsEnabled = true;
            }
            else
            {
                bogoSortIterationsTB.IsEnabled = false;
            }
        }

        private void manualInputRB_Checked(object sender, RoutedEventArgs e)
        {
            if (manualInputRB.IsChecked == true)
            {
                manualInputBtn.IsEnabled = true;
                generateElementsNumberTB.IsEnabled = false;
                minNumberTB.IsEnabled = false;
                maxNumberTB.IsEnabled = false;
                chooseFileBtn.IsEnabled = false;
            }
        }

        private void generateRB_Click(object sender, RoutedEventArgs e)
        {
            if (generateRB.IsChecked == true)
            {
                generateElementsNumberTB.IsEnabled = true;
                manualInputBtn.IsEnabled = false;
                minNumberTB.IsEnabled = true;
                maxNumberTB.IsEnabled = true;
                chooseFileBtn.IsEnabled = false;
            }
        }

        private void fileInputRB_Click(object sender, RoutedEventArgs e)
        {
            if (fileInputRB.IsChecked == true)
            {
                chooseFileBtn.IsEnabled = true;
                manualInputBtn.IsEnabled = false;
                generateElementsNumberTB.IsEnabled = false;
                minNumberTB.IsEnabled = false;
                maxNumberTB.IsEnabled = false;
            }
        }

        private void chooseFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "(*.txt, *.xlsx)|*.txt;*xlsx";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
            }
        }
    }
}