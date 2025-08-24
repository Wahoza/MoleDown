using System.Collections;
using UnityEngine;
[RequireComponent(typeof(PlayerInputHandler),typeof(PlayerActionHandler))]
public class PlayerAttackComponent : IPlayerComponent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Objects")]
    [SerializeField] float pushForce;
    [SerializeField] float pushDuration;
    [SerializeField] float playerDisableGroundedDuration;
    [Header("Terrain")]
    [SerializeField] float terrainModIntensity;
    [SerializeField] float terrainModRange;
    [SerializeField] Transform pushHelper;
    [SerializeField] LayerMask terrainLayer;
    private bool isGrabbing;

    internal float currentPushForce;
    internal float currentPushDuration;
    internal float currentTerrainModRange;
    internal float currentTerrainModIntensity;

    internal override void Start()
    {
        GetComponent<PlayerInputHandler>().RegisterInputAttackFuncs((ctx) => OnAttack());
        GetComponent<PlayerActionHandler>().RegisterGrabAction(OnGrab);

        pushHelper.GetComponent<PlayerPushTrigger>()?.SetPlayerAttackComponent(this);

        base.Start();
    }
    internal override void ResetStatsOnRespawn()
    {
        currentPushDuration = pushDuration;
        currentPushForce = pushForce;
        currentTerrainModRange = terrainModRange;
        currentTerrainModIntensity = terrainModIntensity;
    }
    void OnAttack()
    {
        if (isGrabbing) { return; }

        GetComponent<PlayerActionHandler>().OnAttack();

        if(Physics.OverlapSphere(pushHelper.position, currentTerrainModRange, terrainLayer).Length > 0)
        {
            LevelMarchingPlatform.Instance.DeformSphereAt(pushHelper.position, currentTerrainModRange, currentTerrainModIntensity);
        }

        StartCoroutine(ActivatePushHelper());
    }
    private IEnumerator ActivatePushHelper()
    {
        pushHelper.gameObject.SetActive(true);

        yield return new WaitForSeconds(currentPushDuration);

        pushHelper.gameObject.SetActive(false);
    }
    void OnGrab(bool grab)
    {
        isGrabbing = grab;

        Debug.Log(isGrabbing + "Is grabbing (from attackComponent)");
    }

    public void PushObject(GameObject other)
    {
        if(other == gameObject) return;

        other.GetComponent<PlayerGroundedComponent>()?.DisableGroundedForDuration(playerDisableGroundedDuration);

        if(other.GetComponent<Rigidbody>() is var rb && rb != null)
        {
            rb.AddForce(pushHelper.forward * currentPushForce, ForceMode.Impulse);
        }
    }
    internal override void OnDestroy()
    {
        base.OnDestroy();

        GetComponent<PlayerInputHandler>().UnregisterInputAttackFuncs((ctx) => OnAttack());
        GetComponent<PlayerActionHandler>().UnregisterGrabAction(OnGrab);
    }
}
