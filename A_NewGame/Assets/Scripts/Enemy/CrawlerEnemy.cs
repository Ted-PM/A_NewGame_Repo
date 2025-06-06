using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CrawlerEnemy : EnemyBaseClass
{
    [SerializeField]
    private AudioClip _enemyPassiveAudio;
    [SerializeField]
    private AudioClip _enemyAgroAudio;

    public override void GoToPlayer()
    {
        if (!_enemyMoved && _enemyState == EnemyStates.Agro)
        {
            _enemyAnimator.SetBool("Static", false);
            PlayOneShotAudio(_enemyAgroAudio);
            _enemyMoved = true;
        }
        if (_enemyState == EnemyStates.Agro)
            base.SetEnemyPath();
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
        yield break ;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
