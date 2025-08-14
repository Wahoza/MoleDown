using UnityEngine;

public class IPlayerComponent : MonoBehaviour
{
    internal virtual void Start()
    {
        GetComponent<PlayerActionHandler>().RegisterSpawnAction(ResetStatsOnRespawn);
        ResetStatsOnRespawn();
    }

    internal virtual void ResetStatsOnRespawn()
    {

    }

    internal virtual void OnDestroy()
    {
        GetComponent<PlayerActionHandler>().UnregisterSpawnAction(ResetStatsOnRespawn);
    }
}
