using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzles
{
    public class ButtonPuzzle : MonoBehaviour
    {

        [SerializeField] public List<GameObject> randomButtons;
        [SerializeField] List<GameObject> children;
        [SerializeField] public List<GameObject> clickedButton;
        [SerializeField] public List<Material> materials;
        [SerializeField] public GameObject keyCard;
        [SerializeField] public bool puzzleCompleted = false;

        int addedChild = 0;
        int number;
        public int help = 0;

        // Start is called before the first frame update
        void Start()
        {
            randomButtons = new List<GameObject>();
            children = new List<GameObject>();
            clickedButton = new List<GameObject>();
            //materials = new List<Material>();
            AddChildren();
            RandomizeChildren();
        }
    
    
        void AddChildren()
        {
            foreach (Transform child in transform)
            {
                children.Add(child.gameObject);
            }
        }
    
        void RandomizeChildren()
        {
            if (randomButtons.Count == 0)
            {
                number = Random.Range(0, children.Count);
                randomButtons.Add(children[number]);
                RandomizeChildren();
            }
            else if (randomButtons.Count < children.Count)
            {
                int tempNumber = Random.Range(0, children.Count);
    
                if (randomButtons.Contains(children[tempNumber]))
                {
                    RandomizeChildren();
                }
                else
                {
                    number = tempNumber;
                    randomButtons.Add(children[number]);
                    RandomizeChildren();
                }
    
            }
        }

        public void RestartButtons()
        {
            randomButtons.Clear();
            clickedButton.Clear();
            RandomizeChildren();
            puzzleCompleted = false;

            foreach (Transform child in transform)
            {
                if (child.gameObject.name.Contains("Quad"))
                {
                    child.gameObject.SetActive(false);
                }
            }
            
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    }
    
}

