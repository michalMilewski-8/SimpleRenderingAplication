using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace rysoinator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double a_r, b_r, c_r;
        private bool isRedrawingNeeded;
        private int level, leftToDraw;

        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            drawingSpace.MouseMove += drawingSpace_MouseMove;
            drawingSpace.MouseWheel += drawingSpace_MouseWheel;
            drawingSpace.SizeChanged += DrawingSpace_SizeChanged;
            imaginator = new RealImaginator(new RealElipsoinator());
            a_r = 150;
            b_r = 100;
            c_r = 100;
            imaginator.SetRadiuses(a_r, b_r, c_r);
            isRedrawingNeeded = true;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isRedrawingNeeded)
            {
                leftToDraw = level;
                isRedrawingNeeded = false;
            }
            RealDrawFrame();
            leftToDraw--;
            if (leftToDraw <= 0) leftToDraw = 1;

        }

        private void DrawingSpace_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var dpi = VisualTreeHelper.GetDpi(this);
            width = (int)(e.NewSize.Width * dpi.DpiScaleX);
            height = (int)(e.NewSize.Height * dpi.DpiScaleY);
            DrawFrame();
        }

        private int width, height;
        private System.Windows.Point lastKnownMousePoint;
        private Imaginator imaginator;
        private void drawingSpace_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point currentPosition = e.GetPosition((IInputElement)sender);
            Vector movement = currentPosition - lastKnownMousePoint;
            lastKnownMousePoint = currentPosition;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                imaginator.Translate(movement.X / 1.0, movement.Y / 1.0);
                DrawFrame();
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                imaginator.Rotate(movement.X / 100.0, movement.Y / 100.0);
                DrawFrame();
            }

        }

        private void drawingSpace_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            imaginator.Resize(e.Delta / 1000.0);
            DrawFrame();
        }

        private BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            a_r = e.NewValue;
            if (!(r_a is null))
                r_a.Content = $"{(int)a_r}";
            if (!(imaginator is null)) {
                imaginator.SetRadiuses(a_r, b_r, c_r);
                DrawFrame();
            }
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            b_r = e.NewValue;
            if (!(r_b is null))
                r_b.Content = $"{(int)b_r}";
            if (!(imaginator is null))
            {
                imaginator.SetRadiuses(a_r, b_r, c_r);
                DrawFrame();
            }
        }

        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            c_r = e.NewValue;
            if (!(r_c is null))
                r_c.Content = $"{(int)c_r}";
            if (!(imaginator is null))
            {
                imaginator.SetRadiuses(a_r, b_r, c_r);
                DrawFrame();
            }
        }

        private void ws_m_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!(ws_m_val is null))
            ws_m_val.Content = $"{(int)e.NewValue}";
            if (!(imaginator is null))
            {
                imaginator.SetM(e.NewValue);
                DrawFrame();
            }
        }

        private void Slider_ValueChanged_3(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            level = (int)e.NewValue;
            if (adapt is null)
                return;
            adapt.Content = $"Level: {level}";
            DrawFrame();
        }
        private void RealDrawFrame()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var Con = Convert(imaginator.GenerateImage(width, height, leftToDraw));
            Console.WriteLine($"{Con.PixelWidth}x{Con.Height}");
            drawingSpace.Source = Con;
            sw.Stop();
            this.Title = $"render frame time: {sw.ElapsedMilliseconds} [ms]";
        }

        private void DrawFrame()
        {
            isRedrawingNeeded = true;
        }
    }
}
