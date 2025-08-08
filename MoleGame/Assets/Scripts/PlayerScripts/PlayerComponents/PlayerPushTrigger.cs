using UnityEngine;

public class PlayerPushTrigger : MonoBehaviour
{
    private PlayerAttackComponent attackComponent;

    private void OnTriggerEnter(Collider other)
    {
        if(attackComponent != null)
        {
            attackComponent.PushObject(other.gameObject);
        }
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if (attackComponent != null) {
    //        attackComponent.PushObject(other.gameObject);
    //    }
    //}

    public void SetPlayerAttackComponent(PlayerAttackComponent attackComponent)
    {
        this.attackComponent = attackComponent;
    }
}
