using System;
using System.Collections;
using System.Collections.Generic;
using Sources;
using UnityEngine;
using Random = UnityEngine.Random;

public class Train : MonoBehaviour
{
    [SerializeField] private GameObject _wagonPrefab;
    [SerializeField] private GameObject _headWagonPrefab;
    
    [SerializeField] private AnimationCurve _movingCurve;
    
    private Transform _wagonsTransform;
    private float _animationTime;
    private ContainerPlatform[] _wagonPlatforms;

    private int _trainWindowSize;
    private int _currentWindowIndex;

    private Vector3 _trainLoadPosition;
    private Vector3 _trainLeavePosition;

    private Dictionary<ContainerPlatform, List<Container>> _platformContainers;

    public event Action<Train> Leaved;

    public bool Generate(int minWagons, int maxWagons)
    {
        var containers = FindObjectsOfType<Container>();
        var largeContainers = new List<Container>();
        var smallContainers = new List<Container>();
        foreach (var container in containers)
        {
            if (container.Data.Type == ContainerType.Small)
                smallContainers.Add(container);
            else
                largeContainers.Add(container);
        }

        var wagonsCount = largeContainers.Count + smallContainers.Count / 2;
        _wagonPlatforms = new ContainerPlatform[wagonsCount];
        _platformContainers = new Dictionary<ContainerPlatform, List<Container>>(wagonsCount);

        if (wagonsCount < minWagons)
            return false;

        wagonsCount = Math.Clamp(wagonsCount, minWagons, maxWagons);
        
        GenerateWagons(wagonsCount);

        GenerateList(largeContainers, smallContainers);

        return true;
    }


    public void MoveToLoading(Transform trainLoadPosition, Transform trainLeavePosition, int trainWindow)
    {
        _trainLoadPosition = trainLoadPosition.position;
        _trainLeavePosition = trainLeavePosition.position;
        
        _trainWindowSize = trainWindow;
        _currentWindowIndex = -1;
        MoveWindow();
    }

    private void Awake()
    {
        _wagonsTransform = transform.GetChild(0);
    }
    
    private void GenerateList(List<Container> largeContainers, List<Container> smallContainers)
    {
        var lists = new List<(int Count, List<Container> List)> {(1, largeContainers), (2, smallContainers)};

        foreach (var platform in _wagonPlatforms)
        {
            _platformContainers.Add(platform, new List<Container>());
            
            var listData = lists[Random.Range(0, lists.Count)];
            var list = listData.List;
            var containersCountToTake = listData.Count;

            for (var j = 0; j < containersCountToTake; j++)
            {
                var container = list[Random.Range(0, list.Count)];
                list.Remove(container);
                _platformContainers[platform].Add(container);
            }

            if (listData.List.Count == 0)
            {
                lists.Remove(listData);
            }
        }
    }

    private bool OnPlatformPlacing(ContainerPlatform platform, Container container)
    {
        if (!_platformContainers.ContainsKey(platform))
            return true;

        if (_platformContainers[platform].Contains(container))
        {
            return true;
        }

        return false;
    }
    
    private void GenerateWagons(int count)
    {
        const float wagonLength = 6f;
        const float wagonOffset = 0.2f;

        var localPosition = new Vector3(wagonLength + wagonOffset, 0, 0);

        Instantiate(_headWagonPrefab, _wagonsTransform).transform.localPosition = localPosition;
        
        for (var i = 0; i < count; i++)
        {
            localPosition.x -= wagonLength + wagonOffset;

            var wagon = Instantiate(_wagonPrefab, _wagonsTransform).transform;
            wagon.localPosition = localPosition;
            var platform = wagon.GetChild(0).GetComponentInChildren<ContainerPlatform>();
            _wagonPlatforms[i] = platform;
            platform.Placing += OnPlatformPlacing;
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

    private void EndMoving()
    {
        Leaved?.Invoke(this);
    }
    
    private void MoveWindow()
    {
        var skipPlatforms = _currentWindowIndex * _trainWindowSize;
        if (_wagonPlatforms.Length < skipPlatforms)
        {
            EndMoving();
            return;
        }
        
        // change window index
        
        // set platforms placeable options
        
        // создать какой-то массив ожидающих платформ
        // создать событие Placed, отлавливать его и
        // ждать ситуации, когда всё будет заполнено

        // высчитывание позиции вагона
        // просто добавлять skipPlatforms * wagonX... к _trainLoadPosition
        //StartCoroutine(MovingCoroutine(_trainLoadPosition));
    }
}
