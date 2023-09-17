using System;
using UnityEngine;

namespace Player
{
    public class OnInteractionController : MonoBehaviour
    {
        [SerializeField] EventChannel onStartInteraction;
        [SerializeField] EventChannel onStopInteraction;
        MeshRenderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        void OnEnable()
        {
            onStartInteraction.Action += OnStartInteraction;
            onStopInteraction.Action += OnStopInteraction;
        }
        
        void OnDisable()
        {
            onStartInteraction.Action -= OnStartInteraction;
            onStopInteraction.Action -= OnStopInteraction;
        }

        void OnStopInteraction()
        {
            _renderer.enabled = true;
        }

        void OnStartInteraction()
        {
            _renderer.enabled = false;
        }
    }
}