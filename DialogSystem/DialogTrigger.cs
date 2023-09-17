using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace DialogSystem
{
    [RequireComponent(typeof(Collider))]
    public class DialogTrigger : MonoBehaviour
    {   
        [SerializeField] protected DialogEvent openDialogEvent;
        [SerializeField] protected DialogEvent closeDialogEvent;
        [SerializeField] protected Prompt dialog;
        [SerializeField, Tooltip("If the dialog box should only show in the collider area")] bool areaOnly;

        void OnTriggerEnter(Collider other)
        {
            Open();
        }

        void OnTriggerExit(Collider other)
        {
            if (areaOnly)
                Close();
        }

        protected void Open()
        {
            openDialogEvent.Invoke(dialog);
        }

        protected void Close()
        {
            closeDialogEvent.Invoke(dialog);
        }

        [Obsolete]
        protected virtual IEnumerator OnDialogOpen()
        {
            yield return null;
        }
        
        [Obsolete]
        protected virtual IEnumerator OnDialogClose()
        {
            yield return null;
        }

        void OnValidate()
        {
            Collider c = GetComponent<Collider>();
            if (c.isTrigger)
                return;
            
            Debug.LogError("Collider must be trigger");
            c.isTrigger = true;
        }
    }
}