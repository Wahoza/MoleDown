using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
using DG.Tweening;
using System.Collections;
public class PlayerSetupMenuController : MonoBehaviour
{
    private int PlayerIndex;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private GameObject menuPanel;

    [Header("Animation")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float moveInDuration;

    [Header("MatPreview")]
    [SerializeField] private RawImage MaterialPreview;
    [SerializeField] private List<Color> materials;
    [SerializeField] private GameObject colorTaken;
    private List<int> takenColors;
    private bool currentColorTaken;
    private int colorIndex = 0;

    [Header("ReadySlot")]
    [SerializeField] private Transform Ready;
    [SerializeField] private GameObject ReadyComponent; 
    
    [Header("LeaveSlot")]
    [SerializeField] private Transform Leave;

    private int selectedSlot = 0; //max 2, is to help with switching selection Slots

    private PlayerInputMap controls;

    [SerializeField] private float ignoreInputTime = .01f;
    private bool inputEnabled = false;

    // Update is called once per frame
    private Coroutine animRoutine;
    public void SetPlayerIndex(int pi)
    {
        PlayerIndex = pi;
        titleText.SetText("P" + (pi + 1).ToString());
        controls = new PlayerInputMap();

        Debug.Log("Set PI");
    }
    private void Start()
    {
        transform.localPosition = startPosition;
        transform.DOLocalMove(Vector3.zero, moveInDuration).SetEase(Ease.OutSine);

        colorTaken.SetActive(false);
        currentColorTaken = false;
        ChangeSelection(Vector2Int.zero);

        takenColors = new List<int>();
        PlayerConfigManager.Instance.JoinSetupMenu(this);

        SelectColor(0);

        animRoutine = StartCoroutine(WaitForInputEnable());
    }
    private IEnumerator WaitForInputEnable()
    {
        yield return new WaitForSeconds(ignoreInputTime);

        inputEnabled = true;
        animRoutine = null;
    }
    

    public void SetColor(Color color)
    {
       if (!inputEnabled) { return; }
        PlayerConfigManager.Instance.SetPlayerColor(PlayerIndex, color);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled || currentColorTaken) { return; }

        PlayerConfigManager.Instance.TakeColor(colorIndex);
        PlayerConfigManager.Instance.ReadyPlayer(PlayerIndex);

        ReadyComponent.SetActive(true);

        //transform.GetChild(0).gameObject.SetActive(false);
        inputEnabled = false;
        //enabled = false;
    }    
    public void UnReadyPlayer()
    {
        //if (!inputEnabled) { return; }
        inputEnabled = true;
        PlayerConfigManager.Instance.UnReadyPlayer(PlayerIndex);
        
        PlayerConfigManager.Instance.RemoveColor(colorIndex);
        ReadyComponent.gameObject.SetActive(false);
    }
    public void SelectColor(int inputDirection)
    {
        colorIndex += inputDirection;
        if (colorIndex < 0) { colorIndex = materials.Count - 1; }
        if (colorIndex >= materials.Count) { colorIndex = 0; }
        Debug.Log(colorIndex);

        MaterialPreview.color = materials[colorIndex];
        currentColorTaken = takenColors.Contains(colorIndex);
        colorTaken.SetActive(currentColorTaken);
    }
    public void SetColorsTaken(List<int> newTakenColors)
    {
        takenColors = newTakenColors;

        currentColorTaken = takenColors.Contains(colorIndex);
        colorTaken.SetActive(currentColorTaken);
    }
    
    public void ChangeSelection(Vector2Int inputDirection)
    {
        Debug.Log("ChangeSelection " + inputEnabled);
        if (!inputEnabled) { return; }
        if (Math.Abs(inputDirection.x) > 0)
        {
            switch (selectedSlot)
            {
                case 0:
                    SelectColor(inputDirection.x);
                    return;
                    
                default:
                    break;
            }
        }


        else if (Math.Abs(inputDirection.y) > 0)
        {
            selectedSlot = selectedSlot - inputDirection.y;
            if (selectedSlot < 0) { selectedSlot = 2; }
            if (selectedSlot > 2) { selectedSlot = 0; }

            switch (selectedSlot)
            {
                case 0:
                    MaterialPreview.transform.GetChild(0).gameObject.SetActive(true);
                    Ready.transform.GetChild(0).gameObject.SetActive(false);
                    Leave.transform.GetChild(0).gameObject.SetActive(false);

                    return;
                case 1:
                    MaterialPreview.transform.GetChild(0).gameObject.SetActive(false);
                    Ready.transform.GetChild(0).gameObject.SetActive(true);
                    Leave.transform.GetChild(0).gameObject.SetActive(false);
                    return;
                case 2:
                    MaterialPreview.transform.GetChild(0).gameObject.SetActive(false);
                    Ready.transform.GetChild(0).gameObject.SetActive(false);
                    Leave.transform.GetChild(0).gameObject.SetActive(true);
                    return;

            }
        }

    }
    public void SelectButtonDown()
    {
        if (selectedSlot == 1) {
            SetColor(materials[colorIndex]);
            ReadyPlayer();
        }
        if (selectedSlot == 2)
        {
            if(animRoutine == null)
            {
                animRoutine = StartCoroutine(DisconnectRoutine());
            }
        }
    }
    private IEnumerator DisconnectRoutine()
    {
        transform.DOLocalMove(-startPosition, moveInDuration).SetEase(Ease.InSine);

        yield return new WaitForSeconds(moveInDuration);

        Disconnect();
    }
    public void Disconnect()
    {
        PlayerConfigManager.Instance.LeaveSetupMenu(this, PlayerIndex);

        Destroy(transform.parent.gameObject); //we are using the new setup here
    }
    public void CancelButtonDown()
    {

    }
}