using UnityEngine;

public class CustomCellFloor : MonoBehaviour
{
    public Camera floorCam;
    public RenderTexture floorRT;
    private RenderTexture myText;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (floorCam == null)
            Debug.LogError("Cam Not Set (cust floor)!!");
        else
            floorCam.enabled = false;
        if (floorRT != null)
        {
            myText = Instantiate(floorRT);
            floorCam.targetTexture = myText;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("other is null!");
            return;
        }

        floorCam.enabled = true;


    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("other is null!");
            return;
        }

        floorCam.enabled = false;
    }
}
