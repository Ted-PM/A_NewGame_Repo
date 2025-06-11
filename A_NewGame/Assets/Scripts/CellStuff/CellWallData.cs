using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public struct CellWallData
{
    public CellWall wall;
    // has matrix ID of adjacent cell
    public int[] wallMatrixID;
    public int relativeX;
    public int relativeZ;
    public int relativeY;
    private bool wallDestroyed;

    public CellWallData(CellWall _wall, int _relativeX, int _relativeZ, int _relativeY = 0, int[] _wallMatrixID = null, bool _wallDestroyed = false)//, bool id = false)
    {
        wall = _wall;
        relativeX = _relativeX;
        relativeZ = _relativeZ;
        relativeY = _relativeY;
        wallMatrixID = new int[2];
        wallDestroyed = _wallDestroyed;
    }

    public void SetWallMatrixID(int[] _wallMatrixID)
    {
        if (wallMatrixID == null)
        {
            wallMatrixID = new int[2];
        }

        wallMatrixID[0] = _wallMatrixID[0];
        wallMatrixID[1] = _wallMatrixID[1];
    }

    public Vector3 GetWallPos()
    {
        if (wall != null)
        {
            Vector3 pos = wall.GetWallPosition();

            return new Vector3(pos.x, (pos.y - 5), pos.z);
        }
        else
        {
            Debug.LogError("That wall wa destroyed");
            return Vector3.zero;
        }
    }
    public int[] GetMatrixID() { return wallMatrixID; }
    public int GetMatrixX() { return wallMatrixID[0]; }
    public int GetMatrixZ() { return wallMatrixID[1]; }

    public void DisableWall()
    {
        wall.gameObject.SetActive(false);
    }

    public void DestroyWall()
    {
        wall.DestroyWallProps();
        wallDestroyed = true;
    }

    public bool WallIsDisabled() { return !wall.gameObject.activeSelf; }

    public bool WallIsDestroyed() { return wallDestroyed; }
    public void EnableWall() { wall.gameObject.SetActive(true); }
}
