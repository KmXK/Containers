using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Containers.Core;
using Random = UnityEngine.Random;

public class Container : MonoBehaviour
{
    private static int NextId = 1;
    
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _enterColor;
    [SerializeField] private Color _selectedColor;

    private Material _material;
    private Color _currentStateColor;
    private bool _isSelected;

    public ContainerData Data { get; } = new() {Id = NextId++, Type = ContainerType.Large};

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _material.color = _defaultColor;

        _currentStateColor = _defaultColor;
    }

    private void OnMouseUpAsButton()
    {
        DragController.ContainerClick(this);
    }

    private void OnMouseEnter()
    {
        SetColor(_enterColor);
    }

    private void OnMouseExit()
    {
        SetColor(_defaultColor);
    }

    public void Select()
    {
        _isSelected = true;
        _material.color = _selectedColor;
    }

    public void Deselect()
    {
        _isSelected = false;
        _material.color = _currentStateColor;
    }

    private void SetColor(Color color)
    {
        if (_isSelected)
            return;
        
        _currentStateColor = color;
        _material.color = color;
    }
}
