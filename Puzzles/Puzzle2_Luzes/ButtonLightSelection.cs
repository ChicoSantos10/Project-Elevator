using UnityEngine;
using UnityEngine.Serialization;

namespace Puzzles
{
    class ButtonLightSelection : MonoBehaviour
    {
        [SerializeField] private Collider panelButton;


        public Collider PanelButton => panelButton;
    }
}