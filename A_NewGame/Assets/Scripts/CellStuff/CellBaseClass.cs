using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CellBaseClass : MonoBehaviour
{
    public List<CellTypes> cellTypes;

    [SerializeField]
    protected Material _wallMaterial;

    public List<CellWallData> _xPosVerticleWalls;
    public List<CellWallData> _xNegVerticleWalls;
    public List<CellWallData> _zPosHorizontalWalls;
    public List<CellWallData> _zNegHorizontalWalls;

    [SerializeField]
    private Material _floorMaterial;
    private List<CellFloor> _floors;

    [SerializeField]
    private Material _ceelingMaterial;
    private List<CellCeeling> _ceelings;

    [SerializeField]
    protected List<GameObject> _doorWayPrefabs;
    protected List<GameObject> _doorWays;

    [SerializeField]
    protected List<GameObject> _entrancePrefabs;
    protected List<GameObject> _entrances;

    [SerializeField]
    private List<GameObject> _lights;

    [SerializeField]
    private GameObject _props;

    public int xWidth = 1;
    public int zHeight = 1;

    protected int[] _cellBaseIndex;
    public List<int[]> _cellDerivedIndexes;

    protected bool _hasWallIndexes = false;
    private bool _cellEnabled = true;

    private int _cellPrefabIndex = -1;

    private void Awake()
    {
        InitializeLists();
    }

    protected virtual void InitializeLists()
    {
        _doorWays = new List<GameObject>();
        _entrances = new List<GameObject>();
        _xPosVerticleWalls = new List<CellWallData>();
        _xNegVerticleWalls = new List<CellWallData>();
        _zPosHorizontalWalls = new List<CellWallData>();
        _zNegHorizontalWalls = new List<CellWallData>();
        _floors = new List<CellFloor>();
        _ceelings = new List<CellCeeling>();
        _cellDerivedIndexes = new List<int[]>();
    }

    private void Start()
    {
        if (DimensionsAreValid())
        {
            FindCellComponents();
            ReEndableCellComponents();
            ChangeCellComponentMaterials();
            ReOrganizeWallLists();
        }
    }
    protected virtual bool DimensionsAreValid()
    {
        if (xWidth <= 0 || zHeight <= 0)
        {
            Debug.LogError("Cell Dimensions aren't valid!!");
            return false;
        }

        return true;
    }

    private void FindCellComponents()
    {
        FindCellWalls();
        FindCellFloors();
        FindCellCeelings();        
    }

    private void ReEndableCellComponents()
    {
        ReEnableCellWalls();
        ReEnableCellFloors();
        ReEnableCellCeelings();
    }

    private void ChangeCellComponentMaterials()
    {
        ChangeCellWallMaterial();
        ChangeCellFloorMaterial();
        ChangeCellCeelingMaterial();
    }

    protected virtual void ReOrganizeWallLists()
    {
        ReOrganizePosZWallList();
        ReOrganizeNegZWallList();
        ReOrganizePosXWallLists();
        ReOrganizeNegXWallLists();
    }

    //  CELL WALLS
    protected virtual void FindCellWalls()
    {
        CellWall tempWall = null;
        CellWallData tempCellWallData;

        for (int i = 0; i < 2 * (xWidth + zHeight); i++)
        {
            tempWall = this.GetComponentInChildren<CellWall>();
            tempCellWallData = new CellWallData(tempWall, (int)tempWall.GetWallX(), (int)tempWall.GetWallZ(),0, null);

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
                    _zPosHorizontalWalls.Add(tempCellWallData);
                }
                else
                {
                    _zNegHorizontalWalls.Add(tempCellWallData);
                }
            }
            else
            {
                if (tempWall.GetWallX() > 0f)
                {
                    _xPosVerticleWalls.Add(tempCellWallData);
                }
                else
                {
                    _xNegVerticleWalls.Add(tempCellWallData);
                }
            }

            // disable game object so don't keep finding same one and adding to list
            tempWall.gameObject.SetActive(false);
        }
    }

    protected virtual void ReEnableCellWalls()
    {
        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            _zPosHorizontalWalls[i].wall.gameObject.SetActive(true);
        }

        for (int i = 0; i < _zNegHorizontalWalls.Count; i++)
        {
            _zNegHorizontalWalls[i].wall.gameObject.SetActive(true);
        }

        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            _xPosVerticleWalls[i].wall.gameObject.SetActive(true);
        }

        for (int i = 0; i < _xNegVerticleWalls.Count; i++)
        {
            _xNegVerticleWalls[i].wall.gameObject.SetActive(true);
        }
    }

    protected virtual void ChangeCellWallMaterial()
    {
        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            _zPosHorizontalWalls[i].wall.SetWallMaterial(_wallMaterial);
        }

        for (int i = 0; i < _zNegHorizontalWalls.Count; i++)
        {
            _zNegHorizontalWalls[i].wall.SetWallMaterial(_wallMaterial);
        }

        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            _xPosVerticleWalls[i].wall.SetWallMaterial(_wallMaterial);
        }

        for (int i = 0; i < _xNegVerticleWalls.Count; i++)
        {
            _xNegVerticleWalls[i].wall.SetWallMaterial(_wallMaterial);
        }
    }

    private void ReOrganizePosZWallList()
    {
        bool swapped = false;

        CellWallData tempCellWallData;

        for (int i = 0; i < _zPosHorizontalWalls.Count - 1; i++)
        {
            if (_zPosHorizontalWalls[i].relativeX > _zPosHorizontalWalls[i + 1].relativeX)
            {
                swapped = true;
                tempCellWallData = _zPosHorizontalWalls[i];
                _zPosHorizontalWalls[i] = _zPosHorizontalWalls[i + 1];
                _zPosHorizontalWalls[i + 1] = tempCellWallData;
            }
        }

        if (swapped)
            ReOrganizePosZWallList();
    }

    private void ReOrganizeNegZWallList()
    {
        bool swapped = false;

        CellWallData tempCellWallData;

        for (int i = 0; i < _zNegHorizontalWalls.Count - 1; i++)
        {
            if (_zNegHorizontalWalls[i].relativeX > _zNegHorizontalWalls[i + 1].relativeX)
            {
                swapped = true;
                tempCellWallData = _zNegHorizontalWalls[i];
                _zNegHorizontalWalls[i] = _zNegHorizontalWalls[i + 1];
                _zNegHorizontalWalls[i + 1] = tempCellWallData;
            }
        }

        if (swapped)
            ReOrganizeNegZWallList();
    }

    private void ReOrganizePosXWallLists()
    {
        bool swapped = false;

        CellWallData tempCellWallData;

        for (int i = 0; i < _xPosVerticleWalls.Count - 1; i++)
        {
            if (_xPosVerticleWalls[i].relativeZ > _xPosVerticleWalls[i + 1].relativeZ)
            {
                swapped = true;
                tempCellWallData = _xPosVerticleWalls[i];
                _xPosVerticleWalls[i] = _xPosVerticleWalls[i + 1];
                _xPosVerticleWalls[i + 1] = tempCellWallData;
            }
        }

        if (swapped)
            ReOrganizePosXWallLists();
    }

    private void ReOrganizeNegXWallLists()
    {
        bool swapped = false;

        CellWallData tempCellWallData;

        for (int i = 0; i < _xNegVerticleWalls.Count - 1; i++)
        {
            if (_xNegVerticleWalls[i].relativeZ > _xNegVerticleWalls[i + 1].relativeZ)
            {
                swapped = true;
                tempCellWallData = _xNegVerticleWalls[i];
                _xNegVerticleWalls[i] = _xNegVerticleWalls[i + 1];
                _xNegVerticleWalls[i + 1] = tempCellWallData;
            }
        }

        if (swapped)
            ReOrganizeNegXWallLists();
    }

    //      CELL FLOORS
    private void FindCellFloors()
    {
        CellFloor tempFloor;

        for (int i = 0; i < (xWidth * zHeight); i++)
        {            
            tempFloor = this.GetComponentInChildren<CellFloor>();
            _floors.Add(tempFloor);
            tempFloor.gameObject.SetActive(false);         
        }
    }

    private void ReEnableCellFloors()
    {
        for (int i = 0; i < _floors.Count; i++)
        {
            _floors[i].gameObject.SetActive(true);
        }
    }

    private void ChangeCellFloorMaterial()
    {
        for (int i = 0; i < _floors.Count; i++)
        {
            _floors[i].ChangeFloorMat(_floorMaterial);
        }
    }

    //      CELL CEELINGS
    private void FindCellCeelings()
    {
        CellCeeling tempCeeling;

        for (int i = 0; i < (xWidth * zHeight); i++)
        {
            tempCeeling = this.GetComponentInChildren<CellCeeling>();
            _ceelings.Add(tempCeeling);
            tempCeeling.gameObject.SetActive(false);
        }
    }

    private void ReEnableCellCeelings()
    {
        for (int i = 0; i < _ceelings.Count; i++)
        {
            _ceelings[i].gameObject.SetActive(true);
        }
    }

    private void ChangeCellCeelingMaterial()
    {
        for (int i = 0; i < _ceelings.Count; i++)
        {
            _ceelings[i].ChangeCeelingMat(_floorMaterial);
        }
    }

    //      DISABLE CELL
    public virtual void DisableCell()
    {
        DisableCellWalls();
        DisableCellFloors();
        DisableCellCeelings();
        DisableCellDoorways();
        DisableCellEntrances();
        DisableCellLights();
        DisableCellProps();
        _cellEnabled = false;
    }

    public virtual void DisableCellWalls()
    {
        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            if (!_zPosHorizontalWalls[i].WallIsDestroyed())
                _zPosHorizontalWalls[i].wall.DisableWall();
        }
        for (int i = 0; i < _zNegHorizontalWalls.Count; i++)
        {
            if (!_zNegHorizontalWalls[i].WallIsDestroyed())
                _zNegHorizontalWalls[i].wall.DisableWall();
        }
        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            if (!_xPosVerticleWalls[i].WallIsDestroyed())
                _xPosVerticleWalls[i].wall.DisableWall();
        }
        for (int i = 0; i < _xNegVerticleWalls.Count; i++)
        {
            if (!_xNegVerticleWalls[i].WallIsDestroyed())
                _xNegVerticleWalls[i].wall.DisableWall();
        }
    }

    public void DisableCellFloors()
    {
        for (int i =0; i < _floors.Count; i++)
        {
            _floors[i].DisableFloor();
        }
    }

    private void DisableCellCeelings()
    {
        for (int i = 0; i < _ceelings.Count; i++)
        {
            _ceelings[i].DisableCeeling();
        }
    }

    private void DisableCellDoorways()
    {
        for (int i = 0; i < _doorWays.Count; i++)
        {
            if (_doorWays[i] != null)
            {
                _doorWays[i].GetComponent<DoorSpawnPoint>().DisableDoor();
                _doorWays[i].SetActive(false);
            }               
        }
    }

    private void DisableCellEntrances()
    {
        for (int i = 0; i < _entrances.Count;i++)
        {
            if (_entrances[i] != null)
                _entrances[i].SetActive(false);
        }
    }

    private void DisableCellLights()
    {
        for (int i = 0; i < _lights.Count;i++)
        {
            if (_lights[i] != null)
            {
                _lights[i].GetComponent<LightSpawnPoint>().DisableLight();
                _lights[i].SetActive(false);
            }
        }
    }

    private void DisableCellProps()
    {
        if (_props != null)
            _props.SetActive(false);
    }

    //      DISABLE CELL WALLS
    public virtual void DisablePosZWalls()
    {
        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            _zPosHorizontalWalls[i].wall.gameObject.SetActive(false);
        }
    }

    public virtual void DisableNegZWalls()
    {
        for (int i = 0; i < _zNegHorizontalWalls.Count; i++)
        {
            _zNegHorizontalWalls[i].wall.gameObject.SetActive(false);
        }
    }

    public virtual void DisablePosXWalls()
    {
        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            _xPosVerticleWalls[i].wall.gameObject.SetActive(false);
        }
    }

    public virtual void DisableNegXWalls()
    {
        for (int i = 0; i < _xNegVerticleWalls.Count; i++)
        {
            _xNegVerticleWalls[i].wall.gameObject.SetActive(false);
        }
    }

    //      ENABLE CELL
    public virtual void EnableCell()
    {
        EnableCellWalls();
        EnableCellFloors();
        EnableCellCeelings();
        EnableCellDoorways();
        EnableCellEntrances();
        EnableCellLights();
        EnableCellProps();
        _cellEnabled = true;
    }

    public virtual void EnableCellWalls()
    {
        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            if (!_zPosHorizontalWalls[i].WallIsDestroyed())
                _zPosHorizontalWalls[i].wall.EnableWall();
        }
        for (int i = 0; i < _zNegHorizontalWalls.Count; i++)
        {
            if (!_zNegHorizontalWalls[i].WallIsDestroyed())
                _zNegHorizontalWalls[i].wall.EnableWall();
        }
        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            if (!_xPosVerticleWalls[i].WallIsDestroyed())
                _xPosVerticleWalls[i].wall.EnableWall();
        }
        for (int i = 0; i < _xNegVerticleWalls.Count; i++)
        {
            if (!_xNegVerticleWalls[i].WallIsDestroyed())
                _xNegVerticleWalls[i].wall.EnableWall();
        }
    }

    public void EnableCellFloors()
    {
        for (int i = 0; i < _floors.Count; i++)
        {
            _floors[i].EnableFloor();
        }
    }

    private void EnableCellCeelings()
    {
        for (int i = 0; i < _ceelings.Count; i++)
        {
            _ceelings[i].EnableCeeling();
        }
    }

    private void EnableCellDoorways()
    {
        for (int i = 0; i < _doorWays.Count; i++)
        {
            if (_doorWays[i] != null)           
                _doorWays[i].SetActive(true);
            
        }
    }

    private void EnableCellEntrances()
    {
        for (int i = 0; i < _entrances.Count; i++)
        {
            if (_entrances[i] != null)
                _entrances[i].SetActive(true);
        }
    }

    private void EnableCellLights()
    {
        for (int i = 0; i < _lights.Count; i++)
        {
            if (_lights[i] != null)           
                _lights[i].SetActive(true);          
        }
    }

    private void EnableCellProps()
    {
        if (_props != null)
            _props.SetActive(true);
    }

    //      CELL MATRIX DATA
    public virtual void SetCellBaseIndex(int x, int z)
    {
        _cellBaseIndex = new int[2];
        _cellBaseIndex[0] = x;
        _cellBaseIndex[1] = z;
    }

    public void SetCellDerivedIndex(int x, int z)
    {
        int[] tempVal = new int[2];
        tempVal[0] = x;
        tempVal[1] = z;
        _cellDerivedIndexes.Add(tempVal);
    }

    public virtual void SetCellWallIndexes(int baseX, int baseZ)
    {
        if (_hasWallIndexes == true)
            return;
        else      
            _hasWallIndexes = true;
        

        int[] tempID = null;

        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            tempID = new int[2];
            tempID[0] = baseX + i;          // x
            tempID[1] = baseZ + zHeight;    // z
            _zPosHorizontalWalls[i].SetWallMatrixID(tempID);

        }

        for (int i = 0; i < _zNegHorizontalWalls.Count; i++)
        {
            tempID = new int[2];
            tempID[0] = baseX + i;
            tempID[1] = baseZ - 1;
            _zNegHorizontalWalls[i].SetWallMatrixID(tempID);
        }

        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            tempID = new int[2];
            tempID[0] = baseX + xWidth;
            tempID[1] = baseZ + i;
            _xPosVerticleWalls[i].SetWallMatrixID(tempID);
        }

        for (int i = 0; i < _xNegVerticleWalls.Count; i++)
        {
            tempID = new int[2];
            tempID[0] = baseX - 1;
            tempID[1] = baseZ + i;
            _xNegVerticleWalls[i].SetWallMatrixID(tempID);
        }
    }

    public void SetCellPrefabIndex(int index)
    {
        _cellPrefabIndex = index;
    }

    //      DESTROY CELL WALLS
    public void DestroySpecificPosZWall(int index, bool addDorway = false, bool addEntrance = false)
    {
        if (index +1 > _zPosHorizontalWalls.Count)
        {
            Debug.LogError("Wrong function called for multi floor cell!!");
            return;
        }
        if (addDorway && !_zPosHorizontalWalls[index].WallIsDestroyed())
        {
            Vector3 newObjectPos = _zPosHorizontalWalls[index].GetWallPos();
            GameObject newObject = null;

            if (addEntrance)
                addEntrance = !OneInXChances(5);

            if ((!addEntrance || _entrancePrefabs.Count <= 0) && _doorWayPrefabs.Count > 0)
            {
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

        CellWallData wallData = _zPosHorizontalWalls[index];
        wallData.DestroyWall();
        Destroy(wallData.wall.gameObject);
        _zPosHorizontalWalls[index] = wallData;
    }

    public void DestroySpecificNegZWall(int index, bool addDorway = false, bool addEntrance = false)
    {
        if (index + 1 > _zNegHorizontalWalls.Count)
        {
            Debug.LogError("Wrong function called for multi floor cell!!");
            return;
        }
        if (addDorway && !_zNegHorizontalWalls[index].WallIsDestroyed())
        {
            Vector3 newObjectPos = _zNegHorizontalWalls[index].GetWallPos();
            GameObject newObject = null;

            if (addEntrance)
                addEntrance = !OneInXChances(5);

            if ((!addEntrance || _entrancePrefabs.Count <= 0) && _doorWayPrefabs.Count > 0)
            {
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

        CellWallData wallData = _zNegHorizontalWalls[index];
        wallData.DestroyWall();
        Destroy(wallData.wall.gameObject);
        _zNegHorizontalWalls[index] = wallData;
    }

    public void DestroySpecificPosXWall(int index, bool addDorway = false, bool addEntrance = false)
    {
        if (index + 1 > _xPosVerticleWalls.Count)
        {
            Debug.LogError("Wrong function called for multi floor cell!!");
            return;
        }
        if (addDorway && !_xPosVerticleWalls[index].WallIsDestroyed())
        {
            Vector3 newObjectPos = _xPosVerticleWalls[index].GetWallPos();
            GameObject newObject = null;

            if (addEntrance)
                addEntrance = !OneInXChances(5);

            if ((!addEntrance || _entrancePrefabs.Count <= 0) && _doorWayPrefabs.Count > 0)
            {
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


        CellWallData wallData = _xPosVerticleWalls[index];
        wallData.DestroyWall();
        Destroy(wallData.wall.gameObject);
        _xPosVerticleWalls[index] = wallData;
    }

    public void DestroySpecificNegXWall(int index, bool addDorway = false, bool addEntrance = false)
    {
        if (index + 1 > _xNegVerticleWalls.Count)
        {
            Debug.LogError("Wrong function called for multi floor cell!!");
            return;
        }
        if (addDorway && !_xNegVerticleWalls[index].WallIsDestroyed())
        {
            Vector3 newObjectPos = _xNegVerticleWalls[index].GetWallPos();
            GameObject newObject = null;

            if (addEntrance)
                addEntrance = !OneInXChances(5);

            if ((!addEntrance || _entrancePrefabs.Count <= 0) && _doorWayPrefabs.Count > 0)
            {
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

        CellWallData wallData = _xNegVerticleWalls[index];
        wallData.DestroyWall();
        Destroy(wallData.wall.gameObject);
        _xNegVerticleWalls[index] = wallData;
    }

    //      GET CELL DATA
    public int GetCellXWidth()
    {
        return xWidth;
    }

    public int GetCellZHeight()
    {
        return zHeight;
    }

    public virtual int GetCellYFloors()
    {
        return 1;
    }

    public bool CellIsEnabled()
    {
        return _cellEnabled;
    }

    public List<int[]> GetCellDerivedIndexs()
    {
        if (_cellDerivedIndexes == null)
            return new List<int[]>();

        return _cellDerivedIndexes;
    }

    public int[] GetCellBaseIndex()
    {
        if (_cellBaseIndex == null)     
            return null;      

        return _cellBaseIndex;
    }

    public bool HasDeadCells()
    {
        for (int i = 0; i < cellTypes.Count; i ++)
        {
            if (cellTypes[i] == CellTypes.dead)
                return true;
        }
        return false;
    }

    public bool CellIsTransitional()
    {
        for (int i = 0; i < cellTypes.Count; i++)
        {
            if (cellTypes[i] == CellTypes.transitional)
                return true;
        }
        return false;
    }

    public bool CellHasEnemy()
    {
        for (int i = 0; i < cellTypes.Count; i++)
        {
            if (cellTypes[i] == CellTypes.enemy)
                return true;
        }
        return false;
    }

    public bool CellIsOfType(CellTypes type)
    {
        for (int i = 0; i < cellTypes.Count; i++)
        {
            if (cellTypes[i] == type)
                return true;
        }
        return false;
    }

    public List<CellTypes> GetCellTypes()
    {
        return cellTypes;
    }

    public int GetCellPrefabIndex()
    {
        return _cellPrefabIndex;
    }

    //      UTILITY
    protected int GetRandomNum(int max, int min = 0)
    {
        if (min < 0 || min > max)
            min = 0;
        return Random.Range(min, max);
    }

    protected bool OneInXChances(int num)
    {
        return Random.Range(0, num) == 0;
    }
}