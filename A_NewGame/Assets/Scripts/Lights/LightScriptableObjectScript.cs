using UnityEngine;

[CreateAssetMenu(fileName = "LightScriptableObjectScript", menuName = "Scriptable Objects/LightScriptableObjectScript")]
public class LightScriptableObjectScript : ScriptableObject
{
    public LightTypes lightPrefabType;
    public LightType lightType = LightType.Point;
    public Color lightColor = Color.white;
    public float lightIntensity = 10;
    public float lightRange = 10;

    [Range(0f, 1f)]
    public float shadowStrength = 1f;

    public AudioClip passiveLightAudio = null;
    public AudioClip lightToggledAudio = null;

}
