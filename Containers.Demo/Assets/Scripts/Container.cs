using Sources;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Container : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private ContainerData _data;

    private bool _isSelected;
    private TextMeshPro[] _text;
    private ContainerState _state;
    private Material _material;
    private float _alpha = 1f;

    private ContainerMaterialGroup MaterialGroup => ColorManager.Instance.MaterialGroups[_state];
    
    public int WindowIndex { get; set; }
    public ContainerState State => _state;
    
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
        _alpha = 1f;
        SetMaterialColor(MaterialGroup.StandardColor);
    }

    public void Unfocus()
    {
        _alpha = ColorManager.Instance.UnfocusedContainerAlpha;
        SetMaterialColor(MaterialGroup.StandardColor);
    }

    public void ResetFocus()
    {
        _alpha = 1f;
        SetMaterialColor(MaterialGroup.StandardColor);
    }

    public void SetState(ContainerState state)
    {
        _state = state;
        SetMaterialColor(MaterialGroup.StandardColor);
    }

    private void SetMaterialColor(Color color)
    {
        color.a = _alpha;
        _material.color = color;
    }

    private void Awake()
    {
        _data.Container = this;
        
        VisualTransform = transform.GetChild(0);
        _material = VisualTransform.GetComponent<Renderer>().material;
        _text = GetComponentsInChildren<TextMeshPro>();
    }

    private void Start()
    {
        SetState(ContainerState.Default);
        foreach (var t in _text)
        {
            t.text = _data.Id.ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!_isSelected)
        {
            SetMaterialColor(MaterialGroup.EnterColor);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!_isSelected)
        {
            SetMaterialColor(MaterialGroup.StandardColor);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ContainerSelector.Instance.ContainerClick(this);
    }
}
