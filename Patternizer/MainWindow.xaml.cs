using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
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

        private System.Drawing.Image _LoadedImg;
        public System.Drawing.Image LoadedImg {
            get
            {
                return _LoadedImg;
            }
            set
            {
                _LoadedImg = value;

                // enable disabled buttons
                if (value == null)
                {
                    UI_Image_previewFromImage.Visibility = Visibility.Hidden;
                    UI_Grid_Path.IsEnabled = false;
                    UI_Grid_FileName.IsEnabled = false;
                    UI_txtBox_FileName.Text = "";
                    UI_txtBox_Path.Text = "";

                    UI_Grid_OutputButtons.IsEnabled = false;
                    UI_StackPanel_dragText.Visibility = Visibility.Visible;
                    UI_Grid_OutputTypes.IsEnabled = false;
                    UI_Btn_UpdatePreview.IsEnabled = false;
                    UI_CheckBox_AutoUpdate.IsEnabled = false;
                    UI_CheckBox_AutoUpdate.IsChecked = false;

                }
                else
                {
                    UI_Image_previewFromImage.Visibility = Visibility.Visible;
                    UI_StackPanel_dragText.Visibility = Visibility.Hidden;
                    UI_Grid_Path.IsEnabled = true;
                    UI_Grid_FileName.IsEnabled = true;
                    UI_Grid_OutputTypes.IsEnabled = true;
                    UI_Btn_UpdatePreview.IsEnabled = true;
                    UI_CheckBox_AutoUpdate.IsEnabled = true;
                }
            }
        }

        public string InputFile { get; set; }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            _LoadedImg = null;
        }
        private void reloadImgFromTemp()
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(tempImgFile, UriKind.Relative);
            bitmap.EndInit();
            UI_Image_previewFromImage.Source = bitmap;
        }

        private void UI_Btn_UpdatePreview_Click(object sender, RoutedEventArgs e)
        {
            using (Bitmap bmp = new Bitmap(LoadedImg))
            {
                RegionFill.FillRegionList(getShapeType(), bmp, (int)UI_CustomSlider_X.Value + 1, (int)UI_CustomSlider_Y.Value + 1);
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

        private void UI_Btn_OutputFolder_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo explorer = new ProcessStartInfo("explorer");
            explorer.Arguments = UI_txtBox_Path.Text;
            Process.Start(explorer);
        }

        public string OutputExtention { get; set; }
        private void UI_Btn_Output_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Path.Combine(UI_txtBox_Path.Text, UI_txtBox_FileName.Text) + OutputExtention);
            }
            catch(System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Problem opening file.", "File open error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UI_Btn_Compute_Click(object sender, RoutedEventArgs e)
        {
            string OutputFile = Path.Combine(UI_txtBox_Path.Text, UI_txtBox_FileName.Text);
            using (Bitmap bmp = new Bitmap(LoadedImg))
            {
                if (UI_RadioBtn_Svg.IsChecked == true)
                {
                    OutputExtention = ".svg";
                    OutputFile += OutputExtention;
                    if(!UI_Btn_Compute_Click_ContinueOperations(OutputFile))
                        return;

                    BasicSVG svg = RegionFill.FillRegionList(getShapeType(), bmp, (int)UI_CustomSlider_X.Value + 1, (int)UI_CustomSlider_Y.Value + 1);
                    svg.endInit();
                    File.WriteAllText(OutputFile, svg.ToString());
                }
                else if (UI_RadioBtn_Image.IsChecked == true)
                {
                    OutputExtention = Path.GetExtension(InputFile);
                    OutputFile += OutputExtention;
                    if (!UI_Btn_Compute_Click_ContinueOperations(OutputFile))
                        return;

                    RegionFill.FillRegionList(getShapeType(), bmp, (int)UI_CustomSlider_X.Value + 1, (int)UI_CustomSlider_Y.Value + 1);
                    bmp.Save(tempImgFile);
                    bmp.Save(OutputFile);
                }

                UI_Grid_OutputButtons.IsEnabled = true;
            }
        }
        private static bool UI_Btn_Compute_Click_ContinueOperations(string AbsolutePath)
        {
            if(File.Exists(AbsolutePath))
            {
                if(MessageBox.Show("File exists. Do you want to overrite the file?", "File exits", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
                {
                    return false; 
                }
            }
            return true;
        }

        #region Window Events
        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        List<string> supportedImageTypes;
        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = (String[])e.Data.GetData(DataFormats.FileDrop);
                if (data.Length > 0)
                {
                    supportedImageTypes = new List<string>() // must be >= 1 in list count
                    {
                        "bmp",
                        "gif",
                        "jpeg",
                        "exif",
                        "png",
                        "tiff",
                        "jpg"
                    };

                    if (!supportedImageTypes.Contains(Path.GetExtension(data[0]).ToLower().TrimStart('.')))
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.Append("(");
                        if (String.IsNullOrEmpty(data[0]))
                        {
                            sb.Append("Folder)");
                        }
                        else
                        {
                            sb.Append(Path.GetExtension(data[0]).ToLower());
                            sb.Append(") extention");
                        }

                        sb.Append(" not supported. Supported types are: (");

                        sb.Append(supportedImageTypes[0]);
                        for (int i = 1; i < supportedImageTypes.Count; i++)
                        {
                            sb.Append(", ");
                            sb.Append(supportedImageTypes[i]);
                        }
                        sb.Append(").");

                        UI_textBlock_DragEvent_ErrorMessage.Text = sb.ToString();
                        LoadedImg = null;
                        return;
                    }

                    InputFile = data[0];

                    LoadedImg = Bitmap.FromFile(data[0]);
                    using (Bitmap tmpImg = new Bitmap(LoadedImg))
                    {
                        tmpImg.Save(tempImgFile);
                    }

                    reloadImgFromTemp();

                    // if no text in path info then change path
                    UI_txtBox_Path.Text = Path.GetDirectoryName(data[0]);

                    UI_txtBox_FileName.Text = Path.GetFileNameWithoutExtension(data[0]);

                }
            }
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            File.Delete(tempImgFile); // cleanup temporary file
        }
        #endregion

        private void UI_CheckBox_AutoUpdate_Checked(object sender, RoutedEventArgs e)
        {
            UI_CustomSlider_X.ValueChanged += UI_CustomSlider_ValueChanged;
            UI_CustomSlider_Y.ValueChanged += UI_CustomSlider_ValueChanged;
        }

        private void UI_CustomSlider_ValueChanged(object sender, EventArgs e)
        {
            UI_Btn_UpdatePreview_Click(null, null);
        }

        private void UI_CheckBox_AutoUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            UI_CustomSlider_X.ValueChanged -= UI_CustomSlider_ValueChanged;
            UI_CustomSlider_Y.ValueChanged -= UI_CustomSlider_ValueChanged;
        }

        private void UI_CheckBox_SyncronizeSlider_Unchecked(object sender, RoutedEventArgs e)
        {
            UI_CustomSlider_X.ValueChanged -= UI_CustomSlider_Syncronize;
            UI_CustomSlider_Y.ValueChanged -= UI_CustomSlider_Syncronize;
        }

        private void UI_CheckBox_SyncronizeSlider_Checked(object sender, RoutedEventArgs e)
        {
            UI_CustomSlider_X.ValueChanged += UI_CustomSlider_Syncronize;
            UI_CustomSlider_Y.ValueChanged += UI_CustomSlider_Syncronize;
            UI_CustomSlider_Y.Value = UI_CustomSlider_X.Value; // syncronize start
        }
        private void UI_CustomSlider_Syncronize(object sender, EventArgs e)
        {
            CustomSlider obj = sender as CustomSlider;
            UI_CustomSlider_X.Value = obj.Value;
            UI_CustomSlider_Y.Value = obj.Value;
        }
    }
}
