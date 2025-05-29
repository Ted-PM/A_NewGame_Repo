using System;
using System.Collections;
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

    private void Awake()
    {
        if (numberOfFloors > mazeFloorPrefabs.Length && !generateDefault)
            numberOfFloors = mazeFloorPrefabs.Length;

        _mazeFloorContainer = new GameObject("MazeFloors");
        _mazeFloors = new MazeFloor[numberOfFloors];
    }

    private void Start()
    {
        //SpawnFloor(0);
        SpawnFloors();
    }

    private void SpawnFloors()
    {
        for (int i = 0; i < numberOfFloors; i++)
        {
            if (generateDefault)
                SpawnFloorDefault(i);
            else
                SpawnFloor(i);
        }
        //
    }

    private void SpawnFloorDefault(int floorIndex)
    {
        if (baseX - (2 * floorIndex) < 8 || baseZ - (2 * floorIndex) < 8)
                return;
        GameObject newFloor = Instantiate(mazeFloorPrefabs[0], _mazeFloorContainer.transform);
        _mazeFloors[floorIndex] = newFloor.GetComponent<MazeFloor>();
        _mazeFloors[floorIndex].InitializeFloor((baseX - (2 * floorIndex)), (baseZ - (2 * floorIndex)));
        if (floorIndex > 0)
            _mazeFloors[floorIndex].SetPrevFloorData(_mazeFloors[floorIndex - 1].GetTransitionalCell(),
                _mazeFloors[floorIndex - 1].GetTransitionX(), _mazeFloors[floorIndex - 1].GetTransitionZ(),
                (floorIndex - 1));
        if (floorIndex == numberOfFloors - 1)
            _mazeFloors[floorIndex].hasNextFloor = false;
        _mazeFloors[floorIndex].SpawnFloor();
        StartCoroutine(WaitThenGenerateMaze(floorIndex));
    }

    private void SpawnFloor(int floorIndex)
    {
        GameObject newFloor = Instantiate(mazeFloorPrefabs[floorIndex], _mazeFloorContainer.transform);
        _mazeFloors[floorIndex] = newFloor.GetComponent<MazeFloor>();
        _mazeFloors[floorIndex].InitializeFloor(baseX, baseZ);
        if (floorIndex > 0)
            _mazeFloors[floorIndex].SetPrevFloorData(_mazeFloors[floorIndex - 1].GetTransitionalCell(),
                _mazeFloors[floorIndex - 1].GetTransitionX(), _mazeFloors[floorIndex - 1].GetTransitionZ(),
                (floorIndex - 1));
        _mazeFloors[floorIndex].SpawnFloor();
        StartCoroutine(WaitThenGenerateMaze(floorIndex));
    }

    private IEnumerator WaitThenGenerateMaze(int floorIndex)
    {
        yield return new WaitForSeconds(1f);
        GenerateMaze(floorIndex);
    }

    private void GenerateMaze(int floorIndex)
    {
        _generator.GenerateMaze(_mazeFloors[floorIndex].GetCellMatrix(), _mazeFloors[floorIndex].GetCellMatrixBool(), _mazeFloors[floorIndex].GetFloorXWidth(), _mazeFloors[floorIndex].GetFloorZHeight(), _mazeFloors[floorIndex].GetStartX(), _mazeFloors[floorIndex].GetStartZ());
        if (floorIndex > 0)
            _mazeFloors[floorIndex].DisableCellsAboveTransitional();
    }

}
