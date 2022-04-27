using Sources;
using UnityEngine;
using Random = System.Random;

public class ContainersGenerator : MonoBehaviour
{
    [SerializeField] private ContainerBuilder _builder;
    [SerializeField] private Transform _platformsContainer;

    [Header("Generation Options")] 
    [SerializeField] private int _containersCount;
    [SerializeField] private int _randomSeed;
    
    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        var platforms = _platformsContainer.GetComponentsInChildren<ContainerPlatform>();

        var random = new Random(_randomSeed);

        for (var i = 0; i < _containersCount; i++)
        {
            var type = random.Next(0, 2) == 0 ? ContainerType.Small : ContainerType.Large;

            var container = _builder.GenerateContainer(type);

            for (var j = 0; j < 4; j++)
            {
                var platform = platforms[random.Next(platforms.Length)];
                if (platform.CanPlace(container))
                {
                    platform.Place(container);
                    break;
                }

                if (j == 3)
                {
                    Destroy(container.gameObject);
                }
            }
        }
    }
}
