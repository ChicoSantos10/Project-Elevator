using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using DG.Tweening;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] GameObject mainCamera;
        [SerializeField] CharacterController controller;
        [SerializeField] float speed = 5f;
        [SerializeField] float jumpHeight = 3.5f;
        [SerializeField] float gravity = -40f; // -9.81
        [SerializeField] LayerMask groundMask;

        //Stats
        [SerializeField] public int lives = 3;
        float originalHeight;
        [SerializeField] float crouchedHeight;

        [SerializeField] InputReader input;
        [SerializeField] LayerMask ceilingLayer;
        [SerializeField] Puzzles.ButtonPuzzle bPuzzle;
        [SerializeField] LevelManagement.LevelManager manager;

        [SerializeField] PlayableDirector director;
        [SerializeField] PlayableDirector cutsceneDirector;
        [SerializeField] GameObject dirObject;
        [SerializeField] private AudioClip walkingSound;

        [Header("Ragdoll")]
        [SerializeField] GameObject ragdollPrefab;
        GameObject _ragdoll;
        [SerializeField] GameObject playerBody;
        
        private AudioSource audioSource;
        Vector2 horizontalInput;
        bool jump;
        bool isGrounded;
        Vector3 verticalVelocity = Vector3.zero;
        Ray crouchRay;
        RaycastHit hitData;
        bool waitForSpace;
        Vector3 startPosition;

        public bool hasKeycard = false;
        int currentScene;

        [SerializeField] Material eyeVignetteMat;
        public bool dying = false;
        float eyeVignetteTimer = 0f;
        AudioManager audioM;
        Interaction_System.SelectionManager sManager;
        [SerializeField] bool canbeDetected = true;
        public bool invincible = false;
        
        bool cutscene = false;
        int helpJ = 0;

        private void Awake()
        {
            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;
            //director.played += DirectorPlayed;
            //director.stopped += DirectorStopped;
            audioSource = GetComponent<AudioSource>();
        }


        private void Start()
        {
            startPosition = gameObject.transform.position;
            originalHeight = controller.height;

            eyeVignetteMat.SetInt("_PlayerDying", 0);
            eyeVignetteMat.SetFloat("_Exponential", 0);
            eyeVignetteTimer = 0;
            dying = false;
            audioM = FindObjectOfType<AudioManager>();
            sManager = FindObjectOfType<Interaction_System.SelectionManager>();
        }

        void OnEnable()
        {
            input.OnJumpAction += OnJumpPressed;
            input.OnMoveAction += ReceiveInput;
            input.OnCrouchEnableAction += OnCrouchPressed;
            input.OnCrouchDisableAction += OnCrouchReleased;
        }

        void OnDisable()
        {
            input.OnJumpAction -= OnJumpPressed;
            input.OnMoveAction -= ReceiveInput;
            input.OnCrouchEnableAction -= OnCrouchPressed;
            input.OnCrouchDisableAction -= OnCrouchReleased;
        }

        private void Update()
        {
            CheckForAnomaly();
            Move();
            Jump();


            if (waitForSpace)
            {
                OnCrouchReleased();
            }


            if (dying)
            {
                if (eyeVignetteTimer < 15)
                {
                    eyeVignetteTimer += Time.deltaTime * 2;
                    eyeVignetteMat.SetFloat("_Exponential", eyeVignetteTimer);
                }
            }
            else
            {
                if (eyeVignetteTimer >= 0)
                {
                    eyeVignetteTimer -= Time.deltaTime * 2;
                    eyeVignetteMat.SetFloat("_Exponential", eyeVignetteTimer);
                }
            }

        }

        #region Movement

        void CheckForAnomaly()
        {
            if (gameObject.transform.position.y <= -20)
            {
                gameObject.transform.position = new Vector3(1.752489f, 0.08f, -0.03f);
            }
        }


        void Move()
        {
            isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, gameObject.transform.position.y, transform.position.z), 0.1f, groundMask);



            //Debug.Log(isGrounded);

            if (isGrounded)
            {
                verticalVelocity.y = 0;
            }

            Vector3 horizontalVelocity = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * speed;
            controller.Move(horizontalVelocity * Time.deltaTime);

            if (horizontalVelocity.sqrMagnitude > 0)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = walkingSound;
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }
        }

        void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector3(transform.position.x, gameObject.transform.position.y, transform.position.z), 0.1f);
        }

        void Jump()
        {
            // Jump: v = sqrt(-2 * jumpHeight * gravity)
            if (jump && isGrounded)
            {
                verticalVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);

                jump = false;
            }

            verticalVelocity.y += gravity * Time.deltaTime;
            controller.Move(verticalVelocity * Time.deltaTime);
        }

        #endregion

        #region Input

        public void ReceiveInput(Vector2 _horizontalInput)
        {
            horizontalInput = _horizontalInput;
        }

        public void OnJumpPressed()
        {
            if (isGrounded)
            {
                audioM.Play("grunt");
                jump = true;
            }
        }

        public void OnCrouchPressed()
        {
            Debug.Log("crouch");
            controller.height = crouchedHeight;
            controller.center = new Vector3(0, 0.5f, 0);
            mainCamera.transform.localPosition = new Vector3(0, controller.center.y + controller.height / 2, 0);
            speed = 3f;
        
        }

        public void OnCrouchReleased()
        {
            crouchRay = new Ray(transform.position, transform.up);
            //Debug.DrawLine(crouchRay.origin, hitData.point, Color.green);

            if (!Physics.Raycast(crouchRay, out hitData, 2f, ceilingLayer))
            {
                waitForSpace = false;
                controller.height = originalHeight;
                controller.center = new Vector3(0, 0.9f, 0);
                mainCamera.transform.localPosition = new Vector3(0, 1.623561f, 0);
                speed = 5f;

            }
            else
            {
                waitForSpace = true;
            }
        

        }

        #endregion



        public void Detected()
        {

            
            if (canbeDetected)
            {
                //sManager.ExitInteractionWhenCaught();   
                //Gets Tasered
                lives -= 1;

                //Update lives UI

                RespawnAnimation();
            }
            


        }

        
        public void Restart()
        {
            Debug.Log("morreu, restart");
            //gameObject.transform.position = startPosition;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

        
        public void RespawnAnimation()
        {
            input.DisableGameplay();
            invincible = true;
            director.Play();
            eyeVignetteMat.SetInt("_PlayerDying", 1);
            dying = true;
            audioM.Play("playerCaught");

            _ragdoll = Instantiate(ragdollPrefab, gameObject.transform);
            playerBody.SetActive(false);
            
            Debug.Log(audioM.IsPlaying("playerCaught"));
            Debug.Log("Play animation");
        }

        public void BackToElevator()
        {

   
            if (lives <= 0)
            {
                Restart();
            }
            else
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    if (SceneManager.GetSceneAt(i).name != "Elevator")
                    {
                        currentScene = SceneManager.GetSceneAt(i).buildIndex;
                    }

                }

                Debug.Log("aaaaaa");
                //SceneManager.UnloadSceneAsync(currentScene);
                manager.LoadLevel(currentScene);
            }
             
            input.EnableGameplay();
            
            Destroy(_ragdoll);
            playerBody.SetActive(true);
            
            gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
            gameObject.transform.position = new Vector3(1.752489f, 0.08f, -0.03f);
            gameObject.GetComponent<CharacterController>().center = new Vector3(0, 0.9f, 0);
            mainCamera.transform.localPosition = new Vector3(-0.0599999987f, 1.62356114f, 0.0240000002f);
            mainCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            mainCamera.GetComponent<Camera>().fieldOfView = 60;
            //eyeVignetteMat.SetInt("_PlayerDying", 0);
            //eyeVignetteMat.SetFloat("_Exponential", 0);
            //eyeVignetteTimer = 0;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            //eyeVignetteMat.SetFloat("_Exponential", 8);
            dying = false;
            invincible = false;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }


        void StartCutscene()
        {
            input.DisableGameplay();
            cutsceneDirector.Play();
        }

        public void EndCutscene()
        {
            Debug.Log("acabou");
        }

        public void NearPortal()
        {
            GameObject.Find("PlaneATimeline").GetComponent<PlayableDirector>().Play();
        }

        public void StartAlienCutscene()
        {
            GameObject.Find("AlienTimeline").GetComponent<PlayableDirector>().Play();
        }


        private void OnTriggerEnter(Collider other)
        {

            
            
            if (other.gameObject.tag == "TriggerCutscene" && cutscene == false)
            {
                cutscene = true;
                other.gameObject.GetComponent<BoxCollider>().enabled = false;
                StartCutscene();
            }
        }
    }
}
