using System;
using System.Collections;
using SSSTools.FunText.Core;
using UnityEngine;

namespace DialogSystem
{
    [CreateAssetMenu(menuName = "Dialog", fileName = "Dialog", order = 0)]
    public class Prompt : ScriptableObject
    {
        [SerializeField] Dialog[] dialog;
        [SerializeField] bool disableGameplay;
        [SerializeField] bool enableGameplay = true;

        [NonSerialized] int _current = -1;

        public bool DisableGameplay => disableGameplay;
        public bool EnableGameplay => enableGameplay;

        public Dialog CurrentDialog => dialog[_current];

        public bool Finished => _current >= dialog.Length;

        public bool GetNextText(out string text)
        {
            if (++_current < dialog.Length)
            {
                text = dialog[_current].Text;
                return true;
            }

            //Reset();
            text = default;
            return false;
        }

        public virtual IEnumerator WaitGoNext()
        {   
            CurrentDialog.Wait.Enable();
            return CurrentDialog.Wait;
        }

        public bool UseInput => CurrentDialog.UseInput;

        public virtual IEnumerator Timer => CurrentDialog.Timer;

        public void Reset() => _current = -1;
    }

    [Serializable]
    public class Dialog
    {
        [SerializeField, TextArea] string text;
        [SerializeField] bool useInput;
        [SerializeField] WaitInput wait;
        [SerializeField, Tooltip("How long for the text to disappear")] CustomWaitForSeconds timer;

        public string Text => text;

        public WaitInput Wait => wait;

        public CustomWaitForSeconds Timer => timer;

        public bool UseInput => useInput;
    }
}