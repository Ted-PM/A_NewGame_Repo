using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.ProBuilder;

[RequireComponent(typeof(ParticleSystem), typeof(ParticleSystemRenderer), typeof(Renderer))]
public class ParticleController : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private ParticleSystemRenderer _particleSystemRenderer;
    //    ParticleSystemModule

    //struct in UnityEngine/Implemented in:
    private Renderer _renderer;
    public AudioSource _audioSource;
    Particles_ScriptableObjectScript _currentType = null;
    private void Awake()
    {
        if (_particleSystem == null)
            _particleSystem = GetComponent<ParticleSystem>();
        if (_renderer == null)
            _renderer = GetComponent<Renderer>();
        if (_particleSystemRenderer == null)
            _particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
    }


    //public Color color;
    //public float lifeTime;
    //public float speed;
    //public float size;
    ////public ParticleSystemCurveMode sizeOverLifetime;
    ////public InspectorCurveEditor curve;
    ////public ColorCurves a;
    ////public ParticleSystemCustomDataMode aMode;
    //public float radius;
    //public float gravitiyModifier;
    //public ParticleSystemRenderMode renderMode;
    //public Mesh mesh;
    //public Material material;
    //public bool hasTrails;
    //public Shape shape;

    public void UpdateParticleData(Particles_ScriptableObjectScript _particleData)
    {
        if (_currentType != null && _currentType.particleType == _particleData.particleType)
            return;
        _currentType = _particleData;

        SetParticleSizes(_particleSystem.main);
        var main = _particleSystem.main;
        var shape = _particleSystem.shape;
        var fOL = _particleSystem.forceOverLifetime;
        var COL = _particleSystem.colorOverLifetime;
        //ParticleSystemRenderer renderer = new ParticleSystemRenderer();
        //var rendermode = _particleSystem.ParticleSystem;
        //_particleSystem.mo
        //_particleSystem.customData.SetColor(_particleData.color);
        main.loop = _particleData.looping;
        //main.startColor = _particleData.startColor;
       
        main.startLifetime = _particleData.lifeTime;
        main.startSpeed = _particleData.speed;
        //main.startSize = _particleData.size;
        //main.startSize3D = true;
        //main.startSize = _particleData.size;
        //main.star
        //main.startSizeX = new ParticleSystem.MinMaxCurve(_particleData.startSize.Min.x, _particleData.startSize.Max.x);
        //main.startSizeY = new ParticleSystem.MinMaxCurve(_particleData.startSize.Min.y, _particleData.startSize.Max.y);
        //main.startSizeZ = new ParticleSystem.MinMaxCurve(_particleData.startSize.Min.z, _particleData.startSize.Max.z);
        //main.startSize.x = new ParticleSystem.MinMaxCurve(_particleData.sizeOverLifetime.Min, );
        //main.startRotationX = _particleData.rotation.x;
        //_particleSystem.rad = _particleData.radius;
        shape.radius = _particleData.radius;
        shape.shapeType = _particleData.shapeType;
        main.gravityModifier = _particleData.gravitiyModifier;
        _particleSystemRenderer.renderMode = _particleData.renderMode;
        _particleSystemRenderer.SetMeshes(_particleData.meshes, _particleData.meshes.Length);// = _particleData.mesh.Count;
        _particleSystemRenderer.meshDistribution = ParticleSystemMeshDistribution.UniformRandom;
        _particleSystemRenderer.material = _particleData.material;
        //_particleSystemRenderer.material.color = _particleData.colorOverLifetime.color;
        //_particleSystemRenderer.material.e = _particleData.colorOverLifetime.color;
        shape.rotation = _particleData.rotation;

        //COL.color = _particleData.colorOverLifetime;
        //if (_particleData.colorOverLifetime.GetType() == typeof(Color))
        if (_particleData.color.mode == ParticleSystemGradientMode.Color)
        {
            COL.enabled = false;
            main.startColor = _particleData.color;
        }
        else if (_particleData.color.mode == ParticleSystemGradientMode.Gradient)
        {
            COL.enabled = true;
            COL.color = _particleData.color;
        }

        //COL.enabled = true;
        //COL.color = _particleData.colorOverLifetime;
        //fOL.y = new ParticleSystem.MinMaxCurve(_particleData.MinMaxAABB);
        //_particleData.MinMaxAABB.Min, _particleData.MinMaxAABB.Max;
        //module.yMultiplier = _particleData.MinMaxAABB.y;
        //_particleSystem.forceOverLifetime = 
        //_particleSystem.GetComponent<ParticleSystemModule>
        //m = 
        //_particleSystem.re
        //_particleSystem. = _particleData.color;
        //_particleSystem.startLifetime = _particleData.lifeTime;

        _particleSystem.Play();
    }

    private void SetParticleSizes(ParticleSystem.MainModule main)
    {
        main.startSize3D = true;
        main.startSizeX = new ParticleSystem.MinMaxCurve(_currentType.startSize.Min.x, _currentType.startSize.Max.x);
        main.startSizeY = new ParticleSystem.MinMaxCurve(_currentType.startSize.Min.y, _currentType.startSize.Max.y);
        main.startSizeZ = new ParticleSystem.MinMaxCurve(_currentType.startSize.Min.z, _currentType.startSize.Max.z);

        var sizeOverLifetime = _particleSystem.sizeOverLifetime;

        if (_currentType.sizeOverLifetime.mode == ParticleSystemCurveMode.Constant)
        {
            sizeOverLifetime.enabled = false;
            return;
        }

        sizeOverLifetime.size = _currentType.sizeOverLifetime;
        //sizeOverLifetime.
        sizeOverLifetime.enabled = true;
        //sizeOverLifetime.sizeen
    }
    private void OnBecameInvisible()
    {
        StopAllCoroutines();
        DisableParticle();
    }

    public void DisableParticle()
    {
        if ( _particleSystem != null  && _particleSystem.isPlaying)
            _particleSystem.Stop();

        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource.enabled = false;
        }
    }
    private void OnBecameVisible()
    {
        StopAllCoroutines();
        EnableParticle();
    }
    public void EnableParticle()
    {
        if (_particleSystem != null && !_particleSystem.isPlaying)
            _particleSystem.Play();

        if (_audioSource != null)
        {
            _audioSource.enabled = true;
            _audioSource.Play();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
