using System;
using System.Drawing;
using System.Windows;

namespace Patternizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Image img = Bitmap.FromFile("images.jpg");
            using (Bitmap bmp = new Bitmap(img))
            {
                RegionFill.FillRegionList(bmp, 50, 50);
                bmp.Save("SDFK.bmp");

            }
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
            //Bitmap map = RegionFill.CreateNonIndexedImage(Bitmap.FromFile("Bitmap1.bmp"));
            //RegionFill.FillRegionList(map, 50, 50);

            //img.Source = new BitmapImage(new Uri("SDFK.bmp",UriKind.Relative));
            //img.UpdateLayout();

        }
    }
}
