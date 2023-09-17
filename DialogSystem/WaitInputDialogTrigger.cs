using System;
using System.Collections;
using SSSTools.FunText.Core;
using UnityEngine;

namespace DialogSystem
{
    [Obsolete]
    public class WaitInputDialogTrigger : DialogTrigger
    {
        [SerializeField] WaitInput inputKey;
        [SerializeField] CustomWaitForSeconds timer;
        [SerializeField] InputReader input;


        protected override IEnumerator OnDialogOpen()
        {
            inputKey.InputActionReference.action.Enable();
            input.DisableDialog();
            
            yield return inputKey;
            yield return timer;
            
            inputKey.InputActionReference.action.Disable();
            input.EnableDialog();
            Close();
        }

        protected override IEnumerator OnDialogClose()
        {
            StopAllCoroutines();
            yield break;
        }
    }
}