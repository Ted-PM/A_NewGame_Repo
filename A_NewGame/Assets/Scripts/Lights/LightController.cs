using System.Collections;
using UnityEngine;

[RequireComponent (typeof(Light), typeof(Renderer))]
public class LightController : MonoBehaviour
{
    public Light _light;
    public Renderer _renderer;
    public AudioSource _audioSource;

    LightScriptableObjectScript _currentType = null;

    private bool _flicker = false;

    private void Awake()
    {
        if (_light == null)
            _light = GetComponent<Light>();
        if (_renderer == null)
            _renderer = GetComponent<Renderer>();
    }

    public void UpdateLightData(LightScriptableObjectScript newType, bool flicker)
    {
        if (_currentType != null && _currentType.lightPrefabType == newType.lightPrefabType)
            return;
        _light.type = newType.lightType;
        _light.color = newType.lightColor;
        _light.intensity = newType.lightIntensity;
        _light.range = newType.lightRange;
        _renderer.transform.localScale = new Vector3(newType.lightRange, newType.lightRange, newType.lightRange);
        _light.shadowStrength = newType.shadowStrength;

        _flicker = flicker;

        //if (newType.bakeLight)
        //    _light.lightmapBakeType = LightmapBakeType.Baked;
        //else
        //    _light.lightmapBakeType = LightmapBakeType.Mixed;

        _currentType = newType;

        EnableLight();
    }

    private void OnBecameInvisible()
    {
        StopAllCoroutines();
        DisableLight();
        //StartCoroutine(WaitTheReturnLightToPool());
    }

    public void DisableLight()
    {
        _light.enabled = false;

        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource.enabled = false;
        }
    }

    private IEnumerator WaitTheReturnLightToPool()
    {
        float t = 0; 

        while (t<1 && !_light.enabled)
        {
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (t >= 1)
            LightSpawner.Instance.DisableLight(this.gameObject);
            
    }

    private void OnBecameVisible()
    {
        StopAllCoroutines();
        EnableLight();
    }

    private void EnableLight()
    {
        if (_light == null || !this.gameObject.activeInHierarchy)
            return;
        _light.enabled = true;

        if (_audioSource != null)
        {
            _audioSource.enabled = true;
            PlayPassiveAudio();
        }

        if (_flicker)
            StartCoroutine(LightFlicker());
    }

    private void PlayPassiveAudio()
    {
        if (_currentType == null)
            return;
        //AudioSource audioSource = null;
        if (_audioSource == null)
        {
            Debug.LogError("Light Doesn't have audio source!!");
            return;
        }
        if (_currentType.passiveLightAudio == null)
        {
            Debug.Log("No light passive audio data!!");
            return;
        }

        _audioSource.enabled = true;
        _audioSource.clip = _currentType.passiveLightAudio;
        _audioSource.loop = true;
        _audioSource.Play();
    }


    private IEnumerator LightFlicker()
    {
        yield return null;
        while (this.gameObject.activeInHierarchy)
        {
            ToggleLightAndAudio(false);
            yield return new WaitForSeconds(_currentType.flickerTimeOff);
            ToggleLightAndAudio(true);
            yield return new WaitForSeconds(_currentType.flickerTimeOn);
        }

        DisableLight();
    }

    private void ToggleLightAndAudio(bool state)
    {
        if (_light != null)
            _light.enabled = state;

        if (_audioSource != null)
            _audioSource.Stop();// = state;
    }



}
