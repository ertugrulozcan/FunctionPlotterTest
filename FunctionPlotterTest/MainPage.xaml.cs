using HesapMakinesi.Calculator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace FunctionPlotterTest
{
    /// <summary>
    /// FunctionPlotter
    /// @Test
    /// Ahmet Ertuğrul Özcan
    /// 2015
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        #region Sabitler

        private readonly SolidColorBrush AXIS_LINES_COLOR = new SolidColorBrush(Color.FromArgb(255, 164, 164, 164));
        private readonly SolidColorBrush PLANE_LINES_COLOR = new SolidColorBrush(Color.FromArgb(255, 77, 77, 77));
        private readonly SolidColorBrush FUNCTION1_COLOR = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush FUNCTION2_COLOR = new SolidColorBrush(Colors.Green);
        private readonly SolidColorBrush FUNCTION3_COLOR = new SolidColorBrush(Colors.Blue);
        private readonly string PARAMETER1_SYMBOL = "Χ";
        
        // 1 birimlik aralığın alabileceği min değer. (pixel)
        private readonly double MIN_INTERVAL = 50;
        private readonly double MIN_DRAWING_INTERVAL = 40;

        #endregion

        #region Değişkenler

        private string equationString;
        /// <summary>
        /// Çizimi yapılacak fonksiyon
        /// </summary>
        public string EquationString
        {
            get { return equationString; }
            set
            { 
                equationString = value;
                this.NotifyPropertyChanged("EquationString");
            }
        }

        /// <summary>
        /// Fonksiyon1'in grafiği
        /// </summary>
        private Polyline graph1 = new Polyline();

        /// <summary>
        /// Yatay ve dikeyde zoom oranına göre iki çizgi arasındaki aralık değerlerini belirleyen katsayılar
        /// </summary>
        private double horizontalFactor = 1;
        public double HorizontalFactor
        {
            get { return horizontalFactor; }
            set
            { 
                horizontalFactor = value;
                if (horizontalFactor < 1)
                    horizontalFactor = 1;

                this.NotifyPropertyChanged("HorizontalFactor");
            }
        }

        private double verticalFactor = 1;
        public double VerticalFactor
        {
            get { return verticalFactor; }
            set
            { 
                verticalFactor = value;
                if (verticalFactor < 1)
                    verticalFactor = 1;

                this.NotifyPropertyChanged("VerticalFactor");
            }
        }

        private double zoomLevel = 1;
        /// <summary>
        /// Yakınlaştırma katsayısı
        /// </summary>
        public double ZoomLevel
        {
            get { return zoomLevel; }
            set
            { 
                zoomLevel = value;
                if (zoomLevel < 1)
                    zoomLevel = 1;

                this.NotifyPropertyChanged("ZoomLevel");
            }
        }

        private double drawingSensibility = 10;
        /// <summary>
        /// Fonksiyon çizim hassasiyeti
        /// </summary>
        public double DrawingSensibility
        {
            get { return drawingSensibility; }
            set
            { 
                drawingSensibility = value;
                if (drawingSensibility < 1)
                    this.drawingSensibility = 1;

                this.NotifyPropertyChanged("DrawingSensibility");
            }
        }

        #endregion

        #region Kurucu Metod - Constructor

        /// <summary>
        /// Kurucu Metod - Constructor
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;

            this.EquationString = "sin(Χ)";

            this.Loaded += (s, e) =>
                {
                    this.DrawCoordinatePlane();
                    this.DrawFunction();
                };
        }

        #endregion

        #region Koordinat düzleminin çizimi

        /// <summary>
        /// Koordinat düzlemini çizer
        /// </summary>
        private void DrawCoordinatePlane()
        {
            this.CoordinatePlaneCanvas.Children.Clear();

            //
            // X ekseninin çizimi
            //
            Line axisX = new Line()
            {
                StrokeThickness = 2,
                Stroke = this.AXIS_LINES_COLOR,

                X1 = 0,
                Y1 = this.GraphicBaseGrid.ActualHeight / 2,
                X2 = this.GraphicBaseGrid.ActualWidth,
                Y2 = this.GraphicBaseGrid.ActualHeight / 2,
            };
            this.CoordinatePlaneCanvas.Children.Add(axisX);

            //
            // Y ekseninin çizimi
            //
            Line axisY = new Line()
            {
                StrokeThickness = 2,
                Stroke = this.AXIS_LINES_COLOR,

                X1 = this.GraphicBaseGrid.ActualWidth / 2,
                Y1 = 0,
                X2 = this.GraphicBaseGrid.ActualWidth / 2,
                Y2 = this.GraphicBaseGrid.ActualHeight,
            };
            this.CoordinatePlaneCanvas.Children.Add(axisY);

            double intervalX = this.CalculateHorizontalInterval();
            double intervalY = this.CalculateVerticalInterval();

            //
            // Yatay çizgilerin çizimi
            //
            int miny = (int)((this.GraphicBaseGrid.ActualHeight / 2) / intervalY);
            int skipCountY = (int)(this.MIN_DRAWING_INTERVAL / intervalY) + 1;
            for(int i = -miny; i <= miny; i++)
            {
                if (i == 0)
                    continue;

                // Yeterinden fazla sıklıkta çizgi çizilmemesi için kaç sıra çizginin atlanacağı hesaplanır;
                if (i % skipCountY == 0)
                {
                    this.CoordinatePlaneCanvas.Children.Add(new Line()
                    {
                        StrokeThickness = 0.5,
                        Stroke = this.PLANE_LINES_COLOR,

                        X1 = 0,
                        Y1 = i * intervalY + this.GraphicBaseGrid.ActualHeight / 2,
                        X2 = this.GraphicBaseGrid.ActualWidth,
                        Y2 = i * intervalY + this.GraphicBaseGrid.ActualHeight / 2,
                    });

                    // Eksen numaralarının çizimi;
                    this.CoordinatePlaneCanvas.Children.Add(new TextBlock()
                    {
                        Text = (-i).ToString(),
                        Foreground = this.AXIS_LINES_COLOR,
                        Margin = new Thickness()
                        {
                            Top = i * intervalY + this.GraphicBaseGrid.ActualHeight / 2,
                            Left = this.GraphicBaseGrid.ActualWidth / 2 + 5
                        }
                    });
                }
            }

            //
            // Dikey çizgilerin çizimi
            //
            int minx = (int)((this.GraphicBaseGrid.ActualWidth / 2) / intervalX);
            int skipCountX = (int)(this.MIN_DRAWING_INTERVAL / intervalX) + 1;
            for (int i = -minx; i <= minx; i++)
            {
                if (i == 0)
                    continue;

                // Yeterinden fazla sıklıkta çizgi çizilmemesi için kaç sıra çizginin atlanacağı hesaplanır;
                if (i % skipCountX == 0)
                {
                    this.CoordinatePlaneCanvas.Children.Add(new Line()
                    {
                        StrokeThickness = 0.5,
                        Stroke = this.PLANE_LINES_COLOR,

                        X1 = i * intervalX + this.GraphicBaseGrid.ActualWidth / 2,
                        Y1 = 0,
                        X2 = i * intervalX + this.GraphicBaseGrid.ActualWidth / 2,
                        Y2 = this.GraphicBaseGrid.ActualHeight,
                    });

                    // Eksen numaralarının çizimi;
                    this.CoordinatePlaneCanvas.Children.Add(new TextBlock() 
                    { 
                        Text = i.ToString(),
                        Foreground = this.AXIS_LINES_COLOR,
                        Margin = new Thickness()
                        {
                            Top = this.GraphicBaseGrid.ActualHeight / 2 - 15,
                            Left = i * intervalX + this.GraphicBaseGrid.ActualWidth / 2
                        }
                    });
                }
            }

        }

        /// <summary>
        /// Dikey eksendeki iki yatay çizgi arası uzunluğu hesaplar
        /// </summary>
        /// <returns></returns>
        private double CalculateVerticalInterval()
        {
            return (this.MIN_INTERVAL / this.VerticalFactor) * this.ZoomLevel;
        }

        /// <summary>
        /// Yatay eksendeki iki dikey çizgi arası uzunluğu hesaplar
        /// </summary>
        /// <returns></returns>
        private double CalculateHorizontalInterval()
        {
            return (this.MIN_INTERVAL / this.HorizontalFactor) * this.ZoomLevel;
        }

        private void HorizontalSpreadButton_Click(object sender, RoutedEventArgs e)
        {
            this.HorizontalFactor -= 1.5;
            this.Update();
        }

        private void HorizontalConstrictButton_Click(object sender, RoutedEventArgs e)
        {
            this.HorizontalFactor += 1.5;
            this.Update();
        }

        private void VerticalSpreadButton_Click(object sender, RoutedEventArgs e)
        {
            this.VerticalFactor -= 1.5;
            this.Update();
        }

        private void VerticalConstrictButton_Click(object sender, RoutedEventArgs e)
        {
            this.VerticalFactor += 1.5;
            this.Update();
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            this.ZoomLevel += 1.5;
            this.Update();
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            this.ZoomLevel -= 1.5;
            this.Update();
        }

        #endregion

        #region Fonksiyonun çizimi

        /// <summary>
        /// Fonksiyonu ekrana çizer
        /// </summary>
        private void DrawFunction()
        {
            this.FunctionCanvas.Children.Clear();
            PointCollection graphicsPath = GenerateGraphicsPath(EquationString);

            graph1.Stroke = this.FUNCTION1_COLOR;
            graph1.StrokeThickness = 1;
            graph1.Points = graphicsPath;
            this.FunctionCanvas.Children.Add(graph1);
        }

        /// <summary>
        /// Çizimi yapılacak grafiğik için her noktanın koordinatını tutan bir path üretir.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <returns></returns>
        private PointCollection GenerateGraphicsPath(string equationString)
        {
            PointCollection result = new PointCollection();
            double intervalX = this.CalculateHorizontalInterval();
            double intervalY = this.CalculateVerticalInterval();

            int maxX = (int)((this.GraphicBaseGrid.ActualWidth / 2) / intervalX);
            int minX = -maxX;

            for (double x = minX; x <= maxX; x+=DrawingSensibility)
            {
                double y = Equation.Parse(this.EquationString.Replace(this.PARAMETER1_SYMBOL, x.ToString())).Exe();

                result.Add(new Point(x * intervalX + this.GraphicBaseGrid.ActualWidth / 2, -y * intervalY + this.GraphicBaseGrid.ActualHeight / 2));
            }

            return result;
        }

        /// <summary>
        /// Çizim hassasiyetini artırır (düşük performans)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SensivityIncrease_Click(object sender, RoutedEventArgs e)
        {
            this.DrawingSensibility -= 5;
            this.Update();
        }

        /// <summary>
        /// Çizim hassasiyetini azaltır (yüksek performans)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SensivityDecrease_Click(object sender, RoutedEventArgs e)
        {
            this.DrawingSensibility += 5;
            this.Update();
        }

        /// <summary>
        /// Grafiği günceller
        /// </summary>
        private void Update()
        {
            this.DrawCoordinatePlane();
            this.DrawFunction();
        }

        #endregion

        #region NotifyPropertyChanged Metodu

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
