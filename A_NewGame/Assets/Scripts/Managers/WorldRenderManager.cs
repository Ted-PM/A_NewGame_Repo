using UnityEngine;
using System.Collections;


public class WorldRenderManager : MonoBehaviour
{
    //private int playerFloorLevel = 0;
    private MazeFloor[] _mazeFloors;
    //private GameObject playerTransform;
    //private bool playerFound = false;

    public void SetMazeFloors(MazeFloor[] mazeFloors)
    {
        _mazeFloors = mazeFloors;
    }

    //private IEnumerator TryFindPlayer()
    //{
    //    yield return null;

    //    while (Camera.allCamerasCount <= 0)
    //    {
    //        yield return new WaitForSeconds(1f);
    //    }

    //    if (Camera.allCamerasCount == 1)
    //    {
    //        playerTransform = FindAnyObjectByType<PlayerController>().gameObject;
    //        playerFound = true;
    //    }
    //}

    //private void Start()
    //{
    //    //StartCoroutine(WaitTheDisableInitialFloors());
    //    StartCoroutine(TryFindPlayer());
    //}

    //private IEnumerator WaitTheDisableInitialFloors()
    //{

    //    while (_mazeFloors == null)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    yield return new WaitForSeconds(0.5f);

    //    DisableInitialFloorRenderers();
    //}


    //public void DisableInitialFloorRenderers()
    //{
    //    for (int i = 0; i < _mazeFloors.Length; i++)
    //    {
    //       _mazeFloors[i].DisableFloorRenderers(0);
    //    }
    //}

    //private void FixedUpdate()
    //{
    //    if (playerFound && _mazeFloors != null)
    //    {
    //        UpdateRenderers();
    //    }
    //}

    public void UpdateRenderers(int floorLevel)
    {
        if (_mazeFloors[floorLevel] == null)
        {
            Debug.LogError("Maze Floor not in Render Manager!!");
            return;
        }
        //playerFloorLevel =(int) (((int)playerTransform.transform.position.y) / 10);

        _mazeFloors[floorLevel].EnableFloorRenderers();
        for (int i = 0; i < _mazeFloors.Length; i++)
        {
            //if (i == 1)
            _mazeFloors[i].DisableFloorRenderers(floorLevel);
            //else
            //    _mazeFloors[i].DisableFloorRenderers(playerFloorLevel);
        }
    }

}
