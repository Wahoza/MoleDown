using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerAssign : MonoBehaviour
{
    void Start()
    {
        PlayerConfiguration pc = new PlayerConfiguration();
        pc.playerID = 0;
        pc.Input = GetComponent<PlayerInput>();


        GetComponent<PlayerInputHandler>().Initialize(pc);
    }

}
