using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    [Header("Container Colors")] 
    [SerializeField]
    private ContainerMaterialGroup _defaultMaterialGroup;
    [SerializeField] 
    private ContainerMaterialGroup _focusedMaterialGroup;
    [SerializeField] 
    private ContainerMaterialGroup _unfocusedMaterialGroup;

    public IDictionary<ContainerState, ContainerMaterialGroup> MaterialGroups = 
        new Dictionary<ContainerState, ContainerMaterialGroup>();
    
    public static ColorManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            MaterialGroups[ContainerState.Default] = _defaultMaterialGroup;
            MaterialGroups[ContainerState.Focused] = _focusedMaterialGroup;
            MaterialGroups[ContainerState.Unfocused] = _unfocusedMaterialGroup;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
