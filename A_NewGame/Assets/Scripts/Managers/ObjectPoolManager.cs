using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance { get { return _instance; } }

    [SerializeField]
    private bool spawnEnemies = false;
    [SerializeField]
    private GameObject _enemySpawnerObject;
    private EnemySpawner _enemySpawner;

    [SerializeField]
    private bool spawnDoors = false;
    [SerializeField]
    private GameObject _doorSpawnerObject;
    private DoorSpawner _doorSpawner;

    [SerializeField]
    private bool spawnLights = false;
    [SerializeField]
    private GameObject _lightSpawnerObject;
    private LightSpawner _lightSpawner;


    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        InstantiateSpawners();
    }

    private void InstantiateSpawners()
    {
        if (spawnEnemies && _enemySpawnerObject != null)
        {
            _enemySpawner = Instantiate(_enemySpawnerObject, this.transform).GetComponent<EnemySpawner>();
        }
        else
            spawnEnemies = false;

        if (spawnDoors && _doorSpawnerObject != null)
        {
            _doorSpawner = Instantiate(_doorSpawnerObject, this.transform).GetComponent<DoorSpawner>();
        }
        else
            spawnDoors = false;

        if (spawnLights && _lightSpawnerObject != null)
        {
            _lightSpawner = Instantiate(_lightSpawnerObject, this.transform).GetComponent<LightSpawner>();
        }
        else
            spawnLights = false;
    }


    public GameObject GetEnemyFromPool(int poolIndex)
    {
        if (!spawnEnemies) 
            return null;

        return _enemySpawner.GetObjectFromPool(poolIndex);
    }
    public void DisableEnemy(GameObject enemy)
    {
        if (enemy != null)
            enemy.SetActive(false);
    }

    public GameObject GetDoorFromPool(int poolIndex)
    {
        if (!spawnDoors)
            return null;

        return _doorSpawner.GetDoorFromPool(poolIndex);
    }

    public GameObject GetLightFromPool()
    {
        if (!spawnLights)
            return null;

        return _lightSpawner.GetLightFromPool();
    }

    public void DisableLight(GameObject light)
    {
        if (light != null)
            light.SetActive(false);
    }
}
