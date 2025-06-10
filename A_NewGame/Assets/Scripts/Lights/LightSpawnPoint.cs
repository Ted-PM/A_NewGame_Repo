using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LightSpawnPoint : MonoBehaviour
{
    public LightTypes _lightType;

    public LightScriptableObjectScript lightData;

    [Range(0f, 100f)]
    public int LikelyHoodOfFlickering;
    private bool willFLicker = false;
    public float timeOff = 1f;
    public float timeOn = 1f;

    private GameObject _newLight = null;
    private AudioSource _newLightAudioSource = null;

    private void Start()
    {
        willFLicker = LightFlickers();
    }
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
            SpawnLight();
        }
    }

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
            UpdateLightData();
            _newLight.name = _lightType.ToString();
            _newLight.SetActive(true);
            _newLight.GetComponent<Light>().enabled = true;
            PlayLightPassiveAudio();
            if (willFLicker)
                StartCoroutine(Flicker());
        }
    }

    private void UpdateLightData()
    {
        Light light = _newLight.GetComponent<Light>();
        
        light.type = lightData.lightType;
        light.color = lightData.lightColor;
        light.intensity = lightData.lightIntensity;
        light.range = lightData.lightRange;
        light.shadowStrength = lightData.shadowStrength;
    }

    private void PlayLightPassiveAudio()
    {
        //AudioSource audioSource = null;
        if (_newLightAudioSource == null && !_newLight.TryGetComponent<AudioSource>(out _newLightAudioSource))
        {
            Debug.LogError("Light Doesn't have audio source!!");
            return;
        }
        if (lightData.passiveLightAudio == null)
        {
            Debug.Log("No light passive audio data!!");
            return;
        }
        _newLightAudioSource.enabled = true;
        _newLightAudioSource.clip = lightData.passiveLightAudio;
        _newLightAudioSource.loop = true;
        _newLightAudioSource.Play();
    }

    private void StopAudio()
    {
        _newLightAudioSource.Stop();
    }

    private void PlayLightToggledAudio(bool playPassive)
    {
        if (_newLightAudioSource == null || lightData.lightToggledAudio == null)
        {
            Debug.Log("No light toggled audio data!!");
            return;
        }

        _newLightAudioSource.Stop();
        _newLightAudioSource.loop = false;
        _newLightAudioSource.clip = lightData.lightToggledAudio;
        _newLightAudioSource.Play();
        if (playPassive) 
            StartCoroutine(WaitUntilToggledAudioDone());
    }

    private IEnumerator WaitUntilToggledAudioDone()
    {
        while (_newLightAudioSource.isPlaying)
        {
            yield return new WaitForFixedUpdate();
        }
        PlayLightPassiveAudio();
    }

    public void DisableLight()
    {
        StopAllCoroutines();
        if (_newLightAudioSource != null)
        {
            _newLightAudioSource.enabled = false;
            _newLightAudioSource = null;
        }
        if (_newLight != null)
        {
            _newLight.SetActive(false);
            _newLight = null;
        }
    }

    private IEnumerator Flicker()
    {
        yield return null;
        Light light = null;

        if (_newLight != null && _newLight.activeInHierarchy)
            light = _newLight.GetComponent<Light>();
            
        while (_newLight != null && _newLight.activeInHierarchy)
        {
            ToggleLight(light);
            //PlayLightToggledAudio(false);
            StopAudio();
            yield return new WaitForSeconds(timeOff);
            ToggleLight(light);
            //PlayLightToggledAudio(true);
            PlayLightPassiveAudio();
            yield return new WaitForSeconds(timeOn);
        }
    }

    private void ToggleLight(Light light)
    {
        if (_newLight != null)
            light.enabled = !light.enabled;
    }

    public bool LightFlickers()
    {
        return Random.Range(0, 100) <= LikelyHoodOfFlickering;
    }
}
