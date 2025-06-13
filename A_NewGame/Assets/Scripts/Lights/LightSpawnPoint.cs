using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LightSpawnPoint : MonoBehaviour
{
    public LightTypes _lightType;

    public LightScriptableObjectScript lightData;


    private bool willFLicker = false;


    private GameObject _newLight = null;
    //private AudioSource _newLightAudioSource = null;

    //public Renderer _newLightRenderer = null;

    //private bool _waitingToTurnOff = false;

    private void Start()
    {
        //_newLightRenderer = this.GetComponentInParent<Renderer>();
        //MakeMeshSizeOfLightRange();
        willFLicker = LightFlickers();
    }

    //private void MakeMeshSizeOfLightRange()
    //{
    //    if (lightData != null)
    //        transform.localScale = new Vector3(lightData.lightRange, lightData.lightRange, lightData.lightRange);
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogError("Door spawn point collison is null!!");
            return;
        }
        else if (_newLight == null || !_newLight.activeInHierarchy)
        {
            //Debug.Log("Collision w enemy at: " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
            //Debug.Log("Distance between player & enemy: " + Vector3.Distance(transform.position, other.gameObject.transform.position));
            //_doorSpawnCollider.enabled = false;
            _newLight = null;
            StartCoroutine(TryGetNewLight());
            //SpawnLight();
        }
    }

    //private void OnBecameVisible()
    //{
    //    if (_newLight != null && _newLight.activeInHierarchy && !_newLight.GetComponent<Light>().enabled)
    //    {
    //        _newLight.GetComponent<Light>().enabled = true;      
    //    }
    //}

    //private IEnumerator WaitThenDisableLight()
    //{
    //    yield return new WaitForSeconds(0.75f);

    //    if (!this.GetComponent<Renderer>().isVisible)
    //    {
    //        _newLight.GetComponent<Light>().enabled = false;
    //    }

    //    _waitingToTurnOff = false;
    //}

    //private void OnBecameInvisible()
    //{
    //    if (_newLight != null && _newLight.activeInHierarchy && _newLight.GetComponent<Light>().enabled && !_waitingToTurnOff)
    //    {
    //        _waitingToTurnOff = true;
    //        StartCoroutine(WaitThenDisableLight());
    //        //_newLight.GetComponent<Light>().enabled = false;
    //    }
    //}

    //private void DisableShadows()
    //{
    //    if (_newLightRenderer != null && !_newLightRenderer.isVisible && _newLight.GetComponent<Light>().enabled)
    //        _newLight.GetComponent<Light>().enabled = false;
    //    else if (_newLightRenderer != null && _newLightRenderer.isVisible && !_newLight.GetComponent<Light>().enabled)
    //        _newLight.GetComponent<Light>().enabled = true;

    //}
    private void OnTriggerExit(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogError("Door Wall other is null!!");
        }
        DisableLight();
    }

    private void SpawnLight()
    {
        _newLight = LightSpawner.Instance.GetLightFromPool();

        if (_newLight == null)
        {
            Debug.Log("Light pool empty!!");
            return;
        }
        else
        {
            _newLight.transform.position = transform.position;
            _newLight.transform.rotation = transform.rotation;
            //UpdateLightData();
            _newLight.name = _lightType.ToString();
            _newLight.SetActive(true);
            _newLight.GetComponent<Light>().enabled = true;
            _newLight.GetComponent<LightController>().UpdateLightData(lightData, willFLicker);
            //_newLightRenderer = _newLight.GetComponent<Renderer>();
            //PlayLightPassiveAudio();
            //if (willFLicker)
            //    StartCoroutine(Flicker());
        }
    }

    private IEnumerator TryGetNewLight()
    {
        yield return null;

        while (_newLight == null)
        {
            SpawnLight();
            yield return new WaitForFixedUpdate();
        }
    }

    //private void UpdateLightData()
    //{
    //    //LightController

    //    //Light light = _newLight.GetComponent<Light>();

    //    //light.type = lightData.lightType;
    //    //light.color = lightData.lightColor;
    //    //light.intensity = lightData.lightIntensity;
    //    //light.range = lightData.lightRange;
    //    //light.shadowStrength = lightData.shadowStrength;

    //    //if (lightData.bakeLight)
    //    //    light.lightmapBakeType = LightmapBakeType.Baked;
    //    //else
    //    //    light.lightmapBakeType = LightmapBakeType.Mixed;
    //}

    //private void PlayLightPassiveAudio()
    //{
    //    //AudioSource audioSource = null;
    //    if (_newLightAudioSource == null && !_newLight.TryGetComponent<AudioSource>(out _newLightAudioSource))
    //    {
    //        Debug.LogError("Light Doesn't have audio source!!");
    //        return;
    //    }
    //    if (lightData.passiveLightAudio == null)
    //    {
    //        Debug.Log("No light passive audio data!!");
    //        return;
    //    }
    //    _newLightAudioSource.enabled = true;
    //    _newLightAudioSource.clip = lightData.passiveLightAudio;
    //    _newLightAudioSource.loop = true;
    //    _newLightAudioSource.Play();
    //}

    //private void StopAudio()
    //{
    //    _newLightAudioSource.Stop();
    //}

    //private void PlayLightToggledAudio(bool playPassive)
    //{
    //    if (_newLightAudioSource == null || lightData.lightToggledAudio == null)
    //    {
    //        Debug.Log("No light toggled audio data!!");
    //        return;
    //    }

    //    _newLightAudioSource.Stop();
    //    _newLightAudioSource.loop = false;
    //    _newLightAudioSource.clip = lightData.lightToggledAudio;
    //    _newLightAudioSource.Play();
    //    if (playPassive) 
    //        StartCoroutine(WaitUntilToggledAudioDone());
    //}

    //private IEnumerator WaitUntilToggledAudioDone()
    //{
    //    while (_newLightAudioSource.isPlaying)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }
    //    PlayLightPassiveAudio();
    //}

    public void DisableLight()
    {
        StopAllCoroutines();
        //if (_newLightAudioSource != null)
        //{
        //    _newLightAudioSource.enabled = false;
        //    _newLightAudioSource = null;
        //}
        if (_newLight != null)
        {
            _newLight.GetComponent<LightController>().DisableLight();
            _newLight.SetActive(false);
            _newLight = null;
        }
    }

    //private IEnumerator Flicker()
    //{
    //    yield return null;
    //    Light light = null;

    //    if (_newLight != null && _newLight.activeInHierarchy)
    //        light = _newLight.GetComponent<Light>();

    //    while (_newLight != null && _newLight.activeInHierarchy)
    //    {
    //        ToggleLight(light);
    //        //PlayLightToggledAudio(false);
    //        StopAudio();
    //        yield return new WaitForSeconds(lightData.flickerTimeOff);
    //        ToggleLight(light);
    //        //PlayLightToggledAudio(true);
    //        PlayLightPassiveAudio();
    //        yield return new WaitForSeconds(lightData.flickerTimeOn);
    //    }
    //}

    //private void ToggleLight(Light light)
    //{
    //    if (_newLight != null)
    //        light.enabled = !light.enabled;
    //}

    //private void OnEnable()
    //{
    //    if ( _newLight != null )
    //        DisableLight();        
    //}

    //private void OnDisable()
    //{
    //    if ( _newLight != null )
    //        DisableLight();
    //}

    public bool LightFlickers()
    {
        return Random.Range(0, 100) < lightData.LikelyHoodOfFlickering;
    }
}
