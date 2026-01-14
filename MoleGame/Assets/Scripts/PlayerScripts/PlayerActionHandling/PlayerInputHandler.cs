using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerActionHandler))]
public class PlayerInputHandler : MonoBehaviour
{
    public PlayerConfig playerConfig {get; private set;}
    private PlayerInputMap controls;

    private PlayerStateComponent playerState;
    //input listeners
    private List<Action<Vector2>> inputMoveFuncs = new();
    private List<Action<CallbackContext>> inputGrabFuncs = new();
    private List<Action<CallbackContext>> inputJumpFuncs = new();
    private List<Action<CallbackContext>> inputAttackFuncs = new();


    public async void Initialize(PlayerConfig pc, bool isClone)
    {
        controls = new();

        playerConfig = pc;

        if(playerConfig.Input!= null)
        {
            playerConfig.Input.onActionTriggered += Input_onActionTriggered;
        }

        playerState = GetComponent<PlayerStateComponent>();
        GetComponent<PlayerModelControllerComponent>().SetColor(pc.playerColor);

        if (!isClone)
        {
            GameManager.Instance.JoinPlayer(pc, gameObject);

            await GetComponent<SpawnPlatformCreator>().SpawnPlatformAsync(true);
        }
        else
        {
            GameManager.Instance.RegisterClone(pc, gameObject);
        }
    }
    void Input_onActionTriggered(CallbackContext ctx)
    {
        if(playerState.IsDead()) { return; }

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
        for (int i = inputMoveFuncs.Count - 1; i >= 0; i--)
        {
            if (i >= inputMoveFuncs.Count) { continue; }
            {
                {
                    inputMoveFuncs[i].Invoke(move);
                }
            }
        }
    }

    void OnGrab(CallbackContext ctx)
    {
        for (int i = inputGrabFuncs.Count - 1; i >= 0; i--)
        {
            if (i >= inputGrabFuncs.Count) { continue; }
            {
                inputGrabFuncs[i].Invoke(ctx);
            }
        }
    }
    void OnJump(CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            for (int i = inputJumpFuncs.Count - 1; i >= 0; i--)
            {
                if (i >= inputJumpFuncs.Count) { continue; }
                {
                    inputJumpFuncs[i].Invoke(ctx);
                }
            }
        }
    }
    void OnAttack(CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            for (int i = inputAttackFuncs.Count - 1; i >= 0; i--)
            {
                if (i >= inputAttackFuncs.Count) { continue; }
                {
                    inputAttackFuncs[i].Invoke(ctx);
                }
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

        if (playerConfig.Input != null)
        {
            playerConfig.Input.onActionTriggered -= Input_onActionTriggered;
        }
    }
}
