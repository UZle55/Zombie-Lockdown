using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public enum ZombieType
    {
        first,
        second, 
        third
    }
    public ZombieType type;
    public bool isExample;
    public int currentHealth = 15;
    public int maxHealth = 15;
    private bool isMoving = false;
    public GameObject currentCell;
    private GameObject[,] map;
    private float t = 0;
    private bool isWayFounded = false;
    private List<List<GameObject>> waysToCell = new List<List<GameObject>>();
    private List<GameObject> bestWay = new List<GameObject>();
    private float timeToWait = 0;
    private int currentTargetCellIndex = 1;
    public GameObject mapObject;
    public bool isEndMoving = false;
    public bool isStartMoving = false;
    public int maxStepsCount = 3;
    private int zombieChasingDistance = 5;
    private int currentStepsCount = 0;
    public GameObject player;
    public GameObject slider;
    public bool isCanBeAttackedByRangeWeapon = false;
    public bool isCanBeAttackedByMeleeWeapon = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        slider.GetComponent<Slider>().value = (float)currentHealth / maxHealth;

        t += Time.deltaTime;

        if (isMoving && isWayFounded && timeToWait == 0 && Manager.PlayerState == Manager.PlayerGameState.Finishing)
        {
            if(currentStepsCount < maxStepsCount && bestWay.Count - currentStepsCount > 2)
            {
                var newStart = new Vector3(currentCell.transform.position.x, currentCell.transform.position.y, -1);
                var newEnd = new Vector3(bestWay[currentTargetCellIndex].transform.position.x, bestWay[currentTargetCellIndex].transform.position.y, -1);
                Flip(currentCell, bestWay[currentTargetCellIndex]);
                transform.position = Vector3.Lerp(newStart, newEnd, t * 2);


                if (Mathf.Abs(transform.position.x - newEnd.x) < 0.01f && Mathf.Abs(transform.position.y - newEnd.y) < 0.01f)
                {
                    currentCell.GetComponent<Cell>().isWithZombie = false;
                    currentCell.GetComponent<Cell>().zombieOnThatCell = null;
                    currentCell = bestWay[currentTargetCellIndex];
                    currentCell.GetComponent<Cell>().isWithZombie = true;
                    currentCell.GetComponent<Cell>().zombieOnThatCell = this.gameObject;
                    currentTargetCellIndex++;
                    currentStepsCount++;
                    if (currentTargetCellIndex == bestWay.Count)
                    {
                        
                    }
                    timeToWait = 0.25f;
                }
            }
            else if (bestWay.Count - currentStepsCount == 2)
            {
                isMoving = false;
                isWayFounded = false;
                timeToWait = 0;
                t = 0;
                currentTargetCellIndex = 1;
                Invoke("AttackPlayer", 1f);
            }
            else
            {
                isMoving = false;
                isWayFounded = false;
                timeToWait = 0;
                t = 0;
                currentTargetCellIndex = 1;
                isEndMoving = true;
                isStartMoving = false;
            }

        }
        if (timeToWait > 0)
        {
            timeToWait -= Time.deltaTime;
            if (timeToWait < 0)
            {
                timeToWait = 0;
                t = 0;
            }
        }


    }

    private void Flip(GameObject cellStart, GameObject cellEnd)
    {
        var startX = cellStart.GetComponent<Cell>().indexX;
        var startY = cellStart.GetComponent<Cell>().indexY;
        var endX = cellEnd.GetComponent<Cell>().indexX;
        var endY = cellEnd.GetComponent<Cell>().indexY;
        if(type == ZombieType.first || type == ZombieType.third)
        {
            if (startX > endX)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (startX < endX)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        else
        {
            if (startX < endX)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (startX > endX)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        
    }

    private void AttackPlayer()
    {
        if(type == ZombieType.first)
        {
            player.GetComponent<Player>().DecreaseHealth(10);
        }
        else if(type == ZombieType.second)
        {
            player.GetComponent<Player>().DecreaseHealth(5);
        }
        else if(type == ZombieType.third)
        {
            player.GetComponent<Player>().DecreaseHealth(25);
        }
        isMoving = false;
        isWayFounded = false;
        timeToWait = 0;
        t = 0;
        currentTargetCellIndex = 1;
        isEndMoving = true;
        isStartMoving = false;
    }

    private void FindCell(int stepsCount, GameObject cell, List<GameObject> sequence)
    {
        var newSequence = new List<GameObject>();
        newSequence.AddRange(sequence);
        newSequence.Add(cell);
        if (cell.GetComponent<Cell>().isCurrentPlayerPosition)
        {
            waysToCell.Add(newSequence);
        }
        else if (stepsCount > 0 && !cell.GetComponent<Cell>().isWithZombie && !cell.GetComponent<Cell>().isWithCard)
        {
            stepsCount--;
            //cell.GetComponent<Cell>().HighLight();
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Right))
            {
                FindCell(stepsCount, map[cell.GetComponent<Cell>().indexX + 1, cell.GetComponent<Cell>().indexY], newSequence);
            }
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Left))
            {
                FindCell(stepsCount, map[cell.GetComponent<Cell>().indexX - 1, cell.GetComponent<Cell>().indexY], newSequence);
            }
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Up))
            {
                FindCell(stepsCount, map[cell.GetComponent<Cell>().indexX, cell.GetComponent<Cell>().indexY + 1], newSequence);
            }
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Down))
            {
                FindCell(stepsCount, map[cell.GetComponent<Cell>().indexX, cell.GetComponent<Cell>().indexY - 1], newSequence);
            }
        }
    }

    public void MoveToCell(GameObject[,] map)
    {
        currentStepsCount = 0;
        isEndMoving = false;
        isStartMoving = true;
        isMoving = true;
        this.map = map;
        waysToCell.Clear();
        var sequence = new List<GameObject>();
        sequence.Add(currentCell);
        if (!currentCell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Right))
        {
            FindCell(zombieChasingDistance, map[currentCell.GetComponent<Cell>().indexX + 1, currentCell.GetComponent<Cell>().indexY], sequence);
        }
        if (!currentCell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Left))
        {
            FindCell(zombieChasingDistance, map[currentCell.GetComponent<Cell>().indexX - 1, currentCell.GetComponent<Cell>().indexY], sequence);
        }
        if (!currentCell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Up))
        {
            FindCell(zombieChasingDistance, map[currentCell.GetComponent<Cell>().indexX, currentCell.GetComponent<Cell>().indexY + 1], sequence);
        }
        if (!currentCell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Down))
        {
            FindCell(zombieChasingDistance, map[currentCell.GetComponent<Cell>().indexX, currentCell.GetComponent<Cell>().indexY - 1], sequence);
        }
        bestWay.Clear();
        var minCount = int.MaxValue;
        foreach (var way in waysToCell)
        {
            if (way.Count < minCount)
            {
                minCount = way.Count;
            }
        }
        foreach (var way in waysToCell)
        {
            if (way.Count == minCount)
            {
                bestWay.AddRange(way);
                break;
            }
        }

        isWayFounded = true;
        t = 0;

    }

    public void IncreaseHealth(int value)
    {
        currentHealth += value;
        //UpdateInfectionAndHealth();
    }

    public void DecreaseHealth(int value)
    {
        currentHealth -= value;
        if (currentHealth <= 0)
        {
            Die();
        }
        //UpdateInfectionAndHealth();
    }

    public void UpdateInfectionAndHealth()
    {
        //interface1.GetComponent<Interface>().SetInfection(GetInfection());
        //interface1.GetComponent<Interface>().SetHealth(GetHealth());
    }

    public void Die()
    {
        currentCell.GetComponent<Cell>().isWithZombie = false;
        currentCell.GetComponent<Cell>().zombieOnThatCell = null;
        mapObject.GetComponent<Map>().enemies.Remove(this.gameObject);
        mapObject.GetComponent<Map>().HighLightCells(1);
        GameObject.Destroy(gameObject);
    }

    public int GetHealth()
    {
        return currentHealth;
    }
}
