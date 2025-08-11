using System.Collections;
using UnityEngine;
[RequireComponent(typeof(PlayerInputHandler),typeof(PlayerActionHandler))]
public class PlayerAttackComponent : MonoBehaviour
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
    
    void Start()
    {
        GetComponent<PlayerInputHandler>().RegisterInputAttackFuncs((ctx) => OnAttack());
        GetComponent<PlayerActionHandler>().RegisterGrabAction(OnGrab);

        pushHelper.GetComponent<PlayerPushTrigger>()?.SetPlayerAttackComponent(this);
    }

    void OnAttack()
    {
        if (isGrabbing) { return; }

        GetComponent<PlayerActionHandler>().OnAttack();

        LevelMarchingPlatform.Instance.DeformSphereAt(pushHelper.position, terrainModRange, terrainModIntensity);

        StartCoroutine(ActivatePushHelper());
    }
    private IEnumerator ActivatePushHelper()
    {
        pushHelper.gameObject.SetActive(true);

        yield return new WaitForSeconds(pushDuration);

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
            rb.AddForce(pushHelper.forward * pushForce, ForceMode.Impulse);
        }
    }
    void OnDestroy()
    {
        GetComponent<PlayerInputHandler>().UnregisterInputAttackFuncs((ctx) => OnAttack());
        GetComponent<PlayerActionHandler>().UnregisterGrabAction(OnGrab);
    }
}
