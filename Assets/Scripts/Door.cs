using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    public GameObject nearCell1;
    public Cell.BlockDirection nearCell1DoorBlockDirection;
    public GameObject nearCell2;
    public Cell.BlockDirection nearCell2DoorBlockDirection;
    public bool isClosed = true;
    public float openAngle;
    public bool isClosedOnKey = true;
    public Sprite closedOnKeySprite;
    public Sprite openedSprite;
    private bool previousIsClosedOnKey;
    // Start is called before the first frame update
    void Start()
    {
        if (isClosedOnKey)
        {
            GetComponent<SpriteRenderer>().sprite = closedOnKeySprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
        previousIsClosedOnKey = isClosedOnKey;
    }

    // Update is called once per frame
    void Update()
    {
        if(previousIsClosedOnKey != isClosedOnKey)
        {
            previousIsClosedOnKey = isClosedOnKey;
            if (isClosedOnKey)
            {
                GetComponent<SpriteRenderer>().sprite = closedOnKeySprite;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = openedSprite;
            }
        }
        //transform.parent.localEulerAngles = new Vector3(0, 0, openAngle);
    }

    public void Open()
    {
        if(nearCell1.GetComponent<Cell>().isCurrentPlayerPosition || nearCell2.GetComponent<Cell>().isCurrentPlayerPosition)
        {
            isClosed = false;
            transform.parent.localEulerAngles = new Vector3(0, 0, openAngle);
            nearCell1.GetComponent<Cell>().blockDirections.Remove(nearCell1DoorBlockDirection);
            nearCell2.GetComponent<Cell>().blockDirections.Remove(nearCell2DoorBlockDirection);
        }
    }

    public void Close()
    {
        if (nearCell1.GetComponent<Cell>().isCurrentPlayerPosition || nearCell2.GetComponent<Cell>().isCurrentPlayerPosition)
        {
            isClosed = true;
            transform.parent.localEulerAngles = new Vector3(0, 0, 0);
            nearCell1.GetComponent<Cell>().blockDirections.Add(nearCell1DoorBlockDirection);
            nearCell2.GetComponent<Cell>().blockDirections.Add(nearCell2DoorBlockDirection);
        }
    }

    public bool isPlayerNearDoor()
    {
        return nearCell1.GetComponent<Cell>().isCurrentPlayerPosition || nearCell2.GetComponent<Cell>().isCurrentPlayerPosition;
    }
}
