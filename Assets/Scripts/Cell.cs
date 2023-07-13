using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum BlockDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    // Start is called before the first frame update
    public List<BlockDirection> blockDirections = new List<BlockDirection>();
    public int roundedX;
    public int roundedY;
    public int indexX;
    public int indexY;
    public bool isCurrentPlayerPosition = false;
    public bool isHighLighted { get; private set; } = false;
    public bool isWithCard = false;
    public bool isWithZombie;
    private Color defaultColor;
    public Color highlightColor;
    public GameObject zombieOnThatCell;
    
    void Start()
    {
        defaultColor = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HighLight()
    {
        isHighLighted = true;
        GetComponent<SpriteRenderer>().color = highlightColor;
    }

    public void UnHighLight()
    {
        isHighLighted = false;
        GetComponent<SpriteRenderer>().color = defaultColor;
    }
}
