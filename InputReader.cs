using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Input", menuName = "Input", order = 0)]
public class InputReader : ScriptableObject, InputMaster.IGameplayActions, InputMaster.IPaintActions, InputMaster.IGlobalActions, InputMaster.IDialogActions
{
    InputMaster _input;

    void OnEnable()
    {   
        if (_input == null)
        {
            _input = new InputMaster();
            
            _input.Gameplay.SetCallbacks(this);
            _input.Paint.SetCallbacks(this);
            _input.Global.SetCallbacks(this);
            _input.Dialog.SetCallbacks(this);
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SetGameplay();
        EnableGlobal();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void EnableGameplay() => _input.Gameplay.Enable();
    public void DisableGameplay() => _input.Gameplay.Disable();
    
    public void EnablePaint() => _input.Paint.Enable();
    public void DisablePaint() => _input.Paint.Disable();
    
    public void EnableInteraction() => _input.Global.Interact.Enable();
    public void DisableInteraction() => _input.Global.Interact.Disable();
    
    public void EnableDialog() => _input.Dialog.Enable();
    public void DisableDialog() => _input.Dialog.Disable();
    
    void EnableGlobal() => _input.Global.Enable();
    
    public void SetPaint()
    {
        DisableGameplay();
        DisableDialog();
        
        EnablePaint();
    }

    public void SetGameplay()
    {
        DisablePaint();
        DisableDialog();
        
        EnableGameplay();
    }

    public void SetDialog()
    {
        DisableGameplay();
        DisableInteraction();
        
        EnableDialog();
    }

    #region Gameplay
        
    public event UnityAction<Vector2> OnMoveAction = delegate {  };  
    public event UnityAction<Vector2> OnLookAction = delegate {  };  
    public event UnityAction OnJumpAction = delegate {  };  
    public event UnityAction OnCrouchEnableAction = delegate {  };  
    public event UnityAction OnCrouchDisableAction = delegate {  };  
    public event UnityAction OnCameraDefaultAction = delegate {  };  
    public event UnityAction OnCameraNightVisionAction = delegate {  };  
    public event UnityAction OnCameraXRayAction = delegate {  };  
    public event UnityAction OnCameraEnableAction = delegate {  };  
    public event UnityAction OnCameraDisableAction = delegate {  };  
    public event UnityAction OnClickAction = delegate {  };  

    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveAction.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //if (context.phase == InputActionPhase.Performed)
        OnLookAction.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnJumpAction.Invoke();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                OnCrouchEnableAction.Invoke();
                break;
            case InputActionPhase.Canceled:
                OnCrouchDisableAction.Invoke();
                break;
        }
    }

    public void OnCameraDefaultMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnCameraDefaultAction.Invoke();
    }

    public void OnCameraNightVisionMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnCameraNightVisionAction.Invoke();
    }

    public void OnCameraXRayMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnCameraXRayAction.Invoke();
    }

    public void OnUseCamera(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                OnCameraEnableAction.Invoke();
                break;
            case InputActionPhase.Canceled:
                OnCameraDisableAction.Invoke();
                break;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnClickAction.Invoke();
    }

    #endregion

    #region Paint
        
    public event UnityAction OnPaintBrushAction = delegate {  };
    public event UnityAction OnEraserBrushAction = delegate {  };
    public event UnityAction<int> OnBrushSizeAction = delegate {  };
    public event UnityAction<int> OnPageChangeAction = delegate {  };
        
    public void OnPaintBrush(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnPaintBrushAction.Invoke();
    }

    public void OnEraserBrush(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnEraserBrushAction.Invoke();
    }

    public void OnBrushSize(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnBrushSizeAction.Invoke((int) context.ReadValue<float>());
    }

    public void OnChangePage(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnPageChangeAction.Invoke((int) context.ReadValue<float>());
    }

    #endregion

    #region Global
    
    public event UnityAction OnInteractAction = delegate {  };  
    public event UnityAction OnNotepadAction = delegate {  };  
    public event UnityAction OnWeightAction = delegate {  };  

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnInteractAction.Invoke();
    }

    public void OnNotepad(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnNotepadAction.Invoke();
    }

    public void OnConfirmWeight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnWeightAction.Invoke();
    }

    #endregion

    #region Dialog

    public event UnityAction OnNextDialog = delegate {  };  
    public event UnityAction OnCloseDialog = delegate {  };  
    
    public void OnNext(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnNextDialog.Invoke();
    }

    public void OnClose(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    #endregion
    
}