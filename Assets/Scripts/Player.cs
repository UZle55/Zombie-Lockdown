using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private bool isCanGo;
    public int currentInfection = 25;
    public int currentHealth = 75;
    private bool isMoving = false;
    public GameObject currentCell;
    private GameObject endCell;
    public GameObject[,] map;
    private float t = 0;
    private bool isWayFounded = false;
    private List<List<GameObject>> waysToCell = new List<List<GameObject>>();
    private List<GameObject> bestWay = new List<GameObject>();
    private float timeToWait = 0;
    private int currentTargetCellIndex = 1;
    public GameObject interface1;
    public GameObject mapObject;
    public bool canMeleeAttack = false;
    public List<GameObject> zombiesCanBeAttacked = new List<GameObject>();
    private Text info;
    public GameObject inventory;
    // Start is called before the first frame update
    void Start()
    {
        info = GameObject.Find("Info").GetComponent<Text>();
        Manager.player = this.gameObject;
        interface1.GetComponent<Interface>().SetInfection(GetInfection());
        interface1.GetComponent<Interface>().SetHealth(GetHealth());
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        
        if(isMoving && isWayFounded && timeToWait == 0 && Manager.PlayerState == Manager.PlayerGameState.Moving)
        {
            var newStart = new Vector3(currentCell.transform.position.x, currentCell.transform.position.y, -1);
            var newEnd = new Vector3(bestWay[currentTargetCellIndex].transform.position.x, bestWay[currentTargetCellIndex].transform.position.y, -1);
            Flip(currentCell, bestWay[currentTargetCellIndex]);
            transform.position = Vector3.Lerp(newStart, newEnd, t * 2);
            info.text = transform.position.x + " " + transform.position.y + " " + transform.position.z;

            if (Mathf.Abs(transform.position.x - newEnd.x) < 0.01f && Mathf.Abs(transform.position.y - newEnd.y) < 0.01f)
            {
                
                currentCell = bestWay[currentTargetCellIndex];
                
                currentTargetCellIndex++;
                if(currentTargetCellIndex == bestWay.Count)
                {
                    var infection = int.Parse(mapObject.GetComponent<Map>().cellWithPlayer.transform.Find("InfectionNumberCanvas")
                        .transform.Find("InfectionNumber_Cell").gameObject.GetComponent<Text>().text);
                    IncreaseInfection(infection);
                    interface1.GetComponent<Interface>().SetInfection(GetInfection());
                    interface1.GetComponent<Interface>().SetHealth(GetHealth());
                    mapObject.GetComponent<Map>().cellWithPlayer.transform.Find("InfectionNumberCanvas")
                        .transform.Find("InfectionNumber_Cell").gameObject.GetComponent<Text>().text = "0";

                    isMoving = false;
                    isWayFounded = false;
                    timeToWait = 0;
                    t = 0;
                    
                    currentTargetCellIndex = 1;
                    Manager.DecreasePlayerMovesCount();
                    if (Manager.PlayerMovesCount > 0)
                    {
                        mapObject.GetComponent<Map>().HighLightCells(1);
                    }
                    else
                    {
                        Manager.StartFinishing();
                    }
                    interface1.GetComponent<Interface>().movesCount.GetComponent<Text>().text = Manager.PlayerMovesCount.ToString();
                    Manager.EndMoving();
                }

                timeToWait = 0.25f;
            }
            
        }
        if(timeToWait > 0)
        {
            timeToWait -= Time.deltaTime;
            if(timeToWait < 0)
            {
                timeToWait = 0;
                t = 0;
            }
        }

        
    }

    public void CheckZombies(int stepsCount, GameObject cell)
    {
        foreach (var zombie in zombiesCanBeAttacked)
        {
            zombie.GetComponent<Enemy>().isCanBeAttackedByMeleeWeapon = false;
            zombie.GetComponent<Enemy>().isCanBeAttackedByRangeWeapon = false;
        }
        zombiesCanBeAttacked.Clear();
        if (stepsCount > 0)
        {
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Right))
            {
                CheckZombie(stepsCount, map[cell.GetComponent<Cell>().indexX + 1, cell.GetComponent<Cell>().indexY]);
            }
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Left))
            {
                CheckZombie(stepsCount, map[cell.GetComponent<Cell>().indexX - 1, cell.GetComponent<Cell>().indexY]);
            }
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Up))
            {
                CheckZombie(stepsCount, map[cell.GetComponent<Cell>().indexX, cell.GetComponent<Cell>().indexY + 1]);
            }
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Down))
            {
                CheckZombie(stepsCount, map[cell.GetComponent<Cell>().indexX, cell.GetComponent<Cell>().indexY - 1]);
            }
        }
    }

    public void CheckZombie(int stepsCount, GameObject cell)
    {
        if (stepsCount > 0)
        {
            stepsCount--;
            if (cell.GetComponent<Cell>().isWithZombie && stepsCount == 2)
            {
                cell.GetComponent<Cell>().zombieOnThatCell.GetComponent<Enemy>().isCanBeAttackedByMeleeWeapon = true;
                cell.GetComponent<Cell>().zombieOnThatCell.GetComponent<Enemy>().isCanBeAttackedByRangeWeapon = true;
                zombiesCanBeAttacked.Add(cell.GetComponent<Cell>().zombieOnThatCell);
            }
            else if (cell.GetComponent<Cell>().isWithZombie && stepsCount < 2)
            {
                cell.GetComponent<Cell>().zombieOnThatCell.GetComponent<Enemy>().isCanBeAttackedByRangeWeapon = true;
                zombiesCanBeAttacked.Add(cell.GetComponent<Cell>().zombieOnThatCell);
            }
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Right))
            {
                CheckZombie(stepsCount, map[cell.GetComponent<Cell>().indexX + 1, cell.GetComponent<Cell>().indexY]);
            }
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Left))
            {
                CheckZombie(stepsCount, map[cell.GetComponent<Cell>().indexX - 1, cell.GetComponent<Cell>().indexY]);
            }
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Up))
            {
                CheckZombie(stepsCount, map[cell.GetComponent<Cell>().indexX, cell.GetComponent<Cell>().indexY + 1]);
            }
            if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Down))
            {
                CheckZombie(stepsCount, map[cell.GetComponent<Cell>().indexX, cell.GetComponent<Cell>().indexY - 1]);
            }
        }
    }

    private void Flip(GameObject cellStart, GameObject cellEnd)
    {
        var startX = cellStart.GetComponent<Cell>().indexX;
        var startY = cellStart.GetComponent<Cell>().indexY;
        var endX = cellEnd.GetComponent<Cell>().indexX;
        var endY = cellEnd.GetComponent<Cell>().indexY;
        if(startX > endX)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (startX < endX)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private void FindCell(int stepsCount, GameObject cell, List<GameObject> sequence)
    {
        var newSequence = new List<GameObject>();
        newSequence.AddRange(sequence);
        newSequence.Add(cell);
        if (cell.name.Equals(endCell.name) && stepsCount == 3)
        {
            waysToCell.Add(newSequence);
        }
        else if (stepsCount > 0)
        {
            stepsCount--;
            if (cell.GetComponent<Cell>().isWithZombie && stepsCount == 2)
            {
                cell.GetComponent<Cell>().zombieOnThatCell.GetComponent<Enemy>().isCanBeAttackedByMeleeWeapon = true;
                cell.GetComponent<Cell>().zombieOnThatCell.GetComponent<Enemy>().isCanBeAttackedByRangeWeapon = true;
                zombiesCanBeAttacked.Add(cell.GetComponent<Cell>().zombieOnThatCell);
            }
            else if(cell.GetComponent<Cell>().isWithZombie && stepsCount < 2)
            {
                cell.GetComponent<Cell>().zombieOnThatCell.GetComponent<Enemy>().isCanBeAttackedByRangeWeapon = true;
                zombiesCanBeAttacked.Add(cell.GetComponent<Cell>().zombieOnThatCell);
            }
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

    public void FastMoveToCell(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, -1);
    }

    public void MoveToCell(GameObject startCell, GameObject endCell)
    {
        //zombiesCanBeAttacked.Clear();
        //CheckZombies(3, currentCell);
        /*foreach (var zombie in zombiesCanBeAttacked)
        {
            zombie.GetComponent<Enemy>().isCanBeAttackedByMeleeWeapon = false;
            zombie.GetComponent<Enemy>().isCanBeAttackedByRangeWeapon = false;
        }
        zombiesCanBeAttacked.Clear();*/
        isMoving = true;
        canMeleeAttack = false;
        this.currentCell = startCell;
        this.endCell = endCell;
        waysToCell.Clear();
        var sequence = new List<GameObject>();
        sequence.Add(startCell);
        if (!startCell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Right))
        {
            FindCell(3, map[startCell.GetComponent<Cell>().indexX + 1, startCell.GetComponent<Cell>().indexY], sequence);
        }
        if (!startCell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Left))
        {
            FindCell(3, map[startCell.GetComponent<Cell>().indexX - 1, startCell.GetComponent<Cell>().indexY], sequence);
        }
        if (!startCell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Up))
        {
            FindCell(3, map[startCell.GetComponent<Cell>().indexX, startCell.GetComponent<Cell>().indexY + 1], sequence);
        }
        if (!startCell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Down))
        {
            FindCell(3, map[startCell.GetComponent<Cell>().indexX, startCell.GetComponent<Cell>().indexY - 1], sequence);
        }
        bestWay.Clear();
        var minCount = int.MaxValue;
        foreach(var way in waysToCell)
        {
            if(way.Count < minCount)
            {
                minCount = way.Count;
            }
        }
        foreach(var way in waysToCell)
        {
            if(way.Count == minCount)
            {
                bestWay.AddRange(way);
                break;
            }
        }

        isWayFounded = true;
        t = 0;

    }

    public void SetMovesCount(int count)
    {
        Manager.IncreasePlayerMovesCount(count);
        isCanGo = true;
        mapObject.GetComponent<Map>().HighLightCells(1);
    }

    public void IncreaseHealth(int value)
    {
        currentHealth += value;
        if(currentHealth > 100 - currentInfection)
        {
            currentHealth = 100 - currentInfection;
        }
        UpdateInfectionAndHealth();
    }

    public void DecreaseHealth(int value)
    {
        currentHealth -= value;
        if(currentHealth <= 0)
        {
            Die();
        }
        UpdateInfectionAndHealth();
    }

    public void IncreaseInfection(int value)
    {
        currentInfection += value;
        if(currentInfection > 100 - currentHealth)
        {
            currentHealth = 100 - currentInfection;
        }
        UpdateInfectionAndHealth();
    }

    public void DecreaseInfection(int value)
    {
        currentInfection -= value;
        if(currentInfection < 0)
        {
            currentInfection = 0;
        }
        UpdateInfectionAndHealth();
    }

    public void UpdateInfectionAndHealth()
    {
        interface1.GetComponent<Interface>().SetInfection(GetInfection());
        interface1.GetComponent<Interface>().SetHealth(GetHealth());
    }

    public void Die()
    {

    }

    public int GetInfection()
    {
        return currentInfection;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void Attack(GameObject zombie, string weaponName)
    {
        if(weaponName.Equals("bat"))
            zombie.GetComponent<Enemy>().DecreaseHealth(8);
        if (weaponName.Equals("knife"))
            zombie.GetComponent<Enemy>().DecreaseHealth(4);
        if (weaponName.Equals("fists"))
            zombie.GetComponent<Enemy>().DecreaseHealth(2);
        if (weaponName.Equals("revolver"))
        {
            zombie.GetComponent<Enemy>().DecreaseHealth(16);
            inventory.GetComponent<Inventory>().UpdateRevolverAmmoCount();
        }
            
    }
}
