using System;
using OxyPlot;
using System.Linq;
using System.Windows;
using OxyPlot.Series;
using OxyPlot.Annotations;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using org.mariuszgromada.math.mxparser;
using Expression = org.mariuszgromada.math.mxparser.Expression;

namespace Labs_WPF
{
    /// <summary>
    /// Логика взаимодействия для IntegralWindow.xaml
    /// </summary>
    public partial class IntegralWindow : Window
    {
        private Expression expression;
        private Function function;
        private int precision;
        private bool isGraphPlotted = false;

        public IntegralWindow()
        {
            InitializeComponent();
        }

        private double LowerLimit()
        {
            return Convert.ToDouble(lowerLimitTB.Text.Replace(",", "."));
        }

        private double UpperLimit()
        {
            return Convert.ToDouble(upperLimitTB.Text.Replace(",", "."));
        }

        private uint PartitionsCount()
        {
            return Convert.ToUInt32(partitionsCountTB.Text);
        }

        private int Epsilon()
        {
            if (tbE.Text.Replace(",", ".").Contains(","))
            {
                MessageBox.Show("Неправильно задано значение E, оно будет заменено на значение по умолчанию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return 3;
            }

            precision = Convert.ToInt16(tbE.Text);

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

            return precision;
        }

        private void ShowResult(List<string> output)
        {
            MessageBox.Show($"{string.Join("\n", output)}", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void calculateBtn_Click(object sender, RoutedEventArgs e)
        {
            List<string> results = new List<string>();

            if (!isGraphPlotted)
            {
                PlotGraph();
                isGraphPlotted = true;
            }

            if (IsValuesValid())
            {
                if (rectangleMethodCB.IsChecked == true)
                {
                    if (rectangleVisualizationCB.IsChecked == true)
                    {
                        RectangleVisualization(function, LowerLimit(), UpperLimit(), PartitionsCount());
                    }
                    double rectangleResult = RectangleMethod(function, LowerLimit(), UpperLimit(), PartitionsCount());
                    results.Add($"Метод прямоугольников: {Math.Round(rectangleResult, Epsilon())}");
                }
                if (trapezoidMethodCB.IsChecked == true)
                {
                    if (trapezoidVisualizationCB.IsChecked == true)
                    {
                        TrapezoidVisualization(function, LowerLimit(), UpperLimit(), PartitionsCount());
                    }
                    double trapezoidResult = TrapezoidMethod(function, LowerLimit(), UpperLimit(), PartitionsCount());
                    results.Add($"Метод трапеций: {Math.Round(trapezoidResult, Epsilon())}");
                }
                if (simpsonMethodCB.IsChecked == true)
                {
                    if (simpsonVisualizationCB.IsChecked == true)
                    {

                    }
                    double simpsonResult = SimpsonMethod(function, LowerLimit(), UpperLimit(), PartitionsCount());
                    results.Add($"Метод симпсона: {Math.Round(simpsonResult, Epsilon())}");
                }

                ShowResult(results);
            }
        }

        private void plotBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!isGraphPlotted)
            {
                PlotGraph();
                isGraphPlotted = true;
            }
        }

        private void integralTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            isGraphPlotted = false;
        }

        private bool IsValuesValid()
        {
            Regex regex = new Regex(@"^[\d,.-]+$");
            bool result = true;

            if (string.IsNullOrEmpty(lowerLimitTB.Text) || !regex.IsMatch(lowerLimitTB.Text) || lowerLimitTB.Text.Count(f => f == '-') > 1)
            {
                result = false;
                MessageBox.Show("Неправильно задана точка A", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (string.IsNullOrEmpty(upperLimitTB.Text) || !regex.IsMatch(upperLimitTB.Text) || upperLimitTB.Text.Count(f => f == '-') > 1)
            {
                result = false;
                MessageBox.Show("Неправильно задана точка B", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (string.IsNullOrEmpty(tbE.Text) || !regex.IsMatch(tbE.Text) || tbE.Text.Contains(".") || tbE.Text.Contains(",") || partitionsCountTB.Text.Contains("-"))
            {
                result = false;
                MessageBox.Show("Неправильно задано значение E", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (string.IsNullOrEmpty(partitionsCountTB.Text) || !regex.IsMatch(partitionsCountTB.Text) || partitionsCountTB.Text.Contains(".") || partitionsCountTB.Text.Contains(",") || partitionsCountTB.Text.Contains("-"))
            {
                result = false;
                MessageBox.Show("Неправильно задано кол-во разбиений", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Convert.ToDouble(lowerLimitTB.Text.Replace(",", ".")) > Convert.ToDouble(upperLimitTB.Text.Replace(",", ".")) || Convert.ToDouble(lowerLimitTB.Text.Replace(",", ".")) == Convert.ToDouble(upperLimitTB.Text.Replace(",", ".")))
            {
                result = false;
                MessageBox.Show("Неправильно задано значение A или B", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return result;
        }

        private void PlotGraph()
        {
            if (IsValuesValid())
            {                
                double left = LowerLimit();
                double right = UpperLimit();
                List<DataPoint> dot = new List<DataPoint>();

                if (left < 5 && left > -5)
                {
                    left = -5;
                }
                if (right < 5 && right > -5)
                {
                    right = 5;
                }

                var plotModel = new PlotModel { Title = "График интергала" };

                var absicc = new LineSeries
                {
                    Title = "Ось абсцисс",
                    Color = OxyColors.Black,
                    StrokeThickness = 2
                };

                absicc.Points.Add(new DataPoint(left, 0));
                absicc.Points.Add(new DataPoint(right, 0));

                var ordinate = new LineSeries
                {
                    Title = "Ось ординат",
                    Color = OxyColors.Black,
                    StrokeThickness = 2,
                };

                ordinate.Points.Add(new DataPoint(0, right));
                ordinate.Points.Add(new DataPoint(0, left));

                var lineSeries = new LineSeries
                {
                    Title = "f(x)",
                    Color = OxyColors.Green
                };

                function = new Function("f(x) = " + integralTB.Text);

                for (double pointIndex = left; pointIndex <= right; ++pointIndex)
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
        }

        private double SolveFunction(Function function, string x)
        {
            return new Expression($"f({x})", function).calculate();
        }

        private double RectangleMethod(Function function, double a, double b, uint n)
        {
            double result = 0;
            double h = (b - a) / n;
            double current = a;
            double currentResult;
            string x;

            while (current <= b - h)
            {
                x = current.ToString().Replace(",", ".");
                currentResult = SolveFunction(function, x) * h;

                result += Math.Abs(currentResult);
                current += h;
            }

            return result;
        }

        private void RectangleVisualization(Function function, double a, double b, uint n)
        {
            var model = graph.Model;
            double h = (b - a) / n;
            double current = a;
            double currentResult;
            string x;

            while (current <= b - h)
            {
                x = current.ToString().Replace(",", ".");
                currentResult = SolveFunction(function, x);

                var rectangleLineSeries = new LineSeries
                {
                    Color = OxyColors.Blue
                };

                rectangleLineSeries.Points.Add(new DataPoint(current, 0));
                rectangleLineSeries.Points.Add(new DataPoint(current, currentResult));
                rectangleLineSeries.Points.Add(new DataPoint(current + h, currentResult));
                rectangleLineSeries.Points.Add(new DataPoint(current + h, 0));

                model.Series.Add(rectangleLineSeries);
                isGraphPlotted = false;
                this.graph.Model = model;

                current += h;
            }
        }

        private double TrapezoidMethod(Function function, double a, double b, uint n)
        {
            double result = 0;
            double h = (b - a) / n;
            double current = a + h;
            string x1;
            string x2;
            double function1;
            double function2;

            while (current <= b)
            {
                x1 = (current - h).ToString().Replace(",", ".");
                x2 = current.ToString().Replace(",", ".");
                function1 = SolveFunction(function, x1);
                function2 = SolveFunction(function, x2);

                result += Math.Abs((function1 + function2) * h / 2);
                current += h;
            }

            return result;
        }

        private void TrapezoidVisualization(Function function, double a, double b, uint n)
        {
            var model = graph.Model;
            double h = (b - a) / n;
            double current = a + h;
            string x1;
            string x2;
            double function1;
            double function2;

            while (current <= b)
            {
                x1 = (current - h).ToString().Replace(",", ".");
                x2 = current.ToString().Replace(",", ".");
                function1 = SolveFunction(function, x1);
                function2 = SolveFunction(function, x2);

                var trapezoid = new PolygonAnnotation
                {
                    Fill = OxyColors.LightBlue,
                    Points =
                    {
                        new DataPoint(current - h, 0),
                        new DataPoint(current, 0),
                        new DataPoint(current, 0),
                        new DataPoint(current, function2),
                        new DataPoint(current - h, function1)
                    }
                };

                var trapezoidLineSeries = new LineSeries
                {
                    Color = OxyColors.Blue
                };

                trapezoidLineSeries.Points.Add(new DataPoint(current - h, 0));
                trapezoidLineSeries.Points.Add(new DataPoint(current - h, function1));
                trapezoidLineSeries.Points.Add(new DataPoint(current, function2));
                trapezoidLineSeries.Points.Add(new DataPoint(current, 0));
                                
                model.Series.Add(trapezoidLineSeries);
                isGraphPlotted = false;
                this.graph.Model = model;

                current += h;
            }
        }

        private double SimpsonMethod(Function function, double a, double b, uint n)
        {
            if (n % 2 != 0)
            {
                n += 1;
            }

            double result;
            double evenResult = 0;
            double oddResult = 0;
            double h = (b - a) / n;
            double current;
            double currentResult;
            string x;

            for (int evenIndex = 0; evenIndex < n; evenIndex += 2)
            {
                current = a + evenIndex * h;
                x = current.ToString().Replace(",", ".");
                currentResult = SolveFunction(function, x);

                evenResult += Math.Abs(currentResult);
            }

            evenResult *= 2;

            for (int oddIndex = 1; oddIndex < n; oddIndex += 2)
            {
                current = a + oddIndex * h;
                x = current.ToString().Replace(",", ".");
                currentResult = SolveFunction(function, x);

                oddResult += Math.Abs(currentResult);
            }

            oddResult *= 4;

            result = h / 3 * (evenResult + oddResult);
            return result;
        }
    }
}