using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyCell : CellBaseClass
{
    //[SerializeField]
    //private List<GameObject> _enemySpawnPoints;

    public override void DisableCell()
    {
        //DisableEnemies();
        base.DisableCell();
    }

    public override void EnableCell()
    {
        base.EnableCell();
        //EnableEnemies();
    }

    //private void DisableEnemies()
    //{
    //    if (_enemySpawnPoints == null)
    //        return;
    //    foreach (GameObject enemy in _enemySpawnPoints)
    //    {
    //        if (enemy != null)
    //            enemy.SetActive(false);
    //    }
    //}

    //private void EnableEnemies()
    //{
    //    if (_enemySpawnPoints == null)
    //        return;
    //    foreach (GameObject enemy in _enemySpawnPoints)
    //    {
    //        if (enemy != null)
    //            enemy.SetActive(true);
    //    }
    //}
}
