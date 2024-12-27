using System;
using System.Data;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using Microsoft.Win32;

namespace Labs_WPF
{
    /// <summary>
    /// Логика взаимодействия для SLAEWindow.xaml
    /// </summary>
    public partial class SLAEWindow : Window
    {
        private DataTable _matrixA = new DataTable();
        private DataTable _vectorB = new DataTable();
        private DataTable _vectorGauss = new DataTable();
        private DataTable _vectorGaussJordan = new DataTable();
        private DataTable _vectorCramer = new DataTable();
        private string _filePath = null;

        public SLAEWindow()
        {
            InitializeComponent();
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

        private void btnSetSize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(tbMatrixSize.Text, out int size) || size < 2 || size > 50)
                {
                    MessageBox.Show("Размер матрицы должен быть от 2 до 50.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _matrixA.Clear();
                _vectorB.Clear();
                _vectorGauss.Clear();
                _vectorGaussJordan.Clear();
                _vectorCramer.Clear();
                _matrixA.Columns.Clear();
                _vectorB.Columns.Clear();
                _vectorGauss.Columns.Clear();
                _vectorGaussJordan.Columns.Clear();
                _vectorCramer.Columns.Clear();
                dgMatrixA.ItemsSource = null;
                dgVectorB.ItemsSource = null;
                dgVectorGauss.ItemsSource = null;
                dgVectorGaussJordan.ItemsSource = null;
                dgVectorCramer.ItemsSource = null;

                for (int i = 0; i < size; ++i)
                {
                    _matrixA.Columns.Add($"X{i + 1}", typeof(double));
                }

                for (int i = 0; i < size; ++i)
                {
                    var row = _matrixA.NewRow();

                    for (int j = 0; j < size; ++j)
                    {
                        row[j] = 0;
                    }

                    _matrixA.Rows.Add(row);
                }

                _vectorB.Columns.Add("B", typeof(double));
                _vectorGauss.Columns.Add("Гаусс", typeof(double));
                _vectorGaussJordan.Columns.Add("Гаусс-Жордан", typeof(double));
                _vectorCramer.Columns.Add("Крамер", typeof(double));

                for (int i = 0; i < size; ++i)
                {
                    var rowB = _vectorB.NewRow();
                    var rowGauss = _vectorGauss.NewRow();
                    var rowGaussJordan = _vectorGaussJordan.NewRow();
                    var rowCramer = _vectorCramer.NewRow();
                    rowB[0] = 0;
                    rowGauss[0] = 0;
                    rowGaussJordan[0] = 0;
                    rowCramer[0] = 0;
                    _vectorB.Rows.Add(rowB);
                    _vectorGauss.Rows.Add(rowGauss);
                    _vectorGaussJordan.Rows.Add(rowGaussJordan);
                    _vectorCramer.Rows.Add(rowCramer);
                }

                dgMatrixA.ItemsSource = _matrixA.DefaultView;
                dgVectorB.ItemsSource = _vectorB.DefaultView;
                dgVectorGauss.ItemsSource = _vectorGauss.DefaultView;
                dgVectorGaussJordan.ItemsSource = _vectorGaussJordan.DefaultView;
                dgVectorCramer.ItemsSource = _vectorCramer.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            btnSetSize_Click(sender, e);

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

                for (int i = 0; i < _matrixA.Rows.Count; ++i)
                {
                    for (int j = 0; j < _matrixA.Columns.Count; ++j)
                    {
                        if (rbInt.IsChecked == true)
                        {
                            _matrixA.Rows[i][j] = random.Next(minValue, maxValue);
                        }
                        else
                        {
                            _matrixA.Rows[i][j] = random.Next(minValue, maxValue) + Math.Round(random.NextDouble(), 3);
                        }
                    }
                }

                for (int i = 0; i < _vectorB.Rows.Count; ++i)
                {
                    if (rbInt.IsChecked == true)
                    {
                        _vectorB.Rows[i][0] = random.Next(minValue, maxValue);
                    }
                    else
                    {
                        _vectorB.Rows[i][0] = random.Next(minValue, maxValue) + Math.Round(random.NextDouble(), 3);
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

                    (_matrixA, _vectorB) = DataFromFile(_filePath);

                    _vectorGauss.Clear();
                    _vectorGaussJordan.Clear();
                    _vectorCramer.Clear();                    
                    _vectorGauss.Columns.Clear();
                    _vectorGaussJordan.Columns.Clear();
                    _vectorCramer.Columns.Clear();                    
                    dgVectorGauss.ItemsSource = null;
                    dgVectorGaussJordan.ItemsSource = null;
                    dgVectorCramer.ItemsSource = null;

                    _vectorGauss.Columns.Add("Гаусс", typeof(double));
                    _vectorGaussJordan.Columns.Add("Гаусс-Жордан", typeof(double));
                    _vectorCramer.Columns.Add("Крамер", typeof(double));

                    for (int i = 0; i < _vectorB.Rows.Count; ++i)
                    {
                        var rowGauss = _vectorGauss.NewRow();
                        var rowGaussJordan = _vectorGaussJordan.NewRow();
                        var rowCramer = _vectorCramer.NewRow();
                        rowGauss[0] = 0;
                        rowGaussJordan[0] = 0;
                        rowCramer[0] = 0;
                        _vectorGauss.Rows.Add(rowGauss);
                        _vectorGaussJordan.Rows.Add(rowGaussJordan);
                        _vectorCramer.Rows.Add(rowCramer);
                    }

                    dgMatrixA.ItemsSource = _matrixA.DefaultView;
                    dgVectorB.ItemsSource = _vectorB.DefaultView;
                    dgVectorGauss.ItemsSource = _vectorGauss.DefaultView;
                    dgVectorGaussJordan.ItemsSource = _vectorGaussJordan.DefaultView;
                    dgVectorCramer.ItemsSource = _vectorCramer.DefaultView;
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
                int size = _matrixA.Rows.Count;
                double[,] matrixA = new double[size, size];
                double[] vectorB = new double[size];

                if (cbCramer.IsChecked == true && size > 10)
                {
                    throw new Exception("Метод Крамера для матриц размерностью более 10 - ЗАПРЕЩЕН!!!");
                }

                for (int i = 0; i < size; ++i)
                {
                    for (int j = 0; j < size; ++j)
                    {
                        matrixA[i, j] = Convert.ToDouble(_matrixA.Rows[i][j]);
                    }

                    vectorB[i] = Convert.ToDouble(_vectorB.Rows[i][0]);
                }

                if (cbGauss.IsChecked == true)
                {
                    double[,] matrixForSolve = new double[size, size];
                    double[] vectorForSolve = new double[size];
                    matrixForSolve = (double[,])matrixA.Clone();
                    vectorB.CopyTo(vectorForSolve, 0);
                    double[] result = SolveGauss(matrixForSolve, vectorForSolve);

                    for (int i = 0; i < size; ++i)
                    {
                        _vectorGauss.Rows[i][0] = result[i];
                    }

                    dgVectorGauss.ItemsSource = _vectorGauss.DefaultView;
                }

                if (cbGaussJordan.IsChecked == true)
                {
                    double[,] matrixForSolve = new double[size, size];
                    double[] vectorForSolve = new double[size];
                    matrixForSolve = (double[,])matrixA.Clone();
                    vectorB.CopyTo(vectorForSolve, 0);
                    double[] result = SolveGaussJordan(matrixForSolve, vectorForSolve);

                    for (int i = 0; i < size; ++i)
                    {
                        _vectorGaussJordan.Rows[i][0] = result[i];
                    }

                    dgVectorGaussJordan.ItemsSource = _vectorGaussJordan.DefaultView;
                }

                if (cbCramer.IsChecked == true)
                {
                    double[,] matrixForSolve = new double[size, size];
                    double[] vectorForSolve = new double[size];
                    matrixForSolve = (double[,])matrixA.Clone();
                    vectorB.CopyTo(vectorForSolve, 0);
                    double[] result = SolveCramer(matrixForSolve, vectorForSolve);

                    for (int i = 0; i < size; ++i)
                    {
                        _vectorCramer.Rows[i][0] = result[i];
                    }

                    dgVectorCramer.ItemsSource = _vectorCramer.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private (DataTable, DataTable) DataFromFile(string path)
        {            
            DataTable dtA = new DataTable();
            DataTable dtB = new DataTable();
            var workbook = new XLWorkbook(path);
            var worksheet = workbook.Worksheet(1);
            var range = worksheet.RangeUsed();

            for (int i = 0; i < range.FirstRow().CellCount() - 1; ++i)
            {
                dtA.Columns.Add($"X{i + 1}");
            }

            dtB.Columns.Add("B");

            foreach (var row in range.RowsUsed())
            {
                var drA = dtA.NewRow();
                var drB = dtB.NewRow();

                for (int i = 0; i < row.Cells().Count() - 1; ++i)
                {
                    drA[i] = row.Cell(i + 1).Value;
                    drB[0] = row.Cell(i + 2).Value;
                }

                dtA.Rows.Add(drA);
                dtB.Rows.Add(drB);
            }

            return (dtA, dtB);
        }

        private double[] SolveGauss(double[,] matrixA, double[] vectorB)
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

                for (int k = i + 1; k < n; ++k)
                {
                    double factor = matrixA[k, i] / matrixA[i, i];

                    for (int j = i; j < n; j++)
                    {
                        matrixA[k, j] -= factor * matrixA[i, j];
                    }

                    vectorB[k] -= factor * vectorB[i];
                }
            }

            double[] result = new double[n];

            for (int i = n - 1; i >= 0; --i)
            {
                result[i] = Math.Round(vectorB[i] / matrixA[i, i], 3);

                for (int k = i - 1; k >= 0; --k)
                {
                    vectorB[k] -= matrixA[k, i] * result[i];
                }
            }

            return result;
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

        private double[] SolveCramer(double[,] matrixA, double[] vectorB)
        {
            int n = vectorB.Length;
                        
            double detA = Determinant(matrixA);

            if (detA == 0)
            {
                throw new Exception("Матрица вырожденная, решение невозможно.");
            }

            double[] result = new double[n];

            for (int i = 0; i < n; ++i)
            {
                double[,] tempMatrix = (double[,])matrixA.Clone();

                for (int j = 0; j < n; ++j)
                {
                    tempMatrix[j, i] = vectorB[j];
                }

                double detTemp = Determinant(tempMatrix);
                result[i] = Math.Round(detTemp / detA, 3);
            }

            return result;
        }

        private double Determinant(double[,] matrix)
        {
            int n = matrix.GetLength(0);

            if (n == 1)
            {
                return matrix[0, 0];
            }

            if (n == 2)
            {
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            }

            double det = 0;

            for (int i = 0; i < n; ++i)
            {
                double[,] subMatrix = new double[n - 1, n - 1];

                for (int j = 1; j < n; ++j)
                {
                    for (int k = 0; k < n; ++k)
                    {
                        if (k < i)
                        {
                            subMatrix[j - 1, k] = matrix[j, k];
                        }
                        else if (k > i)
                        {
                            subMatrix[j - 1, k - 1] = matrix[j, k];
                        }
                    }
                }

                det += (i % 2 == 0 ? 1 : -1) * matrix[0, i] * Determinant(subMatrix);
            }

            return det;
        }        
    }
}