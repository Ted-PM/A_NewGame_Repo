using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerControllerPrefab;
    [SerializeField]
    private GameObject _playerPrefab;

    private PlayerController _playerController;
    private GameObject _player;

    public bool usePlayerController;

    public void SpawnPlayer(int x, int z, int y = 0)
    {
        if (usePlayerController)
        {
            GameObject newPlayer = Instantiate(_playerControllerPrefab, this.transform.parent);
            _playerController = newPlayer.GetComponent<PlayerController>();
            _playerController.transform.position = new Vector3 (x, y, z);
            _playerController.name = "Player";
        }
        else
        {
            GameObject newPlayer = Instantiate(_playerPrefab, this.transform.parent);
            _player = newPlayer;
            _player.transform.position = new Vector3(x, y, z);
            _player.name = "Player";
        }
    }

    private void OnDisable()
    {
        if (usePlayerController)
            Destroy( _playerController );
        else
            Destroy( _player );

        Destroy(this.gameObject );
    }
}
