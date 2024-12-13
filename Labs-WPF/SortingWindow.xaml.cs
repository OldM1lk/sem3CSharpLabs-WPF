using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using ClosedXML.Excel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Labs_WPF
{
    /// <summary>
    /// Логика взаимодействия для SortingWindow.xaml
    /// </summary>
    public partial class SortingWindow : Window
    {
        public ObservableCollection<ResultRow> Results = new ObservableCollection<ResultRow>();
        public string FilePath = null;

        public SortingWindow()
        {
            InitializeComponent();

            resultGrid.ItemsSource = Results;
        }

        private void bogoSortCB_Checked(object sender, RoutedEventArgs e)
        {
            if (bogoSortIterationsTB != null)
            {
                bogoSortIterationsTB.IsEnabled = true;
            }
        }

        private void bogoSortCB_Unchecked(object sender, RoutedEventArgs e)
        {
            if (bogoSortIterationsTB != null)
            {
                bogoSortIterationsTB.IsEnabled = false;
            }
        }

        private void manualInputRB_Checked(object sender, RoutedEventArgs e)
        {
            if (manualInputTB != null)
            {
                manualInputTB.IsEnabled = true;
            }
        }

        private void manualInputRB_Unchecked(object sender, RoutedEventArgs e)
        {
            if (manualInputTB != null)
            {
                manualInputTB.IsEnabled = false;
            }
        }

        private void generateRB_Checked(object sender, RoutedEventArgs e)
        {
            if (generateElementsNumberTB != null && numbersTypeSP != null && minNumberTB != null && maxNumberTB != null)
            {
                generateElementsNumberTB.IsEnabled = true;
                numbersTypeSP.IsEnabled = true;
                minNumberTB.IsEnabled = true;
                maxNumberTB.IsEnabled = true;
            }
        }

        private void generateRB_Unchecked(object sender, RoutedEventArgs e)
        {
            if (generateElementsNumberTB != null && numbersTypeSP != null && minNumberTB != null && maxNumberTB != null)
            {
                generateElementsNumberTB.IsEnabled = false;
                numbersTypeSP.IsEnabled = false;
                minNumberTB.IsEnabled = false;
                maxNumberTB.IsEnabled = false;
            }
        }

        private void fileInputRB_Checked(object sender, RoutedEventArgs e)
        {
            if (chooseFileBtn != null)
            {
                chooseFileBtn.IsEnabled = true;
            }
        }

        private void fileInputRB_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chooseFileBtn != null)
            {
                chooseFileBtn.IsEnabled = false;
            }
        }

        private void chooseFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "(*.txt, *.xlsx)|*.txt;*xlsx";

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
            }
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            Results.Clear();
        }

        private void sortBtn_Click(object sender, RoutedEventArgs e)
        {
            byte inputType = InputType();

            switch (inputType)
            {
                case 1:
                {
                    ManualSorting();
                    break;
                }
                case 2:
                {
                    GenerateSorting();
                    break;
                }
                case 3:
                {
                    FileSorting();
                    break;
                }
            }
        }

        private uint BogoSortIterations()
        {
            if (string.IsNullOrEmpty(bogoSortIterationsTB.Text) || !Regex.IsMatch(bogoSortIterationsTB.Text, @"^\d+$") || bogoSortIterationsTB.Text == "0")
            {
                MessageBox.Show("Неправильно задано кол-во итераций для болотной сортировки.\nОно будет заменено на значение по умолчанию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                bogoSortIterationsTB.Text = "1000";
                return Convert.ToUInt32(bogoSortIterationsTB.Text);
            }

            return Convert.ToUInt32(bogoSortIterationsTB.Text);
        }

        private uint GenerateElementsNumber()
        {
            if (string.IsNullOrEmpty(generateElementsNumberTB.Text) || !Regex.IsMatch(generateElementsNumberTB.Text, @"^\d+$") || manualInputTB.Text == "0")
            {
                MessageBox.Show("Неправильно введено кол-во элементов массива.\nОно будет заменено на значение по умолчанию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                generateElementsNumberTB.Text = "100";
            }

            return Convert.ToUInt32(generateElementsNumberTB.Text);
        }

        private int MinNumber()
        {
            if (string.IsNullOrEmpty(minNumberTB.Text) || !Regex.IsMatch(minNumberTB.Text, @"^-?\d+$"))
            {
                MessageBox.Show("Неправильно задано минимальное число.\nОно будет заменено на значение по умолчанию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                minNumberTB.Text = "-100";
                return Convert.ToInt32(minNumberTB.Text);
            }

            return Convert.ToInt32(minNumberTB.Text);
        }

        private int MaxNumber()
        {
            if (string.IsNullOrEmpty(maxNumberTB.Text) || !Regex.IsMatch(maxNumberTB.Text, @"^-?\d+$"))
            {
                MessageBox.Show("Неправильно задано максимальное число.\nОно будет заменено на значение по умолчанию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                maxNumberTB.Text = "100";
                return Convert.ToInt32(maxNumberTB.Text);
            }

            return Convert.ToInt32(maxNumberTB.Text);
        }

        private (double[], int) BubbleSort(double[] array)
        {
            int iterations = 1;

            if (ascendingSortRB.IsChecked == true)
            {
                for (int j = 1; j < array.Length; ++j)
                {
                    bool isSorted = true;

                    for (int i = 0; i < array.Length - j; ++i)
                    {
                        if (array[i] > array[i + 1])
                        {
                            (array[i], array[i + 1]) = (array[i + 1], array[i]);
                            isSorted = false;
                        }
                    }

                    if (isSorted)
                    {
                        break;
                    }

                    ++iterations;
                }
            }
            else
            {
                for (int j = 1; j < array.Length; ++j)
                {
                    bool isSorted = true;

                    for (int i = 0; i < array.Length - j; ++i)
                    {
                        if (array[i] < array[i + 1])
                        {
                            (array[i + 1], array[i]) = (array[i], array[i + 1]);
                            isSorted = false;
                        }

                        ++iterations;
                    }

                    if (isSorted)
                    {
                        break;
                    }
                }
            }

            return (array, iterations);
        }

        private (double[], int) InsertSort(double[] array)
        {
            int iterations = 1;

            if (ascendingSortRB.IsChecked == true)
            {
                for (int j = 1; j < array.Length; ++j)
                {
                    double key = array[j];
                    int i = j - 1;

                    while (i >= 0 && array[i] > key)
                    {
                        array[i + 1] = array[i];
                        --i;
                    }

                    array[i + 1] = key;
                    ++iterations;
                }
            }
            else
            {
                for (int j = 2; j < array.Length; ++j)
                {
                    double key = array[j];
                    int i = j - 1;

                    while (i >= 0 && array[i] < key)
                    {
                        array[i + 1] = array[i];
                        --i;
                    }

                    array[i + 1] = key;
                    ++iterations;
                }
            }

            return (array, iterations);
        }

        private (double[], int) QuickSort(double[] array, int minIndex, int maxIndex, int iterations = 0)
        {
            if (minIndex >= maxIndex)
            {
                return (array, iterations);
            }

            (int pivotIndex, int pivotIterations) = GetPivotIndex(array, minIndex, maxIndex);
            iterations += pivotIterations;
            ++iterations;

            (array, iterations) = QuickSort(array, minIndex, pivotIndex - 1, iterations);
            (array, iterations) = QuickSort(array, pivotIndex + 1, maxIndex, iterations);

            return (array, iterations);
        }

        private (int, int) GetPivotIndex(double[] array, int minIndex, int maxIndex)
        {
            int iterations = 0;
            int pivot = minIndex - 1;

            for (int i = minIndex; i <= maxIndex; ++i)
            {
                if (ascendingSortRB.IsChecked == true)
                {
                    if (array[i] < array[maxIndex])
                    {
                        ++pivot;
                        (array[pivot], array[i]) = (array[i], array[pivot]);
                    }
                }
                else
                {
                    if (array[i] > array[maxIndex])
                    {
                        ++pivot;
                        (array[pivot], array[i]) = (array[i], array[pivot]);
                    }
                }
            }

            ++iterations;
            ++pivot;
            (array[pivot], array[maxIndex]) = (array[maxIndex], array[pivot]);

            return (pivot, iterations);
        }

        private (double[], int) ShakerSort(double[] array)
        {
            int iterations = 0;
            int left = 0, right = array.Length - 1;
            bool flag = true;

            while (left < right && flag)
            {
                flag = false;

                for (int i = left; i < right; ++i)
                {
                    if (ascendingSortRB.IsChecked == true)
                    {
                        if (array[i] > array[i + 1])
                        {
                            (array[i], array[i + 1]) = (array[i + 1], array[i]);
                            flag = true;
                        }
                    }
                    else
                    {
                        if (array[i] < array[i + 1])
                        {
                            (array[i], array[i + 1]) = (array[i + 1], array[i]);
                            flag = true;
                        }
                    }
                }
                --right;

                for (int i = right; i > left; --i)
                {
                    if (ascendingSortRB.IsChecked == true)
                    {
                        if (array[i - 1] > array[i])
                        {
                            (array[i], array[i - 1]) = (array[i - 1], array[i]);
                            flag = true;
                        }
                    }
                    else
                    {
                        if (array[i - 1] < array[i])
                        {
                            (array[i], array[i - 1]) = (array[i - 1], array[i]);
                            flag = true;
                        }
                    }
                }

                ++left;
                ++iterations;

                if (!flag)
                {
                    break;
                }
            }

            return (array, iterations);
        }

        private (double[], int) BogoSort(double[] array, uint bogoSortIterations)
        {
            int iterations = 0;

            while (!IsSorted(array) && iterations < bogoSortIterations)
            {
                Random rnd = new Random();

                for (int i = 0; i < array.Length; ++i)
                {
                    int rndIndex = rnd.Next(array.Length);
                    (array[i], array[rndIndex]) = (array[rndIndex], array[i]);
                }

                ++iterations;
            }

            return (array, iterations);
        }

        private bool IsSorted(double[] array)
        {
            bool isSorted = true;

            for (int i = 0; i < array.Length - 1; ++i)
            {
                if (ascendingSortRB.IsChecked == true)
                {
                    if (array[i] > array[i + 1])
                    {
                        isSorted = false;
                    }
                }
                else
                {
                    if (array[i] < array[i + 1])
                    {
                        isSorted = false;
                    }
                }
            }

            return isSorted;
        }

        private byte InputType()
        {
            byte inputType = 0;

            if (manualInputRB.IsChecked == true)
            {
                inputType = 1;
            }
            if (generateRB.IsChecked == true)
            {
                inputType = 2;
            }
            if (fileInputRB.IsChecked == true)
            {
                inputType = 3;
            }

            return inputType;
        }

        private double[] ManualInputArray()
        {
            if (string.IsNullOrEmpty(manualInputTB.Text) || !Regex.IsMatch(manualInputTB.Text, @"^(-?\d+(\,\d+)?)(\s(-?\d+(\,\d+)?))*$") || manualInputTB.Text == "0")
            {
                MessageBox.Show("Неправильно введены элементы массива", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                manualInputTB.Text = "1 2 3 4 5 6 7 8 9 10";
                manualInputTB.Text.Split(' ').Select(double.Parse).ToArray();
            }

            return manualInputTB.Text.Split(' ').Select(double.Parse).ToArray();
        }

        private double[] GenerateArray()
        {
            uint size = GenerateElementsNumber();
            int minValue = MinNumber();
            int maxValue = MaxNumber();
            double[] generatedArray = new double[size];
            Random rnd = new Random();

            for (int i = 0; i < size; ++i)
            {
                if (doubleRB.IsChecked == true)
                {
                    generatedArray[i] = rnd.Next(minValue, maxValue) + Math.Round(rnd.NextDouble(), 3);
                }
                else
                {
                    generatedArray[i] = rnd.Next(minValue, maxValue);
                }
            }

            return generatedArray;
        }

        private double[] ArrayFromFile()
        {
            if (FilePath != null)
            {
                if (Path.GetExtension(FilePath) == ".xlsx")
                {
                    var workbook = new XLWorkbook(FilePath);
                    var worksheet = workbook.Worksheet(1);
                    double[] array = new double[worksheet.LastRowUsed().RowNumber()];

                    for (int i = 1; i <= worksheet.LastRowUsed().RowNumber(); ++i)
                    {
                        string cellValue = worksheet.Cell(i, 1).GetValue<string>();

                        if (cellValue != null && Regex.IsMatch(cellValue, @"^-?\d+(\,\d+)?$"))
                        {
                            array[i - 1] = Convert.ToDouble(cellValue);
                        }
                    }

                    return array;
                }
                else if (Path.GetExtension(FilePath) == ".txt")
                {
                    string[] lines = File.ReadAllLines(FilePath);
                    double[] array = new double[lines.Length];

                    for (int i = 0; i < lines.Length; ++i)
                    {
                        string value = lines[i];

                        if (value != null && Regex.IsMatch(value, @"^-?\d+(\,\d+)?$"))
                        {
                            array[i] = Convert.ToDouble(value);
                        }
                    }

                    return array;
                }
            }
            
            return null;
        }

        private void StartBubbleSort(double[] array)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            (double[] sortedArray, int iterations) = BubbleSort(array);
            sw.Stop();
            double time = sw.Elapsed.TotalMilliseconds;

            Results.Add(new ResultRow
            {
                NameOfSort = "Пузырьковая",
                SortedArray = string.Join(" ", sortedArray),
                Iterations = iterations,
                Time = time
            });
        }

        private void StartInsertSort(double[] array)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            (double[] sortedArray, int iterations) = InsertSort(array);
            sw.Stop();
            double time = sw.Elapsed.TotalMilliseconds;

            Results.Add(new ResultRow
            {
                NameOfSort = "Вставками",
                SortedArray = string.Join(" ", sortedArray),
                Iterations = iterations,
                Time = time
            });
        }

        private void StartQuickSort(double[] array)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            (double[] sortedArray, int iterations) = QuickSort(array, 0, array.Length - 1);
            sw.Stop();
            double time = sw.Elapsed.TotalMilliseconds;

            Results.Add(new ResultRow
            {
                NameOfSort = "Быстрая",
                SortedArray = string.Join(" ", sortedArray),
                Iterations = iterations,
                Time = time
            });
        }

        private void StartShakerSort(double[] array)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            (double[] sortedArray, int iterations) = ShakerSort(array);
            sw.Stop();
            double time = sw.Elapsed.TotalMilliseconds;

            Results.Add(new ResultRow
            {
                NameOfSort = "Шейкерная",
                SortedArray = string.Join(" ", sortedArray),
                Iterations = iterations,
                Time = time
            });
        }

        private void StartBogoSort(double[] array)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            (double[] sortedArray, int iterations) = BogoSort(array, BogoSortIterations());
            sw.Stop();
            double time = sw.Elapsed.TotalMilliseconds;

            Results.Add(new ResultRow
            {
                NameOfSort = "Болотная",
                SortedArray = string.Join(" ", sortedArray),
                Iterations = iterations,
                Time = time
            });
        }

        private void ManualSorting()
        {
            Results.Clear();
            double[] inputArray = ManualInputArray();
            Results.Add(new ResultRow
            {
                NameOfSort = "Массив",
                SortedArray = string.Join(" ", inputArray)
            });

            if (bubbleSortCB.IsChecked == true)
            {
                double[] arrayForSort = new double[inputArray.Length];
                inputArray.CopyTo(arrayForSort, 0);
                StartBubbleSort(arrayForSort);
            }
            if (insertSortCB.IsChecked == true)
            {
                double[] arrayForSort = new double[inputArray.Length];
                inputArray.CopyTo(arrayForSort, 0);
                StartInsertSort(arrayForSort);
            }
            if (quickSortCB.IsChecked == true)
            {
                double[] arrayForSort = new double[inputArray.Length];
                inputArray.CopyTo(arrayForSort, 0);
                StartQuickSort(arrayForSort);
            }
            if (shakerSortCB.IsChecked == true)
            {
                double[] arrayForSort = new double[inputArray.Length];
                inputArray.CopyTo(arrayForSort, 0);
                StartShakerSort(arrayForSort);
            }
            if (bogoSortCB.IsChecked == true)
            {
                double[] arrayForSort = new double[inputArray.Length];
                inputArray.CopyTo(arrayForSort, 0);
                StartBogoSort(arrayForSort);
            }
        }

        private void GenerateSorting()
        {
            Results.Clear();
            double[] inputArray = GenerateArray();
            Results.Add(new ResultRow
            {
                NameOfSort = "Массив",
                SortedArray = string.Join(" ", inputArray)
            });

            if (bubbleSortCB.IsChecked == true)
            {
                double[] arrayForSort = new double[inputArray.Length];
                inputArray.CopyTo(arrayForSort, 0);
                StartBubbleSort(arrayForSort);
            }
            if (insertSortCB.IsChecked == true)
            {
                double[] arrayForSort = new double[inputArray.Length];
                inputArray.CopyTo(arrayForSort, 0);
                StartInsertSort(arrayForSort);
            }
            if (quickSortCB.IsChecked == true)
            {
                double[] arrayForSort = new double[inputArray.Length];
                inputArray.CopyTo(arrayForSort, 0);
                StartQuickSort(arrayForSort);
            }
            if (shakerSortCB.IsChecked == true)
            {
                double[] arrayForSort = new double[inputArray.Length];
                inputArray.CopyTo(arrayForSort, 0);
                StartShakerSort(arrayForSort);
            }
            if (bogoSortCB.IsChecked == true)
            {
                double[] arrayForSort = new double[inputArray.Length];
                inputArray.CopyTo(arrayForSort, 0);
                StartBogoSort(arrayForSort);
            }
        }

        private void FileSorting()
        {
            double[] inputArray = ArrayFromFile();

            if (inputArray != null)
            {
                Results.Clear();
                Results.Add(new ResultRow
                {
                    NameOfSort = "Массив",
                    SortedArray = string.Join(" ", inputArray)
                });

                if (bubbleSortCB.IsChecked == true)
                {
                    double[] arrayForSort = new double[inputArray.Length];
                    inputArray.CopyTo(arrayForSort, 0);
                    StartBubbleSort(arrayForSort);
                }
                if (insertSortCB.IsChecked == true)
                {
                    double[] arrayForSort = new double[inputArray.Length];
                    inputArray.CopyTo(arrayForSort, 0);
                    StartInsertSort(arrayForSort);
                }
                if (quickSortCB.IsChecked == true)
                {
                    double[] arrayForSort = new double[inputArray.Length];
                    inputArray.CopyTo(arrayForSort, 0);
                    StartQuickSort(arrayForSort);
                }
                if (shakerSortCB.IsChecked == true)
                {
                    double[] arrayForSort = new double[inputArray.Length];
                    inputArray.CopyTo(arrayForSort, 0);
                    StartShakerSort(arrayForSort);
                }
                if (bogoSortCB.IsChecked == true)
                {
                    double[] arrayForSort = new double[inputArray.Length];
                    inputArray.CopyTo(arrayForSort, 0);
                    StartBogoSort(arrayForSort);
                }
            }
            else
            {
                MessageBox.Show("Не выбран файл", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ResultRow
    {
        public string NameOfSort { get; set; }
        public string SortedArray { get; set; }
        public int Iterations { get; set; }
        public double Time { get; set; }
    }
}