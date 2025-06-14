using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;

public enum EnemyType
{
    Crawler,
    Mannequin,
    Lurker,
    Invis,
    TallGuy
};

public enum EnemyStates
{
    Disabled,           // not being rendered
    Static,             // being rendered but not near player
    InRange,            // being rendered and near player
    Agro,               // chasing player
    Returning           // returning to spawn point
};

public class EnemyBaseClass : MonoBehaviour
{
    [SerializeField]
    private EnemyType enemyType;

    [SerializeField]
    protected EnemyStates _enemyState;

    [SerializeField]
    protected NavMeshAgent agent;

    [SerializeField]
    protected GameObject _enemyBody;
    protected Renderer _enemyRenderer;

    [SerializeField]
    protected float _distanceBeforeAgro;
    [SerializeField]
    protected float _distanceBeforeDeAgro = -1;
    [SerializeField]
    private float _timeWithoutSeeingPlayerBeforeLeave;

    [SerializeField]
    protected Animator _enemyAnimator;

    [SerializeField]
    private LayerMask _enemyLayer;
    [SerializeField]
    private LayerMask _playerLayer;
    //[SerializeField]
    protected AudioSource _enemyAudioSource;

    //[SerializeField]
    //protected EnemyMovement _enemyMovement;

    protected GameObject _playerTransform;
    protected bool _playerFound = false;
    //private bool _canGoToPlayer = false;
    protected Vector3 _playerPos;

    //protected bool _enemyDisabled = false;
    protected Vector3 _enemySpawn;

    protected Rigidbody _enemyRB;

    protected NavMeshPath _enemyPath;
    protected bool _enemyMoved = false;
    protected bool _enemyVisible = false;

    private Camera _playerCam;

    //protected bool _playerCanSeeEnemy = false;
    //private float _enemySpeed;

    private int _camPixelWidth;
    private int _camPixelHeight;

    //protected bool _enemySeenForFirstTime = false;

    protected virtual void Awake()
    {
        if (_enemyBody != null)
        {
            if (!_enemyBody.TryGetComponent<Renderer>(out _enemyRenderer))
                Debug.LogError("Enemy Renderer NULL!!");
            else
                _enemyRenderer.enabled = false;
        }
        else
            Debug.LogError("Enemy Body is NULL!!");

        if (!TryGetComponent<Rigidbody>(out _enemyRB))
        {
            Debug.LogError("No Enemy RigidBody!!");
        }
        if (agent == null)
            Debug.LogError("NavMesh Agent NULL !!");

        //if (_enemyMovement == null)
        //{
        //    if (!TryGetComponent<EnemyMovement>(out _enemyMovement))
        //        Debug.Log("No EnemyMovement Script!!");
        //}


        _enemyState = new EnemyStates();
        //_enemySpeed = agent.speed;
        _enemyPath = new NavMeshPath();
        _enemyAudioSource = gameObject.AddComponent<AudioSource>();
        _enemyAudioSource.spatialBlend = 1;
        _enemyAudioSource.dopplerLevel = 3;

        _enemyState = EnemyStates.Static;

        if (_distanceBeforeDeAgro <= 0)
            _distanceBeforeDeAgro = (_distanceBeforeAgro / 3);
        StartCoroutine(TryFindPlayer());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        //_enemySpawn = transform.position;
        //_enemyState = EnemyStates.Static;
        //StartCoroutine(TryFindPlayer());
    }

    protected virtual void Update()
    {
        //if (_canGoToPlayer && !_enemyDisabled && _enemySeenForFirstTime)
        if (_enemyState == EnemyStates.Agro)//  && _enemySeenForFirstTime)
        {
            //_enemyMovement.SynchronizeAnimatorAndAgent();
            GoToPlayer();

        }
    }

    public virtual void GoToPlayer()
    {
        Debug.Log("Virtual Function in Base Called");
    }

    protected virtual void FixedUpdate()
    {
        //Debug.Log("Enemy: " + enemyType.ToString() + " -- Enemy State: " + _enemyState.ToString());
        if (_playerFound && _enemyState != EnemyStates.Disabled)
        {
            _playerPos = _playerTransform.transform.position;

            if (_enemyState == EnemyStates.Agro && !_enemyAnimator.applyRootMotion)
                agent.CalculatePath(_playerPos, _enemyPath);
            //else if (_enemyMoved && !EnemyVisibleToPlayer())
            //    StartCoroutine(GoBackToSpawn());
        }
    }

    protected void SetEnemyPath()
    {
        //if (_enemyMovement != null) 
        //    _enemyMovement.SynchronizeAnimatorAndAgent();
        //else
        //if (_enemyAnimator.enabled)
        //    _enemyAnimator
        if (!_enemyAnimator.applyRootMotion)
            agent.SetPath(_enemyPath);
        else
            agent.destination = _playerPos;
    }

    private IEnumerator TryFindPlayer()
    {
        yield return null;

        while (Camera.allCamerasCount <= 0)
        {
            //yield return new WaitForSeconds(1f);
            yield return new WaitForFixedUpdate();
        }

        if (Camera.allCamerasCount == 1)
        {
            _playerCam = Camera.allCameras[0];
            _camPixelWidth = _playerCam.pixelWidth;
            _camPixelHeight = _playerCam.pixelHeight;

            _playerTransform = FindAnyObjectByType<PlayerController>().gameObject;
            _playerFound = true;
            _playerPos = _playerTransform.transform.position;

            //if (!_enemyDisabled)
            if (_enemyState != EnemyStates.Disabled)
            {
                StartCoroutine(CheckIfEnemyAgroed());
                //StartCoroutine(EnemySeenForFirstTime());
                StartCoroutine(CanPathToPlayer());
                //agent.enabled = true;
                //_enemyRenderer.enabled = true;
                //EnableEnemy();
                //StartCoroutine(WaitThenEnableAgent());
                //StartCoroutine(CanPathToPlayer());
                //StartCoroutine(EnemySeenForFirstTime());
                //PlayPassiveAudio();
            }

        }
    }

    protected virtual void EnableAgent()
    {
        agent.enabled = true;
        _enemyRenderer.enabled = true;
    }
    protected virtual IEnumerator WaitThenEnableAgent()
    {
        yield return new WaitForSeconds(1f);
        //if (!_enemyDisabled)
        if (_enemyState != EnemyStates.Disabled)
        {
            agent.enabled = true;
            _enemyRenderer.enabled = true;
        }
    }

    protected virtual IEnumerator CanPathToPlayer()
    {
        //while (!_enemyDisabled)
        float distanceToPlayer = 0;
        while (_enemyState != EnemyStates.Disabled)
        {
            distanceToPlayer = GetDistanceToPlayer();
            if (_enemyState != EnemyStates.Agro && _enemyState != EnemyStates.Returning)
            {
                if (distanceToPlayer < _distanceBeforeAgro)
                    _enemyState = EnemyStates.InRange;
                else if (distanceToPlayer < 2 * _distanceBeforeAgro)
                    _enemyState = EnemyStates.Static;
                else
                {
                    //Debug.Log("Max Distance before disabled: " + (2 * _distanceBeforeGoToPlayer));
                    //Debug.Log("Distance to Player: " + Vector3.Distance(_playerPos, _enemyBody.transform.position));
                    DisableEnemy();
                }
            }
            
                //_enemyState = EnemyStates.Static;
            //yield return new WaitForSeconds(1f);
            yield return new WaitForFixedUpdate();
            //_canGoToPlayer = CheckIfPlayerInRange();
        }
    }
    /// ------------
    protected virtual IEnumerator CheckIfEnemyAgroed()
    {
        StartCoroutine(EnemySeenForFirstTime());
        yield return null;
    }
    /// ------------
    protected IEnumerator EnemySeenForFirstTime()
    {
        //while (!_enemySeenForFirstTime && !_enemyDisabled)
        while (_enemyState != EnemyStates.Agro && _enemyState != EnemyStates.Disabled)
        {
            yield return new WaitForFixedUpdate();
            if (EnemyVisibleToPlayer())
            {
                _enemyState = EnemyStates.Agro;
                StartCoroutine(CheckIfEnemyDeAgroed());
                //StartCoroutine(CanNoLongerSeePlayer());
            }
        }
    }

    public virtual void DisableEnemy()
    {
        if (_enemyState == EnemyStates.Agro)
            return;
        //Debug.Log(enemyType + " disabled at " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
        StopAllCoroutines();
        _enemyState = EnemyStates.Disabled;

        if (_enemyRenderer != null)
            _enemyRenderer.enabled = false;
        if (agent != null)
        {
            agent.enabled = false;
        }
        if (_enemyAudioSource != null) 
            _enemyAudioSource.enabled = false;
        if (_enemyAnimator != null)
        {
            //_enemyAnimator.SetBool("Static", true);
            _enemyAnimator.SetBool("move", false);  
            _enemyAnimator.enabled = false;
        }
        EnemySpawner.Instance.DisableEnemy(this.gameObject);
        //_canGoToPlayer = false;
        //_enemyDisabled = true;
    }

    public virtual void EnableEnemy()
    {
        _enemyState = EnemyStates.Static;
        StopAllCoroutines();
        
        if (_enemyAudioSource != null)
            _enemyAudioSource.enabled = true;
        EnableAgent();
        _enemySpawn = transform.position;
        if (!_playerFound)
            StartCoroutine(TryFindPlayer());
        else
        {
            _playerPos = _playerTransform.transform.position;

            StartCoroutine(CheckIfEnemyAgroed());
            StartCoroutine(CanPathToPlayer());
        }
    }

    private bool CheckIfPlayerInRange(float range = 0)
    {
        if (range == 0)
            range = _distanceBeforeAgro;
        return Vector3.Distance(_playerPos, _enemyBody.transform.position) <= range;
    }

    protected float GetDistanceToPlayer()
    {
        return Vector3.Distance(_playerPos, _enemyBody.transform.position);
    }

    protected bool PlayerVisibleToEnemy()
    {
        Vector3 directionToPlayer = _playerCam.transform.position - (transform.position + new Vector3(0,2,0));
        //Debug.DrawRay((transform.position + new Vector3(0, 2, 0)), directionToPlayer, Color.red, 5f);
        RaycastHit hit;
        //return Physics.Raycast((transform.position + new Vector3(0, 2, 0)), directionToPlayer, _distanceBeforeGoToPlayer, _playerLayer, QueryTriggerInteraction.Ignore);
        Physics.Raycast((transform.position + new Vector3(0, 2, 0)), directionToPlayer, out hit, _distanceBeforeAgro, ~0, QueryTriggerInteraction.Ignore);
        //return (hit == null)
        if (hit.collider == null || hit.collider.gameObject == null)
            return false;

        if (hit.transform.gameObject.layer == _playerLayer)
            return true;

        return false;
    }

    protected bool EnemyVisibleToPlayer()
    {
        //Debug.Log("is renderer visible: " + _enemyRenderer.isVisible + ", Player in Range: " + CheckIfPlayerInRange());
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
        {
            //Debug.DrawRay(camPos[i], directionToEnemy[i], Color.red,1f);
            Physics.Raycast(camPos[i], directionToEnemy[i], out hits[i], (_distanceBeforeAgro * 2), _enemyLayer, QueryTriggerInteraction.Collide);
        }

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

    protected bool PlayerLookingAtEnemyDirectly(float distance = 0f)
    {
        if (!_enemyRenderer.isVisible || !CheckIfPlayerInRange())
            return false;
        if (distance <= 0f)
            distance = (_distanceBeforeAgro * 2);

        RaycastHit hit;

        //Vector3 directionToEnemy = (transform.position + new Vector3(0,2,0)) - _playerCam.transform.position; ;
        //return Physics.Raycast(_playerCam.transform.position, directionToEnemy, distance, _enemyLayer);

        Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward, out hit, distance, _enemyLayer);
        if (hit.collider == null || hit.collider.gameObject == null)
            return false;
        return hit.collider.gameObject.tag == "Enemy";
    }

    /// ------------
    protected virtual IEnumerator CheckIfEnemyDeAgroed()
    {
        StartCoroutine(CanNoLongerSeePlayer());
        yield return null;
    }
    /// ------------
    protected IEnumerator CanNoLongerSeePlayer()
    {
        yield return new WaitForFixedUpdate();
        bool playerSeen = false;
        float time = 0f;
        while (_enemyState == EnemyStates.Agro)
        {
            playerSeen = false;
            time = 0f;
            while (time < _timeWithoutSeeingPlayerBeforeLeave)
            {
                time += Time.fixedDeltaTime;
                if (PlayerVisibleToEnemy() || CheckIfPlayerInRange(_distanceBeforeDeAgro))
                {
                    playerSeen = true;
                    break;
                }
                yield return new WaitForFixedUpdate();
            }

            if (!playerSeen)
                break;
            yield return new WaitForSeconds(0.1f);
        }

        if (!playerSeen)
            StartCoroutine(GoBackToSpawn());
        
    }

    private IEnumerator GoBackToSpawn()
    {
        _enemyMoved = false;
        _enemyState = EnemyStates.Returning;
        StartCoroutine(CheckIfEnemyAgroed());
        //StartCoroutine(EnemySeenForFirstTime());

        while (!EnemyNearPosition(_enemySpawn) && _enemyState == EnemyStates.Returning)
        {
            agent.CalculatePath(_enemySpawn, _enemyPath);
            yield return new WaitForFixedUpdate();
            if (_enemyState != EnemyStates.Disabled)
                agent.SetPath(_enemyPath);
        }

        if (EnemyNearPosition(_enemySpawn) && _enemyState != EnemyStates.Disabled)
        {
            //_enemyAnimator.SetBool("Static", true);
            _enemyAnimator.SetBool("move", false);

            _enemyState = EnemyStates.Static;            
        }
    }

    protected bool EnemyNearPosition(Vector3 pos)
    {
        return Vector3.Distance(pos, transform.position) <= 2;
        //return Vector3.Distance(pos, _enemyBody.transform.position) <= 2;
    }

    protected void PlayLoopedAudio(AudioClip _enemyAudioClip)
    {
        if (_enemyAudioClip == null)
        {
            Debug.Log("Looped audio clip NULL.");
            return;
        }
        _enemyAudioSource.clip = _enemyAudioClip;
        _enemyAudioSource.loop = true;
        _enemyAudioSource.Play();
    }

    protected void PlayOneShotAudio(AudioClip _enemyAudioClip)
    {
        if (_enemyAudioClip == null)
        {
            Debug.Log("OneShot audio clip NULL.");
            return;
        }

        _enemyAudioSource.clip = _enemyAudioClip;
        _enemyAudioSource.loop = false;
        _enemyAudioSource.Play();
    }

    protected void StopAudio()
    {
        _enemyAudioSource.Stop();
    }

    public EnemyStates GetEnemyState()
    {
        return _enemyState;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enemy Trigger ");
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("(ENTER) Enemy Trigger gameobject is null!");
            return;
        }

        if (other.gameObject.tag == "Door")
        {
            CellDoor door = other.gameObject.GetComponentInParent<CellDoor>();

            if (door == null)
            {
                Debug.LogError("Door Interactable parent is null");
                return;
            }
            if (!door.DoorIsOpen())
            {
                door.InteractWithDoor(transform.position);
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        Debug.Log("Base enemy trigger exit");
    }
}
