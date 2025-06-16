using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ParticleSpawnPoint : MonoBehaviour
{
    public ParticleTypes _particleType;

    public Particles_ScriptableObjectScript particleData;

    private GameObject _newParticle = null;



    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogError("Door spawn point collison is null!!");
            return;
        }
        else if (_newParticle == null || !_newParticle.activeInHierarchy)
        {
            _newParticle = null;
            SpawnParticle();
        }
    }

    private void SpawnParticle()
    {
        _newParticle = ParticleSpawner.Instance.GetParticleFromPool();

        if (_newParticle == null)
        {
            Debug.LogWarning("Couldn't get particle from pool!!");
            return;
        }
        else
        {
            _newParticle.transform.position = transform.position;
            _newParticle.transform.rotation = transform.rotation;
            //UpdateLightData();
            _newParticle.name = _particleType.ToString();
            _newParticle.SetActive(true);
            _newParticle.GetComponent<ParticleController>().UpdateParticleData(particleData);

            //_newLight.GetComponent<Light>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogError("Door Wall other is null!!");
        }
        DisableParticle();
    }

    private void DisableParticle()
    {
        if (_newParticle != null)
        {
            _newParticle.GetComponent<ParticleController>().DisableParticle();
            _newParticle.SetActive(false);
            _newParticle = null;
        }
    }
}
