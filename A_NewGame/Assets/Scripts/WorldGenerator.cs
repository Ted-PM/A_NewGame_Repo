using System.Collections;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject myFloorSpawner;
    [SerializeField]
    private GameObject myPlayerSpawner;
    [SerializeField]
    private WorldRenderManager _worldRenderManager;
    //[SerializeField]
    //public GameObject myWorldRenderManager;

    public bool spawnPlayer;
    public bool useRenderCulling;

    private FloorSpawner _floorSpawner;
    private PlayerSpawner _playerSpawner;
    

    private MazeFloor[] _mazeFloors;

    private void Awake()
    {
        SpawnFloors();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (useRenderCulling)
        {
            StartCoroutine(WaitThenEnableWorldRenderer());
        }
        else
        {
            if (_worldRenderManager != null)
                Destroy(_worldRenderManager);
        }
        if (spawnPlayer)
            StartCoroutine(WaitThenSpawnPlayer());
    }

    private IEnumerator WaitThenEnableWorldRenderer()
    {
        while (!_floorSpawner.mazeGenerated)
        {
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForFixedUpdate();


        _mazeFloors = new MazeFloor[_floorSpawner.numberOfFloors];
        _mazeFloors = _floorSpawner.GetMazeFloors();
        if (_worldRenderManager != null)
            _worldRenderManager.SetMazeFloors(_mazeFloors);
            //StartCoroutine(_floorSpawner.DisableInitialFloorRenderers());
    }

    private IEnumerator WaitThenSpawnPlayer()
    {
        yield return new WaitForSeconds(1f);

        Vector2 playerSpawn = _floorSpawner.GetPlayerSpawnPoint();
        SpawnPlayer((int)playerSpawn.x, (int)playerSpawn.y);
    }

    private void SpawnFloors()
    {
        GameObject floorSpawnerObject = Instantiate(myFloorSpawner, this.transform.parent);
        _floorSpawner = floorSpawnerObject.GetComponent<FloorSpawner>();
    }

    private void SpawnPlayer(int x, int z)
    {
        GameObject playerSpawnerObject = Instantiate(myPlayerSpawner, this.transform.parent);
        _playerSpawner = playerSpawnerObject.GetComponent<PlayerSpawner>();
        _playerSpawner.SpawnPlayer(x, z);
    }
}
