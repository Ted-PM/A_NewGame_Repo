using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;


public enum EnemyType
{
    Crawler,
    Manequin
};

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyType enemyType;

    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private GameObject _enemyBody;
    private Renderer _enemyRenderer;

    [SerializeField]
    private float _distanceBeforeGoToPlayer;

    [SerializeField]
    private Animator _enemyAnimator;

    private GameObject playerTransform;
    private bool playerFound = false;
    private bool canGoToPlayer = false;
    private Vector3 playerPos;

    private bool enemyDisabled = false;
    private Vector3 _enemySpawn;

    private NavMeshPath _enemyPath;
    private bool _enemyMoved = false;

    private Camera _playerCam;

    private bool _playerCanSeeEnemy = false;
    private float _enemySpeed;

    private int _camPixelWidth;
    private int _camPixelHeight;

    private bool _enemySeenForFirstTime = false;

    //Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        //agent.enabled = false;
        //agent.gameObject.SetActive(false);
        if (_enemyBody != null)
        {
            //if (!_enemyBody.TryGetComponent<Animator>(out _enemyAnimator))
            //    Debug.LogError("Enemy Animator is NULL!!");
            if (!_enemyBody.TryGetComponent<Renderer>(out _enemyRenderer))
                Debug.LogError("Enemy Renderer NULL!!");
            else
                _enemyRenderer.enabled = false;
        }
        else
            Debug.LogError("Enemy Body is NULL!!");
        

        if (agent == null)
            Debug.LogError("NavMesh Agent NULL !!");
        //else
        //    agent.updatePosition = false;
        _enemySpeed = agent.speed;
        _enemyPath = new NavMeshPath();
        
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemySpawn = transform.position;
        StartCoroutine(TryFindPlayer());
    }

    public void DisableEnemy()
    {
        if (CheckIfPlayerInRange(10f))
            return;
        if (_enemyRenderer != null)
            _enemyRenderer.enabled = false;
        if (agent != null)
        {
            agent.enabled = false;
            //agent.updatePosition = false;
        }

        canGoToPlayer = false;
        enemyDisabled = true;
    }

    public void EnableEnemy()
    {
        if (_enemyRenderer != null)
            _enemyRenderer.enabled = true;
        if (agent != null)
        {
            agent.enabled = true;
            //agent.updatePosition = true;
        }
        enemyDisabled = false;
        StartCoroutine(CanPathToPlayer());
    }

    private IEnumerator TryFindPlayer()
    {
        yield return null;

        while (Camera.allCamerasCount <= 0)
        {
            yield return new WaitForSeconds(1f);
        }

        if (Camera.allCamerasCount == 1)
        {
            _playerCam = Camera.allCameras[0];
            _camPixelWidth = _playerCam.pixelWidth;
            _camPixelHeight = _playerCam.pixelHeight;
            playerTransform = FindAnyObjectByType<PlayerController>().gameObject;
            playerFound = true;

            if (!enemyDisabled)
            {
                //agent.enabled = true;
                //_enemyRenderer.enabled = true;
                StartCoroutine(WaitThenEnableAgent());
                //agent.updatePosition = true;
                StartCoroutine(CanPathToPlayer());
                StartCoroutine(EnemySeenForFirstTime());
            }                 
            
        }
    }


    private void Update()
    {
        if (canGoToPlayer && !enemyDisabled && _enemySeenForFirstTime) 
            GoToPlayer(enemyType);
    }

    private void FixedUpdate()
    {
        if (playerFound && !enemyDisabled)
        {
            playerPos = playerTransform.transform.position;
            if (canGoToPlayer) 
                agent.CalculatePath(playerPos, _enemyPath);
            else if (_enemyMoved && !EnemyVisibleToPlayer())
                StartCoroutine(GoBackToSpawn());
            if (enemyType == EnemyType.Manequin)
                _playerCanSeeEnemy = EnemyVisibleToPlayer();
        }

        //if (EnemyVisibleToPlayer())
        //    Debug.Log("Enemy Visible");
    }

    private IEnumerator EnemySeenForFirstTime()
    {
        while (!_enemySeenForFirstTime && !enemyDisabled)
        {
            yield return new WaitForFixedUpdate();
            _enemySeenForFirstTime = EnemyVisibleToPlayer();
        }
    }

    private IEnumerator WaitThenEnableAgent()
    {
        yield return new WaitForSeconds(1f);
        if (!enemyDisabled)
        {
            agent.enabled = true;
            _enemyRenderer.enabled = true;
        }
    }

    private IEnumerator CanPathToPlayer()
    {
        while (!enemyDisabled)
        {
            yield return new WaitForSeconds(1f);
            canGoToPlayer = CheckIfPlayerInRange();            
        }
    }

    private bool CheckIfPlayerInRange(float range = 0)
    {
        if (range == 0)
            range = _distanceBeforeGoToPlayer;
        return Vector3.Distance(playerPos, _enemyBody.transform.position) <= range;
    }

    private void GoToPlayer(EnemyType _type)
    {
        switch (_type)
        {
            case EnemyType.Crawler:
                if (!_enemyMoved)
                {
                    //_enemyRenderer.enabled = true;
                    _enemyAnimator.SetBool("Static", false);
                    _enemyMoved = true;                   
                }
                if (!enemyDisabled)
                    agent.SetPath(_enemyPath);
                break;
            case EnemyType.Manequin:
                if (!_enemyMoved)
                {
                    _enemyAnimator.SetBool("Static", false);
                    _enemyMoved = true;
                }
                ToggleAnimator();
                if (!_playerCanSeeEnemy && !enemyDisabled)
                    agent.SetPath(_enemyPath);
                break;
        }
        
        //agent.destination = playerTransform.transform.position;
    }

    private void ToggleAnimator()
    {
        _enemyAnimator.enabled = !_playerCanSeeEnemy;
        agent.isStopped = _playerCanSeeEnemy;
        if (_playerCanSeeEnemy)
            agent.velocity = Vector3.zero;
        //else
        //    agent.speed = 0f;
    }

    private void DisableAnimator()
    {
        _enemyAnimator.enabled = false;
    }

    private void EnableAnimator()
    {
        _enemyAnimator.enabled = true;
    }

    private IEnumerator GoBackToSpawn()
    {
        //_enemyRenderer.enabled = false;
        _enemyMoved = false;
        
        //agent.CalculatePath(_enemySpawn, _enemyPath);
        while (agent.transform.position != _enemySpawn && !canGoToPlayer && !enemyDisabled)
        {
            agent.CalculatePath(_enemySpawn, _enemyPath);
            yield return new WaitForFixedUpdate();
            if (!enemyDisabled)
                agent.SetPath(_enemyPath);
        }

        if (agent.transform.position == _enemySpawn)
        {
            _enemyAnimator.SetBool("Static", true);
            _enemySeenForFirstTime = false;
            StartCoroutine(EnemySeenForFirstTime());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enemy Trigger ");
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("(ENTER) Enemy Trigger gameobject is null!");
            return;
        }

        if (other.gameObject.tag == "Door")
        {
            
            //Debug.Log("Enemy Interact With Door Now");
            CellDoor door = other.gameObject.GetComponentInParent<CellDoor>();

            if (door == null)
            {
                Debug.LogError("Door Interactable parent is null");
                return;
            }
            if (!door.DoorIsOpen())
            {
                if (enemyType != EnemyType.Manequin)
                    door.InteractWithDoor(transform.position);
                else
                    StartCoroutine(WaitTillEnemyNotSeen(door));

            }
        }
    }

    private IEnumerator WaitTillEnemyNotSeen(CellDoor door)
    {
        while (EnemyVisibleToPlayer())
        {
            yield return new WaitForEndOfFrame();
        }

        door.InteractWithDoor(transform.position);
    }

    private bool EnemyVisibleToPlayer()
    {
        if (!_enemyRenderer.isVisible || !CheckIfPlayerInRange())
            return false;

        Vector3[] camPos = new Vector3[3];

        camPos[0] = _playerCam.transform.position;
        camPos[1] = _playerCam.ScreenToWorldPoint(new Vector3(_camPixelWidth, _camPixelHeight / 2, 1f));
        camPos[2] = _playerCam.ScreenToWorldPoint(new Vector3(0, _camPixelHeight / 2, 1f));

        Vector3[] directionToEnemy = new Vector3[3];

        for (int i = 0; i < 3; i++)
            directionToEnemy[i] = (transform.position + new Vector3(0, 2, 0)) - camPos[i];

        RaycastHit[] hits = new RaycastHit[3];

        for (int i = 0; i < 3; i++)
            Physics.Raycast(camPos[i], directionToEnemy[i], out hits[i], (_distanceBeforeGoToPlayer*2), ~0);

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

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Enemy Collision");

    //    if (collision == null)
    //    {
    //        Debug.LogError("Enemy Collision is NULL");
    //        return;
    //    }

    //    if (collision.gameObject.tag == "Door")
    //    {
    //        Debug.Log("Enemy Interact With Door Now");
    //        CellDoor door = collision.gameObject.GetComponentInParent<CellDoor>();

    //        if (door == null)
    //        {
    //            Debug.LogError("Door Interactable parent is null");
    //            return;
    //        }

    //        door.InteractWithDoor(transform.position);
    //    }
    //}

    //private void GoBackToSpawn()
    //{
    //    //agent.CalculatePath(playerPos, _enemyPath);
    //    //_enemySpawn
    //}

}
