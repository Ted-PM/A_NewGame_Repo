using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private GameObject _enemyBody;
    private Renderer _enemyRenderer;

    private GameObject playerTransform;
    private bool playerFound = false;
    private bool canGoToPlayer = false;
    private Vector3 playerPos;

    private bool enemyDisabled = false;
    private Vector3 _enemySpawn;

    private NavMeshPath _enemyPath;
    private bool _enemyMoved = false;

    //Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        //agent.enabled = false;
        //agent.gameObject.SetActive(false);
        if (_enemyBody == null || !_enemyBody.TryGetComponent<Renderer>(out _enemyRenderer))
            Debug.LogError("Couldn't Get Enemy rendere or enemy body not set!!");
        else
            _enemyRenderer.enabled = false;

        if (agent == null)
            Debug.LogError("NavMesh Agent NULL !!");
        //else
        //    agent.updatePosition = false;

        _enemyPath = new NavMeshPath();
        _enemySpawn = transform.position;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TryFindPlayer());
    }

    public void DisableEnemy()
    {
        if (CheckIfPlayerInRange())
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
            playerTransform = FindAnyObjectByType<PlayerController>().gameObject;
            playerFound = true;

            if (!enemyDisabled)
            {
                agent.enabled = true;
                _enemyRenderer.enabled = true;
                //agent.updatePosition = true;
                StartCoroutine(CanPathToPlayer());
            }                 
            
        }
    }


    private void Update()
    {
        if (canGoToPlayer && !enemyDisabled) 
            GoToPlayer();
    }

    private void FixedUpdate()
    {
        if (playerFound && !enemyDisabled)
        {
            playerPos = playerTransform.transform.position;
            agent.CalculatePath(playerPos, _enemyPath);
        }
        else if (!enemyDisabled && _enemyMoved && !canGoToPlayer)
        {
            StartCoroutine(GoBackToSpawn());
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

    private bool CheckIfPlayerInRange()
    {
        return Vector3.Distance(playerPos, _enemyBody.transform.position) <= 20;
    }

    private void GoToPlayer()
    {
        _enemyMoved = true;
        agent.SetPath(_enemyPath);
        //agent.destination = playerTransform.transform.position;
    }

    private IEnumerator GoBackToSpawn()
    {
        _enemyMoved = false;
        agent.CalculatePath(_enemySpawn, _enemyPath);

        while (agent.transform.position != _enemySpawn && !canGoToPlayer)
        {
            yield return new WaitForFixedUpdate();
            agent.SetPath(_enemyPath);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null)
        {
            Debug.LogError("Enemy Collision is NULL");
            return;
        }

        if (collision.gameObject.tag == "Door")
        {
            Debug.Log("Enemy Interact With Door Now");
            CellDoor door = collision.gameObject.GetComponentInParent<CellDoor>();
            
            if (door == null)
            {
                Debug.LogError("Door Interactable parent is null");
                return;
            }

            door.InteractWithDoor(transform.position);
        }
    }

    //private void GoBackToSpawn()
    //{
    //    //agent.CalculatePath(playerPos, _enemyPath);
    //    //_enemySpawn
    //}

}
