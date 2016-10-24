using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Patternizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Image loadedImg = Bitmap.FromFile("bitmap2.jpg"); // load once
        public MainWindow()
        {
            InitializeComponent();

            long start = Environment.TickCount;

            using (Bitmap bmp = new Bitmap(loadedImg))
            {
                SVG svg = RegionFill.FillRegionList(bmp, 50, 50);
                //bmp.Save("SDFK.bmp");
                svg.endInit();
                File.WriteAllText("SDF.html","<html>" + svg.s() + "</html>");
            }
            long end = Environment.TickCount;
            Console.WriteLine(end - start);
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //string data = (String)e.Data.GetData(DataFormats.FileDrop);
                //BitmapImage img = new BitmapImage(new Uri(data));
                //img.
            }
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
        }


        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            long start = Environment.TickCount;

            using (Bitmap bmp = new Bitmap(loadedImg))
            {
                SVG svg = RegionFill.FillRegionList(bmp, (int)slider2.Value + 1, (int)slider2.Value + 1);
                bmp.Save("SDFK.bmp");
                svg.endInit();
                File.WriteAllText("SDF.html", "<html>" + svg.s() + "</html>");
            }
            long end = Environment.TickCount;
            Console.WriteLine(end - start);



            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri("SDFK.bmp", UriKind.Relative);
            bitmap.EndInit();
            img.Source = bitmap;
        }
    }
}
