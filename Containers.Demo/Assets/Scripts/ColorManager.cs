using UnityEngine;

public class ColorManager : MonoBehaviour
{
    [Header("Container Colors")]
    public Color DefaultContainerColor;
    public Color EnteredContainerColor;
    public Color SelectedContainerColor;
    
    public static ColorManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
