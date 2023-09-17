using UnityEngine;
using UnityEngine.Events;

public abstract class EventChannel<T> : ScriptableObject
{
    public event UnityAction<T> Action;

    public virtual void Invoke(T arg)
    {
        Action?.Invoke(arg);
    }
}

[CreateAssetMenu(menuName = MenuName + "/Event Channel", fileName = "New Event Channel", order = 0)]
public class EventChannel : ScriptableObject
{
    public const string MenuName = "Create Event Channel";
    
    public event UnityAction Action;

    public void Invoke()
    {
        Action?.Invoke();
    }
}