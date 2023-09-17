using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelManagement
{
    [CreateAssetMenu(menuName = "Elevator Code/Code", fileName = "Code", order = 0)]
    public class Code : ScriptableObject
    {
        [SerializeField] protected int a, b, c, d;

        public int A => a;
        public int B => b;
        public int C => c;
        public int D => d;

        public bool IsCodeCorrect(List<int> code)
        {
            return a == code[0] && b == code[1] && 
                   c == code[2] && d == code[3];
        }

        public override string ToString()
        {
            return $"{a}{b}{c}{d}";
        }

        void OnValidate()
        {
            if (a < 0 || a > 9)
                Debug.LogError("Number must be between 0 and 9. Code A");
            if (b < 0 || b > 9)
                Debug.LogError("Number must be between 0 and 9. Code B");
            if (c < 0 || c > 9)
                Debug.LogError("Number must be between 0 and 9. Code C");
            if (d < 0 || d > 9)
                Debug.LogError("Number must be between 0 and 9. Code D");
        }
    }
}