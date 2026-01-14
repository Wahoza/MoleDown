using UnityEngine;

public class OnButtonOpenLink : MonoBehaviour
{
    [SerializeField] string url;
    
    public void OpenLink()
    {
        Application.OpenURL(url);
    }
}
