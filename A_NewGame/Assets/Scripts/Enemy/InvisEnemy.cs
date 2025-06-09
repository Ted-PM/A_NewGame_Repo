using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.ProBuilder.Shapes;

public class InvisEnemy : EnemyBaseClass
{
    [SerializeField]
    private AudioClip _enemyPassiveAudio;
    [SerializeField]
    private AudioClip _enemyAgroAudio;
    private bool _playerCantSeeEnemy;

    public override void GoToPlayer()
    {
        _playerCantSeeEnemy = !PlayerLookingAtEnemyDirectly();

        if (!_enemyMoved && _enemyState == EnemyStates.Agro)
        {
            _enemyAnimator.SetBool("Static", false);
            PlayOneShotAudio(_enemyAgroAudio);
            _enemyMoved = true;
        }
        if (!_playerCantSeeEnemy != _enemyAnimator.enabled)
            ToggleAnimator();

        if (!_playerCantSeeEnemy && _enemyState == EnemyStates.Agro)
            base.SetEnemyPath();
    }

    private void ToggleAnimator()
    {
        //Debug.Log("Mannequin Toggle Animator");
        _enemyAnimator.enabled = !_playerCantSeeEnemy;
        agent.isStopped = _playerCantSeeEnemy;

        if (_playerCantSeeEnemy)
            agent.velocity = Vector3.zero;
    }

    protected override void EnableAgent()
    {
        base.EnableAgent();
        _enemyAnimator.enabled = true;
    }

    protected override IEnumerator WaitThenEnableAgent()
    {
        if (_enemyState != EnemyStates.Disabled)
        {
            _enemyAnimator.enabled = true;
            StartCoroutine(base.WaitThenEnableAgent());
        }
        yield break;
    }
    /// ------------
    protected override IEnumerator CheckIfEnemyAgroed()
    {
        StartCoroutine(base.CheckIfEnemyAgroed());
        yield return null;
    }

    protected override IEnumerator CheckIfEnemyDeAgroed()
    {
        StartCoroutine(base.CheckIfEnemyDeAgroed());
        yield return null;
    }
    /// ------------
    protected override IEnumerator EnemySeenForFirstTime()
    {
        if (_enemyState != EnemyStates.Disabled)
            StartCoroutine(base.EnemySeenForFirstTime());
        yield break;
    }

    protected override void OnTriggerEnter(Collider other)
    {
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
                StartCoroutine(WaitTillEnemyNotSeen(door));
            }
        }
    }

    private IEnumerator WaitTillEnemyNotSeen(CellDoor door)
    {
        while (EnemyVisibleToPlayer())
        {
            yield return new WaitForFixedUpdate();
        }

        door.InteractWithDoor(transform.position);
    }
}

