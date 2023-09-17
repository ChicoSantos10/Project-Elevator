using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interaction_System;
using UnityEngine;

namespace Puzzles
{
    [Serializable]
    class TabletInteraction : InteractableBehaviour
    {
        [SerializeField] private GameObject lightObject;
        
        private List<Collider> clicks = new List<Collider>();
        LightPuzzle lightPuzzle;

        [SerializeField] float duration = 2;
        [SerializeField] float speed = 2;
        [SerializeField] float minDistance = 0.15f;
        [SerializeField] private AudioClip buttonSound, failSound, correctSound;
        private AudioSource audioSource;
        static readonly int Distance = Shader.PropertyToID("_Distance");
        static readonly int Position = Shader.PropertyToID("_Position");
        static readonly int MinDistance = Shader.PropertyToID("_MinDistance");

        List<Task> runningTasks = new List<Task>();

        public override bool Finished { get; protected set; }

        protected override void OnInitialize()
        {
            lightObject.GetComponent<Interactable>().TryGetBehaviour(out lightPuzzle);
            audioSource = Transform.GetComponent<AudioSource>();
        }

        public override void OnLeftMouse(Vector2 pos)
        {
            Collider? c = SelectionManager.GetObjectScreenPoint(pos, out RaycastHit info);
            
            if (c == null || !c.TryGetComponent(out ButtonLightSelection b))
            {
                return;
            }
            
            audioSource.PlayOneShot(buttonSound);
            
            if (clicks.Contains(b.PanelButton)) 
                return;
            
            clicks.Add(b.PanelButton);
            runningTasks.Add(PlayAnimation(c.GetComponent<MeshRenderer>().material, info.point));
            
            if (clicks.Count != 3) return;
            
            if (lightPuzzle.CheckSolution(clicks))
            {
                Debug.Log("Correct");
                Finished = true;
                audioSource.PlayOneShot(correctSound);
            }
            else
            {
                OnFail();
                audioSource.PlayOneShot(failSound);
                Reset();
            }
        }

        async Task PlayAnimation(Material m, Vector3 position)
        {
            float t = 0;
            float halfDuration = duration * 0.5f;
            float maxD = 0;
            m.SetVector(Position, position);
            while (t < duration)
            {
                maxD += speed * Time.deltaTime;
                m.SetFloat(Distance, maxD);

                if (t >= halfDuration)
                {
                    float minD = Mathf.Lerp(0, minDistance, (t - halfDuration) / halfDuration);
                    m.SetFloat(MinDistance, minD);
                }
                
                t += Time.deltaTime;
                await Task.Yield();
            }
        }

        async void Reset()
        {
            await Task.WhenAll(runningTasks);
            
            runningTasks.Clear();
            
            foreach (Transform child in Transform)
            {
                if (!child.TryGetComponent(out MeshRenderer meshRenderer))
                    continue;

                Material material = meshRenderer.material;
                material.SetFloat(Distance, 0);
                material.SetFloat(MinDistance, 0);
            }
            
            clicks.Clear();
        }
    }
}