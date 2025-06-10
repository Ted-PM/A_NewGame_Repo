using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class LightSpawner : MonoBehaviour
{
    private static LightSpawner _instance;
    public static LightSpawner Instance {  get { return _instance; } }

    public LightTypes lightTypeOrder;
    [SerializeField]
    private GameObject _lightPrefab;

    [SerializeField]
    private int numLightsInPool;

    //private List<GameObject> _ligthTypeContainers;

    private List<GameObject> _lightObjectPool;



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
        //InitializeContainers();
        FillObjectPool();
    }

    private void InitializeLists()
    {
        _lightObjectPool = new List<GameObject>();
    }

    //private void InitializeContainers()
    //{
    //    GameObject temp;
    //    for (int i = 0; i < _lightPrefabList.Count; i++)
    //    {
    //        temp = new GameObject(_lightPrefabList[i].name);
    //        temp.transform.parent = this.transform;
    //        _ligthTypeContainers.Add(temp);
    //    }
    //}

    private void FillObjectPool()
    {
        //if (numLightsInPool.Count < _lightPrefabList.Count)
        //{
        //    Debug.LogError("Number of prefabs != list of light counts!!");
        //    return;
        //}

        GameObject tempObject;
        for (int i = 0; i < numLightsInPool; i++)
        {
            tempObject = Instantiate(_lightPrefab, this.transform);
            tempObject.SetActive(false);
            _lightObjectPool.Add(tempObject);
        }

    }

    public GameObject GetLightFromPool()
    {
        //if (poolIndex < 0 || poolIndex >= _lightPrefabList.Count)
        //{
        //    Debug.LogError("Light prefab index out of scope!!");
        //    return null;
        //}

        for (int i = 0; i < _lightObjectPool.Count; i++)
        {
            if (!_lightObjectPool[i].activeInHierarchy)
                return _lightObjectPool[i];
        }

        Debug.Log("Not enough Lights in pool!!");
        return null;
    }

    private void OnDisable()
    {
        for (int i = 0; i < _lightObjectPool.Count; i++)
        {
            Destroy(_lightObjectPool[i]);
        }

        Destroy(this.gameObject);
    }
}

public enum LightTypes
{
    wallLamp,
    ceelingLamp,
    candle,
    wallLight
}

