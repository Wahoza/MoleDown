using UnityEngine;
using System.Collections.Generic;
public class MineObstacle : IGroundedObstacle
{
    [SerializeField] List<IActionComponent> objectActions = new List<IActionComponent>();
    [SerializeField] LayerMask grabCollisionEffectLayermask;
    [SerializeField] string explosionSoundName;
    public void TriggerExplosion()
    {
        if (!isOnGrabbedTerrain)
        {
            Explode();
        }
    }
    private void Explode() {
        Debug.Log("Exploded");
        AudioManager.Instance.Play3DAudioAtPosition(explosionSoundName, transform.position, true);
        foreach (var action in objectActions)
        {
            action.DoAction();
        }
    }

    public override void SetNewTerrain(ObjectActionHandler objectActionHandler)
    {
        base.SetNewTerrain(objectActionHandler);

        if (objectActionHandler.gameObject.GetComponent<DoActionsOnColllision>() == null)
        {
            objectActionHandler.gameObject.AddComponent<DoActionsOnColllision>();
        }

        var colComponent = objectActionHandler.GetComponent<DoActionsOnColllision>();

        foreach (var action in objectActions)
        {
            colComponent.actionLayerMasks.Add(action, grabCollisionEffectLayermask);
        }
    }

    internal override void OnGroundInvalidate()
    {
        Explode();
        base.OnGroundInvalidate();
    }
}
