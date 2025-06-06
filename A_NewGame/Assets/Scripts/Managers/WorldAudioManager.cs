using UnityEngine;

public class WorldAudioManager : MonoBehaviour
{
    //private AudioClip _floorAmbiantAudioClip;
    private AudioSource _worldAudioSource;
    //private void Awake()
    //{
    //    if ( _worldAudioSource == null )
    //        _worldAudioSource = gameObject.AddComponent<AudioSource>();
    //}

    //public void SetFloorAmbiantClip( AudioClip floorAmbiantAudioClip )
    //{
    //    _floorAmbiantAudioClip = floorAmbiantAudioClip;
    //}

    public void SetWorldAudioSource(AudioSource worldAudioSource)
    {
        _worldAudioSource = worldAudioSource;
    }

    public void PlayFloorAmbiance(AudioClip floorAmbiantAudioClip)
    {
        if ( _worldAudioSource == null )
        {
            Debug.LogError("Floor Ambiant Clip is Null!!");
            return;
        }

        //Debug.Log("New Floor Audio");
        _worldAudioSource.Stop();
        _worldAudioSource.clip = floorAmbiantAudioClip;
        _worldAudioSource.loop = true;
        _worldAudioSource.Play();
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
