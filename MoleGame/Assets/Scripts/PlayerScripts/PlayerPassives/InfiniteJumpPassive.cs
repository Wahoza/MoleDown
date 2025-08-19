using DG.Tweening;
using UnityEngine;
[CreateAssetMenu(fileName = "InfiniteJumpPassive", menuName = "Scriptable Objects/InfiniteJumpPassive")]

public class InfiniteJumpPassive : IPlayerPassive
{
    [SerializeField] GameObject flowervisual;
    [SerializeField] float flowerMoveUpTime;
    [SerializeField] float flowerMoveUpHeight;
    [SerializeField] float effectDuration;

    private GameObject instantiatedFlower;
    private Transform flowerPositionTarget;
    private float elapsedTime;
    
    public override void OnStart() {
        elapsedTime = 0;
        
        instantiatedFlower = Instantiate(flowervisual);
        flowerPositionTarget = player.GetComponent<PlayerGrabComponent>().pocketTransform;

        player.GetComponent<PlayerStateComponent>().SetAlwaysGrounded(true);

        base.OnStart();
    }
    public override void OnUpdate() {
        base.OnUpdate();

        elapsedTime += Time.deltaTime;

        instantiatedFlower.transform.position = flowerPositionTarget.position;

        player.GetComponent<PlayerStateComponent>().SetAlwaysGrounded(true);

        if (elapsedTime > effectDuration)
        {
            OnEnd();
            Destroy(this);
        }
    }
    public override void OnEnd() {

        player.GetComponent<PlayerStateComponent>().SetAlwaysGrounded(false);
        if(instantiatedFlower != null)
        {
            instantiatedFlower.transform.DOScale(Vector3.zero, flowerMoveUpTime).SetEase(Ease.OutQuad);
            instantiatedFlower.transform.DOMoveY(flowerMoveUpHeight, flowerMoveUpHeight).SetEase(Ease.InSine).OnComplete(() => Destroy(instantiatedFlower));
        }
        base.OnEnd();
    }
}
