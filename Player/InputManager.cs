using System;
using Camera_Controller;
using UnityEngine;

namespace Player
{
    [Obsolete]
    public class InputManager : MonoBehaviour
    {
        [SerializeField] PlayerController pController;
        [SerializeField] CameraModesController cmController;
        [SerializeField] Grid.GameGrid gameGrid;

        private InputMaster controls;
        private InputMaster.GameplayActions playerIA;
        private Vector2 horizontalInput;
        private Vector2 posMouseClicked;


        private void Awake()
        {
            controls = new InputMaster();
            playerIA = controls.Gameplay;

            playerIA.Move.performed += context => horizontalInput = context.ReadValue<Vector2>();
            playerIA.Jump.performed += _ => pController.OnJumpPressed();
            playerIA.CameraDefaultMode.performed += _ => cmController.ActivateDefaultMode();
            playerIA.CameraNightVisionMode.performed += _ => cmController.ActivateNightVisionMode();
            playerIA.CameraXRayMode.performed += _ => cmController.ActivateXRayMode();
            playerIA.UseCamera.performed += _ => cmController.CameraOn();
            playerIA.UseCamera.canceled += _ => cmController.CameraOff();
            playerIA.Crouch.performed += _ => pController.OnCrouchPressed();
            playerIA.Crouch.canceled += _ => pController.OnCrouchReleased();
            playerIA.Click.performed += _ => gameGrid.ReceiveMouseClick();
        }

        private void Update()
        {
            pController.ReceiveInput(horizontalInput);

        }

        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }
    }
}
