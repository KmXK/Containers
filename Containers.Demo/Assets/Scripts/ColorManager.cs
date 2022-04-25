using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    [Header("Container Colors")]
    [SerializeField] [Range(0, 1)]
    private float _unfocusedContainerAlpha;
    [SerializeField]
    private ContainerMaterialGroup[] _materialGroups;

    public float UnfocusedContainerAlpha => _unfocusedContainerAlpha;
    public IDictionary<ContainerState, ContainerMaterialGroup> MaterialGroups = 
        new Dictionary<ContainerState, ContainerMaterialGroup>();
    
    public static ColorManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            foreach (var materialGroup in _materialGroups)
            {
                MaterialGroups[materialGroup.State] = materialGroup;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
