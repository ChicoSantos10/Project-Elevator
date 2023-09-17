using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    static readonly int Strength = Shader.PropertyToID("_Strength");
    
    [SerializeField] Slider slider;
    [SerializeField] Material filterMat;
    [SerializeField] GameObject menu;
    [SerializeField] InputReader input;

    CursorLockMode _lockMode;
    float defaultInt;

    void Awake()
    {
        defaultInt = filterMat.GetFloat(Strength);
    }

    void OnEnable()
    {
        slider.onValueChanged.AddListener(SetIntensity);
    }

    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(SetIntensity);
    }

    void Update()
    {
        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
            return;
        
        menu.SetActive(!menu.activeSelf);

        if (menu.activeSelf)
        {
            _lockMode = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            input.DisableGameplay();
        }
        else
        {
            input.EnableGameplay();
            Cursor.lockState = _lockMode;
        }
    }

    void OnDestroy()
    {
        SetIntensity(defaultInt);
    }

    void SetIntensity(float intensity)
    {
        filterMat.SetFloat(Strength, intensity);
    }
}
