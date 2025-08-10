using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActionHandler : MonoBehaviour
{
    List<Action<bool>> GrabActions = new();
    private void Start()
    {
        ObjectManager.Instance.RegisterObject(gameObject);
    }
    public void RegisterGrabAction(Action<bool> action)
    {
        GrabActions.Add(action);
    }
    public void OnGrab(bool grabbed)
    {
        foreach (var action in GrabActions)
        {
            action(grabbed);
        }
    }

    public void UnregisterGrabAction(Action<bool> action)
    {
        if (GrabActions.Contains(action))
        {
            GrabActions.Remove(action);
        }
    }
}
