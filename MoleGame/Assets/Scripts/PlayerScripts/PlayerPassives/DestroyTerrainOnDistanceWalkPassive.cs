using UnityEngine;

[CreateAssetMenu(fileName = "DestroyTerrainOnWalk", menuName = "Scriptable Objects/DestroyTerrainOnWalk")]
public class DestroyTerrainOnDistanceWalkPassive : IPlayerPassive
{
    [SerializeField] float distanceRequiredToDestroySqr;
    [SerializeField] float deformRadius;
    [SerializeField] Vector3 offset;
    [SerializeField] float intensity;

    private float distanceWalkedSinceDeform;
    public override void OnStart()
    {
        base.OnStart();

        player.gameObject.GetComponent<PlayerActionHandler>().RegisterMoveAction(OnMove);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.gameObject.GetComponent<PlayerActionHandler>().UnregisterMoveAction(OnMove);
    }

    public void OnMove(Vector3 movement)
    {
        var distance = movement;
        distance.y = 0;
        distanceWalkedSinceDeform += distance.sqrMagnitude * Time.deltaTime;

        if (distanceWalkedSinceDeform > distanceRequiredToDestroySqr)
        {
            distanceWalkedSinceDeform = 0;

            LevelMarchingPlatform.Instance.DeformSphereAt(player.transform.position + offset, deformRadius, intensity);
        }
    }
}
