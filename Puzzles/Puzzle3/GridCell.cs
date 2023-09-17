using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class GridCell : MonoBehaviour
    {
        private int posX;
        private int posY;
        [SerializeField] int number;
        
        // Start is called before the first frame update
        void Start()
        {
            //usedNumbers = new List<int>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetPosition(int x, int y)
        {
            posX = x;
            posY = y;

        }

        public Vector2Int GetPosition()
        {
            return new Vector2Int(posX, posY);
        }

        public void GetNumber(int n)
        {
            number = n;
        }
        

    }
}

