using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkiaSharp;



namespace cube_lut
{
    public class Hald
    {
        static int redStep;
        static int greenStep;
        static int blueStep;
        static  SKColor[] pixelArray;
        static  Dictionary<SKColor,int> lookup;
        static List<int> identityMap;
        static public SKBitmap GenerateClutImage(int level)
        {
            var cube_size = level * level;
            var image_size = level * level * level;
            //data = p = malloc((sizeof *data) * image_size * image_size * 3);
            var image = new SKBitmap(image_size, image_size);

            int i = 0;
            for (var blue = 0; blue < cube_size; blue++)
            {
                for (var green = 0; green < cube_size; green++)
                {
                    for (var red = 0; red < cube_size; red++)
                    {
                        var R = (byte)(((float)red / (float)(cube_size - 1)) * 255);
                        var G = (byte)(((float)green / (float)(cube_size - 1)) * 255);
                        var B = (byte)(((float)blue / (float)(cube_size - 1)) * 255);

                        // var pos = red + green + blue;

                        var x = i % image_size;
                        var y = i / image_size;
                        var color = new SKColor(R, G, B);
                        image.SetPixel(x, y, color);
                        i++;
                    }
                }
            }

            return image;
        }

        static public void Generatelookup(int level)
        {
            lookup = new Dictionary<SKColor, int>();
            var cube_size = level * level;
            var image_size = level * level * level;
            //data = p = malloc((sizeof *data) * image_size * image_size * 3);
            var image = new SKBitmap(image_size, image_size);

            int i = 0;
            for (var blue = 0; blue < cube_size; blue++)
            {
                for (var green = 0; green < cube_size; green++)
                {
                    for (var red = 0; red < cube_size; red++)
                    {
                        var R = (byte)(((float)red / (float)(cube_size - 1)) * 255);
                        var G = (byte)(((float)green / (float)(cube_size - 1)) * 255);
                        var B = (byte)(((float)blue / (float)(cube_size - 1)) * 255);

                        // var pos = red + green + blue;

                       // var x = i % image_size;
                       // var y = i / image_size;
                        var color = new SKColor(R, G, B);
                        lookup.Add(color, i);
                       // image.SetPixel(x, y, color);
                        i++;
                    }
                }
            }

       
        }

        static public void GenerateIdentityMap(int level)
        {
            identityMap = new List<int>();
            var cube_size = level * level;
            var image_size = level * level * level;
            //data = p = malloc((sizeof *data) * image_size * image_size * 3);
            var image = new SKBitmap(image_size, image_size);

            int i = 0;
            for (var blue = 0; blue < cube_size; blue++)
            {
                for (var green = 0; green < cube_size; green++)
                {
                    for (var red = 0; red < cube_size; red++)
                    {
                        i++;
                        if (CheckDivisible(red,level) && CheckDivisible(green, level) && CheckDivisible(blue, level) ) { identityMap.Add(i); }
                        
                       
                    }
                }
            }


        }

        static public bool CheckDivisible(int val,int div)
        {
            return ((val % div) == 0  );
        }
        static public List<string> ConvertClutImageToCube(string imagepath)
        {
            var bitmap = SKBitmap.Decode(imagepath);
            var Cube = new List<string>();
            List<string> data = new List<string>();
            if (bitmap.Width != bitmap.Height) return Cube;

            var lutSize = Math.Round(Math.Pow(bitmap.Width, 1 / 3.0));
            var count = (int)(Math.Pow(lutSize, 6));
            if (bitmap.Pixels.Length != count) return Cube;


            data.Add(string.Format("LUT_3D_SIZE  {0}", lutSize));
            data.Add("DOMAIN_MIN 0.0 0.0 0.0");
            data.Add("DOMAIN_MAX 1.0 1.0 1.0");


            //var cubesize = lutSize * lutSize;
            //redStep = (int)cubesize;
            //greenStep = (int)Math.Pow(cubesize, 2) * ((int)cubesize );
            //blueStep = (int)Math.Pow(cubesize, 4) * ((int)cubesize + 1);
            pixelArray = bitmap.Pixels;


            //Generatelookup((int)lutSize);
           
           

             GenerateIdentityMap((int)lutSize);

            foreach (var j in identityMap)
            {
                var i = pixelArray[j-1];
                data.Add(string.Format("{0} {1} {2}", i.Red / 255.0, i.Green / 255.0, i.Blue / 255.0));
            }






            return data;
        }

       
        private static SKColor LookUP(double ri, double gi, double bi)
        {
          return  pixelArray[(int)(Math.Round(ri * redStep + gi * greenStep + bi * blueStep))];
        }
    }

}