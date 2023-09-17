using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Interaction_System;
using LevelManagement;
using UnityEngine;

namespace Puzzles
{
    [Serializable]
    public class ElevatorCode : InteractableBehaviour
    {
        [SerializeField] LayerMask layer;
        [SerializeField] int codeSize = 4;
        [SerializeField] LevelManager levelManager;
        [SerializeField] float speed = 1;
        [SerializeField] AudioClip click;

        public override bool Finished { get; protected set; }

        AudioSource _audioSource;
        List<int> _code = new List<int>();
        List<Material> highlighted = new List<Material>();
        static readonly int Amount = Shader.PropertyToID("_Amount");
        static readonly int highlightColor = Shader.PropertyToID("_Color");

        protected override void OnInitialize()
        {
            _audioSource = Transform.GetComponent<AudioSource>();
        }

        public override void OnLeftMouse(Vector2 pos)
        {
            Collider? button = SelectionManager.GetObjectScreenPoint(pos, layer: layer);

            if (button == null)
                return;

            button.GetComponent<SafeButton>().OnButtonPressed(_code);

            Transform child = button.transform.GetChild(0);
            Vector3 childPos = child.position;
            
            child.DOMove(childPos + Vector3.forward * 0.0025f, 0.2f).OnComplete(() =>
                child.DOMove(childPos, 0.2f));

            HighlightButton(button);
            
            _audioSource.PlayOneShot(click);

            if (_code.Count != codeSize)
                return;

            TryGoNextLevel();
            _code.Clear();
        }

        async void TryGoNextLevel()
        {
            if (!levelManager.CheckCode(_code))
            {
                OnFail();
                ClearHighlights(Color.red);
                
                return;
            }
            
            ClearHighlights(Color.green);

            Finished = true;

            await Task.Delay(1000);

            Finished = false;

            //levelManager.LoadNextLevel();
        }

        async void HighlightButton(Collider button)
        {
            float t = 0;
            Material mat = button.transform.GetChild(1).GetComponent<MeshRenderer>().material;
            highlighted.Add(mat);
            
            mat.SetColor(highlightColor, Color.green);

            while (t <= 1)
            {
                mat.SetFloat(Amount, t);

                await Task.Yield();

                t += Mathf.Min(speed * Time.deltaTime, t == 1 ? 1 : 1 - t);
            }
        }

        async void ClearHighlights(Color color)
        {
            float t = 0;

            Material[] mats = highlighted.ToArray();
            highlighted.Clear();
            
            foreach (Material material in mats)
            {
                material.SetColor(highlightColor, color);
            }
            
            while (t <= 1)
            {
                foreach (Material material in mats)
                {
                    material.SetFloat(Amount, 1 - t);
                }
                
                await Task.Yield();

                t += Mathf.Min(speed * Time.deltaTime, t == 1 ? 1 : 1 - t);
            }
        }
    }
}