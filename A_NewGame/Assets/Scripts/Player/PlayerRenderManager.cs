using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class PlayerRenderManager : MonoBehaviour
{
    private PlayerController _playerController;
    private FogData_ScriptableObjectScript currentData = null;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public void UpdatePlayerRenders(FogData_ScriptableObjectScript newFogData)
    {
        if (newFogData == null || _playerController == null)
            return;

        if (currentData != null && currentData == newFogData)
            return;

        currentData = newFogData;
        UpdateFogRenderer();
        UpdatePlayerSettings();
    }

    private void UpdateFogRenderer()
    {
        if (!currentData.useFog)
        {
            RenderSettings.fog = false;
            return;
        }

        RenderSettings.fogColor = currentData.fogColor;
        RenderSettings.fogMode = currentData.fogMode;

        if (currentData.fogMode == FogMode.Linear)
        {
            RenderSettings.fogStartDistance = currentData.fogStartDistance;
            RenderSettings.fogEndDistance = currentData.fogEndDistance;
        }
        else
        {
            RenderSettings.fogDensity = currentData.fogDensity;
        }

        return;
    }

    private void UpdatePlayerSettings()
    {
        _playerController.UpdateCameraBackgroundColor(currentData.camBGColor);
        _playerController.ToggleFlashLight(currentData.flashLightEnabled);
    }
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
