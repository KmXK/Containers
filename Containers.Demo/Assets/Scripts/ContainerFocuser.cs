using System.Linq;
using UnityEngine;

public class ContainerFocuser : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;

    public void FocusContainer(string id)
    {
        if (!int.TryParse(id, out var intId))
            return;
        
        var containers = FindObjectsOfType<Container>();

        var container = containers.FirstOrDefault(c => c.Data.Id == intId);
        if (container == null)
            return;

        foreach (var c in containers)
        {
            if (c == container)
            {
                c.Focus();
            }
            else
            {
                c.Unfocus();
            }
        }
    }

    public void ResetFocus()
    {
        foreach (var c in FindObjectsOfType<Container>())
        {
            c.ResetFocus();
        }
    }
}
