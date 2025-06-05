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
        if (_enemyState != EnemyStates.Disabled)
            base.SetEnemyPath();
    }

    protected override IEnumerator EnemySeenForFirstTime()
    {
        if (_enemyState != EnemyStates.Disabled)
            StartCoroutine(base.EnemySeenForFirstTime());
        yield break;
    }

    protected override IEnumerator WaitThenEnableAgent()
    {
        if (_enemyState != EnemyStates.Disabled)
        {
            StartCoroutine(base.WaitThenEnableAgent());
            _enemyAnimator.enabled = true;
        }
        yield break ;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
