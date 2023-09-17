using UnityEngine;

namespace Notepad
{
    internal interface IBrush
    {
        void Paint(Texture2D text, int x, int y);
        void Paint(Color[] pixels, int width, int x, int y, int radius);
    }
}