using System;
using System.Collections;
using System.Collections.Generic;
using Attributes;
using Extras;
using SSSTools.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelManagement
{
    [RequireComponent(typeof(Animation))]
    public class LevelManager : MonoBehaviour
    {   
        [SerializeField] AnimationClip open, close;
        [SerializeField, ExposeScriptableObject] List<Code> codes = new List<Code>();
        [SerializeField] private AudioClip elevatorbellSound;
        [SerializeField] List<AudioClip> levelMusics = new List<AudioClip>();
        [SerializeField] AudioSource levelAudioSource;
        
        private AudioSource audioSource;
        
        Animation _animator;

        int _currentLevel = 0;

        int _loadedScene = -1;
        bool _loading = false;
        
        void Awake()
        {
            _animator = GetComponent<Animation>();
            audioSource = GetComponent<AudioSource>();
        }

        public void LoadFirstLevel()
        {
            _currentLevel = 0;
            LoadNextLevel();
        }

        public void LoadNextLevel()
        {
            LoadLevel(_currentLevel + 1);
        }

        public void RestartLevel()
        {
            LoadLevel(_currentLevel);
        }

        public void LoadLevel(int index)
        {
            if (_loading)
                return;

            _loading = true;
            
            // Close elevator doors
            CloseDoors();
            
            // Unload prev scene
            
            // Start loading next level
            AsyncOperation loader = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
            loader.allowSceneActivation = false;

            StartCoroutine(OpenScene());
            
            loader.completed += o => OpenDoors();

            IEnumerator OpenScene()
            {
                yield return new WaitForSeconds(close.length + 5f);
                loader.allowSceneActivation = true;

                levelAudioSource.clip = levelMusics[index - 1];
                levelAudioSource.Play();
                
                // TODO: Improve
                if (_loadedScene != -1) 
                    SceneManager.UnloadSceneAsync(_loadedScene);

                _loadedScene = index;
                _currentLevel = index - 1;

                _loading = false;
            }
        }

        public void OpenDoors()
        {
            _animator.clip = open;
            _animator.Play();
            audioSource.PlayOneShot(elevatorbellSound);
        }
        
        public void CloseDoors()
        {
            _animator.clip = close;
            _animator.Play();    
        }

        public bool CheckCode(List<int> code)
        {
            foreach ((Code c, int i) in codes.WithIndex())
            {
                if (!c.IsCodeCorrect(code)) 
                    continue;
                
                LoadLevel(i + 2);
                return true;
            }

            return false;

            //return codes[_currentLevel - 1].IsCodeCorrect(code);
        }

        public string GetCode()
        {
            return codes[_currentLevel].ToString();
        }
    }
}