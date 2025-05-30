using System.Collections;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject myFloorSpawner;
    [SerializeField]
    private GameObject myPlayerSpawner;

    public bool spawnPlayer;

    private FloorSpawner _floorSpawner;
    private PlayerSpawner _playerSpawner;



    private void Awake()
    {
        SpawnFloors();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spawnPlayer)
            StartCoroutine(WaitThenSpawnPlayer());
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
