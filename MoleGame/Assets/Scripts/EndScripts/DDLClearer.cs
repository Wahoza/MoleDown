using UnityEngine;

public class DDLClearer : MonoBehaviour
{
    void Start()
    {
        if (GameSettingsManager.Instance != null)
        {
            Destroy(GameSettingsManager.Instance.gameObject);
        }

        if (PlayerConfigManager.Instance != null)
        {
            Destroy(PlayerConfigManager.Instance.gameObject);
        }
    }
}
