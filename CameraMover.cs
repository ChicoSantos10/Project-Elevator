using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform cameraPosition;
    [SerializeField] Transform controlPoint;
    [SerializeField, Range(0, 5)] float travelDuration;
    [SerializeField, Range(0, 5)] float focusDuration;

    void OnTriggerEnter(Collider other)
    {
        Camera cam = Camera.main;
        Transform camTransform = cam.transform;
        Vector3 startPos = camTransform.position, startRotation = camTransform.rotation.eulerAngles;
        
        //inputReader.DisableGameplay();
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(camTransform.DOMove(cameraPosition.position, travelDuration));
        sequence.Join(camTransform.DORotate(cameraPosition.rotation.eulerAngles, travelDuration));
        sequence.AppendInterval(focusDuration);
        sequence.Append(camTransform.DOMove(startPos, travelDuration));
        sequence.Join(camTransform.DORotate(startRotation, travelDuration));
        //sequence.OnComplete(inputReader.EnableGameplay);
        sequence.OnComplete(() => Destroy(this));
    }
}
