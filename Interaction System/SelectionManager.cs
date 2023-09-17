using System;
using System.Collections.Generic;
using System.Linq;
using State_Machine;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Interaction_System
{
    public class SelectionManager : MonoBehaviour
    {
        //static readonly int HighlightIntensity = Shader.PropertyToID("_HighlightIntensity");
        public static readonly Vector2 ViewportCenter = new Vector2(0.5f, 0.5f);
        public static readonly LayerMask AllLayers = ~0;

        //[SerializeField] InputActionReference interactAction;
        [SerializeField] InputReader input;

        static Camera _camera;
        Transform _transform;

        StateMachine _stateMachine;
        [SerializeField] InteractionState interactionState;
        [SerializeField] SearchState searchState;
#nullable enable
        Interactable? _selected;
#nullable disable

        void Awake()
        {
            _transform = GetComponent<Transform>();
            _camera = Camera.main;
                
            _stateMachine = new StateMachine(searchState);
            //interactAction.action.Enable();
        }

        void Start()
        {
            interactionState.Input = input;
            
            interactionState.OnFinish += OnInteraction;
        }

        void OnEnable()
        {
            input.OnInteractAction += OnInteraction;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _stateMachine.ForceChangeState(searchState);
            Cursor.lockState = CursorLockMode.Locked;
        }

        void OnDisable()
        {
            input.OnInteractAction -= OnInteraction;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void Update()
        {
            _stateMachine.Update();
        }

        void OnInteraction()
        {   
            _selected = searchState.Selected;
            
            print(_selected);
            
            if (_selected == null)
                return;
            
            input.DisableInteraction();
            
            if (_stateMachine.Current.Equals(interactionState))
                CancelInteraction();
            else
                Interact();

            void Interact()
            {
                interactionState.Selected = _selected;
                if (!_selected.IsFinished())
                    _stateMachine.ChangeState(interactionState);
                else
                    input.EnableInteraction();
            }

            void CancelInteraction()
            {
                _stateMachine.ChangeState(searchState);
            }

            //void DisableInput() => interactAction.action.Disable();
        }

        #nullable enable
        
        /// <summary>
        /// Gets the object at the center of the screen
        /// </summary>
        /// <param name="maxDist">How far can the object be</param>
        /// <returns></returns>
        public static Collider? GetObject(float maxDist = Mathf.Infinity, int layer = ~0)
        {
            return GetObject(ViewportCenter, maxDist, layer);
        }
        
        /// <summary>
        /// Gets the object in a screen position
        /// </summary>
        /// <param name="viewPos">The position on the screen to get the object
        /// Ranges from bottom left (0,0) to top right (1,1)</param>
        /// <param name="maxDist">How far can the object be</param>
        /// <returns></returns>
        public static Collider? GetObject(Vector2 viewPos, float maxDist = Mathf.Infinity, int layer = ~0)
        {
            return GetObject(viewPos, out _, maxDist, layer);
        }
        
        public static Collider? GetObject(Vector2 viewPos, out RaycastHit info, float maxDist = Mathf.Infinity, int layer = ~0)
        {
            Ray ray = _camera.ViewportPointToRay(viewPos);
            
            return Physics.Raycast(ray, out info, maxDist, layer) ? info.collider : null;
        }

        public static Collider[] GetObjects(Vector2 viewPos, float maxDist = Mathf.Infinity, int layer = ~0)
        {
            Ray ray = _camera.ViewportPointToRay(viewPos);

            return Physics.RaycastAll(ray, maxDist, layer).Select(hit => hit.collider).ToArray();
        }

        public static Collider? GetObjectScreenPoint(Vector2 screenPos, float maxDist = Mathf.Infinity, int layer = ~0)
        {
            return GetObjectScreenPoint(screenPos, out _, maxDist, layer);
        }
        
        public static Collider? GetObjectScreenPoint(Vector2 screenPos, out RaycastHit info, float maxDist = Mathf.Infinity, int layer = ~0)
        {
            Vector2 viewPos = screenPos / new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            return GetObject(viewPos, out info, maxDist, layer);
        }
        
        #nullable disable
    }
}