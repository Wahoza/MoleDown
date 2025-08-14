using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerAssign : MonoBehaviour
{
    [SerializeField] int playerId;
    [SerializeField] Color modelColor;
    void Start()
    {
        PlayerConfig pc = new PlayerConfig(GetComponent<PlayerInput>(), playerId);

        //pc.playerID = playerId;
        pc.playerColor = modelColor;

        //if (pc.playerID == 0)
        //{
        //    pc.Input = GetComponent<PlayerInput>();
        //}

        GetComponent<PlayerInputHandler>().Initialize(pc, false);
    }

}
