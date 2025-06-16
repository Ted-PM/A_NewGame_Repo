using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Rendering.Universal;
//using UnityEngine.Rendering
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine.UIElements;
using Unity.Mathematics.Geometry;

[CreateAssetMenu(fileName = "Particles_ScriptableObjectScript", menuName = "Scriptable Objects/Particles/Particles_ScriptableObjectScript")]
public class Particles_ScriptableObjectScript : ScriptableObject
{
    public ParticleTypes particleType;

    public bool looping;
    //public Color color;
    //public ParticleSystem.MinMaxGradient startColor;
    public ParticleSystem.MinMaxGradient color;
    //public ParticleSystem.MinMaxCurve lifetime;
    //public ParticleSystem.MinMaxCurve speed;
    public MinMaxAABB startSize;
    public ParticleSystem.MinMaxCurve sizeOverLifetime;


    public float lifeTime;
    public float speed;
    //public float size;
    public ParticleSystemShapeType shapeType;
    //public ParticleSystemMeshShapeType shapeType;
    //public ParticleSystemCurveMode sizeOverLifetime;
    //public InspectorCurveEditor curve;
    //public ColorCurves a;
    //public ParticleSystemCustomDataMode aMode;
    public float radius;
    public float gravitiyModifier;
    public Vector3 rotation;
    public ParticleSystemRenderMode renderMode;
    //public ParticleSystemCurveMode curveMode;
    //public MinMaxCurvePropertyDrawer curveDrawer;
    //public MinMaxSlider MinMaxSlider;
    //public MinMaxAABB MinMaxAABB;
    public ParticleSystem.MinMaxCurve curve;
    public Mesh[] meshes;
    public Material material;
    public bool hasTrails;
    //public Shape shape;
}

//[CreateAssetMenu(fileName = "ConeParticles_ScriptableObjectScript", menuName = "Scriptable Objects/Particles/ConeParticles_ScriptableObjectScript")]
//public class ConeParticles_ScriptableObjectScript : Particles_ScriptableObjectScript
//{
//    public float radius;
//    public float angle;
//    public float arc;
//}
