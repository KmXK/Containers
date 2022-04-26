using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sources;
using UnityEngine;
using Random = UnityEngine.Random;

public class Train : MonoBehaviour
{
    [SerializeField] private GameObject _wagonPrefab;
    [SerializeField] private GameObject _headWagonPrefab;
    
    [SerializeField] private AnimationCurve _movingCurve;
    
    private const float WagonLength = 6f;
    private const float WagonOffset = 0.2f;
    
    private Transform _wagonsTransform;
    private float _animationTime;
    private ContainerPlatform[] _wagonPlatforms;
    private List<ContainerPlatform> _platformToWait;

    private int _trainWindowSize;
    private int _currentWindowIndex;
    private bool _isMoving;

    private Vector3 _trainLoadPosition;
    private Vector3 _trainLeavePosition;

    private Dictionary<ContainerPlatform, List<Container>> _platformContainers;
    private Vector3 MovingDirection => (_trainLeavePosition - _trainLoadPosition).normalized;

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
        wagonsCount = Math.Clamp(wagonsCount, minWagons, maxWagons);
        _wagonPlatforms = new ContainerPlatform[wagonsCount];
        _platformContainers = new Dictionary<ContainerPlatform, List<Container>>(wagonsCount);

        if (wagonsCount < minWagons)
            return false;
        
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
        StartCoroutine(MoveWindow());
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
        var localPosition = new Vector3(WagonLength + WagonOffset, 0, 0);

        Instantiate(_headWagonPrefab, _wagonsTransform).transform.localPosition = localPosition;
        
        for (var i = 0; i < count; i++)
        {
            localPosition.x -= WagonLength + WagonOffset;

            var wagon = Instantiate(_wagonPrefab, _wagonsTransform).transform;
            wagon.localPosition = localPosition;
            var platform = wagon.GetChild(0).GetComponentInChildren<ContainerPlatform>();
            _wagonPlatforms[i] = platform;
            platform.Placing += OnPlatformPlacing;
            platform.Placed += OnPlatformPlaced;
            platform.IsPlaceable = false;
        }
    }

    private void OnPlatformPlaced(ContainerPlatform platform, Container container)
    {
        if (_platformToWait.Contains(platform))
        {
            _platformContainers[platform].Remove(container);
            if (!_platformContainers[platform].Any())
            {
                _platformToWait.Remove(platform);

                if (!_platformToWait.Any())
                {
                    StartCoroutine(MoveWindow());
                }
            }
        }
    }

    private IEnumerator MoveTo(Vector3 endPosition)
    {
        _isMoving = true;

        _animationTime = 0;
        var startPosition = transform.position;
        var distance = endPosition - startPosition;
        
        while (Math.Abs(_movingCurve.Evaluate(_animationTime) - 1f) > 0.01f)
        {
            transform.position = startPosition + _movingCurve.Evaluate(_animationTime) * distance;

            yield return null;
            
            _animationTime += Time.deltaTime;
        }

        _isMoving = false;
    }

    private IEnumerator EndMoving()
    {
        var delta = MovingDirection * (_wagonPlatforms.Length * (WagonLength + WagonOffset));
        
        yield return StartCoroutine(MoveTo(_trainLeavePosition + delta));
        Leaved?.Invoke(this);
    }
    
    private IEnumerator MoveWindow()
    {
        while (_isMoving)
        {
            yield return null;
        }
        
        var skipPlatforms = _currentWindowIndex * _trainWindowSize;

        var isPlaceable = false;
        int startIndex = 0, endIndex = 0; 
        for (var t = 0; t < 2; t++)
        {
            startIndex = Math.Max(skipPlatforms, 0);
            endIndex = Math.Min(skipPlatforms + _trainWindowSize, _wagonPlatforms.Length);
            for (var i = startIndex; i < endIndex; i++)
            {
                _wagonPlatforms[i].IsPlaceable = isPlaceable;
            }

            if (t == 0)
            {
                _currentWindowIndex++;
                isPlaceable = true;
                skipPlatforms += _trainWindowSize;
            }
        }
        
        if (_wagonPlatforms.Length <= skipPlatforms)
        {
            yield return EndMoving();
            yield break;
        }

        _platformToWait = new List<ContainerPlatform>(endIndex - startIndex);
        for (var i = 0; i < _platformToWait.Capacity; i++)
        {
            _platformToWait.Add(_wagonPlatforms[startIndex + i]);
        }

        var position = _trainLoadPosition + MovingDirection * (skipPlatforms * (WagonLength + WagonOffset));
        yield return MoveTo(position);
    }
}
