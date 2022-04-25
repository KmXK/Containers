using System;
using Sources;
using TMPro;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] 
    private ContainerData _data;

    private Material _material;
    private bool _isSelected;
    private TextMeshPro _text;

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

    private void Awake()
    {
        VisualTransform = transform.GetChild(0);
        _material = VisualTransform.GetComponent<Renderer>().material;
        _text = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        _material.color = ColorManager.Instance.DefaultContainerColor;
        _text.text = _data.Id.ToString();
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
