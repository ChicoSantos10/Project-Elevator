using System;
using System.Collections;
using SSSTools.FunText.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogSystem
{
    public class DialogManager : MonoBehaviour
    {
        [SerializeField] EventChannel<Prompt> openDialogEvent;
        [SerializeField] EventChannel<Prompt> closeDialogEvent;
        [SerializeField] TextMeshProUGUI textField;
        [SerializeField] FunTextTyper typer;
        [SerializeField] GameObject dialogBox;
        [SerializeField] InputReader input;

        Prompt _currentDialog;
        bool _finishedTyping;
        
        void OnEnable()
        {
            openDialogEvent.Action += OnDialog;
            closeDialogEvent.Action += OnDialogClose;
            //input.OnNextDialog += MoveNext;
            
            typer.OnTyperFinished += OnFinishedTyping;
        }

        void OnFinishedTyping()
        {
            _finishedTyping = true;
        }

        void OnDisable()
        {
            openDialogEvent.Action -= OnDialog;
            openDialogEvent.Action -= OnDialogClose;
            //input.OnNextDialog -= MoveNext;
            
            typer.OnTyperFinished -= OnFinishedTyping;
        }

        void OnDialog(Prompt dialog)
        {   
            if (dialog == _currentDialog || dialog.Finished)
                return;
            
            Close();
            dialogBox.SetActive(true);
            _currentDialog = dialog;
            
            input.DisableInteraction();
            
            if (dialog.DisableGameplay)
                input.DisableGameplay();

            StartCoroutine(MoveNext());

            //input.SetDialog();
            //input.EnableDialog();
        }

        void OnDialogClose(Prompt dialog)
        {
            if (dialog != _currentDialog)
                return;
            
            StopAllCoroutines();
            
            Close();
        }

        void Close()
        {
            dialogBox.SetActive(false);
            
            input.EnableInteraction();
            
            if (_currentDialog != null && _currentDialog.EnableGameplay)
                input.EnableGameplay();
            
            // if (_currentDialog != null) 
            //     _currentDialog.Reset();
        }

        IEnumerator MoveNext()
        {
            while (_currentDialog.GetNextText(out string text))
            {
                _finishedTyping = false;
                typer.StartTyping(text);

                if (_currentDialog.UseInput)
                    yield return _currentDialog.WaitGoNext();
                else
                {
                    while (!_finishedTyping)
                    {
                        yield return null;
                    }
                    yield return _currentDialog.Timer;
                }
                yield return null;
            }

            Close();
        }
    }
}