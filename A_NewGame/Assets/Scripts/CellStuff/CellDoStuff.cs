using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CellDoStuff : MonoBehaviour
{
    private Collider _collider;

    public GameObject thingToDestroy;
    public List<CellDoor> cellDoors;
    //public bool doDoorStuff = false;
    public float timetoToDoDoorStuff = 1f;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.gameObject == null)
            Debug.LogError("No collider found (CellDoStuff)");

        if (other.gameObject.tag == "Player")
        {
            _collider.enabled = false;
            DoStuff();
        }
    }
    public void DoStuff()
    {
        _collider.enabled = false;

        if (cellDoors != null && cellDoors.Count > 0)
        {
            StartCoroutine(WaitThenCloseDoor());
            //cellDoor.InteractWithDoor(transform.position);
        }
        else if (thingToDestroy != null)
        {
            StartCoroutine(WaitThenDestroyThing());
        }
        //switch (stuffID)
        //{
        //    case 0:
        //        Stuff1();
        //        break;
        //    case 1:
        //        break;
        //    default:
        //        break;
        //}
    }


    private IEnumerator WaitThenCloseDoor()
    {
        foreach (CellDoor cellDoor in cellDoors)
            cellDoor.InteractWithDoor(transform.position);

        yield return new WaitForSeconds(timetoToDoDoorStuff);
        foreach (CellDoor cellDoor in cellDoors)
            cellDoor.InteractWithDoor(transform.position);
        yield return new WaitForSeconds(0.1f);
        Destroy(thingToDestroy);
    }

    private IEnumerator WaitThenDestroyThing()
    {
        yield return new WaitForSeconds(1f);
        Destroy(thingToDestroy);
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
