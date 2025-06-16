using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class ParticleSpawner : MonoBehaviour
{
    private static ParticleSpawner _instance;
    public static ParticleSpawner Instance { get { return _instance; } }

    public ParticleTypes particleTypeOrder;

    [SerializeField]
    private GameObject _particlePrefab;
    public int numParticlesInPool;

    private List<GameObject> _particleObjectPool;
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeLists();
        FillObjectPool();
    }

    private void InitializeLists()
    {
        _particleObjectPool = new List<GameObject>();
    }

    private void FillObjectPool()
    {
        if (numParticlesInPool <= 0)
            return;

        GameObject tempParticle;

        for (int i = 0; i < numParticlesInPool; i++)
        {
            tempParticle = Instantiate(_particlePrefab, this.transform);
            tempParticle.SetActive(false);
            _particleObjectPool.Add(tempParticle);
        }
    }
    
    public GameObject GetParticleFromPool()
    {
        if (_particleObjectPool == null || _particleObjectPool.Count <=0)
        {
            Debug.LogError("Particle object pool not initialized!!");
            return null;
        }

        for (int i = 0; i < _particleObjectPool.Count; i++)
        {
            if ( _particleObjectPool[i] != null && !_particleObjectPool[i].activeInHierarchy)
            {
                return _particleObjectPool[i];
            }
        }

        Debug.Log("Not enough Particles in pool!!");
        return null;
    }

    public void DisableParticle(GameObject particle)
    {
        particle.SetActive(false);
    }

    private void OnDisable()
    {
        for (int i = 0; i < _particleObjectPool.Count; i++)
        {
            Destroy(_particleObjectPool[i]);
        }

        Destroy(this.gameObject);
    }
}
