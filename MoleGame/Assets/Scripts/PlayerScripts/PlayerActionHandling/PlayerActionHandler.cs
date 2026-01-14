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
    List<Action<Transform>> pushActions = new();

    List<Action> attackActions = new();
    List<Action> deathActions = new();
    List<Action> spawnActions = new();
    public void OnWalk(Vector3 walkDirection)
    {
        for (int i = moveActions.Count - 1; i >= 0; i--)
        {
            if (i >= moveActions.Count) { continue; }
            moveActions[i].Invoke(walkDirection);
        }
    }
    public void OnJump()
    {
        for (int i = jumpActions.Count - 1; i >= 0; i--)
        {
            if (i >= jumpActions.Count) { continue; }
            jumpActions[i].Invoke();
        }
    }
    public void OnLand(float airTime)
    {
        for (int i = landActions.Count - 1; i >= 0; i--)
        {
            if (i >= landActions.Count) { continue; }
            landActions[i].Invoke(airTime);
        }
    }
    public void OnGrabbable(bool grabbed)
    {
        for(int i = grabbableActions.Count - 1; i >= 0; i--)
        {
            if(i >= grabbableActions.Count) { continue; }
            grabbableActions[i].Invoke(grabbed);
        }
    }
    public void OnGrab(bool grab)
    {
        for (int i = grabActions.Count - 1; i >= 0; i--)
        {
            if (i >= grabActions.Count) { continue; }
            grabActions[i].Invoke(grab);
        }
    }

    public void OnAttack()
    {
        for (int i = attackActions.Count - 1; i >= 0; i--)
        {
            if (i >= attackActions.Count) { continue; }
            attackActions[i].Invoke();
        }
    }
    public void OnPush(Transform target)
    {
        for (int i = pushActions.Count - 1; i >= 0; i--)
        {
            if (i >= pushActions.Count) { continue; }
            pushActions[i].Invoke(target);
        }
    }

    
    public void OnDie()
    {
        var behaviors = GetComponents<MonoBehaviour>();

        GameManager.Instance.OnPlayerDie(gameObject);

        GetComponent<PlayerStateComponent>().SetIsDead(true);
        GetComponent<PlayerStateComponent>().EnableRagdoll();

        for (int i = deathActions.Count - 1; i >= 0; i--)
        {
            if (i >= deathActions.Count) { continue; }
            deathActions[i].Invoke();
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

        for (int i = spawnActions.Count - 1; i >= 0; i--)
        {
            if (i >= spawnActions.Count) { continue; }
            spawnActions[i].Invoke();
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

    public void RegisterPushAction(Action<Transform> action)
    {
        pushActions.Add(action);
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

    public void UnregisterPushAction(Action<Transform> action)
    {
        if (pushActions.Contains(action))
        {
            pushActions.Remove(action);
        }
    }
}
