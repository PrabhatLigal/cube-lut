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
            int lut_size = (int)LutSizeCombo.SelectedItem;
            var image = Hald.GenerateClutImage(lut_size);
            SKImage img = SKImage.FromBitmap(image);
            SKData data = img.Encode(SKImageEncodeFormat.Png, 90);
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

           
        }

        private void GenerateCube_Click(object sender, RoutedEventArgs e)
        {
           
            var lines = new List<string>();
            var filename = "test.cube";

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            
            //dlg.Filter = "JPG Files (*.jpg)|*.jpg | JPEG Files (*.jpeg)|*.jpeg";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result != null)
            {
                if ((bool)result)
                {
                    // Open document 

                   lines= Hald.ConvertClutImageToCube(dlg.FileName);
                    
                    if(lines.Count>0)
                    {

                        SaveFileDialog savefile = new SaveFileDialog();

                        var name=System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                        savefile.FileName = string.Format(name+".cube");
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
                    }
                    
                }
            }

            


        }


       
    }
}
