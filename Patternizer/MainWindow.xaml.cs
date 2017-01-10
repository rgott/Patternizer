using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Patternizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string tempImgFile = Path.GetTempFileName();

        public System.Drawing.Image LoadedImg { get; set; }
        public string OutputFile { get; set; }
        public MainWindow()
        {
            OutputFile = "hello.ext";
            LoadedImg = null;
            InitializeComponent();
        }
        #region Window Events

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = (String[])e.Data.GetData(DataFormats.FileDrop);
                if (data.Length > 0)
                {
                    LoadedImg = Bitmap.FromFile(data[0]);
                    using (Bitmap tmpImg = new Bitmap(LoadedImg))
                    {
                        tmpImg.Save(tempImgFile);
                    }

                    reloadImgFromTemp();
                }
            }
        }

        

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
        }

        #endregion
        private void reloadImgFromTemp()
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(tempImgFile, UriKind.Relative);
            bitmap.EndInit();
            UI_previewFromImage.Source = bitmap;
        }

        private void UI_Btn_UpdatePreview_Click(object sender, RoutedEventArgs e)
        {
            long start = Environment.TickCount;

            using (Bitmap bmp = new Bitmap(LoadedImg))
            {
                RegionFill.FillRegionList(getShapeType(), bmp, 30, 30);
                bmp.Save(tempImgFile);
            }
            reloadImgFromTemp();
        }

        private ShapeType getShapeType()
        {
            ShapeType type;

            if (UI_ComboBox_ShapeSelection.SelectedItem == null)
                return ShapeType.triangle;

            ComboBoxItem item = (ComboBoxItem)UI_ComboBox_ShapeSelection.SelectedItem;
            string str = (string)item.Content;
            switch (str)
            {
                case "Triangle":
                    type = ShapeType.triangle;
                    break;
                case "Square":
                    type = ShapeType.square;
                    break;
                default:
                    type = ShapeType.triangle;
                    break;
            }
            return type;
        }

        private void UI_Btn_Output_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Path.GetFullPath(OutputFile));
        }

        private void UI_Btn_OutputFolder_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo explorer = new ProcessStartInfo("explorer");
            explorer.Arguments = Path.GetDirectoryName(Path.GetFullPath(OutputFile));
            Process.Start(explorer);
        }

        private void UI_Btn_Compute_Click(object sender, RoutedEventArgs e)
        {
            using (Bitmap bmp = new Bitmap(LoadedImg))
            {
                if (UI_RadioBtn_Svg.IsChecked == true)
                {
                    OutputFile = Path.ChangeExtension(OutputFile, "svg");
                    BasicSVG svg = RegionFill.FillRegionList(getShapeType(), bmp, (int)slider2.Value + 1, (int)slider2.Value + 1);
                    svg.endInit();
                    File.WriteAllText(OutputFile, "<html>" + svg + "</html>");
                }
                else if (UI_RadioBtn_Image.IsChecked == true)
                {
                    OutputFile = Path.ChangeExtension(OutputFile, "bmp");

                    RegionFill.FillRegionList(getShapeType(), bmp, 30, 30);
                    bmp.Save(tempImgFile);
                    bmp.Save(OutputFile);
                }
            }
        }
    }
}
