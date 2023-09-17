using System;
using UnityEngine;

namespace LevelManagement
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] LevelManager manager;
        [SerializeField] InputReader input;

        [Header("Buttons")] 
        [SerializeField] PhysicalButton startButton;
        [SerializeField] PhysicalButton optionsButton;
        [SerializeField] PhysicalButton quitButton;
        //[SerializeField] GameObject pannel;

        void Start()
        {
            manager.OpenDoors();
            input.DisableGameplay();
        }
        
        void OnDisable()
        {
            startButton.OnClick -= StartGame;
        }
        
        void StartGame()
        {
            manager.LoadFirstLevel();
            gameObject.SetActive(false);
            //pannel.GetComponent<BoxCollider>().enabled = true;
        }
        
        void OpenOptions()
        {
            
        }

        void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public void EnableGameplay()
        {
            startButton.OnClick += StartGame;
            optionsButton.OnClick += OpenOptions;
            quitButton.OnClick += QuitGame;
            
            input.EnableGameplay();
            manager.CloseDoors();
        }
    }
}