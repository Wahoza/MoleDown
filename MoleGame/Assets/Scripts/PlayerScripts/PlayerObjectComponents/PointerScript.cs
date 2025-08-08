using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointerScript : MonoBehaviour
{
    private Transform player;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] RawImage skullObject;
    [SerializeField] RawImage arrowImage;
    float speed = 0.1f;
    
    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(player.position);
            if ((transform.position - playerScreenPosition).sqrMagnitude > 1)
            {
                transform.position = Vector3.Slerp(playerScreenPosition, transform.position, Time.deltaTime * speed);
            }
        }
    }

    public void InitializePlayer(PlayerConfig pc, Transform playerTransform)
    {
        Debug.Log("INIT");
        if (playerName != null)
        {
            playerName.text = "P" + (pc.playerIndex + 1).ToString();
            playerName.color = pc.playerColor;
            skullObject.color = pc.playerColor;
            arrowImage.color = pc.playerColor;
        }

        player = playerTransform;
        skullObject.gameObject.SetActive(false);
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void SetColor(Color color)
    {
        playerName.color = color;
    }

    public void SetName(string name)
    {
        playerName.text = name;
    }

    public void ShowDead()
    {
        skullObject.gameObject.SetActive(true);
        playerName.gameObject.SetActive(false);
    }

    public void ShowAlive()
    {
        skullObject.gameObject.SetActive(false);
        playerName.gameObject.SetActive(true);
    }
}
