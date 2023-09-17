using System;
using System.Collections;
using SSSTools.FunText.Core;
using UnityEngine;

namespace Custom_Cursor
{
    public class CursorAnimator : MonoBehaviour
    {
        [SerializeField] CursorManager manager;
        
        void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Locked;

            StartCursorAnimation();
        }

        void OnEnable()
        {
            manager.OnAnimationChange += OnAnimationChange;
        }

        void OnDisable()
        {
            manager.OnAnimationChange -= OnAnimationChange;
        }

        void OnAnimationChange()
        {
            StopAllCoroutines();
            StartCursorAnimation();
        }

        void StartCursorAnimation()
        {
            StartCoroutine(AnimateCursor());
            
            IEnumerator AnimateCursor()
            {
                CustomWaitForSeconds timer = new CustomWaitForSeconds(1 / manager.Speed);

                while (true)
                {
                    Cursor.SetCursor(manager.NextFrame(), manager.Offset, CursorMode.Auto);
                    yield return timer.Wait(1 / manager.Speed);
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}