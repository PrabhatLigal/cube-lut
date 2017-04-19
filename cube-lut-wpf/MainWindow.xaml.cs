using cube_lut;
using Microsoft.Win32;
using SkiaSharp;
using System;
using System.Collections.Generic;
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

namespace cube_lut_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<int> LutSize = Enumerable.Range(2, 19).ToList();
        public MainWindow()
        {
            InitializeComponent();
            LutSizeCombo.ItemsSource = LutSize;
            LutSizeCombo.SelectedIndex = (int)(LutSize.Count/2)-1;
        }

        private void GenerateImage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.IsEnabled = false;
            ProgressBar.Visibility = Visibility.Visible;


            int lut_size = (int)LutSizeCombo.SelectedItem;


            Task.Run(

                async () =>
                {
                   await GenerateFunction(lut_size);
                }
                );

            
        }

        async Task GenerateFunction( int lut_size)
        {
            SKData data = new SKData();
            //Task.Run(() => {

            var image = Hald.GenerateClutImage(lut_size);
            SKImage img = SKImage.FromBitmap(image);
            data = img.Encode(SKImageEncodeFormat.Png, 90);


            //  });



            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("identity_{0}.png", lut_size);
            savefile.Filter = "PNG files (*.png)|*.png";
            if (savefile.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(savefile.FileName, FileMode.Create))
                {
                    data.SaveTo(fs);
                }
            }
            EndAnimate();

        }

        void EndAnimate()
        {
            Dispatcher.Invoke(() => {

                ProgressBar.Visibility = Visibility.Collapsed;
                GenerateImage.IsEnabled = true;
                GenerateCube.IsEnabled = true;

            });
            
        }
        

        private void GenerateCube_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.IsEnabled = false;
            ProgressBar.Visibility = Visibility.Visible;

            Error.Text = "";
          
            
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
   
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result != null)
            {
                if ((bool)result)
                {
                    // Open document 
                    var extension = System.IO.Path.GetExtension(dlg.FileName);
                    if (string.Compare(extension, ".jpg", true) != 0 && string.Compare(extension, ".jpeg", true) != 0 && string.Compare(extension, ".png", true) != 0)
                    {
                        Error.Text = "Error: Try JPG HALD image only.";
                        EndAnimate();
                        return;
                    }
                    try
                    {
                        Task.Run(
                            async () => {
                                await GenerateCubeFile(dlg.FileName);
                            });
                        
                      
                    }
                    catch
                    {
                        Error.Text = "Error: Converison failed. Try with HALD image only.";
                        EndAnimate();
                    }
                    
                }
                else
                {
                    EndAnimate();
                }
            }
            else
            {
                EndAnimate();
            }


            
        }


        async Task GenerateCubeFile(string fileName)
        {
            var lines = new List<string>();
            lines = Hald.ConvertClutImageToCube(fileName);
            Dispatcher.Invoke(() =>
            {
            if (lines.Count > 0)
            {

              
                    SaveFileDialog savefile = new SaveFileDialog();

                    var name = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    savefile.FileName = string.Format(name + ".cube");
                    savefile.Filter = "CUBE files (*.cube)|*.cube";
                    if (savefile.ShowDialog() == true)
                    {
                        if (File.Exists(savefile.FileName))
                        {
                            File.Delete(savefile.FileName);
                        }
                        File.WriteAllLines(savefile.FileName, lines.ToArray());

                    }
               
               
            }
            else
            {
                //error
               
                    Error.Text = "Error: Converison failed. Try with HALD image only.";
               
            }
            
                EndAnimate();
            });
        }

       
    }
}
