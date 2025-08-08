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
    List<Action> attackActions = new();
    List<Action> deathActions = new();
    List<Action> spawnActions = new();
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

    public void OnAttack()
    {
        foreach(var action in attackActions)
        {
            action();
        }
    }

    public void OnDie()
    {
        var behaviors = GetComponents<MonoBehaviour>();

        GameManager.Instance.OnPlayerDie(gameObject);

        GetComponent<Rigidbody>().freezeRotation = false;
        GetComponent<PlayerStateComponent>().SetIsDead(true);

        foreach(var action in deathActions)
        {
            action();
        }

        foreach (var behavior in behaviors)
        {
            if(behavior != this)
            {
                behavior.enabled = false;
            }
        }

        Debug.Log("Completed Die");
    }

    public void OnSpawn()
    {
        var behaviors = GetComponents<MonoBehaviour>();
        foreach (var behavior in behaviors)
        {
            behavior.enabled = true;
        }

        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<PlayerStateComponent>().SetIsDead(false);

        foreach (var action in spawnActions)
        {
            action();
        }
    }
    internal void RegisterAttackAction(Action action)
    {
        attackActions.Add(action);
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
    public void RegisterGrabbableAction(Action<bool> action)
    {
        grabbableActions.Add(action);
    }
    public void RegisterGrabAction(Action<bool> action)
    {
        grabActions.Add(action);
    }

    public void RegisterDeathAction(Action action)
    {
        deathActions.Add(action);   
    }

    public void RegisterSpawnAction(Action action)
    {
        spawnActions.Add(action);
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
    public void UnregisterGrabbableAction(Action<bool> action)
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
    public void UnregisterAttackAction(Action action)
    {
        if (attackActions.Contains(action))
            attackActions.Remove(action); 
    }
    public void UnregisterSpawnAction(Action action)
    {
        if (spawnActions.Contains(action))
        {
            spawnActions.Remove(action);
        }
    }
    public void UnregisterDeathAction(Action action)
    {
        if (deathActions.Contains(action))
        {
            deathActions.Remove(action);
        }
    }
}
