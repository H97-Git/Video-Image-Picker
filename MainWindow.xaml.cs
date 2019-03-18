using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.Win32;
using ns;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Video_Image_Picker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        private string path = Path.GetTempPath() + "thumbnail\\";
        private string[] imageList;
        private int index = 0;
        private DirectoryInfo di;

        public MainWindow()
        {            
            InitializeComponent();
        }       

        private void Button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            SetThumbnail();
            imageList = GetThumnail(path);
            mainImage.Source = new BitmapImage(new Uri(imageList[index], UriKind.RelativeOrAbsolute));
        }

        public void SetThumbnail()
        {
            var inputFile = new MediaFile { Filename = Texbox1.Text };

            if (!Directory.Exists(path))
            {
                // Try to create the directory.
                di = Directory.CreateDirectory(path);
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
            }

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                int duration = (int)inputFile.Metadata.Duration.TotalMilliseconds;
                Console.WriteLine("Dur�e {0}", duration);
                for (int i = 0; i < duration; i += 100)
                {
                    var outputFile = new MediaFile { Filename = path + i + ".jpg" };
                    var options = new ConversionOptions { Seek = TimeSpan.FromMilliseconds(i) };
                    engine.GetThumbnail(inputFile, outputFile, options);
                }
            }
        }
        public dynamic GetThumnail(string searchFolder)
        {
            var files = Directory.GetFiles(searchFolder);
            NumericComparer ns = new NumericComparer();
            Array.Sort(files, ns);
            slider.Maximum = files.Length;
            return files;
        }
        public static void DeleteDirectory(string target_dir)
        {
            try
            {
                string[] files = Directory.GetFiles(target_dir);
                string[] dirs = Directory.GetDirectories(target_dir);

                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                Directory.Delete(target_dir, false);
            }
            catch (Exception)
            {
                                
            }            
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                mainImage.Source = new BitmapImage(new Uri(imageList[(int)slider.Value], UriKind.RelativeOrAbsolute));
            }
            catch (Exception)
            {
            }
        }

        private void Texbox1_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void Texbox1_Drop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
            Texbox1.Text = file[0];
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {           
           string output = String.Empty;
           saveFileDialog.Filter = "jpg (*.jpg)|*.jpg|All files (*.*)|*.*";
           saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (saveFileDialog.ShowDialog() == true)                
                System.IO.File.Copy(imageList[(int)slider.Value], saveFileDialog.FileName, true);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DeleteDirectory(path);
            System.Environment.Exit(0);
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
