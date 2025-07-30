using UnityEditor.Rendering.LookDev;
using UnityEngine;
[RequireComponent(typeof(PlayerActionHandler), typeof(PlayerStateComponent), typeof(Rigidbody))]
public class PlayerLookComponent : MonoBehaviour
{
    private PlayerStateComponent movementSettings;

    private Rigidbody rb;

    private void Start()
    {
        GetComponent<PlayerInputHandler>().RegisterMoveFuncs(LookInDirection);
        movementSettings = GetComponent<PlayerStateComponent>();
        rb = GetComponent<Rigidbody>();
    }
    void LookInDirection(Vector2 lookDir)
    {
        if (lookDir.x != 0 || lookDir.y != 0)
        {
            Vector3 targetDir = new Vector3(-lookDir.x, 0, -lookDir.y);
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            rb.MoveRotation(targetRot);
        }
    }

    private void OnDestroy()
    {
        GetComponent<PlayerInputHandler>().UnregisterInputMoveFuncs(LookInDirection);
    }
}
