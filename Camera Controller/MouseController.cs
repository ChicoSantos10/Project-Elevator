using UnityEngine;

namespace Camera_Controller
{
    public class MouseController : MonoBehaviour
    {
        private InputMaster controls;
        [SerializeField] private float mouseSensitivity = 100f;
        private Vector2 mouseLook;
        private float yRotation = 0f;
        private Transform playerBody;

        [SerializeField] InputReader input;

        [HideInInspector] public float mouseX;
        [HideInInspector] public float mouseY;

        private void Awake()
        {
            playerBody = transform.parent;

            controls = new InputMaster();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Look();
        }
        
        private void Look()
        {
            //mouseLook = controls.Gameplay.Look.ReadValue<Vector2>();
            mouseX = mouseLook.x * mouseSensitivity * Time.deltaTime;
            mouseY = mouseLook.y * mouseSensitivity * Time.deltaTime;

            //transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
            // TODO: Find a way to clamp rotation without overriding current rotation 

            transform.Rotate(Vector3.right * -mouseY);
            
            // yRotation -= mouseY;
            // yRotation = Mathf.Clamp(yRotation, -85f, 85f);
            //
            // transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
            
            playerBody.Rotate(Vector3.up * mouseX);
        }

        private void OnEnable()
        {
            controls.Enable();

            input.OnLookAction += OnLook;
        }

        void OnLook(Vector2 v)
        {
            mouseLook = v;
        }

        private void OnDisable()
        {
            //controls.Disable();

            input.OnLookAction -= OnLook;
        }
    }
}
