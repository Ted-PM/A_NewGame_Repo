using System.Collections;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject myFloorSpawner;
    [SerializeField]
    private GameObject myPlayerSpawner;
    [SerializeField]
    private GameObject myEnemySpawner;
    [SerializeField]
    private GameObject myDoorSpawner;

    [SerializeField]
    private WorldRenderManager _worldRenderManager;
    [SerializeField]
    private WorldAudioManager _worldAudioManager;
    //[SerializeField]
    //public GameObject myWorldRenderManager;

    public bool spawnPlayer;
    public bool useRenderCulling;
    public bool useAbmientAudio;
    public bool spawnEnemies;
    public bool spawnDoors;

    private FloorSpawner _floorSpawner;
    private PlayerSpawner _playerSpawner;
    

    private PlayerController _player;
    private MazeFloor[] _mazeFloors;

    private bool _worldReady = false;
    private bool _renderManagerReady = false;
    private bool _playerReady = false;
    private int _playerFloorLevel = -1;

    private void Awake()
    {
        SpawnFloors();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaitThenSpawnOtherShit());
    }

    private void FixedUpdate()
    {
        if (_worldReady && _playerFloorLevel != GetPlayerFloorLevel())
        {
            _playerFloorLevel = GetPlayerFloorLevel();
            if (useRenderCulling)
                _worldRenderManager.UpdateRenderers(_playerFloorLevel);
            if (useAbmientAudio)
                _worldAudioManager.PlayFloorAmbiance(_mazeFloors[_playerFloorLevel].GetFloorAmbientAudioClip());
        }
    }

    private IEnumerator WaitThenSpawnOtherShit()
    {
        yield return new WaitForSeconds(1f);

        if (useRenderCulling)
        {
            StartCoroutine(WaitThenEnableWorldRenderer());
        }
        else
        {
            if (_worldRenderManager != null)
                Destroy(_worldRenderManager);
        }

        if (spawnEnemies)
            StartCoroutine(WaitThenSpawnEnemySpawner());
        if (spawnDoors)
            StartCoroutine(WaitThenSpawnDoorSpawner());
        if (spawnPlayer)
            StartCoroutine(WaitThenSpawnPlayer());

        StartCoroutine(WaitTillWorldReady());
    }

    private IEnumerator WaitTillWorldReady()
    {
        _playerReady = !spawnPlayer;
        _renderManagerReady = !useRenderCulling;

        while (!_playerReady || !_renderManagerReady)
        {
            yield return new WaitForFixedUpdate();
        }

        if (useAbmientAudio)
            _worldAudioManager.SetWorldAudioSource(_player.GetPlayerAudioSource());

        _worldReady = true;
    }

    private int GetPlayerFloorLevel()
    {
        return (int)(((int)_player.transform.position.y) / 10);
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
        _renderManagerReady = true;
            //StartCoroutine(_floorSpawner.DisableInitialFloorRenderers());
    }

    private IEnumerator WaitThenSpawnEnemySpawner()
    {
        yield return new WaitForSeconds(1f);
        Instantiate(myEnemySpawner, this.transform);
    }

    private IEnumerator WaitThenSpawnDoorSpawner()
    {
        yield return new WaitForSeconds(1f);
        Instantiate(myDoorSpawner, this.transform);
    }

    private IEnumerator WaitThenSpawnPlayer()
    {
        yield return new WaitForSeconds(1f);

        Vector2 playerSpawn = _floorSpawner.GetPlayerSpawnPoint();
        SpawnPlayerSpawner((int)playerSpawn.x, (int)playerSpawn.y);
    }

    private void SpawnFloors()
    {
        GameObject floorSpawnerObject = Instantiate(myFloorSpawner, this.transform.parent);
        _floorSpawner = floorSpawnerObject.GetComponent<FloorSpawner>();
    }

    private void SpawnPlayerSpawner(int x, int z)
    {
        GameObject playerSpawnerObject = Instantiate(myPlayerSpawner, this.transform.parent);
        _playerSpawner = playerSpawnerObject.GetComponent<PlayerSpawner>();
        GameObject newPlayer = _playerSpawner.SpawnPlayer(x, z);

        if (!newPlayer.TryGetComponent<PlayerController>(out _player))
        {
            Debug.LogError("Can't Find Player!!");
            return;
        }
        _playerReady = true;
    }
}
