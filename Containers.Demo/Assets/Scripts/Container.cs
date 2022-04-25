using System;
using Sources;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] 
    private ContainerData _data;

    private Material _material;
    private bool _isSelected;

    public Transform VisualTransform { get; private set; }
    public ContainerData Data => _data;
    public ContainerPlatform Platform { get; set; }

    public void Select()
    {
        _isSelected = true;
        _material.color = ColorManager.Instance.SelectedContainerColor;
    }

    public void Deselect()
    {
        _isSelected = false;
        _material.color = ColorManager.Instance.DefaultContainerColor;
    }

    private void Start()
    {
        VisualTransform = gameObject.transform.GetChild(0);

        _material = VisualTransform.GetComponent<Renderer>().material;
        _material.color = ColorManager.Instance.DefaultContainerColor;
    }

    private void OnMouseEnter()
    {
        if(!_isSelected)
        {
            _material.color = ColorManager.Instance.EnteredContainerColor;
        }
    }

    private void OnMouseExit()
    {
        if(!_isSelected)
        {
            _material.color = ColorManager.Instance.DefaultContainerColor;
        }
    }

    private void OnMouseUpAsButton()
    {
        ContainerSelector.Instance.ContainerClick(this);
    }
}
