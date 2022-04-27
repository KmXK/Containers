using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sources;
using UnityEngine;
using Random = System.Random;

public class Train : MonoBehaviour
{
    [SerializeField] private int _randomSeed;
    
    [SerializeField] private GameObject _vanPrefab;
    [SerializeField] private GameObject _headVanPrefab;
    
    [SerializeField] private AnimationCurve _movingCurve;
    
    private StateManager _stateManager;
    
    private const float VanLength = 6f;
    private const float VanOffset = 0.2f;
    
    private Transform _vanTransforms;
    private float _animationTime;
    private Van[] _vans;
    private List<ContainerPlatform> _platformToWait;

    private int _trainWindowSize;
    private int _currentWindowIndex;
    private bool _isMoving;

    private Vector3 _trainLoadPosition;
    private Vector3 _trainLeavePosition;

    private List<Container> _currentWindowContainers;
    private List<Container> _nextContainers;

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

        var vansCount = largeContainers.Count + smallContainers.Count / 2;
        
        if (vansCount < minWagons)
            return false;

        vansCount = Math.Clamp(vansCount, minWagons, maxWagons);
        
        _vans = new Van[vansCount];
        _platformContainers = new Dictionary<ContainerPlatform, List<Container>>(vansCount);

        GenerateWagons(vansCount);
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

    public ContainerState GetContainerState(Container container)
    {
        if (_currentWindowContainers.Contains(container))
        {
            return ContainerState.Loading;
        }
        if (_nextContainers.Contains(container))
        {
            return ContainerState.Preloading;
        }

        return ContainerState.Default;
    }

    private void Awake()
    {
        _vanTransforms = transform.GetChild(0);

        _stateManager = FindObjectOfType<StateManager>();
    }
    
    private void GenerateList(List<Container> largeContainers, List<Container> smallContainers)
    {
        var lists = new List<(int Count, List<Container> List, ContainerType Type)>
        {
            (1, largeContainers, ContainerType.Large),
            (2, smallContainers, ContainerType.Small)
        };

        _currentWindowContainers = new List<Container>();
        _nextContainers = new List<Container>();

        var random = new Random(_randomSeed);

        foreach (var van in _vans)
        {
            var platform = van.Platform; 
            
            _platformContainers.Add(platform, new List<Container>());
            
            var listData = lists[random.Next(0, lists.Count)];
            var list = listData.List;
            var containersCountToTake = listData.Count;

            for (var j = 0; j < containersCountToTake; j++)
            {
                var container = list[random.Next(0, list.Count)];
                list.Remove(container);
                _platformContainers[platform].Add(container);
                
                _nextContainers.Add(container);
            }

            if (listData.Type == ContainerType.Large)
            {
                van.SetLargeText(_platformContainers[platform].First().Data.Id.ToString());
            }
            else
            {
                van.SetSmallTexts(_platformContainers[platform].Select(c => c.Data.Id.ToString()).ToArray());
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
        
        if (container.Platform != null && !container.Platform.CanTake(container))
        {
            return false;
        }

        if (_platformContainers[platform].Contains(container))
        {
            if (container.Platform != null)
                container.Platform.TryRemove(container);
            
            var containers = _platformContainers[platform];

            platform.Placing -= OnPlatformPlacing;
            
            var van = _vans.First(v => v.Platform == platform);
            
            if (containers.Count == 1)
            {
                platform.Place(container);
                van.DisableText();
            }
            else
            {
                var column = platform.ContainerPlace.FirstColumn;
                var index = 0;
                if (containers[0] != container)
                {
                    column = platform.ContainerPlace.SecondColumn;
                    index = 1;
                }

                van.DisableSmallText(index);
                platform.Place(container, column);
            }

            platform.Placing += OnPlatformPlacing;

            return false;
        }

        return false;
    }
    
    private void GenerateWagons(int count)
    {
        var localPosition = new Vector3(VanLength + VanOffset, 0, 0);

        Instantiate(_headVanPrefab, _vanTransforms).transform.localPosition = localPosition;
        
        for (var i = 0; i < count; i++)
        {
            localPosition.x -= VanLength + VanOffset;

            var vanTransform = Instantiate(_vanPrefab, _vanTransforms).transform;
            vanTransform.localPosition = localPosition;
            _vans[i] = vanTransform.GetComponent<Van>();
            
            var platform = _vans[i].Platform;
            platform.Placing += OnPlatformPlacing;
            platform.Placed += OnPlatformPlaced;
            platform.IsPlaceable = false;
        }
    }

    private void OnPlatformPlaced(ContainerPlatform platform, Container container)
    {
        if (_platformToWait.Contains(platform))
        {
            container.SetState(ContainerState.Default);
            _currentWindowContainers.Remove(container);
            
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
        var delta = MovingDirection * (_vans.Length * (VanLength + VanOffset));
        
        _stateManager.CalculateStates();
        
        yield return StartCoroutine(MoveTo(_trainLeavePosition + delta));
        Leaved?.Invoke(this);
        Destroy(gameObject);
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
            endIndex = Math.Min(skipPlatforms + _trainWindowSize, _vans.Length);
            for (var i = startIndex; i < endIndex; i++)
            {
                _vans[i].Platform.IsPlaceable = isPlaceable;
            }

            if (t == 0)
            {
                _currentWindowIndex++;
                isPlaceable = true;
                skipPlatforms += _trainWindowSize;
            }
        }
        
        if (_vans.Length <= skipPlatforms)
        {
            yield return EndMoving();
            yield break;
        }

        _currentWindowContainers.Clear();
        
        _platformToWait = new List<ContainerPlatform>(endIndex - startIndex);
        for (var i = 0; i < _platformToWait.Capacity; i++)
        {
            var platform = _vans[startIndex + i].Platform;
            _platformToWait.Add(platform);

            foreach (var container in _platformContainers[platform])
            {
                _currentWindowContainers.Add(container);
                _nextContainers.Remove(container);
            }
        }
        
        _stateManager.CalculateStates();

        var position = _trainLoadPosition + MovingDirection * (skipPlatforms * (VanLength + VanOffset));
        yield return MoveTo(position);
    }
}
