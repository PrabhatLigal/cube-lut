﻿using System;
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


            redStep = (int)lutSize + 1;
            greenStep = (int)Math.Pow(lutSize, 2) * ((int)lutSize + 1);
            blueStep = (int)Math.Pow(lutSize, 4) * ((int)lutSize + 1);
            pixelArray = bitmap.Pixels;


            Generatelookup((int)lutSize);

            for (double blue = 0; blue < lutSize; blue++)
            {
                for (double green = 0; green < lutSize; green++)
                {
                    for (double red = 0; red < lutSize; red++)
                    {
                        var R = (byte)(((float)red / (float)(lutSize - 1)) * 255);
                        var G = (byte)(((float)green / (float)(lutSize - 1)) * 255);
                        var B = (byte)(((float)blue / (float)(lutSize - 1)) * 255);
                        
                        var color = new SKColor(R, G, B);
                        if (lookup.ContainsKey(color))
                        {
                            var i = bitmap.Pixels[lookup[color]];
                            data.Add(string.Format("{0} {1} {2}", i.Red / 255.0, i.Green / 255.0, i.Blue / 255.0));
                        }
                        else
                        {
                            data.Add(string.Format("{0} {1} {2}", color.Red / 255.0, color.Green / 255.0, color.Blue / 255.0));
                        }
                        
                    }
                }
            }

            //  Generatelookup((int)lutSize);

            //foreach (var i in bitmap.Pixels)
            //{
            //    data.Add(string.Format("{0} {1} {2}", i.Red / 255.0, i.Green / 255.0, i.Blue / 255.0));
            //}






            return data;
        }

       
        private static SKColor LookUP(double ri, double gi, double bi)
        {
          return  pixelArray[(int)(Math.Round(ri * redStep + gi * greenStep + bi * blueStep))];
        }
    }

}