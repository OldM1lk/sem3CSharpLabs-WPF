using System.Windows;
using System.Windows.Controls;

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

        private void dichotomyMethodBtn_Initialized(object sender, System.EventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            StackPanel toolTipPanel = new StackPanel();
            toolTipPanel.Children.Add(new TextBlock { Text = "Используется для нахождения корня уравнения f(x) = 0", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Сущность метода - постепенное приближение к точке пересечения, находящейся в заданном интервале", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Результат - середина найденного отрезка", FontSize = 14 });
            toolTip.Content = toolTipPanel;
            dichotomyMethodBtn.ToolTip = toolTip;
        }

        private void dichotomyMethodBtn_Click(object sender, RoutedEventArgs e)
        {
            DichotomyWindow dichotomyWindow = new DichotomyWindow();
            dichotomyWindow.Show();
        }

        private void goldenRatioMethodBtn_Initialized(object sender, System.EventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            StackPanel toolTipPanel = new StackPanel();
            toolTipPanel.Children.Add(new TextBlock { Text = "Используется для нахождения минимума или максимума функции", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Сущность метода - деление заданного интервала в пропорции золотого сечения", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Результат - середина финального отрезка", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Метод хорош для одномерных задач и медленно сходящихся функций", FontSize = 14 });
            toolTip.Content = toolTipPanel;
            goldenRatioMethodBtn.ToolTip = toolTip;
        }

        private void goldenRatioMethodBtn_Click(object sender, RoutedEventArgs e)
        {
            GoldenRatioWindow goldenRatioWindow = new GoldenRatioWindow();
            goldenRatioWindow.Show();
        }

        private void newtonMethodBtn_Initialized(object sender, System.EventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            StackPanel toolTipPanel = new StackPanel();
            toolTipPanel.Children.Add(new TextBlock { Text = "Используется для нахождения минимума, максимума функции, а также точки пересечения её с осью абсциис", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Сущность метода - нахождение касательных к функции", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Метод хорош для одномерных задач и быстро сходящихся функций", FontSize = 14 });
            toolTip.Content = toolTipPanel;
            newtonMethodBtn.ToolTip = toolTip;
        }

        private void newtonMethodBtn_Click(object sender, RoutedEventArgs e)
        {
            NewtonWindow newtonWindow = new NewtonWindow();
            newtonWindow.Show();
        }

        private void coordinateDescentBtn_Initialized(object sender, System.EventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            StackPanel toolTipPanel = new StackPanel();
            toolTipPanel.Children.Add(new TextBlock { Text = "Используется для нахождения минимума, максимума функции", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Сущность метода - постепенное приближение с заданным шагом", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Метод плох для одномерных задач", FontSize = 14 });
            toolTip.Content = toolTipPanel;
            coordinateDescentBtn.ToolTip = toolTip;
        }

        private void coordinateDescentBtn_Click(object sender, RoutedEventArgs e)
        {
            CoordinateDescentWindow coordinateDescentWindow = new CoordinateDescentWindow();
            coordinateDescentWindow.Show();
        }

        private void sortingBtn_Initialized(object sender, System.EventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            StackPanel toolTipPanel = new StackPanel();
            toolTipPanel.Children.Add(new TextBlock { Text = "Пузырьковая сортировка", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Сортировка вставками", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Шейкерная сортировка", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Быстрая сортировка", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Болотная сортировка", FontSize = 14 });
            toolTip.Content = toolTipPanel;
            sortingBtn.ToolTip = toolTip;
        }

        private void sortingBtn_Click(object sender, RoutedEventArgs e)
        {
            SortingWindow sortingWindow = new SortingWindow();
            sortingWindow.Show();
        }

        private void integralBtn_Initialized(object sender, System.EventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            StackPanel toolTipPanel = new StackPanel();
            toolTipPanel.Children.Add(new TextBlock { Text = "Вычисление определенного интеграла", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "Методы:", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "1. Прямоугольников", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "2. Трапеций", FontSize = 14 });
            toolTipPanel.Children.Add(new TextBlock { Text = "3. Симпсона (парабол)", FontSize = 14 });
        }

        private void integralBtn_Click(object sender, RoutedEventArgs e)
        {
            IntegralWindow integralWindow = new IntegralWindow();
            integralWindow.Show();
        }

        private void slaeBtn_Click(object sender, RoutedEventArgs e)
        {
            SLAEWindow slaeWindow = new SLAEWindow();
            slaeWindow.Show();
        }
    }
}