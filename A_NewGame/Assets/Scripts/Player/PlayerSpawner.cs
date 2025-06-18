using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    //[SerializeField]
    //private GameObject _playerControllerPrefab;
    [SerializeField]
    private GameObject _playerPrefab;

    //private PlayerController _playerController;
    private GameObject _player;
    private PlayerController _playerController;

    //public bool usePlayerController;

    public void UpdatePlayerFog(FogData_ScriptableObjectScript newFogData)
    {
        if (newFogData == null)
            return;

        RenderSettings.fog = newFogData.useFog;

        if (!newFogData.useFog)
            return;

        RenderSettings.fogColor = newFogData.fogColor;
        _playerController.UpdateCameraBackgroundColor(newFogData.camBGColor);

        RenderSettings.fogMode = newFogData.fogMode;

        if (newFogData.fogMode == FogMode.Linear)
        {
            RenderSettings.fogStartDistance = newFogData.fogStartDistance;
            RenderSettings.fogEndDistance = newFogData.fogEndDistance;
        }
        else
            RenderSettings.fogDensity = newFogData.fogDensity;

        _playerController.ToggleFlashLight(newFogData.flashLightEnabled);

        //if (newFogData.useParticleSystem)
        _playerController.EnablePlayerParticleSystem(newFogData.useParticleSystem);
    }
    public GameObject SpawnPlayer(int x, int z, int y = 0)
    {
        //if (usePlayerController)
        //{
        //    GameObject newPlayer = Instantiate(_playerControllerPrefab, this.transform.parent);
        //    _playerController = newPlayer.GetComponent<PlayerController>();
        //    _playerController.transform.position = new Vector3 (x, y, z);
        //    _playerController.name = "Player";
        //}
        //else
        //{
        this.transform.position = new Vector3 (x, y, z);
        Debug.Log("Spawning Player at: " + x + ", " + z);
        GameObject newPlayer = Instantiate(_playerPrefab);
        _player = newPlayer;
        _playerController = _player.GetComponent<PlayerController>();
        _player.GetComponent<PlayerController>().MovePlayerToSpawn(new Vector3(x, y, z));
        //_player.transform.position = new Vector3(x, y, z);
        _player.name = "Player";

        return _player;
        //}
    }

    private void OnDisable()
    {
        //if (usePlayerController)
        //    Destroy( _playerController );
        //else
        Destroy(_player);
        Destroy(this.gameObject );
    }
}
