using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Interaction_System;
using DG.Tweening;
using Robot;

namespace Grid
{
    public class GameGrid : MonoBehaviour
    {


        private InputMaster controls;

        enum LastDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        LastDirection lastDir;

        private int height = 5;
        private int width = 5;

        public float gridSpaceSize = 1f;

        private GameObject[,] gameGrid;
        [SerializeField] GameObject gridCellPrefab;
        [SerializeField] LineRenderer lr;
        private List<GameObject> pointsToDraw;
        
        int number;
        private List<int> usedNumbers;
        [SerializeField] List<GameObject> path;
        [SerializeField] List<GameObject> clickPath;
        [SerializeField] Vector2Int matrix;
        [SerializeField] GameObject gO;
        [SerializeField] Mesh gMesh;

        //random path
        int randomPath;
        Vector3 vectorUp = new Vector3(0, 1f, 0);
        Vector3 vectorDown = new Vector3(0, -1f, 0);
        Vector3 vectorLeft = new Vector3(-1f, 0, 0);
        Vector3 vectorRight = new Vector3(1f, 0, 0);

        Vector2 posClicked;
        [SerializeField] Camera_Controller.MouseController mController;
        [SerializeField] LayerMask gridLayer;
        Vector2 mousePosition;
        bool isClick = false;
        GridCell cellMouseIsOver;
        int pathHelp = 0;
        bool puzzleCompleted = false;

        Color color1;
        [SerializeField] Camera rtCamera;
        [SerializeField] LayerMask tabletScreen;
        [SerializeField] InputReader input;
        [SerializeField] GameObject parentObject;
        Vector3 initialPos;
        [SerializeField] Robot.RobotMovement rMov;
        [SerializeField] GameObject pathPuzzle;

        [SerializeField] Material nightCamMtrl;
        [SerializeField] GameObject door;
        Collider? c;
        Vector2 cursorPos;

        private void Awake()
        {
            usedNumbers = new List<int>();
            path = new List<GameObject>();
            clickPath = new List<GameObject>();

        }

        // Start is called before the first frame update
        void Start()
        {
            initialPos = gameObject.transform.position;
            controls = new InputMaster();
            CreateGrid();
        }

        // Update is called once per frame
        void Update()
        {



            CheckPuzzle();

            cursorPos = Mouse.current.position.ReadValue();

        }


        public void CheckPuzzle()
        {
            if (!puzzleCompleted)
            {
                cellMouseIsOver = IsMouseOverAGridSpace();
                if (cellMouseIsOver)
                {
                    if (clickPath.Count < path.Count)
                    {

                        if (isClick)
                        {
                            clickPath.Add(cellMouseIsOver.gameObject);
                            CheckIfCorrectCells();
                            isClick = false;

                        }
                    }
                    else
                    {
                        puzzleCompleted = true;
                    }


                }
            }

            else
            {
                door.transform.DORotate(new Vector3(0, -90, 0), 2f, RotateMode.Fast);
            }
        }

        private void CheckIfCorrectCells()
        {

            //cell correta
            if (path[pathHelp] == clickPath[clickPath.Count - 1])
            {
                Debug.Log("correto");
                ColorUtility.TryParseHtmlString("#00FF03", out color1);
                cellMouseIsOver.GetComponentInChildren<SpriteRenderer>().material.color = color1;
                // cellMouseIsOver.
                pathHelp++;
            }

            //Cell incorreta
            else
            {

                pathHelp = 0;

                foreach (var cell in clickPath)
                {
                    cell.GetComponentInChildren<SpriteRenderer>().material.color = Color.white;
                }
                clickPath.Clear();

                RobotMovement.ClosestRobotToTarget(pathPuzzle);
                

            }





        }

        void CreateGrid()
        {
            gameObject.transform.position = new Vector3(0,0,0);

            gameGrid = new GameObject[height, width];

            if (gridCellPrefab == null)
            {
                Debug.LogError("ERRO: GridCellPrefab is empty");
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    gameGrid[x, y] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize + gameObject.transform.position.x, y * gridSpaceSize + gameObject.transform.position.y), Quaternion.identity);
                    gameGrid[x, y].GetComponent<GridCell>().SetPosition(x, y);
                    gameGrid[x, y].GetComponent<GridCell>().GetNumber(SetNumber());
                    gameGrid[x, y].transform.parent = transform;
                    //gameGrid[x, y].gameObject.name = $"Grid{x.ToString()}X{y.ToString()}Y";
                    gameGrid[x, y].gameObject.name = $"Grid{number}";

                }
            }

            path.Add(gameGrid[matrix.x, matrix.y]);

            //CreatePath();

            //if (path.Count <= 7)
            //{
            //    Debug.Log("check desempenho");
            //    CreatePath();
            //}

            while (path.Count <= 5 )
            {
                CreatePath();
            }

            gameObject.transform.position = initialPos;
            //foreach (var p in path)
            //{
            //    
            //    //Debug.Log(p.transform.position);
            //}
            //Debug.Log(path.Count);

            SetUpLine(path);

            //UpdateLRPosition(new Vector3(-0.431625366f, 0.531940877f, -0.437516212f));

            //lr.BakeMesh(gO.GetComponent<MeshFilter>().mesh);

        }

        public Vector2Int GetGridPosFromWorld(Vector3 worldPosition)
        {

            int x = Mathf.FloorToInt(worldPosition.x / gridSpaceSize);
            int y = Mathf.FloorToInt(worldPosition.y / gridSpaceSize);
            
            x = Mathf.Clamp(x, 0, width);
            y = Mathf.Clamp(y, 0, height);

            //Debug.Log($"{x}, {y}");

            return new Vector2Int(x, y);


            //return new Vector2Int((int)worldPosition.x, (int)worldPosition.y);
        }

        public Vector3 GetWorldPosFromGrid(Vector3 gridPosition)
        {
            float x = gridPosition.x * gridSpaceSize;
            float y = gridPosition.y * gridSpaceSize;


            Debug.Log($"GridPos{gridPosition}");
            Debug.Log($"Size{gridSpaceSize}");
            Debug.Log($"X {x} | Y {y}");
            Debug.Log($"=============================================");

            return new Vector3(x, 0, y);
        }


        public int SetNumber()
        {
            //Debug.Log(usedNumbers.Count);
            //usedNumbers.Add(1);


            if (usedNumbers.Count == 0)
            {
                number = UnityEngine.Random.Range(0, height * width);
                usedNumbers.Add(number);
                //Debug.Log("teste1");

            }
            else
            {

                int tempNumber = UnityEngine.Random.Range(0, height * width);

                //Debug.Log(tempNumber);

                if (usedNumbers.Contains(tempNumber))
                {
                    //Debug.Log("contem o " + tempNumber);
                    SetNumber();
                }
                else
                {
                    //Debug.Log("não contem o " + tempNumber);
                    number = tempNumber;
                    usedNumbers.Add(number);

                }
            }

            return number;
        }


        //Path Created
        public void CreatePath()
        {

            //Colocar isto num try (codigo deste if) catch CreatePath() elimina os erros mas por pezes puxa pela gpu

            if (path.Count <= 8)
            {
                //Debug.Log(GetGridPosFromWorld(path[path.Count - 1].transform.position));

                //CANTOS
                //DownLeft
                if (GetGridPosFromWorld(path[path.Count - 1].transform.position) == new Vector2(0 , 0))
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]))
                        {
                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]);
                            lastDir = LastDirection.Up;
                            CreatePath();
                        }
                        else
                        {
                            if (lastDir == LastDirection.Down)
                            {
                                CreatePath();
                            }
                        }

                    }
                    else
                    {
                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]))
                        {
                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]);
                            lastDir = LastDirection.Right;
                            CreatePath();

                        }
                        else
                        {
                            if (lastDir == LastDirection.Left)
                            {
                                CreatePath();
                            }
                        }
                    }
                }

                //DownRight
                else if (GetGridPosFromWorld(path[path.Count - 1].transform.position) == new Vector2(4, 0))
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]))
                        {
                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]);
                            lastDir = LastDirection.Up;
                            CreatePath();

                        }
                        else
                        {
                            if (lastDir == LastDirection.Down)
                            {
                                CreatePath();
                            }
                        }

                    }
                    else
                    {
                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]))
                        {
                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]);
                            lastDir = LastDirection.Left;
                            CreatePath();

                        }
                        else
                        {
                            if (lastDir == LastDirection.Right)
                            {
                                CreatePath();
                            }
                        }

                    }
                }

                //UpLeft
                else if (GetGridPosFromWorld(path[path.Count - 1].transform.position) == new Vector2(0, 4))
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]))
                        {
                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]);
                            lastDir = LastDirection.Down;
                            CreatePath();

                        }
                        else
                        {
                            if (lastDir == LastDirection.Up)
                            {
                                CreatePath();
                            }
                        }

                    }
                    else
                    {
                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]))
                        {
                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]);
                            lastDir = LastDirection.Right;
                            CreatePath();

                        }
                        else
                        {
                            if (lastDir == LastDirection.Left)
                            {
                                CreatePath();
                            }
                        }

                    }
                }

                //UpRight
                else if (GetGridPosFromWorld(path[path.Count - 1].transform.position) == new Vector2(4, 4))
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]))
                        {
                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]);
                            lastDir = LastDirection.Down;
                            CreatePath();

                        }
                        else
                        {
                            Debug.LogWarning("ja tem este ponto na lista");
                        }
                    }
                    else
                    {
                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]))
                        {
                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]);
                            lastDir = LastDirection.Left;
                            CreatePath();

                        }
                        else
                        {
                            Debug.LogWarning("ja tem este ponto na lista");
                        }

                    }
                }

                else
                {
                    int randRange = UnityEngine.Random.Range(0, 3);
                    int randRangeMid = UnityEngine.Random.Range(0, 4);


                    //Debug.Log($"{GetGridPosFromWorld(path[path.Count - 1].transform.position)}");

                    //BORDA ESQUERDA/BAIXO
                    if (GetGridPosFromWorld(path[path.Count - 1].transform.position).x == 0 || GetGridPosFromWorld(path[path.Count - 1].transform.position).y == 0)
                    {
                        if (GetGridPosFromWorld(path[path.Count - 1].transform.position).x == 0)
                        {
                            //0 cima
                            //1 direita
                            //2 baixo


                            if (randRange == 0)
                            {
                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]);
                                    lastDir = LastDirection.Up;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Down)
                                    {
                                        CreatePath();
                                    }
                                }

                            }
                            else if (randRange == 1)
                            {
                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]);
                                    lastDir = LastDirection.Right;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Left)
                                    {
                                        CreatePath();
                                    }
                                }

                            }
                            else
                            {
                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]);
                                    lastDir = LastDirection.Down;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Up)
                                    {
                                        CreatePath();
                                    }
                                }

                            }
                        }

                        else if (GetGridPosFromWorld(path[path.Count - 1].transform.position).y == 0)
                        {
                            //0 esquerda
                            //1 cima
                            //2 direita

                            if (randRange == 0)
                            {
                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]);
                                    lastDir = LastDirection.Up;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Down)
                                    {
                                        CreatePath();
                                    }
                                }
                            }
                            else if (randRange == 1)
                            {
                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]);
                                    lastDir = LastDirection.Left;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Right)
                                    {
                                        CreatePath();
                                    }
                                }
                            }
                            else
                            {
                                //Debug.Log(path[path.Count - 1].transform.position);
                                //
                                //
                                //Debug.Log(path[path.Count - 1].transform.position + vectorRight);
                                //
                                //
                                //
                                //Debug.Log(GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight));
                                //
                                //
                                //
                                //
                                //Debug.Log(GetWorldPosFromGrid(path[path.Count - 1].transform.position).y);

                                //Debug.Log(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]);

                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]);
                                    lastDir = LastDirection.Right;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Left)
                                    {
                                        CreatePath();
                                    }
                                }
                            }
                        }
                    }

                    //BORDA DIREITA/CIME
                    else if (GetGridPosFromWorld(path[path.Count - 1].transform.position).x == 4 || GetGridPosFromWorld(path[path.Count - 1].transform.position).y == 4)
                    {
                        if (GetGridPosFromWorld(path[path.Count - 1].transform.position).x == 4)
                        {
                            if (randRange == 0)
                            {
                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]);
                                    lastDir = LastDirection.Up;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Down)
                                    {
                                        CreatePath();
                                    }
                                }
                            }
                            else if (randRange == 1)
                            {
                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]);
                                    lastDir = LastDirection.Left;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Right)
                                    {
                                        CreatePath();
                                    }
                                }
                            }
                            else
                            {
                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]);
                                    lastDir = LastDirection.Down;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Up)
                                    {
                                        CreatePath();
                                    }
                                }
                            }
                        }

                        else if (GetGridPosFromWorld(path[path.Count - 1].transform.position).y == 4)
                        {
                            //0 esquerda
                            //1 baixo
                            //2 direita

                            if (randRange == 0)
                            {
                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]);
                                    lastDir = LastDirection.Down;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Up)
                                    {
                                        CreatePath();
                                    }
                                }
                            }
                            else if (randRange == 1)
                            {
                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]);
                                    lastDir = LastDirection.Left;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Right)
                                    {
                                        CreatePath();
                                    }
                                }
                            }
                            else
                            {
                                if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]))
                                {
                                    path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]);
                                    lastDir = LastDirection.Right;
                                    CreatePath();

                                }
                                else
                                {
                                    if (lastDir == LastDirection.Left)
                                    {
                                        CreatePath();
                                    }
                                }
                            }

                        }

                    }

                    //CENTRO DA MATRIZ
                    else
                    {

                        if (UnityEngine.Random.Range(0, 2) == 0)
                        {
                            switch (GetGridPosFromWorld(path[path.Count - 1].transform.position).x)
                            {

                                case 1:
                                case 2:
                                case 3:

                                    if (randRangeMid == 0)
                                    {
                                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]))
                                        {
                                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]);
                                            lastDir = LastDirection.Up;
                                            CreatePath();

                                        }
                                        else
                                        {
                                            if (lastDir == LastDirection.Down)
                                            {
                                                CreatePath();
                                            }
                                        }
                                    }
                                    else if (randRangeMid == 1)
                                    {
                                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]))
                                        {
                                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]);
                                            lastDir = LastDirection.Down;
                                            CreatePath();

                                        }
                                        else
                                        {
                                            if (lastDir == LastDirection.Up)
                                            {
                                                CreatePath();
                                            }
                                        }
                                    }
                                    else if (randRangeMid == 2)
                                    {
                                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]))
                                        {
                                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]);
                                            lastDir = LastDirection.Left;
                                            CreatePath();

                                        }
                                        else
                                        {
                                            if (lastDir == LastDirection.Right)
                                            {
                                                CreatePath();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]))
                                        {
                                            path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]);
                                            lastDir = LastDirection.Down;
                                            CreatePath();

                                        }
                                        else
                                        {
                                            if (lastDir == LastDirection.Left)
                                            {
                                                CreatePath();
                                            }
                                        }
                                    }

                                    break;
                            }
                        }
                        else
                        {
                            switch (GetGridPosFromWorld(path[path.Count - 1].transform.position).y)
                            {
                            case 1:
                            case 2:
                            case 3:

                                if (randRangeMid == 0)
                                {
                                    if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]))
                                    {
                                        path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorUp).y]);
                                        lastDir = LastDirection.Up;
                                        CreatePath();

                                    }
                                    else
                                    {
                                        if (lastDir == LastDirection.Down)
                                        {
                                                CreatePath();
                                        }
                                    }
                                }
                                else if (randRangeMid == 1)
                                {
                                    if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]))
                                    {
                                        path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorDown).y]);
                                        lastDir = LastDirection.Down;
                                        CreatePath();

                                    }
                                    else
                                    {
                                        if (lastDir == LastDirection.Up)
                                        {
                                                CreatePath();
                                        }
                                    }
                                }
                                else if (randRangeMid == 2)
                                {
                                    if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]))
                                    {
                                        path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorLeft).y]);
                                        lastDir = LastDirection.Left;
                                        CreatePath();

                                    }
                                    else
                                    {
                                        if (lastDir == LastDirection.Right)
                                        {
                                            CreatePath();
                                        }
                                    }
                                }
                                else
                                {
                                    if (!path.Contains(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]))
                                    {
                                        path.Add(gameGrid[GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).x, GetGridPosFromWorld(path[path.Count - 1].transform.position + vectorRight).y]);
                                        lastDir = LastDirection.Down;
                                        CreatePath();

                                    }
                                    else
                                    {
                                        if (lastDir == LastDirection.Left)
                                        {
                                            CreatePath();
                                        }
                                    }
                                }
                                break;
                            }
                        }

                        


                    }


                }
            }
            

        }


        //TODO: Mudar para o outro script
        public void SetUpLine(List<GameObject> points)
        {
            lr.positionCount = points.Count;
            this.pointsToDraw = points;

            lr.startWidth = 0.03f;
            lr.endWidth = 0.03f;

            lr.material = nightCamMtrl;


            GameObject[] garbageGO;
            garbageGO = GameObject.FindGameObjectsWithTag("Garbage");
            List<GameObject> garbage = new List<GameObject>();

            //foreach (var g in garbageGO)
            //{
            //    garbage.Add(g);
            //}

            lr.transform.parent = garbageGO[UnityEngine.Random.Range(0, garbageGO.Length - 1)].gameObject.transform;
            //lr.transform.localPosition = new Vector3(-lr.transform.parent.localScale.x / 2 + 0.05f, lr.transform.parent.localScale.y / 2 + 0.01f, -lr.transform.parent.localScale.z / 2 + 0.05f);
            lr.transform.localPosition = new Vector3(-0.085f, 0.01f, -0.085f);

            if (pointsToDraw != null)
            {
                for (int i = 0; i < pointsToDraw.Count; i++)
                {
                    lr.SetPosition(i, new Vector3((pointsToDraw[i].transform.position.x - pointsToDraw[0].transform.position.x) / 6 + lr.gameObject.transform.position.x,
                                                  (pointsToDraw[i].transform.position.z - pointsToDraw[0].transform.position.z) / 6 + lr.gameObject.transform.position.y,
                                                  (pointsToDraw[i].transform.position.y - pointsToDraw[0].transform.position.y) / 6 + lr.gameObject.transform.position.z));

                }
            }


        }


        public void ReceiveMouseClick()
        {
            isClick = true;
            Mouse mouse = Mouse.current;

            mousePosition = mouse.position.ReadValue();
            
        }


        private GridCell IsMouseOverAGridSpace()
        {

            //TODO: this
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());


            if (Physics.Raycast(ray, out RaycastHit hit, 2f, tabletScreen))
            {
                ray = rtCamera.ViewportPointToRay(new Vector2(hit.textureCoord.x, hit.textureCoord.y));
            
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, gridLayer))
                {
                    return hitInfo.transform.GetComponent<GridCell>();
                }
                else
                {
                    isClick = false;
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void Reset()
        {
            path.Clear();
            clickPath.Clear();

            while (path.Count <= 5)
            {
                CreatePath();
            }

            SetUpLine(path);
        }

        //private void UpdateLRPosition(Vector3 newPos)
        //{
        //    if (pointsToDraw != null)
        //    {
        //        for (int i = 0; i < pointsToDraw.Count; i++)
        //        {
        //            lr.SetPosition(i, new Vector3((pointsToDraw[i].transform.position.x - pointsToDraw[0].transform.position.x) / 5 + lr.GetPosition(0).x, 
        //                                          (pointsToDraw[i].transform.position.z - pointsToDraw[0].transform.position.z) / 5 + lr.GetPosition(0).y, 
        //                                          (pointsToDraw[i].transform.position.y - pointsToDraw[0].transform.position.y) / 5 + lr.GetPosition(0).z));

        //        }
        //    }
        //}

        private void OnEnable()
        {
            input.OnClickAction += ReceiveMouseClick;
        }

        private void OnDisable()
        {
            input.OnClickAction -= ReceiveMouseClick;
        }

    }

}
