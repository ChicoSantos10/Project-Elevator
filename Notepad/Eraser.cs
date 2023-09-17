using UnityEngine;

namespace Notepad
{
    internal struct Eraser : IBrush
    {
        public void Paint(Texture2D text, int x, int y)
        {
            text.SetPixel(x, y, Color.white);
        }

        public void Paint(Color[] pixels, int width, int x, int y, int radius)
        {
            Color color = Color.white;
            
            for (int i = x - radius; i < x + radius; i++)
            {
                for (int j = y - radius; j < y + radius; j++)
                {
                    //float dist = Vector2.Distance(new Vector2(x, y), new Vector2(i, j));
                    float dist = (new Vector2(x, y) - new Vector2(i, j)).sqrMagnitude;

                    if (dist <= radius)
                        pixels[i + j * width] = color;
                }
            }
        }
    }
}