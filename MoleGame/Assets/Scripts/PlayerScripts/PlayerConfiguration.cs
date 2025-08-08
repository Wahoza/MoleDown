using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfig
{
    public PlayerConfig(PlayerInput input, int playerIndex)
    {
        Input = input;
        this.playerIndex = playerIndex;
        finalPoints = 0;
    }

    public PlayerInput Input { get; set; }
    public int playerIndex { get; set; }
    public bool isReady { get; set; }
    public bool isPlaying { get; set; }
    public Color playerColor { get; set; }
    public int finalPoints { get; set; }
}