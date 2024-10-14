using System;
using System.Windows;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using org.mariuszgromada.math.mxparser;
using Expression = org.mariuszgromada.math.mxparser.Expression;
using OxyPlot.Series;
using OxyPlot;

namespace Labs_WPF
{
    /// <summary>
    /// Логика взаимодействия для GoldenRatioWindow.xaml
    /// </summary>
    public partial class GoldenRatioWindow : Window
    {
        private Expression expression;
        private Function function;
        private int precision;
        private bool isGraphPlotted = false;

        public GoldenRatioWindow()
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
                var output = GoldenRatioMethod(function, leftRestriction(), rightRestriction(), epsilon());
                ShowResult(output);
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

        private double SolveFunction(Function function, string x)
        {
            return new Expression($"f({x})", function).calculate();
        }

        private void ShowResult(double result)
        {
            double resultValue = SolveFunction(function, result.ToString().Replace(",", "."));
            resultValue = Math.Round(resultValue, precision);
            resultValue = Math.Abs(resultValue);
            result = Math.Round(result, precision);
            MessageBox.Show($"x = {result}\nf(x) = {resultValue}", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PlotGraph()
        {
            List<DataPoint> dot = new List<DataPoint>();

            var plotModel = new PlotModel { Title = "График функции" };

            var absicc = new LineSeries
            {
                Title = "Ось абсцисс",
                Color = OxyColor.FromRgb(0, 0, 0),
                StrokeThickness = 2
            };

            absicc.Points.Add(new DataPoint(-100, 0));
            absicc.Points.Add(new DataPoint(100, 0));

            var ordinate = new LineSeries
            {
                Title = "Ось ординат",
                Color = OxyColor.FromRgb(0, 0, 0),
                StrokeThickness = 2,
            };

            ordinate.Points.Add(new DataPoint(0, 100));
            ordinate.Points.Add(new DataPoint(0, -100));

            var lineSeries = new LineSeries
            {
                Title = "f(x)",
                Color = OxyColor.FromRgb(0, 255, 0)
            };

            function = new Function("f(x) = " + functionTB.Text);

            for (int pointIndex = -200; pointIndex <= 200; ++pointIndex)
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

        public double GoldenRatioMethod(Function function, double leftRestriction, double rightRestriction, double epsilon)
        {
            if (maxBtn.IsChecked == true)
            {
                function = new Function("f(x) = " + "-(" + functionTB.Text + ")");
            }

            double result = double.NaN;

            double leftValue = SolveFunction(function, leftRestriction.ToString().Replace(",", "."));
            double rightValue = SolveFunction(function, rightRestriction.ToString().Replace(",", "."));

            double d = (Math.Sqrt(5) - 1) / 2;

            double xFirst = rightRestriction - d * (rightRestriction - leftRestriction);
            double xSecond = leftRestriction + d * (rightRestriction - leftRestriction);

            double firstResult = SolveFunction(function, xFirst.ToString().Replace(",", "."));
            double secondResult = SolveFunction(function, xSecond.ToString().Replace(",", "."));


            while (Math.Abs(rightRestriction - leftRestriction) > epsilon)
            {
                if (firstResult < secondResult)
                {
                    rightRestriction = xSecond;
                    xSecond = xFirst;
                    xFirst = rightRestriction - d * (rightRestriction - leftRestriction);
                    firstResult = SolveFunction(function, xFirst.ToString().Replace(",", "."));
                    secondResult = SolveFunction(function, xSecond.ToString().Replace(",", "."));
                }
                else
                {
                    leftRestriction = xFirst;
                    xFirst = xSecond;
                    xSecond = leftRestriction + d * (rightRestriction - leftRestriction);
                    firstResult = SolveFunction(function, xFirst.ToString().Replace(",", "."));
                    secondResult = SolveFunction(function, xSecond.ToString().Replace(",", "."));
                }
            }

            result = (leftRestriction + rightRestriction) / 2;

            return result;
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