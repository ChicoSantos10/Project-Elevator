using System.Collections.Generic;
using Custom_Cursor;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Interaction_System;
using SSSTools.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Notepad
{
    public class Notepad : MonoBehaviour
    {
        [SerializeField, Range(1, 25)] int radius = 2;
        [SerializeField] int resolution = 1920;
        [SerializeField, Range(0.1f, 5)] float openSpeed = 3;
        [SerializeField, Range(0.25f, 1), Tooltip("How much should the brush move forward each stroke")] float increment = 0.25f;

        [Header("Input")] [SerializeField] InputReader input;
        // InputActionReference paintBrush;
        // [SerializeField] InputActionReference eraserBrush;
        // [SerializeField] InputActionReference changeBrushSize;
        // [SerializeField] InputActionReference changePage;

        [Header("Page")] [SerializeField] GameObject page1;
        [SerializeField] GameObject page2;
        [SerializeField, Range(0.1f, 2)] float pageRotationSpeed = .5f;

        [Header("Cursor")/*, SerializeField] Sprite cursor;*/]
        //[SerializeField] Image crosshair;
        [SerializeField] MouseMover mouseMover;
        [SerializeField] CursorManager cursorManager;
        [SerializeField] CursorAnimation brushCursor;
        [SerializeField] CursorAnimation eraserCursor;
        //[SerializeField] CursorAnimation defaultCursor;
        CursorAnimation _currentCursor;

        [SerializeField] GameObject cover;

        Transform _transform;

        List<Texture2D> _textures = new List<Texture2D>(50);
        int _currentPage;

        IBrush _brush = new Brush();
        Color[] px;

        //Sprite crosshairImage;

        Vector2Int _res;
        Vector2 _previousMousePos;

        Color[] _colors;
        (Vector3 position, Quaternion rotation) _reference;

        Vector3 _openPosition; 
        Vector3 _closePosition;
        Quaternion _openRotation;
        Quaternion _closeRotation;

        TweenerCore<Vector3, Vector3, VectorOptions> moveTween;
        TweenerCore<Quaternion, Vector3, QuaternionOptions> rotateTween;

        bool _isOpen;

        Texture2D CurrentTexture => _textures[_currentPage];
        GameObject CurrentPage => _currentPage % 2 == 0 ? page1 : page2;
        (GameObject curr, GameObject other) Pages => _currentPage % 2 == 0 ? (page1, page2) : (page2, page1);
        float Duration => 1 / pageRotationSpeed;
        float OpenDuration => 1 / openSpeed;
         

        public void OnNotepadAction()
        {
            if (_isOpen)
                Close();
            else
                Open();
            
            void Open()
            {
                _isOpen = true;
                FinishAnimations();

                gameObject.SetActive(true);
                moveTween = transform.DOLocalMoveY(_openPosition.y, OpenDuration);
                rotateTween = cover.transform
                    .DOLocalRotate(cover.transform.localRotation.eulerAngles + Vector3.right * 300,  Duration, RotateMode.FastBeyond360);
            }

            void Close()
            {
                _isOpen = false;    
            
                FinishAnimations();

                moveTween = transform.DOLocalMoveY(_closePosition.y, OpenDuration)
                    .OnComplete(() => gameObject.SetActive(false));
                rotateTween = cover.transform
                    .DOLocalRotate(cover.transform.localRotation.eulerAngles + Vector3.right * 300, Duration,  RotateMode.FastBeyond360);
            }
        }

        
        void Awake()
        {
            _transform = transform;

            _openPosition = _transform.localPosition;
            _closePosition = _openPosition - new Vector3(0, 1f, 0);
            
            _transform.localPosition = _closePosition;

            Vector3 scale = _transform.localScale;
            int width = Mathf.RoundToInt(scale.x * resolution),
                height = Mathf.RoundToInt(scale.y * resolution);

            _res = new Vector2Int(width, height);

            _colors = new Color[width * height];
            for (int i = 0; i < _colors.Length; i++)
            {
                _colors[i] = Color.white;
            }

            _reference = (page1.transform.localPosition, page1.transform.localRotation);

            _textures.Add(CreateTexture());
            _currentPage = 0;
            SetTexture();

            _previousMousePos = Mouse.current.position.ReadValue();

            //crosshairImage = crosshair.sprite;
            _currentCursor = brushCursor;
            
            px = CurrentTexture.GetPixels();
            
            //gameObject.SetActive(false);
        }

        void OnEnable()
        {
            // paintBrush.action.performed += SetPaintBrush;
            // eraserBrush.action.performed += SetEraserBrush;
            // changeBrushSize.action.performed += ChangeBrushSize;
            // changePage.action.performed += ChangePage;

            input.SetPaint();

            input.OnPaintBrushAction += SetPaintBrush;
            input.OnEraserBrushAction += SetEraserBrush;
            input.OnBrushSizeAction += ChangeBrushSize;
            input.OnPageChangeAction += ChangePage;

            mouseMover.StartInteraction();
        }

        void OnDisable()
        {
            // paintBrush.action.performed -= SetPaintBrush;
            // eraserBrush.action.performed -= SetEraserBrush;
            // changeBrushSize.action.performed -= ChangeBrushSize;
            // changePage.action.performed -= ChangePage;

            input.SetGameplay();

            input.OnPaintBrushAction -= SetPaintBrush;
            input.OnEraserBrushAction -= SetEraserBrush;
            input.OnBrushSizeAction -= ChangeBrushSize;
            input.OnPageChangeAction -= ChangePage;

            mouseMover.StopInteraction();
            
            FinishAnimations();
        }

        void Update()
        {
            Mouse mouse = Mouse.current;

            Vector2 mousePosition = mouse.position.ReadValue();
            mouseMover.OnMouseMove(mousePosition, Vector2.zero);

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit info, 1f) || info.collider.transform.parent != transform)
            {   
                // TODO: Compute the current position inside the plane and set it to the previous mouse position
                //crosshair.sprite = crosshairImage;
                cursorManager.SetDefaultCursor(); 
                return;
            }

            cursorManager.CursorAnimation = _currentCursor;

            if (!mouse.leftButton.isPressed)
                return;

            Vector2 textureCoord = info.textureCoord * _res;

            if (mouse.leftButton.wasPressedThisFrame)
                _previousMousePos = textureCoord;

            /*Vector2 dist = textureCoord - _previousMousePos;
            int minX = Mathf.RoundToInt(Mathf.Min(_previousMousePos.x, textureCoord.x));
            int minY = Mathf.RoundToInt(Mathf.Min(_previousMousePos.y, textureCoord.y));
            int maxX = Mathf.RoundToInt(Mathf.Max(_previousMousePos.x, textureCoord.x));
            int maxY = Mathf.RoundToInt(Mathf.Max(_previousMousePos.y, textureCoord.y));

            for (int x = Mathf.Clamp(minX - Mathf.CeilToInt(radius), 0, _res.x);
                 x <= Mathf.Clamp(maxX + radius, 0, _res.x);
                 x++)
            {
                for (int y = Mathf.Clamp(minY - Mathf.CeilToInt(radius), 0, _res.y);
                     y <= Mathf.Clamp(maxY + radius, 0, _res.y);
                     y++)
                {
                    float finalDistance = ComputeDistance(dist, x, y);

                    if (finalDistance < radius)
                        _brush.Paint(CurrentTexture, x, y);
                }
            }*/

            Vector2 curr = _previousMousePos;
            // Vector2 dir = (textureCoord - _previousMousePos).normalized * radius * 0.25f;
            // while (curr != textureCoord)
            // {
            //     _brush.Paint(CurrentTexture, Mathf.RoundToInt(curr.x), Mathf.RoundToInt(curr.y), radius);
            //     Vector2 min = Vector2.Min(dir, textureCoord - curr);
            //     curr += min;
            // }

            float t = 0;
            float dist = Vector2.Distance(textureCoord, _previousMousePos);
            float offset = radius * increment;
            float tOff = offset / dist;
            
            while (t <= 1)
            {
                curr = Vector2.Lerp(_previousMousePos, textureCoord, t);
                _brush.Paint(px, CurrentTexture.width, Mathf.RoundToInt(curr.x), Mathf.RoundToInt(curr.y), radius);
                t += tOff;
            }

            _previousMousePos = textureCoord;

            CurrentTexture.SetPixels(px);
            CurrentTexture.Apply();
        }

        Texture2D CreateTexture()
        {
            Texture2D texture = new Texture2D(_res.x, _res.y);
            texture.SetPixels(_colors);
            texture.Apply();
            return texture;
        }

        float ComputeDistance(Vector2 dist, int x, int y)
        {
            float length = dist.sqrMagnitude;

            Vector2 current = new Vector2(x, y);

            if (length == 0)
                return (current - _previousMousePos).magnitude;

            float t = Mathf.Max(0, Mathf.Min(1, Vector2.Dot(current - _previousMousePos, dist) / length));
            Vector2 projection = _previousMousePos + t * dist;

            float finalDistance = Vector2.Distance(current, projection);
            return finalDistance;
        }

        void SetEraserBrush()
        {
            _brush = new Eraser();
            _currentCursor = eraserCursor;
        }

        void SetPaintBrush()
        {
            _brush = new Brush();
            _currentCursor = brushCursor;
        }

        void ChangeBrushSize(int amount)
        {
            radius = Mathf.Max(0, radius + amount);
        }

        void ChangePage(int amount)
        {
            if (amount > 0)
                PageUp();
            else
                PageDown();
            
            px = CurrentTexture.GetPixels();
        }

        void PageUp()
        {
            _currentPage++;

            if (_currentPage >= _textures.Count)
                _textures.Add(CreateTexture());

            SetTexture();

            input.OnPageChangeAction -= ChangePage;
            (GameObject current, GameObject other) = Pages;
            other.transform
                .DOLocalRotate(other.transform.localRotation.eulerAngles + Vector3.left * 355, Duration,
                    RotateMode.FastBeyond360)
                .OnComplete(() => input.OnPageChangeAction += ChangePage);
            
            current.transform.DOLocalMove(_reference.position, 0.1f);
            current.transform.localRotation = _reference.rotation;
        }

        void PageDown()
        {
            if (_currentPage == 0)
                return;

            _currentPage--;

            SetTexture();

            input.OnPageChangeAction -= ChangePage;
            (GameObject current, GameObject other) = Pages;
            current.transform
                .DOLocalRotate(current.transform.localRotation.eulerAngles + Vector3.left * 355, Duration, RotateMode.FastBeyond360)
                .OnComplete(() => input.OnPageChangeAction += ChangePage);

            other.transform.DOMove(current.transform.position, 0.1f);
            other.transform.DORotate(current.transform.rotation.eulerAngles, Duration);
        }

        void SetTexture()
        {
            CurrentPage.GetComponent<MeshRenderer>().material.mainTexture = CurrentTexture;
        }

        void FinishAnimations()
        {
            moveTween?.Kill(true);
            rotateTween?.Kill(true);
        }
    }
}