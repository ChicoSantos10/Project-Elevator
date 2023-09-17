using System;
using TMPro;
using UnityEngine;

namespace SSSTools.FunText.Test
{
    public class Tester : MonoBehaviour
    {
        TextMeshProUGUI _textmesh;
        
        void Awake()
        {
            _textmesh = GetComponent<TextMeshProUGUI>();
        }

        [ContextMenu("Change text")]
        void ChangeText()
        {
            _textmesh.text = "<rainbow>Changed<//> text";
        }
    }
}
