using System;
using System.Windows;
using org.mariuszgromada.math.mxparser;
using OxyPlot;
using OxyPlot.Series;
using Expression = org.mariuszgromada.math.mxparser.Expression;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

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
            return Convert.ToDouble(lowerLimitTB.Text.Replace(".", ","));
        }

        private double UpperLimit()
        {
            return Convert.ToDouble(upperLimitTB.Text.Replace(".", ","));
        }

        private double PartitionsCount()
        {
            return Convert.ToDouble(partitionsCountTB.Text);
        }

        private double Epsilon()
        {
            if (tbE.Text.Replace(".", ",").Contains(","))
            {
                MessageBox.Show("Неправильно задано значение E, оно будет заменено на значение по умолчанию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return 3;
            }

            precision = Convert.ToUInt16(tbE.Text);

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
            else if (Convert.ToDouble(lowerLimitTB.Text.Replace(".", ",")) > Convert.ToDouble(upperLimitTB.Text.Replace(".", ",")) || Convert.ToDouble(lowerLimitTB.Text.Replace(".", ",")) == Convert.ToDouble(upperLimitTB.Text.Replace(".", ",")))
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


    }
}
