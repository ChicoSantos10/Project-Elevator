using System.Collections.Generic;
using UnityEngine;

namespace Puzzles
{
    public abstract class SafeButton : MonoBehaviour
    {

        public abstract void OnButtonPressed(List<int> code);

    }
}


