using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    private static EnemySpawner _instance;
    public static EnemySpawner Instance {  get { return _instance; } }

    [SerializeField]
    private EnemyType _enemyTypesOrderInList;

    [SerializeField]
    private List<GameObject> _enemyPrefabs;
    [SerializeField]
    private List<int> _numberOfEnemies;
    private List<List<GameObject>> _enemyPools;

    private GameObject _enemyContainer;
    private List<GameObject> _enemyTypeContainers;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        InitializeLists();
        InitializeContainers();
        FillObjectPools();
    }

    private void InitializeLists()
    {
        _enemyPools = new List<List<GameObject>>();
        _enemyTypeContainers = new List<GameObject> ();
    }

    private void InitializeContainers()
    {
        _enemyContainer = new GameObject("Enemies");
        _enemyContainer.transform.parent = this.transform;

        GameObject temp;
        for (int i = 0; i < _enemyPrefabs.Count; i++)
        {
            temp = new GameObject(_enemyPrefabs[i].name);
            temp.transform.parent = _enemyContainer.transform;
            _enemyTypeContainers.Add(temp);
        }
    }

    private void FillObjectPools()
    {
        if (_enemyPrefabs.Count != _numberOfEnemies.Count)
        {
            Debug.LogError("Number of prefabs != list of enemy couts!!");
            return;
        }

        GameObject temp;

        for (int i = 0; i < _enemyPrefabs.Count; ++i)
        {
            _enemyPools.Add(new List<GameObject>());
            for (int j = 0; j < _numberOfEnemies[i]; j++)
            {
                temp = Instantiate(_enemyPrefabs[i], _enemyTypeContainers[i].transform);
                temp.SetActive(false);
                _enemyPools[i].Add(temp);
            }
        }
    }

    public GameObject GetObjectFromPool(int poolIndex)
    {
        if (poolIndex >= _enemyPools.Count || poolIndex < 0)
        {
            Debug.LogError("That enemy index isn't in the pool!!");
            return null;
        }

        for (int i = 0; i < _enemyPools[poolIndex].Count; ++i)
        {
            if (!_enemyPools[poolIndex][i].activeInHierarchy)
                return _enemyPools[poolIndex][i];
        }

        Debug.Log("Not enough enemies in pool!");
        return null;
    }

    public void DisableEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
    }
}
