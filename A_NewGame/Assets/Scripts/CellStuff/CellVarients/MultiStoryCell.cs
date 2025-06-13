using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MultiStoryCell : CellBaseClass
{
    public int yFloors = 1;

    public List<List<CellWallData>> _xPosMultiVerticleWalls;
    public List<List<CellWallData>> _xNegMultiVerticleWalls;
    public List<List<CellWallData>> _zPosMultiHorizontalWalls;
    public List<List<CellWallData>> _zNegMultiHorizontalWalls;

    protected override void InitializeLists()
    {
        base.InitializeLists();

        _xPosMultiVerticleWalls = new List<List<CellWallData>>();
        _xNegMultiVerticleWalls = new List<List<CellWallData>>();
        _zPosMultiHorizontalWalls = new List<List<CellWallData>>();
        _zNegMultiHorizontalWalls = new List<List<CellWallData>>();

        for (int i = 0; i < yFloors; i ++)
        {
            _xPosMultiVerticleWalls.Add(new List<CellWallData>());
            _xNegMultiVerticleWalls.Add(new List<CellWallData>());
            _zPosMultiHorizontalWalls.Add(new List<CellWallData>());
            _zNegMultiHorizontalWalls.Add(new List<CellWallData>()); 
        }
    }

    protected override bool DimensionsAreValid()
    {
        if (xWidth <= 0 || zHeight <= 0 || yFloors <= 0)
        {
            Debug.LogError("Cell Dimensions aren't valid!!");
            return false;
        }

        return true;
    }

    //      FIND CELL COMPONENTS

    protected override void FindCellWalls()
    {
        CellWall tempWall = null;
        CellWallData tempCellWallData;

        int yFloorIndex = -1;

        for (int i = 0; i < 2 * (xWidth + zHeight) * yFloors; i++)
        {
            tempWall = this.GetComponentInChildren<CellWall>();
            tempCellWallData = new CellWallData(tempWall, (int)tempWall.GetWallX(), (int)tempWall.GetWallZ(), ((int)tempWall.GetWallY() - 5), null);

            yFloorIndex = ((int)tempWall.GetWallY() - 5) / 10;

            // tempWall.GetWallX() returns local X position of wall
            // walls along x axis have x value incremented by 5 each time
            // walls along z axis have x value incremented by 10 each time
            // if no remainder from ...%10 then it must be on z axis
            if (((float)tempWall.GetWallX() % 10.0f) == 0f)
            {
                // if wall local z position is > 0 then its at the "front"
                // i.e. it's above the cells center so add to positive Z walls
                if (tempWall.GetWallZ() > 0f)
                {
                    _zPosMultiHorizontalWalls[yFloorIndex].Add(tempCellWallData);
                    //_zPosHorizontalWalls.Add(tempCellWallData);
                }
                else
                {
                    _zNegMultiHorizontalWalls[yFloorIndex].Add(tempCellWallData);
                    //_zNegHorizontalWalls.Add(tempCellWallData);
                }
            }
            else
            {
                if (tempWall.GetWallX() > 0f)
                {
                    _xPosMultiVerticleWalls[yFloorIndex].Add(tempCellWallData);
                    //_xPosVerticleWalls.Add(tempCellWallData);
                }
                else
                {
                    _xNegMultiVerticleWalls[yFloorIndex].Add(tempCellWallData);
                    //_xNegVerticleWalls.Add(tempCellWallData);
                }
            }

            // disable game object so don't keep finding same one and adding to list
            tempWall.gameObject.SetActive(false);
        }
    }

    protected override void ReEnableCellWalls()
    {
        for (int j =0; j < yFloors; j++)
        {
            for (int i = 0; i < _zPosMultiHorizontalWalls[j].Count; i++)
            {
                _zPosMultiHorizontalWalls[j][i].wall.gameObject.SetActive(true);
            }

            for (int i = 0; i < _zNegMultiHorizontalWalls[j].Count; i++)
            {
                _zNegMultiHorizontalWalls[j][i].wall.gameObject.SetActive(true);
            }

            for (int i = 0; i < _xPosMultiVerticleWalls[j].Count; i++)
            {
                _xPosMultiVerticleWalls[j][i].wall.gameObject.SetActive(true);
            }

            for (int i = 0; i < _xNegMultiVerticleWalls[j].Count; i++)
            {
                _xNegMultiVerticleWalls[j][i].wall.gameObject.SetActive(true);
            }
        }
        
    }

    //      CHANGE WALL MATERIAL
    protected override void ChangeCellWallMaterial()
    {
        if (_wallMaterial == null)
        {
            Debug.Log("No multi wall material!!");
            return;
        }
        for (int j = 0; j < yFloors; j++)
        {
            for (int i = 0; i < _zPosMultiHorizontalWalls[j].Count; i++)
            {
                _zPosMultiHorizontalWalls[j][i].wall.SetWallMaterial(_wallMaterial);
            }

            for (int i = 0; i < _zNegMultiHorizontalWalls[j].Count; i++)
            {
                _zNegMultiHorizontalWalls[j][i].wall.SetWallMaterial(_wallMaterial);
            }

            for (int i = 0; i < _xPosMultiVerticleWalls[j].Count; i++)
            {
                _xPosMultiVerticleWalls[j][i].wall.SetWallMaterial(_wallMaterial);
            }

            for (int i = 0; i < _xNegMultiVerticleWalls[j].Count; i++)
            {
                _xNegMultiVerticleWalls[j][i].wall.SetWallMaterial(_wallMaterial);
            }
        }
    }

    //      REORGANIZE WALLS
    protected override void ReOrganizeWallLists()
    {
        for (int i = 0; i < yFloors; i++)
        {
            ReOrganizePosZWallList(i);
            ReOrganizeNegZWallList(i);
            ReOrganizePosXWallList(i);
            ReOrganizeNegXWallList(i);
        }
    }


    private void ReOrganizePosZWallList(int yFloorIndex)
    {
        bool swapped = false;

        CellWallData tempCellWallData;

        for (int i = 0; i < _zPosMultiHorizontalWalls[yFloorIndex].Count - 1; i++)
        {
            if (_zPosMultiHorizontalWalls[yFloorIndex][i].relativeX > _zPosMultiHorizontalWalls[yFloorIndex][i + 1].relativeX)
            {
                swapped = true;
                tempCellWallData = _zPosMultiHorizontalWalls[yFloorIndex][i];
                _zPosMultiHorizontalWalls[yFloorIndex][i] = _zPosMultiHorizontalWalls[yFloorIndex][i + 1];
                _zPosMultiHorizontalWalls[yFloorIndex][i + 1] = tempCellWallData;
            }
        }

        if (swapped)
            ReOrganizePosZWallList(yFloorIndex);
    }

    private void ReOrganizeNegZWallList(int yFloorIndex)
    {
        bool swapped = false;

        CellWallData tempCellWallData;

        for (int i = 0; i < _zNegMultiHorizontalWalls[yFloorIndex].Count - 1; i++)
        {
            if (_zNegMultiHorizontalWalls[yFloorIndex][i].relativeX > _zNegMultiHorizontalWalls[yFloorIndex][i + 1].relativeX)
            {
                swapped = true;
                tempCellWallData = _zNegMultiHorizontalWalls[yFloorIndex][i];
                _zNegMultiHorizontalWalls[yFloorIndex][i] = _zNegMultiHorizontalWalls[yFloorIndex][i + 1];
                _zNegMultiHorizontalWalls[yFloorIndex][i + 1] = tempCellWallData;
            }
        }

        if (swapped)
            ReOrganizeNegZWallList(yFloorIndex);
    }

    private void ReOrganizePosXWallList(int yFloorIndex)
    {
        bool swapped = false;

        CellWallData tempCellWallData;

        for (int i = 0; i < _xPosMultiVerticleWalls[yFloorIndex].Count - 1; i++)
        {
            if (_xPosMultiVerticleWalls[yFloorIndex][i].relativeZ > _xPosMultiVerticleWalls[yFloorIndex][i + 1].relativeZ)
            {
                swapped = true;
                tempCellWallData = _xPosMultiVerticleWalls[yFloorIndex][i];
                _xPosMultiVerticleWalls[yFloorIndex][i] = _xPosMultiVerticleWalls[yFloorIndex][i + 1];
                _xPosMultiVerticleWalls[yFloorIndex][i + 1] = tempCellWallData;
            }
        }

        if (swapped)
            ReOrganizePosXWallList(yFloorIndex);
    }

    private void ReOrganizeNegXWallList(int yFloorIndex)
    {
        bool swapped = false;

        CellWallData tempCellWallData;

        for (int i = 0; i < _xNegMultiVerticleWalls[yFloorIndex].Count - 1; i++)
        {
            if (_xNegMultiVerticleWalls[yFloorIndex][i].relativeZ > _xNegMultiVerticleWalls[yFloorIndex][i + 1].relativeZ)
            {
                swapped = true;
                tempCellWallData = _xNegMultiVerticleWalls[yFloorIndex][i];
                _xNegMultiVerticleWalls[yFloorIndex][i] = _xNegMultiVerticleWalls[yFloorIndex][i + 1];
                _xNegMultiVerticleWalls[yFloorIndex][i + 1] = tempCellWallData;
            }
        }

        if (swapped)
            ReOrganizeNegXWallList(yFloorIndex);
    }

    //      DISABLE CELL
    public override void DisableCellWalls()
    {
        for (int j = 0; j < yFloors; j ++)
        {
            for (int i = 0; i < _zPosMultiHorizontalWalls[j].Count; i++)
            {
                if (!_zPosMultiHorizontalWalls[j][i].WallIsDestroyed())
                    _zPosMultiHorizontalWalls[j][i].wall.DisableWall();
            }
            for (int i = 0; i < _zNegMultiHorizontalWalls[j].Count; i++)
            {
                if (!_zNegMultiHorizontalWalls[j][i].WallIsDestroyed())
                    _zNegMultiHorizontalWalls[j][i].wall.DisableWall();
            }
            for (int i = 0; i < _xPosMultiVerticleWalls[j].Count; i++)
            {
                if (!_xPosMultiVerticleWalls[j][i].WallIsDestroyed())
                    _xPosMultiVerticleWalls[j][i].wall.DisableWall();
            }
            for (int i = 0; i < _xNegMultiVerticleWalls[j].Count; i++)
            {
                if (!_xNegMultiVerticleWalls[j][i].WallIsDestroyed())
                    _xNegMultiVerticleWalls[j][i].wall.DisableWall();
            }
        }
    }

    //      DISABLE SPECIFIC CELL WALLS

    public override void DisablePosZWalls()
    {
        for (int j = 0; j < yFloors; j++)
        {
            for (int i = 0; i < _zPosMultiHorizontalWalls[j].Count; i++)
            {
                _zPosMultiHorizontalWalls[j][i].wall.gameObject.SetActive(false);
            }
        }
    }

    public override void DisableNegZWalls()
    {
        for (int j = 0; j < yFloors; j++)
        {
            for (int i = 0; i < _zNegMultiHorizontalWalls[j].Count; i++)
            {
                _zNegMultiHorizontalWalls[j][i].wall.gameObject.SetActive(false);
            }
        }
    }

    public override void DisablePosXWalls()
    {
        for (int j = 0; j < yFloors; j++)
        {
            for (int i = 0; i < _xPosMultiVerticleWalls[j].Count; i++)
            {
                _xPosMultiVerticleWalls[j][i].wall.gameObject.SetActive(false);
            }
        }
    }

    public override void DisableNegXWalls()
    {
        for (int j = 0; j < yFloors; j++)
        {
            for (int i = 0; i < _xNegMultiVerticleWalls[j].Count; i++)
            {
                _xNegMultiVerticleWalls[j][i].wall.gameObject.SetActive(false);
            }
        }
    }

    //      ENABLE CELL
    public override void EnableCellWalls()
    {
        for (int j = 0; j < yFloors; j++)
        {
            for (int i = 0; i < _zPosMultiHorizontalWalls[j].Count; i++)
            {
                if (!_zPosMultiHorizontalWalls[j][i].WallIsDestroyed())
                    _zPosMultiHorizontalWalls[j][i].wall.EnableWall();
            }
            for (int i = 0; i < _zNegMultiHorizontalWalls[j].Count; i++)
            {
                if (!_zNegMultiHorizontalWalls[j][i].WallIsDestroyed())
                    _zNegMultiHorizontalWalls[j][i].wall.EnableWall();
            }
            for (int i = 0; i < _xPosMultiVerticleWalls[j].Count; i++)
            {
                if (!_xPosMultiVerticleWalls[j][i].WallIsDestroyed())
                    _xPosMultiVerticleWalls[j][i].wall.EnableWall();
            }
            for (int i = 0; i < _xNegMultiVerticleWalls[j].Count; i++)
            {
                if (!_xNegMultiVerticleWalls[j][i].WallIsDestroyed())
                    _xNegMultiVerticleWalls[j][i].wall.EnableWall();
            }
        }
    }

    //      SET CELL MATRIX DATA
    public override void SetCellWallIndexes(int baseX, int baseZ)
    {
        if (_hasWallIndexes == true)
            return;
        else
            _hasWallIndexes = true;


        int[] tempID = null;

        for (int i = 0; i < _zPosMultiHorizontalWalls[0].Count; i++)
        {
            tempID = new int[2];
            tempID[0] = baseX + i;          // x
            tempID[1] = baseZ + zHeight;    // z

            for (int j = 0; j < yFloors; j++)
            {
                _zPosMultiHorizontalWalls[j][i].SetWallMatrixID(tempID);
            }
        }

        for (int i = 0; i < _zNegMultiHorizontalWalls[0].Count; i++)
        {
            tempID = new int[2];
            tempID[0] = baseX + i;
            tempID[1] = baseZ - 1;

            for (int j = 0; j < yFloors; j++)
            {
                _zNegMultiHorizontalWalls[j][i].SetWallMatrixID(tempID);
            }
        }

        for (int i = 0; i < _xPosMultiVerticleWalls[0].Count; i++)
        {
            tempID = new int[2];
            tempID[0] = baseX + xWidth;
            tempID[1] = baseZ + i;

            for (int j = 0; j < yFloors; j++)
            {
                _xPosMultiVerticleWalls[j][i].SetWallMatrixID(tempID);
            }
        }

        for (int i = 0; i < _xNegMultiVerticleWalls[0].Count; i++)
        {
            tempID = new int[2];
            tempID[0] = baseX - 1;
            tempID[1] = baseZ + i;

            for (int j = 0; j < yFloors; j++)
            {
                _xNegMultiVerticleWalls[j][i].SetWallMatrixID(tempID);
            }
        }
    }

    //      DESTROY CELL WALLS
    public void DestroySpecificPosZMultiWall(int index, bool addDorway = false, bool addEntrance = false)
    {
        if (addDorway && !_zPosMultiHorizontalWalls[0][index].WallIsDestroyed())
        {
            Vector3 newObjectPos = _zPosMultiHorizontalWalls[0][index].GetWallPos();
            GameObject newObject = null;

            //if (addEntrance)
            //    addEntrance = !OneInXChances(5);

            if ((!addEntrance || _entrancePrefabs.Count <= 0) && _doorWayPrefabs.Count > 0)
            {
                addEntrance = false;
                newObject = Instantiate(_doorWayPrefabs[GetRandomNum(0, _doorWayPrefabs.Count)], this.transform);
                newObject.GetComponent<DoorSpawnPoint>().DoorWayHasDoor(true);
                _doorWays.Add(newObject);
            }
            else if (_entrancePrefabs.Count > 0)
            {
                newObject = Instantiate(_entrancePrefabs[GetRandomNum(0, _entrancePrefabs.Count)], this.transform);
                _entrances.Add(newObject);
            }

            if (newObject != null)
            {
                newObject.transform.position = newObjectPos;
                newObject.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }

        CellWallData wallData;

        if (addEntrance)
        {
            for (int i = 0; i < yFloors; i++)
            {
                wallData = _zPosMultiHorizontalWalls[i][index];
                wallData.DestroyWall();
                Destroy(wallData.wall.gameObject);
                _zPosMultiHorizontalWalls[i][index] = wallData;
            }
        }
        else
        {
            wallData = _zPosMultiHorizontalWalls[0][index];
            wallData.DestroyWall();
            Destroy(wallData.wall.gameObject);
            _zPosMultiHorizontalWalls[0][index] = wallData;
        }
    }

    public void DestroySpecificNegZMultiWall(int index, bool addDorway = false, bool addEntrance = false)
    {
        if (addDorway && !_zNegMultiHorizontalWalls[0][index].WallIsDestroyed())
        {
            Vector3 newObjectPos = _zNegMultiHorizontalWalls[0][index].GetWallPos();
            GameObject newObject = null;

            //if (addEntrance)
            //    addEntrance = !OneInXChances(5);

            if ((!addEntrance || _entrancePrefabs.Count <= 0) && _doorWayPrefabs.Count > 0)
            {
                addEntrance = false;
                newObject = Instantiate(_doorWayPrefabs[GetRandomNum(0, _doorWayPrefabs.Count)], this.transform);
                newObject.GetComponent<DoorSpawnPoint>().DoorWayHasDoor(true);
                _doorWays.Add(newObject);
            }
            else if (_entrancePrefabs.Count > 0)
            {
                newObject = Instantiate(_entrancePrefabs[GetRandomNum(0, _entrancePrefabs.Count)], this.transform);
                _entrances.Add(newObject);
            }

            if (newObject != null)
            {
                newObject.transform.position = newObjectPos;
                newObject.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }

        CellWallData wallData;

        if (addEntrance)
        {
            for (int i = 0; i < yFloors; i++)
            {
                wallData = _zNegMultiHorizontalWalls[i][index];
                wallData.DestroyWall();
                Destroy(wallData.wall.gameObject);
                _zNegMultiHorizontalWalls[i][index] = wallData;
            }
        }
        else
        {
            wallData = _zNegMultiHorizontalWalls[0][index];
            wallData.DestroyWall();
            Destroy(wallData.wall.gameObject);
            _zNegMultiHorizontalWalls[0][index] = wallData;
        }
    }

    public void DestroySpecificPosXMultiWall(int index, bool addDorway = false, bool addEntrance = false)
    {
        if (addDorway && !_xPosMultiVerticleWalls[0][index].WallIsDestroyed())
        {
            Vector3 newObjectPos = _xPosMultiVerticleWalls[0][index].GetWallPos();
            GameObject newObject = null;

            //if (addEntrance)
            //    addEntrance = !OneInXChances(5);

            if ((!addEntrance || _entrancePrefabs.Count <= 0) && _doorWayPrefabs.Count > 0)
            {
                addEntrance = false;
                newObject = Instantiate(_doorWayPrefabs[GetRandomNum(0, _doorWayPrefabs.Count)], this.transform);
                newObject.GetComponent<DoorSpawnPoint>().DoorWayHasDoor(true);
                _doorWays.Add(newObject);
            }
            else if (_entrancePrefabs.Count > 0)
            {
                newObject = Instantiate(_entrancePrefabs[GetRandomNum(0, _entrancePrefabs.Count)], this.transform);
                _entrances.Add(newObject);
            }

            if (newObject != null)
            {
                newObject.transform.position = newObjectPos;
                newObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        CellWallData wallData;

        if (addEntrance)
        {
            for (int i = 0; i < yFloors; i++)
            {
                wallData = _xPosMultiVerticleWalls[i][index];
                wallData.DestroyWall();
                Destroy(wallData.wall.gameObject);
                _xPosMultiVerticleWalls[i][index] = wallData;
            }
        }
        else
        {
            wallData = _xPosMultiVerticleWalls[0][index];
            wallData.DestroyWall();
            Destroy(wallData.wall.gameObject);
            _xPosMultiVerticleWalls[0][index] = wallData;
        }
    }

    public void DestroySpecificNegXMultiWall(int index, bool addDorway = false, bool addEntrance = false)
    {
        if (addDorway && !_xNegMultiVerticleWalls[0][index].WallIsDestroyed())
        {
            Vector3 newObjectPos = _xNegMultiVerticleWalls[0][index].GetWallPos();
            GameObject newObject = null;

            //if (addEntrance)
            //    addEntrance = !OneInXChances(5);

            if ((!addEntrance || _entrancePrefabs.Count <= 0) && _doorWayPrefabs.Count > 0)
            {
                addEntrance = false;
                newObject = Instantiate(_doorWayPrefabs[GetRandomNum(0, _doorWayPrefabs.Count)], this.transform);
                newObject.GetComponent<DoorSpawnPoint>().DoorWayHasDoor(true);
                _doorWays.Add(newObject);
            }
            else if (_entrancePrefabs.Count > 0)
            {
                newObject = Instantiate(_entrancePrefabs[GetRandomNum(0, _entrancePrefabs.Count)], this.transform);
                _entrances.Add(newObject);
            }

            if (newObject != null)
            {
                newObject.transform.position = newObjectPos;
                newObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        CellWallData wallData;

        if (addEntrance)
        {
            for (int i = 0; i < yFloors; i++)
            {
                wallData = _xNegMultiVerticleWalls[i][index];
                wallData.DestroyWall();
                Destroy(wallData.wall.gameObject);
                _xNegMultiVerticleWalls[i][index] = wallData;
            }
        }
        else
        {
            wallData = _xNegMultiVerticleWalls[0][index];
            wallData.DestroyWall();
            Destroy(wallData.wall.gameObject);
            _xNegMultiVerticleWalls[0][index] = wallData;
        }
    }

    // GET DATA
    public override int GetCellYFloors()
    {
        return yFloors;
    }

    public override int GetNumPosZWalls()
    {
        return _zPosMultiHorizontalWalls[0].Count;
    }
    public override int[] GetSpecificPosZWallMatrixID(int i)
    {
        if (i < 0 || i >= _zPosMultiHorizontalWalls[0].Count)
            return null;

        return _zPosMultiHorizontalWalls[0][i].GetMatrixID();
    }

    public override int GetNumNegZWalls()
    {
        return _zNegMultiHorizontalWalls[0].Count;
    }
    public override int[] GetSpecificNegZWallMatrixID(int i)
    {
        if (i < 0 || i >= _zNegMultiHorizontalWalls[0].Count)
            return null;

        return _zNegMultiHorizontalWalls[0][i].GetMatrixID();
    }

    public override int GetNumPosXWalls()
    {
        return _xPosMultiVerticleWalls[0].Count;
    }

    public override int[] GetSpecificPosXWallMatrixID(int i)
    {
        if (i < 0 || i >= _xPosMultiVerticleWalls[0].Count)
            return null;

        return _xPosMultiVerticleWalls[0][i].GetMatrixID();
    }

    public override int GetNumNegXWalls()
    {
        return _xNegMultiVerticleWalls[0].Count;
    }

    public override int[] GetSpecificNegXWallMatrixID(int i)
    {
        if (i < 0 || i >= _xNegMultiVerticleWalls[0].Count)
            return null;

        return _xNegMultiVerticleWalls[0][i].GetMatrixID();
    }
}
