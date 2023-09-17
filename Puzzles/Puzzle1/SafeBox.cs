using System;
using System.Collections.Generic;
using Interaction_System;
using SSSTools.Extensions;
using UnityEngine;
using DG.Tweening;

namespace Puzzles
{
    [Serializable]
    public class SafeBox : InteractableBehaviour
    {
        //override ao onleftMouse, buscar ao selectionmanager o collider, verificar se existe collider, se existir buscar a class safebutton /trygetcomponent/ OnButtonPressed

        List<int> code = new List<int>(3);
        [SerializeField] List<int> solution;
        [SerializeField] Transform door;
        [SerializeField] LayerMask layer;
        List<AddNumberButton> safeButtonList = new List<AddNumberButton>();
        [SerializeField] private AudioClip openSafe;
         AudioSource audioSource;


        bool safeDoorOpen;

        protected override void OnInitialize()
        {
            audioSource = Transform.GetComponent<AudioSource>();
        }


        bool CheckCode() //if checkcode
        {
            foreach ((int code,int i) in code.WithIndex())
            {
                if (code != solution[i])
                    return false;
            }

            return true;
        }

        public override bool Finished { get; protected set; }

        public override void OnLeftMouse(Vector2 pos)
        {          
            Collider? button = SelectionManager.GetObjectScreenPoint(pos, layer: layer);
            Debug.Log(button);
            if (button == null || !button.TryGetComponent(out SafeButton safeButton)) 
                return;
            
            safeButton.OnButtonPressed(code);
            safeButtonList.Add(safeButton as AddNumberButton);

            if (code.Count != solution.Count) 
                return;
                
            if (CheckCode())
            {
                foreach (AddNumberButton safeButton1 in safeButtonList)
                {
                    safeButton1.TurnToGreen();
                }

                door.DOLocalRotate(door.localRotation.eulerAngles + new Vector3(0, 0, -120), 3).SetDelay(1.5f).OnComplete(() =>
                {
                    foreach (AddNumberButton safeButton1 in safeButtonList)
                    {
                        safeButton1.StopHighLight();
                    }

                    Finished = true;
                    Transform.GetComponent<Collider>().enabled = false;
                });

                audioSource.PlayOneShot(openSafe);
            }
            else
            {
                code.Clear();
                OnFail();

                foreach (AddNumberButton safeButton1 in safeButtonList)
                {
                    safeButton1.StopHighLight();
                }
            }

        }

       
    }
}
