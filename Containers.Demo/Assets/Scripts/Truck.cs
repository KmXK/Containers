using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    [SerializeField] private AnimationCurve _movingCurve;

    private float _animationTime;
    private ContainerPlatform _platform;

    public event Action<Truck> Leaved;

    public void MoveToUnloading(IEnumerable<Container> containers, Vector3 endPosition, Vector3 afterPosition)
    {
        LoadContainers(containers);
        _animationTime = 0;
        StartCoroutine(MovingCoroutine(endPosition));

        _platform.Emptied += _ => StartCoroutine(Unload(afterPosition));
    }

    private IEnumerator Unload(Vector3 afterPosition)
    {
        _animationTime = 0;
        yield return StartCoroutine(MovingCoroutine(afterPosition));
        Leaved?.Invoke(this);
        Destroy(gameObject);
    }
    
    private void Awake()
    {
        _platform = transform.GetChild(0).GetComponentInChildren<ContainerPlatform>(false);
    }

    private void LoadContainers(IEnumerable<Container> containers)
    {
        foreach (var container in containers)
        {
            _platform.Place(container);
        }
    }

    private IEnumerator MovingCoroutine(Vector3 endPosition)
    {
        var startPosition = transform.position;
        var distance = endPosition - startPosition;
        
        while (Math.Abs(_movingCurve.Evaluate(_animationTime) - 1f) > 0.01f)
        {
            transform.position = startPosition + _movingCurve.Evaluate(_animationTime) * distance;

            yield return null;
            
            _animationTime += Time.deltaTime;
        }
    }
}
