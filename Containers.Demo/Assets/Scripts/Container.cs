using Sources;
using TMPro;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] private ContainerData _data;

    private bool _isSelected;
    private TextMeshPro _text;
    private ContainerState _state;
    private Renderer _renderer;

    private ContainerMaterialGroup MaterialGroup => ColorManager.Instance.MaterialGroups[_state];
    
    public Transform VisualTransform { get; private set; }
    public ContainerData Data => _data;
    public ContainerPlatform Platform { get; set; }

    public void Select()
    {
        _isSelected = true;
        SetMaterialColor(MaterialGroup.SelectedColor);
    }

    public void Deselect()
    {
        _isSelected = false;       
        SetMaterialColor(MaterialGroup.StandardColor);
    }

    public void Focus()
    {
        SetState(ContainerState.Focused);
    }

    public void Unfocus()
    {
        SetState(ContainerState.Unfocused);
    }

    public void ResetFocus()
    {
        SetState(ContainerState.Default);
    }

    private void SetState(ContainerState state)
    {
        _state = state;
        _renderer.material = MaterialGroup.Material;
        SetMaterialColor(MaterialGroup.StandardColor);
    }

    private void SetMaterialColor(Color color)
    {
        _renderer.material.color = color;
    }

    private void Awake()
    {
        VisualTransform = transform.GetChild(0);
        _renderer = VisualTransform.GetComponent<Renderer>();
        _text = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        SetState(ContainerState.Default);
        _text.text = _data.Id.ToString();
    }

    private void OnMouseEnter()
    {
        if(!_isSelected)
        {
            SetMaterialColor(MaterialGroup.EnterColor);
        }
    }

    private void OnMouseExit()
    {
        if(!_isSelected)
        {
            SetMaterialColor(MaterialGroup.StandardColor);
        }
    }

    private void OnMouseUpAsButton()
    {
        ContainerSelector.Instance.ContainerClick(this);
    }
}
