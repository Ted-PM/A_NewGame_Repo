using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class PlayerSightManager : MonoBehaviour
{
    private PlayerController _playerController;
    private Camera _playerCam;
    private float _camPixelWidth;
    private float _camPixelHeight;
    private float _maxViewDistance;

    [SerializeField]
    private LayerMask _enemyLayer;
    [SerializeField]
    private LayerMask _interactableLayer;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerCam = _playerController.GetPlayerCam();
        _camPixelWidth = _playerCam.pixelWidth;
        _camPixelHeight = _playerCam.pixelHeight;
        _maxViewDistance = _playerCam.farClipPlane;
    }

    private void FixedUpdate()
    {
        
    }

    private bool ObjectVisibleToPlayer(Vector3 objectPos)
    {
        Vector3[] camPos = new Vector3[3];

        camPos[0] = _playerCam.transform.position;
        camPos[1] = _playerCam.ScreenToWorldPoint(new Vector3(_camPixelWidth, _camPixelHeight / 2, 1f));
        camPos[2] = _playerCam.ScreenToWorldPoint(new Vector3(0, _camPixelHeight / 2, 1f));

        Vector3[] directionToObject = new Vector3[3];

        for (int i = 0; i < 3; i++)
            directionToObject[i] = (objectPos + new Vector3(0, 2, 0)) - camPos[i];

        RaycastHit[] hits = new RaycastHit[3];

        for (int i = 0; i < 3; i++)
            Physics.Raycast(camPos[i], directionToObject[i], out hits[i], (_maxViewDistance * 2), ~0);

        bool result = false;

        foreach (var hit in hits)
        {
            if (hit.collider == null)
                Debug.LogError("Enemy Hit is null");
            else if (hit.collider.gameObject.tag == "Enemy")
            {
                result = true;
                break;
            }
        }

        return result;
    }

    private bool PlayerLookingDirectlyAtObject(float distance = 0f)
    {
        if (distance <= 0f)
            distance = _maxViewDistance;

        Vector3 directionToEnemy = transform.position - _playerCam.transform.position; ;

        return Physics.Raycast(_playerCam.transform.position, directionToEnemy, distance, ~0);
    }
}
