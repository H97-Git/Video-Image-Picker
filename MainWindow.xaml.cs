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
        SaveFileDialog saveFileDialog = new SaveFileDialog(); // Create a saveFileDialog to save the image.
        private string path = Path.GetTempPath() + "thumbnail\\"; // Temp folder to save the frames.
        private string[] imageList; // Array of string which contains the path of all frames.
        private int index = 0;//Index of the shown image.
        private DirectoryInfo di;// Create a DirectoryInfo

        public MainWindow()
        {            
            InitializeComponent();
        }       

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            SetThumbnail();// Extract all frames every 100 millisecondes by default.
            imageList = GetThumnail(path);//Populate the array with all the frames.
            mainImage.Source = new BitmapImage(new Uri(imageList[index], UriKind.RelativeOrAbsolute));//Show the image on UI.
        }        

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                mainImage.Source = new BitmapImage(new Uri(imageList[(int)slider.Value], UriKind.RelativeOrAbsolute));//Select the image with the slider value.
            }
            catch (Exception)
            {
            }
        }

        private void Path_texbox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;//Enable Handler ? Don't know what this line do. Need research.
        }

        private void Path_texbox_Drop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);//Drag n Drop 1 or multiples files on Path_texbox.
            Path_texbox.Text = file[0];//Select the first item in the file array/Populate the Path_texbox.
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {           
           string output = String.Empty;//Empty the output.
           saveFileDialog.Filter = "jpg (*.jpg)|*.jpg|All files (*.*)|*.*";//Filter the saveFileDialog with image extensions.
           saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);//Set the InitialDirectory in "MyPictures".
            if (saveFileDialog.ShowDialog() == true)// ShowDialog the saveFileDialog.           
                System.IO.File.Copy(imageList[(int)slider.Value], saveFileDialog.FileName, true);//If true copy the desired image in the saveFileDialog.
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DeleteDirectory(path);//Delete the temp folder.
            System.Environment.Exit(0);//Exit.
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();//Move the view with MouseButton.Left
            }
        }

        public void SetThumbnail()
        {
            var inputFile = new MediaFile { Filename = Path_texbox.Text };//Create a MedialFile with video Input.
            DeleteDirectory(path);//Delete the previous folder.
            if (!Directory.Exists(path))//If the path don't exist create it.
            {
                // Try to create the directory.
                di = Directory.CreateDirectory(path);
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
            }

            using (var engine = new Engine())//Create a new engine to process the video.
            {
                engine.GetMetadata(inputFile);//Get the Metada of the video.
                int duration = (int)inputFile.Metadata.Duration.TotalMilliseconds;//Store the duration of video in a int.
                Console.WriteLine("Dur�e {0}", duration);//Output the duration of the video in the console
                for (int i = 0; i < duration; i += 100)//Loop through the video.
                {
                    var outputFile = new MediaFile { Filename = path + i + ".jpg" };//Create the output path of the file.
                    var options = new ConversionOptions { Seek = TimeSpan.FromMilliseconds(i) };//Cut the video every 100 Milliseconds.
                    engine.GetThumbnail(inputFile, outputFile, options);//Extract the frames in the temp folder.
                }
            }
        }
        public dynamic GetThumnail(string searchFolder)
        {
            var files = Directory.GetFiles(searchFolder);//Populate the array with all the frames.
            NumericComparer ns = new NumericComparer();//New Numeric Comparer.
            Array.Sort(files, ns);//Sort the array with the Numeric Compare.("0","11","2","245") -> ("0","2","11","245")
            slider.Maximum = files.Length;//Set the max value of the slider equal to the number of frames.
            return files;//Return the array of images.
        }
        public static void DeleteDirectory(string target_dir)
        {
            try
            {
                string[] files = Directory.GetFiles(target_dir);//Array of all files in the folder.
                string[] dirs = Directory.GetDirectories(target_dir);//Array of subfolders in the folder.

                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);//Change all files permissions.
                    File.Delete(file);//Delete all files.
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);//Delete all files and folders in the subfolders.
                }

                Directory.Delete(target_dir, false);//Delete the root folder.
            }
            catch (Exception)
            {

            }
        }
    }
}
