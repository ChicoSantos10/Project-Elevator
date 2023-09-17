using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class CofreController : MonoBehaviour
{

    //[SerializeField] Player.PlayerController pController;
    [SerializeField] InputReader input;
    [SerializeField] LayerMask safeLayer;
    [SerializeField] GameObject codePage;
    bool open = false;
    Ray ray;

    // Start is called before the first frame update
    void Start()
    {
        codePage.GetComponent<Text>().text = GameObject.Find("untitled").GetComponent<LevelManagement.LevelManager>().GetCode();

        //Debug.LogError(GameObject.Find("untitled").GetComponent<LevelManagement.LevelManager>().GetCode());
    }

      

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    }

    

    private void Interacted()
    {

        
        if (Physics.Raycast(ray, out RaycastHit hit, 3f, safeLayer))
        {
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player.PlayerController>().hasKeycard)
            {
                if (open == false)
                {
                    gameObject.transform.DORotate(new Vector3(0, -120, 0), 1f, RotateMode.WorldAxisAdd);
                    open = true;
                }
        
                //Debug.Log($"Código {Random.Range(1000, 10000)}");
            }
            else
            {
                Debug.Log("Não tem keycard");
            }
        }

    }



    private void OnEnable()
    {
        input.OnInteractAction += Interacted;
    }

    private void OnDisable()
    {
        input.OnInteractAction -= Interacted;
    }
}
