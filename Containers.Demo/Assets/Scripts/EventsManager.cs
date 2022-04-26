using UnityEngine;

public class EventsManager : MonoBehaviour
{
    [SerializeField] private ContainerBuilder _containerBuilder;
    
    [Header("Truck Events")]
    [SerializeField] private Transform _trucksContainer;
    [SerializeField] private GameObject _truckPrefab;
    [SerializeField] private Transform _truckSpawnPosition;
    [SerializeField] private Transform _truckUnloadPosition;
    [SerializeField] private Transform _truckLeavePosition;
    
    [Header("Train Events")]
    [SerializeField] private Transform _trainContainer;
    [SerializeField] private GameObject _trainPrefab;
    [SerializeField] private Transform _trainSpawnPosition;
    [SerializeField] private Transform _trainLoadPosition;
    [SerializeField] private Transform _trainLeavePosition;
    [SerializeField] private int _trainWindowWagonsCount;

    [Header("Train Properties")] 
    [SerializeField] private int _minWagons;
    [SerializeField] private int _maxWagons;


    private bool _isTruckWaiting;
    private bool _isTrainWaiting;
    
    public void StartUnloading()
    {
        if (_isTruckWaiting)
            return;
        
        var truck = Instantiate(_truckPrefab, _truckSpawnPosition.position, Quaternion.identity, _trucksContainer)
            .GetComponent<Truck>();
        var containers = _containerBuilder.GenerateContainersForTruck();
        
        _isTruckWaiting = true;
        truck.MoveToUnloading(containers, _truckUnloadPosition.position, 
            _truckLeavePosition.position);
        truck.Leaved += _ => _isTruckWaiting = false;
    }

    public void StartLoading()
    {
        if (_isTrainWaiting)
            return;

        var train = Instantiate(_trainPrefab, _trainSpawnPosition.position, _trainSpawnPosition.localRotation, _trainContainer)
            .GetComponent<Train>();
        if (!train.Generate(_minWagons, _maxWagons))
        {
            Destroy(train.gameObject);
            return;
        }

        _isTrainWaiting = true;
        
        train.MoveToLoading(_trainLoadPosition, _trainLeavePosition, _trainWindowWagonsCount);
        train.Leaved += _ => _isTrainWaiting = false;
    }
}
