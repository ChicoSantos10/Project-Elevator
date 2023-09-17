using System;
using UnityEngine;

namespace Notepad
{
    internal struct Brush : IBrush
    {   
        public void Paint(Texture2D text, int x, int y)
        {
            text.SetPixel(x, y, Color.black);
        }

        public void Paint(Color[] pixels, int width, int x, int y, int radius)
        {
            Color color = Color.black;
            float r2 = radius * radius;
            
            Debug.Log(width);
            
            for (int i = x - radius; i < x + radius; i++)
            {
                for (int j = y - radius; j < y + radius; j++)
                {
                    //float dist = Vector2.Distance(new Vector2(x, y), new Vector2(i, j));
                    float dist = (new Vector2(x, y) - new Vector2(i, j)).sqrMagnitude;

                    try
                    {
                        if (dist <= r2)
                            pixels[i + j * width] = color;
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"i: {i}, j: {j} index: {i + j * width}");
                        Console.WriteLine(e);
                    }
                }
            }
        }
    }
}