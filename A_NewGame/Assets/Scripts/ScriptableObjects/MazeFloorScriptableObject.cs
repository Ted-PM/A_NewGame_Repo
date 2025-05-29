using UnityEngine;

[CreateAssetMenu(fileName = "MazeFloorScriptableObject", menuName = "Scriptable Objects/MazeFloorScriptableObject")]
public class MazeFloorScriptableObject : ScriptableObject
{
    public GameObject startCell;

    [SerializeField, Tooltip("Last Prefab in List is start cell, Keep first one as smallest possible cell.")]
    private GameObject[] _prefabs;

    [SerializeField, Tooltip("The likeleyHood the prefab at that index will be chosen. Also always includes 1st Prefab.")]   
    private int[] oddsOfChoosePrefab;

    [SerializeField, Tooltip("Prefabs to be used when moving to second floor.")]
    private GameObject[] _verticleTransitionPrefabs;

    public int xWidth;
    public int zHeight;

    [Tooltip("Next Floor X & Z relative to this ones matrix.")]
    public bool hasNextFloor;
    public int nextFloorX;
    public int nextFloorZ;

    [Tooltip("Previous Floor X & Z relative to previous ones matrix.")]
    public bool hasPrevFloor;
    public int prevFloorX;
    public int prevFloorZ;
}
