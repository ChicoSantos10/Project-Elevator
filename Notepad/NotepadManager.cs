using System;
using UnityEngine;

namespace Notepad
{
    public class NotepadManager : MonoBehaviour
    {
        [SerializeField] InputReader input;
        [SerializeField] GameObject notepad;

        Notepad _notepad;

        void Awake()
        {
            _notepad = notepad.GetComponent<Notepad>();
        }

        void OnEnable()
        {
            input.OnNotepadAction += OnNotepadAction;
        }
        
        void OnDisable()
        {
            input.OnNotepadAction -= OnNotepadAction;
        }

        void OnNotepadAction()
        {
            _notepad.OnNotepadAction();
        }
    }
}