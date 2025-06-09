using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpawner : MonoBehaviour
{
    public GameObject[] mazeFloorPrefabs;
    private MazeFloor[] _mazeFloors;
    public int numberOfFloors;
    private GameObject _mazeFloorContainer;

    public int baseX = -1;
    public int baseZ = -1;

    public bool generateDefault;

    [SerializeField]
    private MazeGenerator _generator;

    private List<int[]> _floorDimensions;

    [HideInInspector]
    public bool mazeGenerated = false;

    private void Awake()
    {
        if (numberOfFloors > mazeFloorPrefabs.Length && !generateDefault)
            numberOfFloors = mazeFloorPrefabs.Length;

        _mazeFloorContainer = new GameObject("MazeFloors");
        _mazeFloors = new MazeFloor[numberOfFloors];
        _floorDimensions = new List<int[]>();
    }

    private void Start()
    {
        PopulateFloorDimensionList();
        SpawnFloors();
        mazeGenerated = true;
        //StartCoroutine(DisableFloorRenderers());
    }

    private void SpawnFloors()
    {
        for (int i = 0; i < numberOfFloors; i++)
        {
            if (generateDefault)
                SpawnFloorDefault(i);
            else
                SpawnFloorCustom(i);
        }

        
        //
    }

    //private IEnumerator WaitThenStateMazeDone()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    mazeGenerated = true;
    //}

    //public IEnumerator DisableInitialFloorRenderers()
    //{
    //    yield return new WaitForSeconds(1f);
    //    for (int i = 0; i < numberOfFloors; i++)
    //    {
    //        //if (i == 1)
    //        //    _mazeFloors[i].DisableFloorRenderers(0);
    //        //else
    //            _mazeFloors[i].DisableFloorRenderers(0);
    //    }

    //    yield return new WaitForFixedUpdate();
    //    mazeGenerated = true;
    //    //foreach (MazeFloor floor in _mazeFloors)

    //}

    private void PopulateFloorDimensionList()
    {
        int[] tempDim;

        for (int i = 0; i < numberOfFloors; ++i)
        {
            
            tempDim = new int[2];
            if (!generateDefault)
            {
                tempDim[0] = mazeFloorPrefabs[i].GetComponent<MazeFloor>().GetFloorXWidth();
                tempDim[1] = mazeFloorPrefabs[i].GetComponent<MazeFloor>().GetFloorZHeight();
            }
            else
            {
                tempDim[0] = mazeFloorPrefabs[0].GetComponent<MazeFloor>().GetFloorXWidth();
                tempDim[1] = mazeFloorPrefabs[0].GetComponent<MazeFloor>().GetFloorZHeight();
            }
            _floorDimensions.Add(tempDim);
            Debug.Log("Floor: " + i + " - Added Floor Dimensions: " + _floorDimensions[i][0] + ", " + _floorDimensions[i][1]);
        }
    }

    private void SpawnFloorDefault(int floorIndex)
    {
        if (baseX - (2 * floorIndex) < 8 || baseZ - (2 * floorIndex) < 8)
                return;

        GameObject newFloor = Instantiate(mazeFloorPrefabs[0], _mazeFloorContainer.transform);
        _mazeFloors[floorIndex] = newFloor.GetComponent<MazeFloor>();
        _mazeFloors[floorIndex].InitializeFloor((baseX - (2 * floorIndex)), (baseZ - (2 * floorIndex)));

        if (floorIndex == 0)
            _mazeFloors[floorIndex].SetStartCells();

        if (floorIndex > 0)
            _mazeFloors[floorIndex].SetPrevFloorData(_mazeFloors[floorIndex - 1], _mazeFloors[floorIndex - 1].GetTransitionalCell(),
                _mazeFloors[floorIndex - 1].GetTransitionX(), _mazeFloors[floorIndex - 1].GetTransitionZ(),
                (floorIndex - 1));
        if (floorIndex == numberOfFloors - 1)
            _mazeFloors[floorIndex].hasNextFloor = false;

        if (floorIndex != numberOfFloors - 1)
        {
            _mazeFloors[floorIndex].SetNextFloorDimensions(_floorDimensions[floorIndex + 1][0], _floorDimensions[floorIndex + 1][1]);
        }

        _mazeFloors[floorIndex].SpawnFloor();
        StartCoroutine(WaitThenGenerateMaze(floorIndex));
    }

    private void SpawnFloorCustom(int floorIndex)
    {
        GameObject newFloor = Instantiate(mazeFloorPrefabs[floorIndex], _mazeFloorContainer.transform);
        _mazeFloors[floorIndex] = newFloor.GetComponent<MazeFloor>();
        _mazeFloors[floorIndex].InitializeFloor(baseX, baseZ);

        if (floorIndex == 0)
            _mazeFloors[floorIndex].SetStartCells();

        if (floorIndex > 0)
            _mazeFloors[floorIndex].SetPrevFloorData(_mazeFloors[floorIndex - 1], _mazeFloors[floorIndex - 1].GetTransitionalCell(),
                _mazeFloors[floorIndex - 1].GetTransitionX(), _mazeFloors[floorIndex - 1].GetTransitionZ(),
                (floorIndex - 1));

        if (floorIndex != numberOfFloors - 1)
        {
            _mazeFloors[floorIndex].SetNextFloorDimensions(_floorDimensions[floorIndex + 1][0], _floorDimensions[floorIndex + 1][1]);
        }

        _mazeFloors[floorIndex].SpawnFloor();

        if (!_mazeFloors[floorIndex].isEmptyFloor)
            StartCoroutine(WaitThenGenerateMaze(floorIndex));
        else
            StartCoroutine(WaitThenGenerateEmptyFloor(floorIndex));
    }

    private IEnumerator WaitThenGenerateMaze(int floorIndex)
    {
        yield return new WaitForSeconds(1f);
        GenerateMazeFloor(floorIndex);
        yield return new WaitForFixedUpdate();
        _mazeFloors[floorIndex].AddNavmeshSurfaceData();
        Debug.Log("Floor: " + floorIndex);
        _generator.PrintUnvisitedCells(_mazeFloors[floorIndex].GetCellMatrixBool(), _mazeFloors[floorIndex].GetFloorXWidth(), _mazeFloors[floorIndex].GetFloorZHeight());
    }

    private IEnumerator WaitThenGenerateEmptyFloor(int floorIndex)
    {
        yield return new WaitForSeconds(1f);
        _mazeFloors[floorIndex].DisableEmptyFloorCellWalls();
        yield return new WaitForFixedUpdate();
        _mazeFloors[floorIndex].AddNavmeshSurfaceData();
    }

    private void GenerateMazeFloor(int floorIndex)
    {
        _generator.GenerateMaze(_mazeFloors[floorIndex].GetCellMatrix(), _mazeFloors[floorIndex].GetCellMatrixBool(), _mazeFloors[floorIndex].GetFloorXWidth(), _mazeFloors[floorIndex].GetFloorZHeight(), _mazeFloors[floorIndex].GetStartX(), _mazeFloors[floorIndex].GetStartZ());
        if (floorIndex > 0)
            _mazeFloors[floorIndex].DisableCellsAboveTransitional();
    }

    public Vector2 GetPlayerSpawnPoint()
    {
        int playerX = _mazeFloors[0].GetStartX();
        int playerZ = _mazeFloors[0].GetStartZ();

        return new Vector2(playerX*10, playerZ*10);
    }

    public MazeFloor[] GetMazeFloors()
    {
        return _mazeFloors;
    }

    //private bool CanGenerateMaze(int floorIndex)
    //{
    //    bool result = true;

    //    try
    //    {
    //        GameObject[,] tempMatrix = _mazeFloors[floorIndex].GetCellMatrix();
    //    }
    //    catch
    //    {
    //        Debug.LogWarning("Couldn't Get Floor Cell Matrix.");
    //        result = false;
    //    }
    //    try
    //    {
    //        bool[,] tempBoolMatrix = _mazeFloors[floorIndex].GetCellMatrixBool();
    //    }
    //    catch
    //    {
    //        Debug.LogWarning("Couldn't Get Floor Bool Matrix.");
    //        result = false;
    //    }
    //    try
    //    {
    //        int tempX = _mazeFloors[floorIndex].GetFloorXWidth();
    //        Debug.Log("XWidt: " + tempX);
    //    }
    //    catch
    //    {
    //        Debug.LogWarning("Couldn't Get Floor X Width.");
    //        result = false;
    //    }
    //    try
    //    {
    //        int tempZ = _mazeFloors[floorIndex].GetFloorZHeight();
    //        Debug.Log("ZHeight: " + tempZ);

    //    }
    //    catch
    //    {
    //        Debug.LogWarning("Couldn't Get Floor Z Height.");
    //        result = false;
    //    }
    //    try
    //    {
    //        int startX = _mazeFloors[floorIndex].GetStartX();
    //        Debug.Log("start X: " + startX);
    //    }
    //    catch
    //    {
    //        Debug.LogWarning("Couldn't Get Floor Start X.");
    //        result = false;
    //    }
    //    try
    //    {
    //        int startZ = _mazeFloors[floorIndex].GetStartZ();
    //        Debug.Log("start Z: " + startZ);
    //    }
    //    catch
    //    {
    //        Debug.LogWarning("Couldn't Get Floor Start Z.");
    //        result = false;
    //    }

    //    return result;
    //}

}
