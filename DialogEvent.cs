using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = EventChannel.MenuName + "/Dialog Event", fileName = "Dialog Event", order = 0)]
public class DialogEvent : EventChannel<DialogSystem.Prompt>
{
}