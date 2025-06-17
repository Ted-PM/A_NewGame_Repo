using UnityEngine;

[CreateAssetMenu(fileName = "FogData_ScriptableObjectScript", menuName = "Scriptable Objects/FogData_ScriptableObjectScript")]
public class FogData_ScriptableObjectScript : ScriptableObject
{
    public bool useFog = true;
    public Color fogColor = Color.black;
    public Color camBGColor = Color.black;
    public bool flashLightEnabled = true;
    public FogMode fogMode = FogMode.Linear;
    [Header("Only for Linear fog")]
    public float fogStartDistance = 0f;
    public float fogEndDistance = 35f;
    [Header("Only for Exponential & Exponential Squared fog")]
    [Range(0,1)]
    public float fogDensity = 0.075f;
}
