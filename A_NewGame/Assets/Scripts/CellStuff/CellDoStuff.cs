using System.Collections;
using UnityEngine;

public class CellDoStuff : MonoBehaviour
{
    public CellDoor cellDoor;
    public GameObject thingToDestroy;
    public Collider _collider;

    public bool doDoorStuff = false;
    public float timetoToDoDoorStuff = 1f;
    public void DoStuff(int stuffID)
    {
        _collider.enabled = false;

        if (cellDoor != null && doDoorStuff)
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

    private void Stuff1()
    {
        if (cellDoor != null)
        {
            StartCoroutine(WaitThenCloseDoor());
            //cellDoor.InteractWithDoor(transform.position);
        }
    }

    private IEnumerator WaitThenCloseDoor()
    {
        cellDoor.InteractWithDoor(transform.position);
        yield return new WaitForSeconds(timetoToDoDoorStuff);
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
