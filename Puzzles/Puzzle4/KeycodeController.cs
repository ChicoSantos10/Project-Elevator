using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class KeycodeController : MonoBehaviour
{
    [SerializeField] Player.PlayerController pController;
    [SerializeField] InputReader input;
    [SerializeField] LayerMask keycodeLayer;
    Ray ray;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    }


    

    private void Interacted()
    {
        Debug.Log("Interagiu");

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (!GameObject.Find("PlayerPrefab").GetComponent<Player.PlayerController>().hasKeycard)
        {
        

            if (Physics.Raycast(ray, out RaycastHit hit, 3f, keycodeLayer))
            {
                //gameObject.SetActive(false);
                
                Debug.Log("Apanhou keycard");
                hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                hit.collider.gameObject.GetComponent<MeshFilter>().mesh = null;
                GameObject.Find("PlayerPrefab").GetComponent<Player.PlayerController>().hasKeycard = true;
                //GameObject.FindGameObjectWithTag("Safe").gameObject.SetActive(true);
            }
        
        
        }
        
    }


    private void OnEnable()
    {
        Debug.LogWarning("YESKEY");
        input.OnInteractAction += Interacted;
    }

    private void OnDisable()
    {
        Debug.LogWarning("NOKEY");
        input.OnInteractAction -= Interacted;
    }
}
