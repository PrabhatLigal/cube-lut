using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkiaSharp;



namespace cube_lut
{
    public class Hald
    {
        SKBitmap GenerateClutImage(int level)
        {
            var cube_size = level * level;
            var image_size = level * level * level;
            //data = p = malloc((sizeof *data) * image_size * image_size * 3);
            var image = new SKBitmap(image_size, image_size);


            for (var blue = 0; blue < cube_size; blue++)
            {
                for (var green = 0; green < cube_size; green++)
                {
                    for (var red = 0; red < cube_size; red++)
                    {
                        var R = (byte)((float)red / (float)(cube_size - 1) * 255);
                        var G = (byte)((float)green / (float)(cube_size - 1) * 255);
                        var B = (byte)((float)blue / (float)(cube_size - 1) * 255);

                        var pos = red + green + blue;

                        var x = pos % image_size;
                        var y = pos / image_size;
                        var color = new SKColor(R, G, B);
                        image.SetPixel(x, y, color);

                    }
                }
            }

            return image;
        }
    }

}