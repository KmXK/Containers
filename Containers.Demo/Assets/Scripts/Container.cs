using System;
using Sources;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] 
    private ContainerData _data;

    private Material _material;

    public ContainerData Data => _data;
    public ContainerPlatform Platform { get; set; }

    private void Start()
    {
        _material = transform.GetComponent<Renderer>().material;
        _material.color = ColorManager.Instance.DefaultContainerColor;
    }

    private void OnMouseEnter()
    {
        _material.color = ColorManager.Instance.EnteredContainerColor;
    }

    private void OnMouseExit()
    {
        _material.color = ColorManager.Instance.DefaultContainerColor;
    }

    private void OnMouseUpAsButton()
    {
        Debug.Log("Container click!");

        ContainerSelector.Instance.SelectContainer(this);
    }
}
