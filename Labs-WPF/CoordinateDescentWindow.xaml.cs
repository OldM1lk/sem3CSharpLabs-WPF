using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Expression = org.mariuszgromada.math.mxparser.Expression;
using Function = org.mariuszgromada.math.mxparser.Function;
using Window = System.Windows.Window;

namespace Labs_WPF
{
    /// <summary>
    /// Логика взаимодействия для CoordinateDescentWindow.xaml
    /// </summary>
    public partial class CoordinateDescentWindow : Window
    {
        private Expression expression;
        private Function function;
        private int precision;
        private bool isGraphPlotted = false;

        public CoordinateDescentWindow()
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
                var output = CoordinateDescentMethod(function, leftRestriction(), rightRestriction(), epsilon());
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

        //private int maxIteration()
        //{
        //    if (IsTextValid())
        //    {
        //        return Convert.ToInt32(tbIteration.Text);
        //    }
        //    else
        //    {
        //        return 1000;
        //    }
        //}

        private void functionTB_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            isGraphPlotted = false;
        }

        private void PlotGraph()
        {
            if (IsTextValid())
            {
                double left = leftRestriction();
                double right = rightRestriction();
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

                function = new Function("f(x) = " + functionTB.Text);

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

        private void ShowResult(double result)
        {
            double resultValue = SolveFunction(function, result.ToString().Replace(",", "."));
            resultValue = Math.Round(resultValue, precision);
            result = Math.Round(result, precision);
            MessageBox.Show($"x = {result}\nf(x) = {resultValue}", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool IsTextValid()
        {
            Regex regex = new Regex(@"^[\d,.-]+$");
            bool result = true;

            if (string.IsNullOrEmpty(tbA.Text) || !regex.IsMatch(tbA.Text) || tbA.Text.Count(f => f == '-') > 1)
            {
                result = false;
                MessageBox.Show("Неправильно задана точка A", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (string.IsNullOrEmpty(tbB.Text) || !regex.IsMatch(tbB.Text) || tbB.Text.Count(f => f == '-') > 1)
            {
                result = false;
                MessageBox.Show("Неправильно задана точка B", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (string.IsNullOrEmpty(tbE.Text) || !regex.IsMatch(tbE.Text) || tbE.Text.Contains(".") || tbE.Text.Contains(",") || tbE.Text.Contains("-"))
            {
                result = false;
                MessageBox.Show("Неправильно задано значение E", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Convert.ToDouble(tbA.Text.Replace(".", ",")) > Convert.ToDouble(tbB.Text.Replace(".", ",")) || Convert.ToDouble(tbA.Text.Replace(".", ",")) == Convert.ToDouble(tbB.Text.Replace(".", ",")))
            {
                result = false;
                MessageBox.Show("Неправильно задано значение A или B", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (string.IsNullOrEmpty(tbIteration.Text) || !regex.IsMatch(tbIteration.Text) || tbIteration.Text.Contains(".") || tbIteration.Text.Contains(",") || tbIteration.Text.Contains("-"))
            {
                result = false;
                MessageBox.Show("Неправильно задано кол-во итераций", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return result;
        }

        private double CoordinateDescentMethod(Function function, double leftRestriction, double rightRestriction, double epsilon)
        {
            double current;
            double step = epsilon;
            int iteration = 1;
            int maxIterations = Convert.ToInt32(tbIteration.Text);

            if (maxBtn.IsChecked == true)
            {
                if (SolveFunction(function, leftRestriction.ToString().Replace(",", ".")) > SolveFunction(function, rightRestriction.ToString().Replace(",", ".")))
                {
                    current = leftRestriction;
                }
                else
                {
                    current = rightRestriction;
                }

                function = new Function("f(x) = " + "-(" + functionTB.Text + ")");
            }
            else
            {
                if (SolveFunction(function, leftRestriction.ToString().Replace(",", ".")) > SolveFunction(function, rightRestriction.ToString().Replace(",", ".")))
                {
                    current = rightRestriction;
                }
                else
                {
                    current = leftRestriction;
                }
            }

            while (iteration < maxIterations)
            {
                double currentValue = SolveFunction(function, current.ToString().Replace(",", "."));
                double leftX = current - step;
                double leftValue = SolveFunction(function, leftX.ToString().Replace(",", "."));
                double rightX = current + step;
                double rightValue = SolveFunction(function, rightX.ToString().Replace(",", "."));

                if (leftValue <= currentValue)
                {
                    current = leftX;
                }
                else
                {
                    current = rightX;
                }

                if (current < leftRestriction)
                {
                    return leftRestriction;
                }
                if (current > rightRestriction)
                {
                    return rightRestriction;
                }

                ++iteration;
            }

            return current;
        }
    }
}