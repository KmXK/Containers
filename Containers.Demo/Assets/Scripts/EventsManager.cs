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

    private bool _isTruckWaiting;
    
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
}
