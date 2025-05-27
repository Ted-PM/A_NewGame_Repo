//using System.Diagnostics;
//using NUnit.Framework;
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
    //public bool idSet;

    public CellWallData(CellWall _wall, int _relativeX, int _relativeZ, int[] _wallMatrixID = null)//, bool id = false)
    {
        wall = _wall;
        relativeX = _relativeX;
        relativeZ = _relativeZ;
        wallMatrixID = new int[2];
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
    public int[] GetMatrixID() { return wallMatrixID; }
    public int GetMatrixX() {  return wallMatrixID[0]; }
    public int GetMatrixZ() {  return wallMatrixID[1]; }

    public void DisableWall() { wall.gameObject.SetActive(false); }
    public void EnableWall() { wall.gameObject.SetActive(true); }

    //public void SetID(bool _id) {  idSet = _id; }
    //public bool GetID() { return idSet; }
}

public class Cell : MonoBehaviour
{
    //[SerializeField]
    //private CellWall[] _cellWalls;

    private GameObject CellFloorContainer;
    private GameObject CellCeelingContainer;

    [SerializeField]
    private Material _wallMaterial;
    [SerializeField]
    private Material _floorMaterial;
    [SerializeField]
    private Material _ceelingMaterial;

    private Color _wallColor;

    public List<int[]> _indexInArrayList;

    public List<CellWallData> _xPosVerticleWalls;
    public List<CellWallData> _xNegVerticleWalls;
    public List<CellWallData> _zPosHorizontalWalls;
    public List<CellWallData> _zNegHorizontalWalls;

    public List<GameObject> _floors;
    public List<GameObject> _ceelings;


    //public WallID DirectionToExit;

    //[SerializeField, Tooltip("IF false, the wall renderers will be disabled & only planes used for graphics.")]
    private bool keepCellWalls = true;

    public int xWidth = 1;
    public int zHeight = 1;

    private int[] _indexInArray;

    private bool _hasWallMatrixId;

    private void Awake()
    {
        //_cellWalls = new CellWall[8];
        
        _hasWallMatrixId = false;
        _indexInArrayList = new List<int[]>();
        _xPosVerticleWalls = new List<CellWallData>();
        _xNegVerticleWalls = new List<CellWallData>();
        _zPosHorizontalWalls = new List<CellWallData>();
        _zNegHorizontalWalls = new List<CellWallData>();
        _floors = new List<GameObject>();
        _ceelings = new List<GameObject>();
        CreateCeelingAndFloorContainers();
    }

    void Start()
    {
        //FindWallsInChildren();
        //SetColor();
        FindWallsInChildren();
    }

    private void SetColor()
    {
        _wallColor = new Color(((float)xWidth / (float)(xWidth + zHeight)), ((float)(xWidth + zHeight) / 5), ((float) zHeight / (float)(xWidth + zHeight)));
        //_wallMaterial.color = _wallColor;
    }

    private void CreateCeelingAndFloorContainers()
    {
        CellFloorContainer = new GameObject();
        CellFloorContainer.transform.parent = this.transform;
        CellFloorContainer.name = "Floors";

        CellCeelingContainer = new GameObject();
        CellCeelingContainer.transform.parent = this.transform;
        CellCeelingContainer.name = "Ceelings";
    }

    private void FindWallsInChildren()
    {
        CellWall tempWall = null;
        CellWallData tempCellWallData;
        for (int i = 0; i < 2*(xWidth+zHeight); i++)
        {
            try
            {
                tempWall = this.GetComponentInChildren<CellWall>();
                tempCellWallData = new CellWallData(tempWall, (int)tempWall.GetWallX(), (int)tempWall.GetWallZ(), null);

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
            catch {
                Debug.Log("Couldn't Find Wall.");
            }
        }

        FindFloors_FindCeelins();


        // then re enable all walls when done
        ReEnableWallLists();

        // organizes them from left to right (1st in list is furthest to left, last is furthes to right in world space)
        ReOrganizeZWallLists();
        // organises from bottum up (1st in list is furthest down in world space) 
        ReOrganizeXWallLists();
    }

    private void FindFloors_FindCeelins()
    {
        GameObject temp;
        Debug.Log("Looking for Floors / ceelings");
        for (int i = 0; i < (xWidth * zHeight) * 2; i++)
        {
            //Debug.Log("In For Loop");

            try
            {
                Debug.Log("In Try");

                temp = this.gameObject.GetComponentInChildren<GameObject>();
                if (temp.transform.localPosition.y > 0f)
                {
                    Debug.Log("Found Ceeling");
                    temp.transform.parent = CellCeelingContainer.transform;
                    _ceelings.Add(temp);
                }
                else
                {
                    Debug.Log("Found Floor");
                    temp.transform.parent = CellFloorContainer.transform;
                    _floors.Add(temp);
                }
                temp.gameObject.SetActive(false);
            }
            catch { Debug.Log("No Floor / Ceeling"); }
        }

        for (int i = 0; i < _ceelings.Count; i++)
        {
            _ceelings[i].gameObject.SetActive(true);
            _ceelings[i].GetComponent<Renderer>().material = _ceelingMaterial;
            //_ceelings[i].gameObject.transform.parent = CellCeelingContainer.transform;

            _floors[i].gameObject.SetActive(true);
            _floors[i].GetComponent<Renderer>().material = _floorMaterial;
            //_floors[i].gameObject.transform.parent = CellFloorContainer.transform;
        }
    }

    private void ReEnableWallLists()
    {
        try
        {
            Material wallMat = _wallMaterial;
            //Color color = _wallColor;
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

        if (!keepCellWalls)
            DisableWallRenderers();
    }

    public void DisableWallRenderers()
    {
        for (int i = 0; i < _zPosHorizontalWalls.Count; i++)
        {
            _zPosHorizontalWalls[i].wall.DisableRenderer();
            _zNegHorizontalWalls[i].wall.DisableRenderer();
        }
        for (int i = 0; i < _xPosVerticleWalls.Count; i++)
        {
            _xPosVerticleWalls[i].wall.DisableRenderer();
            _xNegVerticleWalls[i].wall.DisableRenderer();
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

    public void SetIndexInArray(int x, int z)
    {
        _indexInArray = new int[2];
        _indexInArray[0] = x;
        _indexInArray[1] = z;
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
    public void DisableAllWalls()
    {
        for (int i = 0; i < zHeight; i++)
        {
            _zPosHorizontalWalls[i].wall.gameObject.SetActive(false);
            _zNegHorizontalWalls[i].wall.gameObject.SetActive(false);
        }
        for (int i = 0; i < xWidth; i++)
        {
            _xPosVerticleWalls[i].wall.gameObject.SetActive(false);
            _xPosVerticleWalls[i].wall.gameObject.SetActive(false);
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

    public int GetCellXWidth()
    {
        return xWidth;
    }

    public int GetCellZHeight()
    {
        return zHeight;
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
