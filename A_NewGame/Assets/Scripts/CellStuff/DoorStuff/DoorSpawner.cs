using UnityEngine;
using System.Collections.Generic;
public class DoorSpawner : MonoBehaviour
{
    private static DoorSpawner _instance;
    public static DoorSpawner Instance { get { return _instance; } }

    [SerializeField]
    private DoorWayTypes _doorWayTypesOrderInList;

    [SerializeField]
    private List<GameObject> _doorPrefabList;
    [SerializeField]
    private List<GameObject> _doorWayPrefabList;

    private List<List<GameObject>> _doorObjectPool;
    private List<List<GameObject>> _doorWayObjectPool;

    private GameObject _doorContainer;
    private GameObject _doorWayContainer;

    private List<GameObject> _doorTypeContainers;
    private List<GameObject> _doorWayTypeContainers;

    [SerializeField]
    private List<int> _numberOfDoorsInPool;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }
    void Start()
    {
        InitializeLists();
        InitializeContainers();
        FillObjectPools();
    }

    private void InitializeLists()
    {
        _doorObjectPool = new List<List<GameObject>> ();
        _doorTypeContainers = new List<GameObject> ();

        _doorWayObjectPool = new List<List<GameObject>>();
        _doorWayTypeContainers = new List<GameObject> ();
    }

    private void InitializeContainers()
    {
        _doorContainer = new GameObject("Door Container");
        _doorContainer.transform.parent = this.transform;

        GameObject temp;
        for (int i = 0; i < _doorPrefabList.Count; i++)
        {
            temp = new GameObject(_doorPrefabList[i].name);
            temp.transform.parent = _doorContainer.transform;
            _doorTypeContainers.Add (temp);
        }

        _doorWayContainer = new GameObject("DoorWay Container");
        _doorWayContainer.transform.parent = this.transform;

        for (int i = 0; i < _doorWayPrefabList.Count; i++)
        {
            temp = new GameObject(_doorWayPrefabList[i].name);
            temp.transform.parent = _doorWayContainer.transform;
            _doorWayTypeContainers.Add(temp);
        }
    }

    private void FillObjectPools()
    {
        GameObject temp;

        if (_numberOfDoorsInPool.Count != _doorWayPrefabList.Count)
        {
            Debug.LogError("Number of prefabs != list of door counts!!");
            return;
        }

        for (int i =0; i <  _doorPrefabList.Count; i++)
        {
            _doorObjectPool.Add(new List<GameObject>());

            for (int j =0; j < _numberOfDoorsInPool[i]; j++)
            {
                temp = Instantiate(_doorPrefabList[i], _doorTypeContainers[i].transform);
                temp.SetActive(false);
                _doorObjectPool[i].Add(temp);
            }
        }

        for (int i = 0; i < _doorWayPrefabList.Count; i++)
        {
            _doorWayObjectPool.Add(new List<GameObject>());

            for (int j = 0; j < _numberOfDoorsInPool[i]; j++)
            {
                temp = Instantiate(_doorWayPrefabList[i], _doorWayTypeContainers[i].transform);
                temp.SetActive(false);
                _doorWayObjectPool[i].Add(temp);
            }
        }
    }

    public GameObject GetDoorFromPool(int prefabIndex)
    {
        if (prefabIndex < 0 || prefabIndex >= _doorPrefabList.Count)
        {
            Debug.LogError("Requested Door index is outside the prefab list!!");
            return null;
        }

        for (int i = 0; i < _doorObjectPool[prefabIndex].Count; i++)
        {
            if (!_doorObjectPool[prefabIndex][i].activeInHierarchy)
                return _doorObjectPool[prefabIndex][i];
        }

        Debug.Log("Not enough doors in pool!!");
        return null;
    }

    public GameObject GetDoorWayFromPool(int prefabIndex)
    {
        if (prefabIndex < 0 || prefabIndex >= _doorWayPrefabList.Count)
        {
            Debug.LogError("Requested DoorWay index is outside the prefab list!!");
            return null;
        }

        for (int i = 0; i < _doorWayObjectPool[prefabIndex].Count; i++)
        {
            if (!_doorWayObjectPool[prefabIndex][i].activeInHierarchy)
            {
                //Debug.Log("Doorway removed from pool at index: " + i);
                return _doorWayObjectPool[prefabIndex][i];
            }
        }

        Debug.Log("Not enough doors in pool!!");
        return null;
    }

    public void DisableDoor(GameObject door)
    {
        door.SetActive(false);
    }
}

public enum DoorWayTypes
{
    Rect,
    Arch
}
