using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab;
    private PlayerController _playerController;

    public void SpawnPlayer(int x, int z, int y = 0)
    {
        GameObject newPlayer = Instantiate(_playerPrefab, this.transform.parent);
        _playerController = newPlayer.GetComponent<PlayerController>();
        _playerController.transform.position = new Vector3 (x, y, z);
        _playerController.name = "Player";
    }

    private void OnDisable()
    {
        Destroy( _playerController );
        Destroy(this.gameObject );
    }
}
