using TMPro;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private ContainerFocuser _focuser;
    [SerializeField] private TMP_InputField _focusId;

    public void FocusId_OnValueChanged()
    {
        if (_focusId.text.Length > 5)
            _focusId.text = _focusId.text[..5];
    }

    public void FocusId_OnDeselect()
    {
        if (!int.TryParse(_focusId.text, out _))
        {
            _focusId.text = "0";
        }
    }

    public void FocusButton_OnClick()
    {
        _focuser.FocusContainer(_focusId.text);
    }

    public void UnfocusButton_OnClick()
    {
        _focuser.ResetFocus();
    }
}
