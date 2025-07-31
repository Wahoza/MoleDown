using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerActionHandler : MonoBehaviour
{
    List<Action<Vector3>> moveActions = new();
    List<Action> jumpActions = new();
    List<Action<bool>> grabbableActions = new(); //bool true: grabbed, false: released
    List<Action<bool>> grabActions = new();
    List<Action<float>> landActions = new();

    public void OnWalk(Vector3 walkDirection)
    {
        foreach (var action in moveActions)
        {
            action(walkDirection);
        }
    }
    public void OnJump()
    {
        foreach(var action in jumpActions)
        {
            action();
        }
    }
    public void OnLand(float airTime)
    {
        foreach(var action in landActions)
        {
            action(airTime);
        }
    }
    public void OnGrabbable(bool grabbed)
    {
        foreach(var action in grabbableActions)
        {
            action(grabbed);
        }
    }
    public void OnGrab(bool grab)
    {
        foreach( var action in grabActions)
        {
            action(grab);
        }
    }

    public void RegisterMoveAction(Action<Vector3> action)
    {
        moveActions.Add(action);
    }
    public void RegisterJumpAction(Action action)
    {
        jumpActions.Add(action);
    }
    public void RegisterLandAction(Action<float> action)
    {
        landActions.Add(action);
    }
    public void RegisterGrabbableActions(Action<bool> action)
    {
        grabbableActions.Add(action);
    }
    public void RegisterGrabAction(Action<bool> action)
    {
        grabActions.Add(action);
    }
    public void UnregisterMoveAction(Action<Vector3> action)
    {
        if (moveActions.Contains(action))
        {
            moveActions.Remove(action);
        }
    }
    public void UnregisterJumpAction(Action action)
    {
        if (jumpActions.Contains(action))
        {
            jumpActions.Remove(action);
        }
    }
    public void UnregisterLandAction(Action<float> action)
    {
        if (landActions.Contains(action))
        {
            landActions.Remove(action);
        }
    }
    public void UnregisterGrabbableActions(Action<bool> action)
    {
        if (grabbableActions.Contains(action))
        {
            grabbableActions.Remove(action);
        }
    }

    public void UnregisterGrabAction(Action<bool> action)
    {
        if (grabActions.Contains(action))
        {
            grabActions.Remove(action);
        }
    }
}
