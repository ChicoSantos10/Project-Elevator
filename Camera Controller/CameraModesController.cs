using System;
using Interaction_System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Camera_Controller
{
    public class CameraModesController : MonoBehaviour
    {
        enum CameraMode
        {
            Default,
            NightVision,
            XRay
        }

        private CameraMode cMode;

        [SerializeField] private Color defaultMode;
        [SerializeField] private Color boostedMode;
        [SerializeField] private Color xRayMode;
        private Color initialLight;
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private GameObject cameraObject;
        [SerializeField] private GameObject cameraDestiny;
        [SerializeField] private GameObject cameraStart;
        [SerializeField] private Material xRayMaterialShdr;

        [SerializeField] private UnityEngine.Rendering.Volume volumeNightVision;
        [SerializeField] private UnityEngine.Rendering.Volume volumeXray;

        [SerializeField] InputReader input;
        [SerializeField] GameObject lineRendererO;
        Color c;
        int a;

        private bool isDefaultMode = false;
        bool usingCamera = false;

        static readonly int StencilId = Shader.PropertyToID("_StencilId");

        [SerializeField] GameObject elevatorCodeJ;
        public bool puzzle1Completed = false;
        AudioManager audioM;

        // Start is called before the first frame update
        void Start()
        {
            audioM = FindObjectOfType<AudioManager>();
            a = 0;
            volumeXray.weight = 0;
            volumeNightVision.weight = 0;
            cMode = CameraMode.Default;
            initialLight = RenderSettings.ambientLight;
        }
        void OnEnable()
        {
            input.OnCameraEnableAction += CameraOn;
            input.OnCameraDisableAction += CameraOff;

            input.OnCameraDefaultAction += ActivateDefaultMode;
            input.OnCameraNightVisionAction += ActivateNightVisionMode;
            input.OnCameraXRayAction += ActivateXRayMode;
        }

        void OnDisable()
        {
            input.OnCameraEnableAction -= CameraOn;
            input.OnCameraDisableAction -= CameraOff;

            input.OnCameraDefaultAction -= ActivateDefaultMode;
            input.OnCameraNightVisionAction -= ActivateNightVisionMode;
            input.OnCameraXRayAction -= ActivateXRayMode;
        }

        // Update is called once per frame
        void Update()
        {

            
            switch (cMode)
            {
                case CameraMode.Default:

                   

                    volumeNightVision.weight = 0;
                    volumeXray.weight = 0;
                    volumeNightVision.gameObject.SetActive(false);
                    volumeXray.gameObject.SetActive(false);
                    RenderSettings.ambientLight = defaultMode;
                    xRayMaterialShdr.SetInt(StencilId, 0);
                    break;
                case CameraMode.NightVision:

                    //lineRendererO.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
                    volumeXray.gameObject.SetActive(false);
                    volumeNightVision.gameObject.SetActive(true);
                    volumeNightVision.weight = 1;
                    volumeXray.weight = 0;
                    RenderSettings.ambientLight = boostedMode;
                    xRayMaterialShdr.SetInt(StencilId, 0);
                    break;
                    
                case CameraMode.XRay:

                    //lineRendererO.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);
                    volumeNightVision.gameObject.SetActive(false);
                    volumeXray.gameObject.SetActive(true);
                    volumeNightVision.weight = 0;
                    volumeXray.weight = 1;
                    RenderSettings.ambientLight = xRayMode;
                    xRayMaterialShdr.SetInt(StencilId, 1);
                    break;
            }

        }




        //maybe criar um toggle depois isHolding = !isholding... setactive(isholding)
        public void CameraOn()
        {
            
            cameraObject.SetActive(true);
            //cameraObject.transform.localPosition = Vector3.MoveTowards(cameraObject.transform.localPosition, new Vector3(0, 0, -0.5f), Time.deltaTime);

            usingCamera = true;

            cameraObject.transform.localPosition = new Vector3(0, 0, -0.5f);


            //TODO: Find a better way to not render the xray material in the main camera
            mainCamera.GetComponent<UnityEngine.Camera>().cullingMask = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) |
                                                                        (1 << 7) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 12) |
                                                                        (1 << 14) | (1 << 16) | (1 << 17) | (1 << 18) | 
                                                                        (1 << 19) | (1 << 20) | (1 << 21) | (1 << 22) | (1 << 23) | (1 << 24)
                                                                        | (1 << 25) | (1 << 26) | (1 << 27);
            Debug.LogWarning("Alterar culling mask aqui");
            audioM.Play("useCam");
        }

        public void CameraOff()
        {
            cameraObject.SetActive(false);

            ActivateDefaultMode();
            usingCamera = false;
            //cameraObject.transform.localPosition = Vector3.MoveTowards(cameraObject.transform.localPosition, new Vector3(2.48000002f, 0, 0.610000014f), Time.deltaTime);
            cameraObject.transform.localPosition = new Vector3(2.48000002f, 0, 0.610000014f);

            mainCamera.GetComponent<UnityEngine.Camera>().cullingMask = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) |
                                                                        (1 << 7) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 12) | 
                                                                        (1 << 13) | (1 << 14) | (1 << 16) | (1 << 17) | (1 << 18) | 
                                                                        (1 << 19) | (1 << 20) | (1 << 21) | (1 << 22) | (1 << 23) | (1 << 24)
                                                                        | (1 << 25) | (1 << 26) | (1 << 27);

            Debug.LogWarning("Alterar culling mask aqui");
            audioM.Play("useCam");

            xRayMaterialShdr.SetInt("_StencilId", 0);

            RenderSettings.ambientLight = initialLight;

        }

        public void ActivateDefaultMode()
        {
            if (usingCamera)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    if (SceneManager.GetSceneAt(i).name == "PuzzleRF")
                    {
                        if (GameObject.Find("LineRenderer") != null)
                        {
                            GameObject.Find("LineRenderer").GetComponent<Renderer>().material.SetFloat("_NightCam", 0);
                        }
                        
                    }
                    else if (SceneManager.GetSceneAt(i).name == "Puzzle1")
                    {
                        if (GameObject.Find("ElevatorCodeJ") != null)
                        {
                            GameObject.Find("ElevatorCodeJ").GetComponent<Text>().enabled = false;
                        }
                        
                        //elevatorCodeJ = GameObject.Find("ElevatorCodeJ");
                        //elevatorCodeJ.SetActive(false);

                        //Debug.LogWarning("caso de erro mudar para gameobject.find(elevatorCodeJ)");
                    }
                    else if (SceneManager.GetSceneAt(i).name == "FloorChicoManu")
                    {
                        if (GameObject.Find("ScaleFinal") != null && GameObject.Find("Tablet") != null && GameObject.Find("codigo") != null)
                        {
                            bool ArePuzzlesFinished()
                            {
                                return GameObject.Find("ScaleFinal").GetComponent<Interactable>().IsFinished() &&
                                       GameObject.Find("Tablet").GetComponent<Interactable>().IsFinished();
                            }
                            if (ArePuzzlesFinished())
                            {
                                GameObject.Find("codigo").GetComponent<TextMeshPro>().enabled = false;
                            }
                        }
                        
                        
                    }
                }

                audioM.Play("changeCamMode");
                cMode = CameraMode.Default;
            }
        }
        public void ActivateNightVisionMode()
        {
            if (usingCamera)
            {

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    if (SceneManager.GetSceneAt(i).name == "PuzzleRF")
                    {
                        if (GameObject.Find("LineRenderer"))
                        {
                            GameObject.Find("LineRenderer").GetComponent<Renderer>().material.SetFloat("_NightCam", 1);
                        }
                        
                    }
                    else if (SceneManager.GetSceneAt(i).name == "Puzzle1" && puzzle1Completed == true )
                    {
                        if (GameObject.Find("ElevatorCodeJ"))
                        {
                            GameObject.Find("ElevatorCodeJ").GetComponent<Text>().enabled = true;
                        }
                        


                        //elevatorCodeJ.SetActive(true);

                        //Debug.Log(GameObject.Find("ElevatorCodeJ").name);
                    }
                    else if (SceneManager.GetSceneAt(i).name == "FloorChicoManu")
                    {
                        if (GameObject.Find("ScaleFinal") != null && GameObject.Find("Tablet") != null && GameObject.Find("codigo") != null)
                        {
                            bool ArePuzzlesFinished()
                            {
                                return GameObject.Find("ScaleFinal").GetComponent<Interactable>().IsFinished() &&
                                       GameObject.Find("Tablet").GetComponent<Interactable>().IsFinished();
                            }
                            if (ArePuzzlesFinished())
                            {
                                GameObject.Find("codigo").GetComponent<TextMeshPro>().enabled = true;
                            }
                        }
                        
                        
                    }
                }
                audioM.Play("changeCamMode");
                cMode = CameraMode.NightVision;

                //Debug.Log(GameObject.Find("Puzzle1").GetComponent<Puzzles.CubePuzzleBehaviour>().Finished);
            }

        }
        public void ActivateXRayMode()
        {
            if (usingCamera)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    if (SceneManager.GetSceneAt(i).name == "PuzzleRF")
                    {
                        if (GameObject.Find("LineRenderer") != null)
                        {
                            GameObject.Find("LineRenderer").GetComponent<Renderer>().material.SetFloat("_NightCam", 0);
                        }
                        
                    }
                    else if (SceneManager.GetSceneAt(i).name == "Puzzle1")
                    {
                        if (GameObject.Find("ElevatorCodeJ") != null)
                        {
                            GameObject.Find("ElevatorCodeJ").GetComponent<Text>().enabled = false;
                        }
                        


                        //elevatorCodeJ.SetActive(false);

                        //Debug.Log(GameObject.Find("ElevatorCodeJ").name);
                    }
                    else if (SceneManager.GetSceneAt(i).name == "FloorChicoManu")
                    {

                        if (GameObject.Find("ScaleFinal") != null && GameObject.Find("Tablet") != null && GameObject.Find("codigo") != null)
                        {
                            bool ArePuzzlesFinished()
                            {
                                return GameObject.Find("ScaleFinal").GetComponent<Interactable>().IsFinished() &&
                                       GameObject.Find("Tablet").GetComponent<Interactable>().IsFinished();
                            }
                            if (ArePuzzlesFinished())
                            {
                                GameObject.Find("codigo").GetComponent<TextMeshPro>().enabled = true;
                            }
                        }
                        
                        
                    }
                }
                audioM.Play("changeCamMode");                
                cMode = CameraMode.XRay;
            }

        }

        


    }
}
