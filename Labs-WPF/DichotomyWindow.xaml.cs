using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using org.mariuszgromada.math.mxparser;
using Expression = org.mariuszgromada.math.mxparser.Expression;
using OxyPlot.Series;
using OxyPlot;

namespace Labs_WPF
{
    /// <summary>
    /// Логика взаимодействия для DichotomyWindow.xaml
    /// </summary>
    public partial class DichotomyWindow : Window
    {
        private Expression expression;
        private Function function;
        private int precision;
        private bool isGraphPlotted = false;

        public DichotomyWindow()
        {
            InitializeComponent();
        }

        private void plotBtn_Click(object sender, RoutedEventArgs e)
        {
            PlotGraph();
        }

        private void calculateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!isGraphPlotted)
            {
                PlotGraph();
            }

            if (IsTextValid())
            {
                var output = DichotomyMethod(function, leftRestriction(), rightRestriction(), epsilon());
                ShowResult(output.Item1, output.Item2);
            }
        }

        private double leftRestriction()
        {
            return Convert.ToDouble(tbA.Text.Replace(".", ","));
        }

        private double rightRestriction()
        {
            return Convert.ToDouble(tbB.Text.Replace(".", ","));
        }

        private double epsilon()
        {
            if (tbE.Text.Replace(".", ",").Contains(","))
            {
                MessageBox.Show("Неправильно задано значение E, оно будет заменено на значение по умолчанию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return 3;
            }

            precision = Convert.ToInt16(tbE.Text.Replace(".", ","));

            if (precision < 0)
            {
                MessageBox.Show("Неправильно задано значение E, оно будет заменено на значение по умолчанию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return 3;
            }
            else if (precision > 13)
            {
                MessageBox.Show("Слишком большое значение E, оно будет заменено на значение по умолчанию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return 3;
            }
            else
            {
                return Math.Pow(10, -precision);
            }
        }

        private void functionTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            isGraphPlotted = false;
        }

        private double SolveFunction(Function function, string x)
        {
            return new Expression($"f({x})", function).calculate();
        }

        private void ShowResult(double result, bool error)
        {
            if (!error)
            {
                double resultValue = SolveFunction(function, result.ToString().Replace(",", "."));
                resultValue = Math.Round(resultValue, precision);
                result = Math.Round(result, precision);
                MessageBox.Show($"x = {result}\nf(x) = {resultValue}", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("В заданном интревале отсутствует корень", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void PlotGraph()
        {
            int left = Convert.ToInt32(tbA.Text);
            int right = Convert.ToInt32(tbB.Text);
            List<DataPoint> dot = new List<DataPoint>();

            if (left < 5 && left > -5)
            {
                left = -5;
            }
            if (right < 5 && right > -5)
            {
                right = 5;
            }

            var plotModel = new PlotModel { Title = "График функции" };

            var absicc = new LineSeries
            {
                Title = "Ось абсцисс",
                Color = OxyColor.FromRgb(0, 0, 0),
                StrokeThickness = 2
            };

            absicc.Points.Add(new DataPoint(left, 0));
            absicc.Points.Add(new DataPoint(right, 0));

            var ordinate = new LineSeries
            {
                Title = "Ось ординат",
                Color = OxyColor.FromRgb(0, 0, 0),
                StrokeThickness = 2,
            };

            ordinate.Points.Add(new DataPoint(0, right));
            ordinate.Points.Add(new DataPoint(0, left));

            var lineSeries = new LineSeries
            {
                Title = "f(x)",
                Color = OxyColor.FromRgb(0, 255, 0)
            };

            function = new Function("f(x) = " + functionTB.Text);

            for (int pointIndex = left; pointIndex <= right; ++pointIndex)
            {
                expression = new Expression($"f({pointIndex})", function);
                double y = expression.calculate();
                dot.Add(new DataPoint(pointIndex, y));
            }

            lineSeries.Points.AddRange(dot);
            plotModel.Series.Add(lineSeries);
            plotModel.Series.Add(ordinate);
            plotModel.Series.Add(absicc);

            this.graph.Model = plotModel;
            isGraphPlotted = true;
        }

        private (double, bool) DichotomyMethod(Function function, double leftRestriction, double rightRestriction, double epsilon)
        {
            bool error = false;
            double current = double.NaN;
            double leftValue = SolveFunction(function, leftRestriction.ToString().Replace(",", "."));
            double rightValue = SolveFunction(function, rightRestriction.ToString().Replace(",", "."));

            if (double.IsNaN(leftValue) || double.IsNaN(rightValue))
            {
                error = true;
                return (current, error);
            }
            else if (leftValue * rightValue >= 0)
            {
                error = true;
                return (current, error);
            }

            while ((rightRestriction - leftRestriction) > epsilon)
            {
                current = (leftRestriction + rightRestriction) / 2;
                double position = SolveFunction(function, current.ToString().Replace(",", "."));

                if (Math.Abs(position) == 0)
                {
                    return (current, error);
                }
                else if (leftValue * position < 0)
                {
                    rightRestriction = current;
                }
                else
                {
                    leftRestriction = current;
                    leftValue = position;
                }
            }

            return (current, error);
        }

        private bool IsTextValid()
        {
            Regex regex = new Regex(@"^[\d,.-]+$");
            bool result = true;

            if (string.IsNullOrEmpty(tbA.Text) || !regex.IsMatch(tbA.Text))
            {
                result = false;
                MessageBox.Show("Неправильно задана точка A", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (string.IsNullOrEmpty(tbB.Text) || !regex.IsMatch(tbB.Text))
            {
                result = false;
                MessageBox.Show("Неправильно задана точка B", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (string.IsNullOrEmpty(tbE.Text) || !regex.IsMatch(tbE.Text))
            {
                result = false;
                MessageBox.Show("Неправильно задано значение E", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return result;
        }
    }
}