using cube_lut;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateImage_Click(object sender, RoutedEventArgs e)
        {

            var image = Hald.GenerateClutImage(8);


            IntPtr len;
            SKImage img = SKImage.FromBitmap(image);


            SKData data = img.Encode(SKImageEncodeFormat.Png, 90);
            using (FileStream fs = new FileStream("test.png", FileMode.Create))
            {
                data.SaveTo(fs);
            }
        }

        private void GenerateCube_Click(object sender, RoutedEventArgs e)
        {
            //  string mydocpath =
            //  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var lines = new List<string>();
            var filename = "test.cube";
            if (Hald.ConvertClutImageToCube("test.jpg", out lines))
            {
                //using (StreamWriter outputFile = new StreamWriter("test.cube"))
                //{
                //    foreach (string line in lines)
                //        outputFile.WriteLine(line);
                //}

                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                File.WriteAllLines(filename, lines.ToArray());

            }


        }
    }
}
