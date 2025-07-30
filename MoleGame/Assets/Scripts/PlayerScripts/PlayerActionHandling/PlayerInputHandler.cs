using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerActionHandler))]
public class PlayerInputHandler : MonoBehaviour
{
    PlayerConfiguration playerConfig;
    private PlayerInputMap controls;

    //input listeners
    private List<Action<Vector2>> inputMoveFuncs = new();
    private List<Action<CallbackContext>> inputGrabFuncs = new();
    private List<Action<CallbackContext>> inputJumpFuncs = new();
    private List<Action<CallbackContext>> inputAttackFuncs = new();


    public void Initialize(PlayerConfiguration pc)
    {
        controls = new();

        playerConfig = pc;

        playerConfig.Input.onActionTriggered += Input_onActionTriggered;
    }
    void Input_onActionTriggered(CallbackContext ctx)
    {
        if (ctx.action.name == controls.Player.Move.name)
        {
            OnMove(ctx);
        }
        else if (ctx.action.name == controls.Player.Jump.name)
        {
            OnJump(ctx);
        }
        else if (ctx.action.name == controls.Player.Grab.name)
        {
            OnGrab(ctx);
        }
        else if (ctx.action.name == controls.Player.Attack.name)
        {
            OnAttack(ctx);
        }
    }

    void OnMove(CallbackContext ctx) 
    {
        Vector2 move = ctx.ReadValue<Vector2>();
        if(move == null ) return;
        foreach (var func in inputMoveFuncs)
        {
            func(move);
        }
    }

    void OnGrab(CallbackContext ctx)
    {
        foreach (var func in inputGrabFuncs)
        {
            func(ctx);
        }
    }
    void OnJump(CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            foreach (var func in inputJumpFuncs)
            {
                func(ctx);
            }
        }
    }
    void OnAttack(CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            foreach (var func in inputAttackFuncs)
            {
                func(ctx);
            }
        }
    }
    public void RegisterMoveFuncs(Action<Vector2> action)
    {
        inputMoveFuncs.Add(action);
    }
    public void RegisterGrabFuncs(Action<CallbackContext> action)
    {
        inputGrabFuncs.Add(action);
    }
    public void RegisterInputJumpFuncs(Action<CallbackContext> action)
    {
        inputJumpFuncs.Add(action);
    }
    public void RegisterInputAttackFuncs(Action<CallbackContext> action)
    {
        inputAttackFuncs.Add(action);
    }

    public void UnregisterInputMoveFuncs(Action<Vector2> action)
    {
        if (inputMoveFuncs.Contains(action))
        {
            inputMoveFuncs.Remove(action);
        }
    }
    public void UnregisterInputGrabFuncs(Action<CallbackContext> action)
    {
        if (inputGrabFuncs.Contains(action))
        {
            inputGrabFuncs.Remove(action);
        }
    }
    public void UnregisterInputJumpFuncs(Action<CallbackContext> action)
    {
        if (inputJumpFuncs.Contains(action))
        {
            inputJumpFuncs.Remove(action);
        }
    }
    public void UnregisterInputAttackFuncs(Action<CallbackContext> action)
    {
        if (inputAttackFuncs.Contains(action))
        {
            inputAttackFuncs.Remove(action);
        }
    }


    private void OnDestroy()
    {
        inputMoveFuncs.Clear();
        inputGrabFuncs.Clear();
        inputJumpFuncs.Clear();

        playerConfig.Input.onActionTriggered -= Input_onActionTriggered;
    }
}
