using UnityEngine;

namespace Camera_Controller
{
    public class CameraSway : MonoBehaviour
    {
        [SerializeField] float amount;
        [SerializeField] float maxAmount;
        [SerializeField] float smoothAmount;

        private Vector2 mouseLook;
        private InputMaster controls;

        private Vector3 initialPosition;

        private void Awake()
        {
            controls = new InputMaster();
        }

        void Start()
        {
            initialPosition = transform.localPosition;
        }


        void Update()
        {
            controls.Gameplay.Look.ReadValue<Vector2>();

            float movementX = -mouseLook.x * amount;
            float movementY = -mouseLook.y * amount;
            movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
            movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

            Vector3 finalPosition = new Vector3(movementX, movementY, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
        }
    }
}
