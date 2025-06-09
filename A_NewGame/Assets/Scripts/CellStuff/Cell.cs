//using System.Diagnostics;
//using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
//using UnityEngine.ProBuilder.Shapes;


public struct CellWallData
{
    public CellWall wall;
    // has matrix ID of adjacent cell
    public int[] wallMatrixID;
    public int relativeX;
    public int relativeZ;
    private bool wallDestroyed;
    private bool isDeadWall;
    //public bool idSet;

    public CellWallData(CellWall _wall, int _relativeX, int _relativeZ, int[] _wallMatrixID = null, bool _wallDestroyed = false, bool _isDead = false)//, bool id = false)
    {
        wall = _wall;
        relativeX = _relativeX;
        relativeZ = _relativeZ;
        wallMatrixID = new int[2];
        //wallDestroyed = new bool();
        wallDestroyed = _wallDestroyed;
        isDeadWall = _isDead;
        //wallMatrixID = _wallMatrixID;
        //idSet = id;
    }

    public void SetWallMatrixID(int[] _wallMatrixID) 
    {

        //Debug.Log("Setting Matrix ID: ");
        if (wallMatrixID == null)
        {
            wallMatrixID = new int[2];            
        }

        wallMatrixID[0] = _wallMatrixID[0];
        wallMatrixID[1] = _wallMatrixID[1];
        //idSet = true;
        //Debug.Log("ID: " + wallMatrixID[0] + ", " + wallMatrixID[1]);
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
    public int GetMatrixX() {  return wallMatrixID[0]; }
    public int GetMatrixZ() {  return wallMatrixID[1]; }

    public int[] GetRelativeIndex()
    {
        int[] result = new int[2];
        result[0] = relativeX;
        result[1] = relativeZ;
        return result;
    }

    public void MarkAsDead()
    {
        isDeadWall = true;
    }

    public bool WallIsDead() { return isDeadWall; }
    public void DisableWall() 
    { 
        wall.gameObject.SetActive(false);
    }

    public void DestroyWall()
    {
        wallDestroyed = true;
    }

    public bool WallIsDisabled() { return !wall.gameObject.activeSelf; }

    public bool WallIsDestroyed() { return wallDestroyed; }
    public void EnableWall() { wall.gameObject.SetActive(true); }

    //public void SetID(bool _id) {  idSet = _id; }
    //public bool GetID() { return idSet; }
}


public class Cell : MonoBehaviour
{
    [SerializeField]
    private Material _wallMaterial;
    [SerializeField]
    private Material _floorMaterial;       
    [SerializeField]
    private Material _ceelingMaterial;      

    private Color _wallColor;

    public List<int[]> _indexInArrayList;

    //public GameObject doorwayPrefab;
    //public GameObject doorPrefab;
    [SerializeField]
    private GameObject _doorWayPrefab;
    private List<GameObject> _doorWays;
    //private List<GameObject> _doorways;
    //private List<GameObject> _doors;

    //[SerializeField]
    //private CellDoStuff _stuffForCellToDo;


    [Tooltip("Add the dead cells as XXZZ, no spaces, must have 4 characters")]
    public List<string> _deadCellListStr;

    private List<(int x, int z)> _deadCellListInt;
    private List<(int x, int z)> _relativeDeadCellListInt;
    //[HideInInspector]
    public bool isTransitionCell = false;
    public int transCellExitX = -1;
    public int transCellExitZ = -1;
    public bool hasDeadCells;

    public List<CellWallData> _xPosVerticleWalls;
    public List<CellWallData> _xNegVerticleWalls;
    public List<CellWallData> _zPosHorizontalWalls;
    public List<CellWallData> _zNegHorizontalWalls;
    private List<CellWall> _otherWalls;

    private List<CellFloor> _floors;
    private List<CellCeeling> _ceelings;

    [SerializeField, Tooltip("IF false, the wall renderers will be disabled & only planes used for graphics.")]
    private bool keepCellWalls;
    //[SerializeField]
    //private bool hasSecondFloor;

    public int xWidth = 1;
    public int zHeight = 1;
    public int yFloors = 1;

    private int[] _indexInArray;

    private bool _hasWallMatrixId;

    private bool _renderersEnabled = true;

    public GameObject cellDoStuffObject;

    private int _cellPrefabIndex = -1;

    //[SerializeField]
    //private List<EnemyBaseClass> _enemyList;

    //[SerializeField]
    //private List<EnemySpawnPoint> _enemySpawnList;

    //private List<EnemyController> _enemyList;

    //[SerializeField]
    //private bool _isYTransitionPrefab = false;

    private void Awake()
    {
        //_cellWalls = new CellWall[8];
        
        _hasWallMatrixId = false;
        _indexInArrayList = new List<int[]>();
        _xPosVerticleWalls = new List<CellWallData>();
        _xNegVerticleWalls = new List<CellWallData>();
        _zPosHorizontalWalls = new List<CellWallData>();
        _zNegHorizontalWalls = new List<CellWallData>();
        _otherWalls = new List<CellWall>();
        _floors = new List<CellFloor>();
        _ceelings = new List<CellCeeling>();
        _deadCellListInt = new List<(int x, int z)>();
        _relativeDeadCellListInt = new List<(int x, int z)>();
        _doorWays = new List<GameObject>();    //
        //_doorways = new List<GameObject> ();
        //_doors = new List<GameObject>();
        keepCellWalls = true;          //---------------        
        FindWallsInChildren();
    }

    void Start()
    {
        //FindWallsInChildren();
        
    }

    private void SetColor()
    {
        _wallColor = new Color(((float)xWidth / (float)(xWidth + zHeight)), ((float)(xWidth + zHeight) / 5), ((float) zHeight / (float)(xWidth + zHeight)));
        //_wallMaterial.color = _wallColor;
    }

    public void SetDeadCells()
    {
        Debug.Log("STR List Count: " + _deadCellListStr.Count);

        if (_deadCellListStr != null && _deadCellListStr.Count > 0)
        {
            SetIntDeadCells();
        }
    }

    private void SetIntDeadCells()
    {
        for (int i = 0;  i < _deadCellListStr.Count;i++)
        {
                if (_deadCellListStr[i].Length == 4)
                {
                    _deadCellListInt.Add((_indexInArray[0] + GetXorZFrom2Char(_deadCellListStr[i][0], _deadCellListStr[i][1]), _indexInArray[1] + GetXorZFrom2Char(_deadCellListStr[i][2], _deadCellListStr[i][3])));
                    //Debug.Log("Dead Cell From Base: " + _indexInArray[0] + ", " + _indexInArray[1] + " at: " + _deadCellListInt[i].x + ", " + _deadCellListInt[i].z);
                }
                else
                    Debug.LogError("Dead Cell Name Wrong!! Name: " + _deadCellListStr[i]);
        }
    }

    private void SetRelativeIntDeadCells()
    {
        for (int i = 0; i < _deadCellListStr.Count; i++)
        {
            if (_deadCellListStr[i].Length == 4)
            {
                _relativeDeadCellListInt.Add((GetXorZFrom2Char(_deadCellListStr[i][0], _deadCellListStr[i][1]), GetXorZFrom2Char(_deadCellListStr[i][2], _deadCellListStr[i][3])));
                //Debug.Log("Dead Cell From Base: " + _indexInArray[0] + ", " + _indexInArray[1] + " at: " + _deadCellListInt[i].x + ", " + _deadCellListInt[i].z);
            }
            else
                Debug.LogError("Dead Cell Name Wrong!! Name: " + _deadCellListStr[i]);
        }

        SetDeadWalls();
    }

    //_relativeDeadCellListInt

    private void SetDeadWalls()
    {
        for (int i = 0; i < _deadCellListInt.Count; i++)
        {
            for (int j = 0; j < _xPosVerticleWalls.Count; j++)
            {
                if (IndexAdjacentToOther(_relativeDeadCellListInt[i].x, _relativeDeadCellListInt[i].z, _xPosVerticleWalls[j].GetRelativeIndex()))
                    _xPosVerticleWalls[j].MarkAsDead();
                if (IndexAdjacentToOther(_relativeDeadCellListInt[i].x, _relativeDeadCellListInt[i].z, _xNegVerticleWalls[j].GetRelativeIndex()))
                    _xNegVerticleWalls[j].MarkAsDead();
            }
            for (int p = 0; p < _zPosHorizontalWalls.Count; p++)
            {
                if (IndexAdjacentToOther(_relativeDeadCellListInt[i].x, _relativeDeadCellListInt[i].z, _zPosHorizontalWalls[p].GetRelativeIndex()))
                    _zPosHorizontalWalls[p].MarkAsDead();
                if (IndexAdjacentToOther(_relativeDeadCellListInt[i].x, _relativeDeadCellListInt[i].z, _zNegHorizontalWalls[p].GetRelativeIndex()))
                    _zNegHorizontalWalls[p].MarkAsDead();
            }
        }
        //MarkAsDead
    }

    public List<int[]> GetCellExits()
    {
        List<int[]> exits = new List<int[]>();

        for (int i = 0; i < _zPosHorizontalWalls.Count; i ++)
        {
            if (!_zPosHorizontalWalls[i].WallIsDead())
                exits.Add(_zPosHorizontalWalls[i].GetMatrixID());
            if (!_zNegHorizontalWalls[i].WallIsDead())
                exits.Add(_zPosHorizontalWalls[i].GetMatrixID());
        }

        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            if (!_xPosVerticleWalls[i].WallIsDead())
                exits.Add(_xPosVerticleWalls[i].GetMatrixID());
            if (!_xNegVerticleWalls[i].WallIsDead())
                exits.Add(_xNegVerticleWalls[i].GetMatrixID());
        }

        return exits;
    }

    public List<int[]> GetAliveCells()
    {
        List<int[]> aliveCells = new List<int[]>();
        bool isDead = false;

        for (int i = 0; i < _indexInArrayList.Count; i++)
        {
            isDead = false;

            for (int j = 0; j < _deadCellListInt.Count; j++)
            {
                if (_indexInArrayList[i][0] == _deadCellListInt[j].x &&
                    _indexInArrayList[i][1] == _deadCellListInt[j].z)
                    isDead = true;
            }
            if (!isDead)
            {
                aliveCells.Add(_indexInArrayList[i]);
            }
        }

        return aliveCells;
    }

    private int GetXorZFrom2Char(char a, char b)
    {
        int result = 0;
        result += GetIntFromChar(a) * 10;
        result += GetIntFromChar(b);
        return result;

    }

    private int GetIntFromChar(char ch)
    {
        return ((int)ch - 48);
    }

    public int GetNumDeadCells()
    {
        return _deadCellListInt.Count;
    }

    public bool HasDeadCell()
    {
        return hasDeadCells;
    }

    public int GetDeadCellX(int num)
    {
        return _deadCellListInt[num].x;
    }

    public int GetDeadCellZ(int num)
    {
        return _deadCellListInt[num].z;
    }

    public List<(int x, int z)> GetDeadCellList()
    {
        if (_deadCellListInt == null)
            return new List<(int x, int z)>();
        return _deadCellListInt;
    }

    public List<(int x, int z)> GetRelativeDeadCellList()
    {
        if (_relativeDeadCellListInt == null)
            return new List<(int x, int z)>();
        return _relativeDeadCellListInt;
    }
    public List<int[]> GetRelativeDeadCells()
    {
        if (_relativeDeadCellListInt == null)
            return new List<int[]>();

        List<int[]> deadCells = new List<int[]>();
        int[] temp;

        for (int i = 0; i < _relativeDeadCellListInt.Count; i++)
        {
            temp = new int[2];
            temp[0] = _relativeDeadCellListInt[i].x;
            temp[1] = _relativeDeadCellListInt[i].z;
            deadCells.Add(temp);
        }

        return deadCells;
    }

    private void FindWallsInChildren()
    {
        CellWall tempWall = null;
        CellWallData tempCellWallData;
        for (int i = 0; i < 2*(xWidth+zHeight)*yFloors; i++)
        {
            try
            {
                tempWall = this.GetComponentInChildren<CellWall>();
                tempCellWallData = new CellWallData(tempWall, (int)tempWall.GetWallX(), (int)tempWall.GetWallZ(), null);

                // tempWall.GetWallX() returns local X position of wall
                // walls along x axis have x value incremented by 5 each time
                // walls along z axis have x value incremented by 10 each time
                // if no remainder from ...%10 then it must be on z axis
                if (tempWall.GetWallY() > 10)
                {
                    _otherWalls.Add(tempWall);
                }
                else if (((float)tempWall.GetWallX() % 10.0f) == 0f)
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
            catch
            {
                //Debug.Log("Couldn't Find Wall.");
            }
        }

        FindFloors_FindCeelins();       

        // then re enable all walls when done
        ReEnableWallLists();

        // organizes them from left to right (1st in list is furthest to left, last is furthes to right in world space)
        ReOrganizeZWallLists();
        // organises from bottum up (1st in list is furthest down in world space) 
        ReOrganizeXWallLists();

        //SetIntDeadCells();
        SetRelativeIntDeadCells();
    }

    private void FindFloors_FindCeelins()
    {
        CellFloor tempFloor;
        CellCeeling tempCeeling;

        for (int i = 0; i < (xWidth * zHeight); i++)
        {
            try
            {
                tempFloor = this.GetComponentInChildren<CellFloor>();
                _floors.Add(tempFloor);
                tempCeeling = this.GetComponentInChildren<CellCeeling>();
                _ceelings.Add(tempCeeling);

                tempFloor.gameObject.SetActive(false);
                tempCeeling.gameObject.SetActive(false);
            }
            catch 
            { 
                Debug.Log("No Floor / Ceeling");
                return;

            }
        }

        for (int i = 0; i < _ceelings.Count; i++)
        {
            _ceelings[i].gameObject.SetActive(true);
            _ceelings[i].ChangeCeelingMat(_ceelingMaterial);

            _floors[i].gameObject.SetActive(true);
            _floors[i].ChangeFloorMat(_floorMaterial);
            _floors[i].gameObject.tag = "Ground";
            _floors[i].gameObject.layer = 6;
        }
    }

    private void ReEnableWallLists()
    {
        try
        {
            Material wallMat = _wallMaterial;
        }
        catch
        {
            Debug.LogError("No Material Set!!");
        }

        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            _zPosHorizontalWalls[i].wall.gameObject.SetActive(true);
            _zPosHorizontalWalls[i].wall.SetWallMaterial(_wallMaterial);
            //_zPosHorizontalWalls[i].wall.SetWallColor(_wallColor);

            _zNegHorizontalWalls[i].wall.gameObject.SetActive(true);
            _zNegHorizontalWalls[i].wall.SetWallMaterial(_wallMaterial);
            //_zNegHorizontalWalls[i].wall.SetWallColor(_wallColor);
        }
        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            _xPosVerticleWalls[i].wall.gameObject.SetActive(true);
            _xPosVerticleWalls[i].wall.SetWallMaterial(_wallMaterial);
            //_xPosVerticleWalls[i].wall.SetWallColor(_wallColor);

            _xNegVerticleWalls[i].wall.gameObject.SetActive(true);
            _xNegVerticleWalls[i].wall.SetWallMaterial(_wallMaterial);
            //_xNegVerticleWalls[i].wall.SetWallColor(_wallColor);
        }

        if (yFloors > 1)
        {
            for (int i = 0; i < _otherWalls.Count; i++)
            {
                _otherWalls[i].gameObject.SetActive(true);
                _otherWalls[i].SetWallMaterial(_wallMaterial);
            }
        }

        if (!keepCellWalls)
            DisableWallRenderers();
    }

    public void DisableWallRenderers()
    {
        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            if (!_zPosHorizontalWalls[i].WallIsDestroyed())
                _zPosHorizontalWalls[i].wall.DisableRenderer();
            if (!_zNegHorizontalWalls[i].WallIsDestroyed())
                _zNegHorizontalWalls[i].wall.DisableRenderer();
        }
        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            if (!_xPosVerticleWalls[i].WallIsDestroyed())
                _xPosVerticleWalls[i].wall.DisableRenderer();
            if (!_xNegVerticleWalls[i].WallIsDestroyed())
                _xNegVerticleWalls[i].wall.DisableRenderer();
        }
        if (yFloors > 1)
        {
            for (int i = 0; i < _otherWalls.Count; i++)
            {
                _otherWalls[i].DisableRenderer();
            }
        }
    }

    public void EnableWallRenderers()
    {
        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            if (!_zPosHorizontalWalls[i].WallIsDestroyed())
                _zPosHorizontalWalls[i].wall.EnableRenderer();
            if (!_zNegHorizontalWalls[i].WallIsDestroyed())
                _zNegHorizontalWalls[i].wall.EnableRenderer();
        }
        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            if (!_xPosVerticleWalls[i].WallIsDestroyed())
                _xPosVerticleWalls[i].wall.EnableRenderer();
            if (!_xNegVerticleWalls[i].WallIsDestroyed())
                _xNegVerticleWalls[i].wall.EnableRenderer();
        }
        if (yFloors > 1)
        {
            for (int i = 0; i < _otherWalls.Count; i++)
            {
                _otherWalls[i].EnableRenderer();
            }
        }
    }

    // gives each of the cells walls the position (in the matrix) of the cell on the other side of it
    // done by adding the zHeight (how many 1x1 cells it has in the z axis) to the base cell's z index &
    // repeating for however many front walls it has
    // same for back walls but subtract 1 from base cell (because base cell is bottom left)
    // same for x walls, but add xWidth for right walls / subtract 1 from base x value for left walls
    public void SetMatrixID(int baseX, int baseZ)
    {
        if (_hasWallMatrixId == true)
            return;
        else
        {
            _hasWallMatrixId = true;
        }

        int[] tempID = null;

        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            tempID = new int[2];
            tempID[0] = baseX + i;          // x
            tempID[1] = baseZ + zHeight;    // z
            _zPosHorizontalWalls[i].SetWallMatrixID(tempID);

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
            tempID = new int[2];
            tempID[0] = baseX - 1;
            tempID[1] = baseZ + i;
            _xNegVerticleWalls[i].SetWallMatrixID(tempID);
        }
    }

    private void ReOrganizeZWallLists()
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
            if (_zNegHorizontalWalls[i].relativeX > _zNegHorizontalWalls[i + 1].relativeX)
            {
                swapped = true;
                tempCellWallData = _zNegHorizontalWalls[i];
                _zNegHorizontalWalls[i] = _zNegHorizontalWalls[i + 1];
                _zNegHorizontalWalls[i + 1] = tempCellWallData;
            }
        }

        if (swapped)
            ReOrganizeZWallLists();
    }

    private void ReOrganizeXWallLists()
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
            if (_xNegVerticleWalls[i].relativeZ > _xNegVerticleWalls[i + 1].relativeZ)
            {
                swapped = true;
                tempCellWallData = _xNegVerticleWalls[i];
                _xNegVerticleWalls[i] = _xNegVerticleWalls[i + 1];
                _xNegVerticleWalls[i + 1] = tempCellWallData;
            }
        }

        if (swapped)
            ReOrganizeXWallLists();
    }

    // good if problem with how maze generated
    public void PrintWallStuff()
    {
        Debug.Log("Base Wall: " + _indexInArray[0] + ", " + _indexInArray[1] + "----------------------");
        Debug.Log("Z Pos Wall Indexes: ");

        for (int i = 0; i < _zPosHorizontalWalls.Count;i++)
        {
            Debug.Log(_zPosHorizontalWalls[i].GetMatrixX() + ", " + _zPosHorizontalWalls[i].GetMatrixZ() + ": " + _zPosHorizontalWalls[i].relativeX + ", " + _zPosHorizontalWalls[i].relativeZ);
        }
        Debug.Log("Z Neg Wall Indexes: ");
        for (int i = 0; i < _zNegHorizontalWalls.Count; i++)
        {
            Debug.Log(_zNegHorizontalWalls[i].wallMatrixID[0] + ", " + _zNegHorizontalWalls[i].wallMatrixID[1] + ": " + _zNegHorizontalWalls[i].relativeX + ", " + _zNegHorizontalWalls[i].relativeZ);
        }
        Debug.Log("X Pos Wall Indexes: ");
        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            Debug.Log(_xPosVerticleWalls[i].wallMatrixID[0] + ", " + _xPosVerticleWalls[i].wallMatrixID[1] + ": " + _xPosVerticleWalls[i].relativeX + ", " + _xPosVerticleWalls[i].relativeZ);
        }
        Debug.Log("X Neg Wall Indexes: ");
        for (int i = 0; i < _xNegVerticleWalls.Count; i++)
        {
            Debug.Log(_xNegVerticleWalls[i].wallMatrixID[0] + ", " + _xNegVerticleWalls[i].wallMatrixID[1] + ": " + _xNegVerticleWalls[i].relativeX + ", " + _xNegVerticleWalls[i].relativeZ);
        }
    }

    

    public void SetIndexInArray(int x, int z)
    {
        _indexInArray = new int[2];
        _indexInArray[0] = x;
        _indexInArray[1] = z;
        //SetIntDeadCells();
    }

    public void SetCellPrefabIndex(int index)
    {
        _cellPrefabIndex = index;
    }

    public int GetCellPrefabIndex()
    {
        return _cellPrefabIndex;
    }

    // adds all "1x1" cells included in this cell to list
    // e.g. if this cell is 2x1 at base (0,0) it adds [0,0] and [1,0] to list
    public void SetIndexInArrayList(int x, int z)
    {
        if (_indexInArrayList == null)
            _indexInArrayList = new List<int[]>();
        int[] tempVal = new int[2];
        tempVal[0] = x;
        tempVal[1] = z;
        _indexInArrayList.Add(tempVal);
    }

    public List<int[]> GetIndexList()
    {
        if ( _indexInArrayList == null )
            return new List<int[]>();

        return _indexInArrayList;
    }

    public int[] GetIndexInArray()
    {
        if (_indexInArray != null)
        {
            return _indexInArray;
        }
        else
            return null;
    }

    public void EnableAllWalls()
    {
        for (int i = 0; i < zHeight; i++)
        {
            _zPosHorizontalWalls[i].wall.gameObject.SetActive(true);
            _zNegHorizontalWalls[i].wall.gameObject.SetActive(true);
        }
        for (int i = 0; i < xWidth; i++)
        {
            _xPosVerticleWalls[i].wall.gameObject.SetActive(true);
            _xPosVerticleWalls[i].wall.gameObject.SetActive(true);
        }
    }

    public void DisableCellRenderers()
    {
        //DisableEnemies();
        DisableWallRenderers();
        DisableFloorRenderers();
        DisableCeelingRenderers();
        DisableCellDoorWays();
        DisableCellDoStuffObject();
        _renderersEnabled = false;
    }

    private void DisableFloorRenderers()
    {
        foreach (CellFloor floor in _floors)
            floor.DisableFloorRenderer();
    }

    private void DisableCeelingRenderers()
    {
        foreach (CellCeeling ceeling in _ceelings)
            ceeling.DisableCeelingRenderer();
    }

    private void DisableCellDoorWays()
    {
        foreach (GameObject doorWay in _doorWays)
        {
            doorWay.GetComponent<DoorSpawnPoint>().DisableDoor();
            doorWay.SetActive(false);
        }
        //foreach (GameObject door in _doors)
        //    door.SetActive(false);
    }

    private void DisableCellDoStuffObject()
    {
        if (cellDoStuffObject != null)
        cellDoStuffObject.SetActive(false);
    }

    //private void DisableEnemies()
    //{
    //    foreach (EnemyBaseClass enemy in _enemyList)
    //        enemy.DisableEnemy();
    //}

    //private bool EnemyHasntLeftCell(int enemyIndex)
    //{
    //    return (Vector3.Distance(_enemyList[enemyIndex].transform.position, transform.position) <= 20 )
    //}

    public void EnableCellRenderers()
    {
        EnableWallRenderers();
        EnableFloorRenderers();
        EnableCeelingRenderers();
        EnableCellDoorWays();
        EnableCellDoStuffObject();
        //EnableEnemies();
        _renderersEnabled = true;
    }

    private void EnableFloorRenderers()
    {
        foreach (CellFloor floor in _floors)
            floor.EnableFloorRenderer();
    }

    private void EnableCeelingRenderers()
    {
        foreach (CellCeeling ceeling in _ceelings)
            ceeling.EnableCeelingRenderer();
    }

    private void EnableCellDoorWays()
    {
        foreach (GameObject doorWay in _doorWays)
        {
            doorWay.SetActive(true);
        }
        //foreach (GameObject door in _doors)
        //    door.SetActive(true);
    }

    private void EnableCellDoStuffObject()
    {
        if (cellDoStuffObject != null)
            cellDoStuffObject.SetActive(true);
    }

    //private void EnableEnemies()
    //{
    //    foreach (EnemyBaseClass enemy in _enemyList)
    //        enemy.EnableEnemy();
    //}

    public bool CellRenderersEnabled()
    {
        return _renderersEnabled;
    }

    public void DisableCell()
    {
        DisableAllWalls();
        DisableFloors();
        DisableCeelings();
    }

    public void DisableFloors()
    {
        for (int i = 0; i < _floors.Count; i++)
        {
            _floors[i].gameObject.SetActive(false);
        }
    }

    public void DisableCeelings()
    {
        for (int i = 0; i < _ceelings.Count; i++)
        {
            _ceelings[i].gameObject.SetActive(false);
        }
    }
    public void DisableAllWalls(bool alsoDisableHigherWalls = false)
    {
        for (int i = 0; i < zHeight; i++)
        {
            _xPosVerticleWalls[i].wall.gameObject.SetActive(false);
            _xNegVerticleWalls[i].wall.gameObject.SetActive(false);
        }
        for (int i = 0; i < xWidth; i++)
        {
            _zPosHorizontalWalls[i].wall.gameObject.SetActive(false);
            _zNegHorizontalWalls[i].wall.gameObject.SetActive(false);
        }
        if (alsoDisableHigherWalls && yFloors > 1)
        {
            for (int i = 0; i < _otherWalls.Count; i++)
            {
                _otherWalls[i].gameObject.SetActive(false);
            }
        }
    }

    public void SwitchAllWallStatus()
    {
        for (int i = 0; i < zHeight; i++)
        {
            _zPosHorizontalWalls[i].wall.gameObject.SetActive(_zPosHorizontalWalls[i].wall.gameObject.activeSelf);
            _zNegHorizontalWalls[i].wall.gameObject.SetActive(_zNegHorizontalWalls[i].wall.gameObject.activeSelf);
        }
        for (int i = 0; i < xWidth; i++)
        {
            _xPosVerticleWalls[i].wall.gameObject.SetActive(_xPosVerticleWalls[i].wall.gameObject.activeSelf);
            _xPosVerticleWalls[i].wall.gameObject.SetActive(_xPosVerticleWalls[i].wall.gameObject.activeSelf);
        }
    }

    public void DisablePosZWalls(bool alsoDisableHigherWalls = false)
    {
        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            _zPosHorizontalWalls[i].wall.gameObject.SetActive(false);
        }

        if (alsoDisableHigherWalls && yFloors > 1)
        {
            for (int i = 0; i < _otherWalls.Count; i++)
            {
                if ((_otherWalls[i].GetWallX() % 10.0f) == 0f && _otherWalls[i].GetWallZ() > 0f)
                {
                    _otherWalls[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void DisableNegZWalls(bool alsoDisableHigherWalls = false)
    {
        for (int i = 0; i < _zNegHorizontalWalls.Count; i++)
        {
            _zNegHorizontalWalls[i].wall.gameObject.SetActive(false);
        }

        if (alsoDisableHigherWalls && yFloors > 1)
        {
            for (int i = 0; i < _otherWalls.Count; i++)
            {
                if ((_otherWalls[i].GetWallX() % 10.0f) == 0f && _otherWalls[i].GetWallZ() <= 0f)
                {
                    _otherWalls[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void DisablePosXWalls(bool alsoDisableHigherWalls = false)
    {
        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            _xPosVerticleWalls[i].wall.gameObject.SetActive(false);
        }
        if (alsoDisableHigherWalls && yFloors > 1)
        {
            for (int i = 0; i < _otherWalls.Count; i++)
            {
                if ((_otherWalls[i].GetWallX() % 10.0f) != 0f && _otherWalls[i].GetWallX() > 0f)
                {
                    _otherWalls[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void DisableNegXWalls(bool alsoDisableHigherWalls = false)
    {
        for (int i = 0; i < _xNegVerticleWalls.Count; i++)
        {
            _xNegVerticleWalls[i].wall.gameObject.SetActive(false);
        }
        if (alsoDisableHigherWalls && yFloors > 1)
        {
            for (int i = 0; i < _otherWalls.Count; i++)
            {
                if ((_otherWalls[i].GetWallX() % 10.0f) != 0f && _otherWalls[i].GetWallX() <= 0f)
                {
                    _otherWalls[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void DestroySpecificPosZWall(int index, bool addDorway = false, bool addDoor = false)
    {
        //if (addDorway && doorwayPrefab != null && !_zPosHorizontalWalls[index].WallIsDestroyed())
        if (addDorway && _doorWayPrefab != null && !_zPosHorizontalWalls[index].WallIsDestroyed())
        {
            Vector3 newDoorPos = _zPosHorizontalWalls[index].GetWallPos();
            GameObject newDoorWay = Instantiate(_doorWayPrefab, this.transform);
            //GameObject newDoorWay = Instantiate(doorwayPrefab, this.transform);

            newDoorWay.GetComponent<DoorSpawnPoint>().DoorWayHasDoor(addDoor);
            //if (doorPrefab != null && addDoor)
            //{
            //    GameObject newDoor;
            //    newDoor = Instantiate(doorPrefab, newDoorWay.transform);
            //    _doors.Add(newDoor);
            //}
            newDoorWay.transform.position = newDoorPos;
            newDoorWay.transform.rotation = Quaternion.Euler(0, 90, 0);
            _doorWays.Add(newDoorWay);
            //_doorways.Add(newDoorWay);
        }

        CellWallData wallData = _zPosHorizontalWalls[index];
        wallData.DestroyWall();
        Destroy(wallData.wall.gameObject);
        _zPosHorizontalWalls[index] = wallData;
    }

    public void DestroySpecificNegZWall(int index, bool addDorway = false, bool addDoor = false)
    {
        if (addDorway && _doorWayPrefab != null && !_zNegHorizontalWalls[index].WallIsDestroyed())
        {
            Vector3 newDoorPos = _zNegHorizontalWalls[index].GetWallPos();
            GameObject newDoorWay = Instantiate(_doorWayPrefab, this.transform);
            //GameObject newDoorWay = Instantiate(doorwayPrefab, this.transform);
            newDoorWay.GetComponent<DoorSpawnPoint>().DoorWayHasDoor(addDoor);

            //if (doorPrefab != null && addDoor)
            //{
            //    GameObject newDoor;
            //    newDoor = Instantiate(doorPrefab, newDoorWay.transform);
            //    _doors.Add(newDoor);
            //}
            newDoorWay.transform.position = newDoorPos;
            newDoorWay.transform.rotation = Quaternion.Euler(0, 90, 0);
            //_doorways.Add(newDoorWay);
            _doorWays.Add(newDoorWay);
        }

        CellWallData wallData = _zNegHorizontalWalls[index];
        wallData.DestroyWall();
        Destroy(wallData.wall.gameObject);
        _zNegHorizontalWalls[index] = wallData;
    }

    public void DestroySpecificPosXWall(int index, bool addDorway = false, bool addDoor = false)
    {
        if (addDorway && _doorWayPrefab != null && !_xPosVerticleWalls[index].WallIsDestroyed())
        {
            Vector3 newDoorPos = _xPosVerticleWalls[index].GetWallPos();
            //GameObject newDoorWay = Instantiate(doorwayPrefab, this.transform);
            GameObject newDoorWay = Instantiate(_doorWayPrefab, this.transform);

            newDoorWay.GetComponent<DoorSpawnPoint>().DoorWayHasDoor(addDoor);

            //if (doorPrefab != null && addDoor)
            //{
            //    GameObject newDoor;
            //    newDoor = Instantiate(doorPrefab, newDoorWay.transform);
            //    _doors.Add(newDoor);
            //}
            newDoorWay.transform.position = newDoorPos;
            newDoorWay.transform.rotation = Quaternion.Euler(0, 0, 0);
            //_doorways.Add(newDoorWay);
            _doorWays.Add(newDoorWay);
        }

        CellWallData wallData = _xPosVerticleWalls[index];
        wallData.DestroyWall();
        Destroy(wallData.wall.gameObject);
        _xPosVerticleWalls[index] = wallData;
    }

    public void DestroySpecificNegXWall(int index, bool addDorway = false, bool addDoor = false)
    {
        if (addDorway && _doorWayPrefab != null && !_xNegVerticleWalls[index].WallIsDestroyed())
        {
            Vector3 newDoorPos = _xNegVerticleWalls[index].GetWallPos();
            //GameObject newDoorWay = Instantiate(doorwayPrefab, this.transform);
            GameObject newDoorWay = Instantiate(_doorWayPrefab, this.transform);
            newDoorWay.GetComponent<DoorSpawnPoint>().DoorWayHasDoor(addDoor);


            //if (doorPrefab != null && addDoor)
            //{
            //    GameObject newDoor;
            //    newDoor = Instantiate(doorPrefab, newDoorWay.transform);
            //    _doors.Add(newDoor);
            //}

            newDoorWay.transform.position = newDoorPos;
            newDoorWay.transform.rotation = Quaternion.Euler(0, 0, 0);
            //_doorways.Add(newDoorWay);
            _doorWays.Add(newDoorWay);
        }

        CellWallData wallData = _xNegVerticleWalls[index];
        wallData.DestroyWall();
        Destroy(wallData.wall.gameObject);
        _xNegVerticleWalls[index] = wallData;
    }

    public int GetCellXWidth()
    {
        return xWidth;
    }

    public int GetCellZHeight()
    {
        return zHeight;
    }

    private bool IndexAdjacentToOther(int[] a, int[] b)
    {
        bool result = false;
        if (a[0] == b[0] && (a[1] == b[1] + 1 || a[1] == b[1] - 1))
            result = true;
        if (a[1] == b[1] && (a[0] == b[0] + 1 || a[0] == b[0] - 1))
            result = true;
        return result;
    }

    private bool IndexAdjacentToOther(int a, int c, int[] b)
    {
        bool result = false;
        if (a == b[0] && (c == b[1] + 1 || c == b[1] - 1))
            result = true;
        if (c == b[1] && (a == b[0] + 1 || a == b[0] - 1))
            result = true;
        return result;
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    //public void DoStuff()
    //{
    //    if (_stuffForCellToDo != null)
    //        _stuffForCellToDo.DoStuff(0);
    //}
}
