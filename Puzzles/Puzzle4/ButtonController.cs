using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Robot;
using System;

public class ButtonController : MonoBehaviour
{
    [SerializeField] Puzzles.ButtonPuzzle bPuzzle;
    [SerializeField] InputReader input;
    [SerializeField] LayerMask button;
    [SerializeField] Robot.RobotMovement rMov;
    bool isClick = false;
    [SerializeField] bool buttonDown = false;
    [SerializeField] bool buttonUp = true;
    float timer = 0;
    Vector3 initialPos;
    [SerializeField] bool canClick = true;
    [SerializeField] GameObject quad;
    int help = 0;
    AudioManager audioM;
    int help2 = 0;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = gameObject.transform.localPosition;
        audioM = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (canClick)
        {

            if (Physics.Raycast(ray, out RaycastHit hit, 2f, button))
            {
                //canClick = true;

                if (hit.collider.gameObject.GetComponent<ButtonController>().isClick && hit.collider.gameObject.GetComponent<ButtonController>().buttonUp && !hit.collider.gameObject.GetComponent<ButtonController>().buttonDown)
                {
                    hit.collider.gameObject.GetComponent<ButtonController>().buttonDown = true;
                    hit.collider.gameObject.GetComponent<ButtonController>().buttonUp = false;
                    hit.collider.gameObject.GetComponent<ButtonController>().isClick = false;

                    Debug.Log("0");
                    RobotMovement.ClosestRobotToTarget(hit.collider.gameObject);
                    Debug.Log("01");
                    audioM.Play("pressButton");
                    //hit.transform.DOLocalMove(hit.transform.localPosition + new Vector3(0, -0.3f, 0), 1f);                    
                    hit.transform.DOLocalMove(hit.collider.gameObject.transform.localPosition + new Vector3(0, -0.3f, 0), 1f).OnComplete(
                        () => ButtonDown(hit));
                    //canClick = false;


                    if (!bPuzzle.puzzleCompleted && !bPuzzle.clickedButton.Contains(hit.collider.gameObject))
                    {
                        if (bPuzzle.randomButtons[bPuzzle.help] == hit.transform.parent.gameObject && bPuzzle.help <= 4)
                        {
                            Debug.Log("3");

                            hit.transform.gameObject.GetComponent<ButtonController>().quad.SetActive(true);
                            Debug.Log("31");

                            hit.transform.gameObject.GetComponent<ButtonController>().quad.GetComponent<Renderer>().material = bPuzzle.materials[bPuzzle.help];
                            Debug.Log("32");

                            if (bPuzzle.clickedButton.Count < 5)
                            {
                                bPuzzle.clickedButton.Add(hit.transform.parent.gameObject);
                            }
                            
                            Debug.Log("33");

                            if (bPuzzle.help < 4)
                            {
                                bPuzzle.help++;
                            }
                        }
                    }
                }
            }

            else
            {

                isClick = false;
                //canClick = false;
            }
        }


        if (bPuzzle.clickedButton.Count == 5)
        {
            PuzzleCompleted();
        }


        //CheckIfFinished();

    }

    void ButtonDown(RaycastHit hit)
    {
        Debug.LogWarning("buttondown");
        //canClick = false;
        hit.collider.gameObject.GetComponent<ButtonController>().buttonDown = true;
        hit.collider.gameObject.GetComponent<ButtonController>().buttonUp = false;

        if (help == 0)
        {
            hit.transform.DOLocalMove(hit.collider.gameObject.transform.localPosition + new Vector3(0, 0.3f, 0), 1).OnComplete(() => ButtonUp(hit));
            help = 1;
        }
    }

    void ButtonUp(RaycastHit hit)
    {
        help = 0;
        hit.collider.gameObject.GetComponent<ButtonController>().buttonDown = false;
        hit.collider.gameObject.GetComponent<ButtonController>().buttonUp = true;
        hit.collider.gameObject.GetComponent<ButtonController>().canClick = true;
    }
    private void CheckIfFinished()
    {

        if (bPuzzle.clickedButton.Count == bPuzzle.randomButtons.Count)
        {
            //Debug.Log("finished");
            bPuzzle.puzzleCompleted = true;
            bPuzzle.keyCard.SetActive(true);
        }
    }

    private void PuzzleCompleted()
    {
        if (help2 == 0)
        {
            audioM.Play("puzzleCompleted");
            Debug.LogWarning("PUZZLE COMPLETED");
            bPuzzle.puzzleCompleted = true;
            bPuzzle.keyCard.GetComponent<MeshRenderer>().enabled = true;
            bPuzzle.keyCard.GetComponent<BoxCollider>().enabled = true;
            help2 = 1;
        }
        
    }

    //private GameObject ClosestButton()
    //{
    //    GameObject[] buttons;
    //    buttons = GameObject.FindGameObjectsWithTag("Button");
    //
    //
    //    return button;
    //}

    public void ReceiveMouseClick()
    {
        isClick = true;
        Mouse mouse = Mouse.current;



    }

    private void OnEnable()
    {
        input.OnClickAction += ReceiveMouseClick;
    }

    private void OnDisable()
    {
        input.OnClickAction -= ReceiveMouseClick;
    }
}