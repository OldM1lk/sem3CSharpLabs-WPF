using System;
using OxyPlot;
using System.Data;
using System.Windows;
using OxyPlot.Series;
using ClosedXML.Excel;
using Microsoft.Win32;
using org.mariuszgromada.math.mxparser;
using Expression = org.mariuszgromada.math.mxparser.Expression;

namespace Labs_WPF
{
    /// <summary>
    /// Логика взаимодействия для MLSWindow.xaml
    /// </summary>
    public partial class MLSWindow : Window
    {
        private DataTable _matrix = new DataTable();
        private string _filePath;

        public MLSWindow()
        {
            InitializeComponent();

            _matrix.Columns.Add("X", typeof(double));
            _matrix.Columns.Add("Y", typeof(double));
            dgPoints.ItemsSource = _matrix.DefaultView;
        }

        private void rbGenerate_Checked(object sender, RoutedEventArgs e)
        {
            if (btnGenerate != null && rbInt != null && rbDouble != null && tbMinNumber != null && tbMaxNumber != null)
            {
                btnGenerate.IsEnabled = true;
                rbInt.IsEnabled = true;
                rbDouble.IsEnabled = true;
                tbMinNumber.IsEnabled = true;
                tbMaxNumber.IsEnabled = true;
            }
        }

        private void rbGenerate_Unchecked(object sender, RoutedEventArgs e)
        {
            if (btnGenerate != null && rbInt != null && rbDouble != null && tbMinNumber != null && tbMaxNumber != null)
            {
                btnGenerate.IsEnabled = false;
                rbInt.IsEnabled = false;
                rbDouble.IsEnabled = false;
                tbMinNumber.IsEnabled = false;
                tbMaxNumber.IsEnabled = false;
            }
        }

        private void rbFileInput_Checked(object sender, RoutedEventArgs e)
        {
            if (btnChooseFile != null)
            {
                btnChooseFile.IsEnabled = true;
            }
        }

        private void rbFileInput_Unchecked(object sender, RoutedEventArgs e)
        {
            if (btnChooseFile != null)
            {
                btnChooseFile.IsEnabled = false;
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(tbPointsCount.Text, out int size) || size < 2)
                {
                    MessageBox.Show("Неправильно задано количество точек", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _matrix.Clear();
                dgPoints.ItemsSource = null;

                for (int i = 0; i < size; ++i)
                {
                    var row = _matrix.NewRow();
                    row[0] = 0;
                    row[1] = 0;
                    _matrix.Rows.Add(row);
                }

                dgPoints.ItemsSource = _matrix.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            btnApply_Click(sender, e);

            try
            {
                int minValue = int.Parse(tbMinNumber.Text);
                int maxValue = int.Parse(tbMaxNumber.Text);

                if (minValue >= maxValue)
                {
                    MessageBox.Show("Минимальное значение должно быть меньше максимального.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Random random = new Random();

                for (int i = 0; i < _matrix.Rows.Count; ++i)
                {
                    for (int j = 0; j < _matrix.Columns.Count; ++j)
                    {
                        if (rbInt.IsChecked == true)
                        {
                            _matrix.Rows[i][j] = random.Next(minValue, maxValue);
                        }
                        else
                        {
                            _matrix.Rows[i][j] = random.Next(minValue, maxValue) + Math.Round(random.NextDouble(), 3);
                        }
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Пожалуйста, введите корректные числовые значения для минимального и максимального диапазона.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "(*.xlsx)|*xlsx"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    _filePath = openFileDialog.FileName;
                    _matrix = DataFromFile(_filePath);
                    dgPoints.ItemsSource = _matrix.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rbLinear.IsChecked == true)
                {
                    double[] result;
                    string function;
                    (result, function) = SolveLinearFunction();
                    string output = "a = " + result[0].ToString() + "\n" +
                                    "b = " + result[1].ToString();
                    PlotGraph(function);
                    MessageBox.Show(output, "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (rbQuadratic.IsChecked == true)
                {
                    double[] result;
                    string function;
                    (result, function) = SolveQuadraticFunction();
                    string output = "a = " + result[0].ToString() + "\n" +
                                    "b = " + result[1].ToString() + "\n" +
                                    "c = " + result[2].ToString();
                    PlotGraph(function);
                    MessageBox.Show(output, "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DataTable DataFromFile(string path)
        {
            DataTable dt = new DataTable();
            var workbook = new XLWorkbook(path);
            var worksheet = workbook.Worksheet(1);
            var range = worksheet.RangeUsed();

            dt.Columns.Add("X");
            dt.Columns.Add("Y");

            foreach (var row in range.RowsUsed())
            {
                var dr = dt.NewRow();
                dr[0] = row.Cell(1).Value;
                dr[1] = row.Cell(2).Value;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private (double[], string) SolveLinearFunction()
        {
            try
            {
                double sumX = 0, sumY = 0, sumX2 = 0, sumXY = 0;
                int n = _matrix.Rows.Count;

                foreach (DataRow row in _matrix.Rows)
                {
                    double x = Convert.ToDouble(row[0]);
                    double y = Convert.ToDouble(row[1]);

                    sumX += x;
                    sumY += y;
                    sumX2 += x * x;
                    sumXY += x * y;
                }

                double[,] matrix = new double[2, 2];
                double[] vector = new double[2];

                matrix[0, 0] = sumX2;
                matrix[0, 1] = sumX;
                matrix[1, 0] = sumX;
                matrix[1, 1] = n;

                vector[0] = sumXY;
                vector[1] = sumY;

                double[] result = SolveGaussJordan(matrix, vector);
                string function;

                if (result[1] > 0)
                {
                    function = result[0].ToString() + "*x + " + result[1].ToString();
                }
                else
                {
                    function = result[0].ToString() + "*x - " + result[1].ToString().Substring(1);
                }

                return (result, function);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return (null, null);
        }

        private (double[], string) SolveQuadraticFunction()
        {
            try
            {
                double sumX = 0, sumY = 0, sumX2 = 0, sumXY = 0, sumX2Y = 0, sumX3 = 0, sumX4 = 0;
                int n = _matrix.Rows.Count;

                foreach (DataRow row in _matrix.Rows)
                {
                    double x = Convert.ToDouble(row[0]);
                    double y = Convert.ToDouble(row[1]);

                    sumX += x;
                    sumY += y;
                    sumX2 += x * x;
                    sumXY += x * y;
                    sumX2Y += x * x * y;
                    sumX3 += x * x * x;
                    sumX4 += x * x * x * x;
                }

                double[,] matrix = new double[3, 3];
                double[] vector = new double[3];

                matrix[0, 0] = sumX4;
                matrix[0, 1] = sumX3;
                matrix[0, 2] = sumX2;
                matrix[1, 0] = sumX3;
                matrix[1, 1] = sumX2;
                matrix[1, 2] = sumX;
                matrix[2, 0] = sumX2;
                matrix[2, 1] = sumX;
                matrix[2, 2] = n;

                vector[0] = sumX2Y;
                vector[1] = sumXY;
                vector[2] = sumY;

                double[] result = SolveGaussJordan(matrix, vector);
                string function;

                if (result[1] > 0)
                {
                    if (result[2] > 0)
                    {
                        function = result[0].ToString() + "*x^2 + " + result[1].ToString() + "*x + " + result[2].ToString();
                    }
                    else
                    {
                        function = result[0].ToString() + "*x^2 + " + result[1].ToString() + "*x - " + result[2].ToString().Substring(1);
                    }
                }
                else
                {
                    if (result[2] > 0)
                    {
                        function = result[0].ToString() + "*x^2 - " + result[1].ToString().Substring(1) + "*x + " + result[2].ToString();
                    }
                    else
                    {
                        function = result[0].ToString() + "*x^2 - " + result[1].ToString().Substring(1) + "*x - " + result[2].ToString().Substring(1);
                    }
                }

                return (result, function);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return (null, null);
        }

        private double[] SolveGaussJordan(double[,] matrixA, double[] vectorB)
        {
            int n = vectorB.Length;

            for (int i = 0; i < n; ++i)
            {
                double maxElement = Math.Abs(matrixA[i, i]);
                int maxRow = i;

                for (int k = i + 1; k < n; ++k)
                {
                    if (Math.Abs(matrixA[k, i]) > maxElement)
                    {
                        maxElement = Math.Abs(matrixA[k, i]);
                        maxRow = k;
                    }
                }

                for (int k = i; k < n; ++k)
                {
                    (matrixA[i, k], matrixA[maxRow, k]) = (matrixA[maxRow, k], matrixA[i, k]);
                }

                (vectorB[i], vectorB[maxRow]) = (vectorB[maxRow], vectorB[i]);

                for (int k = 0; k < n; ++k)
                {
                    if (k != i)
                    {
                        double factor = matrixA[k, i] / matrixA[i, i];

                        for (int j = i; j < n; ++j)
                        {
                            matrixA[k, j] -= factor * matrixA[i, j];
                        }

                        vectorB[k] -= factor * vectorB[i];
                    }
                }
            }

            double[] result = new double[n];

            for (int i = 0; i < n; ++i)
            {
                result[i] = Math.Round(vectorB[i] / matrixA[i, i], 3);
            }

            return result;
        }

        private void PlotGraph(string function)
        {
            try
            {
                double left = Convert.ToDouble(tbMinNumber.Text);
                double right = Convert.ToDouble(tbMaxNumber.Text);
                Function func = new Function("f(x) = " + function.Replace(",", "."));
                var model = new PlotModel { Title = "y = " + function };

                if (left < 5 && left > -5)
                {
                    left = -5;
                }
                if (right < 5 && right > -5)
                {
                    right = 5;
                }

                var absicc = new LineSeries
                {
                    Color = OxyColors.Black,
                    StrokeThickness = 2
                };
                var ordinate = new LineSeries
                {
                    Color = OxyColors.Black,
                    StrokeThickness = 2,
                };
                var scatterSeries = new ScatterSeries
                {
                    MarkerType = MarkerType.Circle,
                    MarkerFill = OxyColors.Red,
                    MarkerSize = 3
                };
                var lineSeries = new LineSeries
                {
                    Color = OxyColors.Green
                };

                absicc.Points.Add(new DataPoint(left, 0));
                absicc.Points.Add(new DataPoint(right, 0));
                ordinate.Points.Add(new DataPoint(0, right));
                ordinate.Points.Add(new DataPoint(0, left));

                foreach (DataRow row in _matrix.Rows)
                {
                    double x = Convert.ToDouble(row[0]);
                    double y = Convert.ToDouble(row[1]);
                    scatterSeries.Points.Add(new ScatterPoint(x, y));
                }

                for (double i = left; i <= right; ++i)
                {
                    Expression expression = new Expression($"f({i})", func);
                    double y = expression.calculate();
                    lineSeries.Points.Add(new DataPoint(i, y));
                }

                model.Series.Add(absicc);
                model.Series.Add(ordinate);
                model.Series.Add(lineSeries);
                model.Series.Add(scatterSeries);
                pvGraph.Model = model;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}