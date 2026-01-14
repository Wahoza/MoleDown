using UnityEngine;

public class ExplodeChickenInRangeAction : IActionComponent
{
    public float radius;
    [SerializeField] private LayerMask chickenLayer;

    private bool selfChicken;
    private bool selfChickenExploded;
    private void Start()
    {
        selfChicken = false;
        selfChickenExploded = false;

        if(GetComponent<ChickenScript>() != null)
        {
            selfChicken = true;
        }
    }
    public override void DoAction()
    {
        if(selfChickenExploded && selfChicken)
            return;

        selfChickenExploded = true;
        var potChickens = Physics.OverlapSphere(transform.position, radius, chickenLayer);

        foreach(Collider c in potChickens)
        {
            if(c.GetComponent<ChickenScript>() != null)
            {
                c.GetComponent<DoActionOnCallComponent>().DoActions();
            }
        }
    }
}
