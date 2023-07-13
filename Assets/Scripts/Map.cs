using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    private GameObject[,] map1;
    private GameObject[] cells;
    public GameObject startCell;
    public GameObject player;
    public GameObject cellWithPlayer;
    public GameObject interface1;
    public GameObject inventory;
    public List<GameObject> enemies;
    private int finishedEnmiesCount = 0;
    private bool isOnceEnemyMoved = false;
    // Start is called before the first frame update
    void Start()
    {
        player.GetComponent<Player>().currentCell = startCell;
        cells = GameObject.FindGameObjectsWithTag("Cell");
        var maxX = int.MinValue;
        var maxY = int.MinValue;
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        foreach (var cell in cells)
        {
            var x = (int)Mathf.Round(cell.transform.localPosition.x);
            var y = (int)Mathf.Round(cell.transform.localPosition.y);
            cell.GetComponent<Cell>().roundedX = x;
            cell.GetComponent<Cell>().roundedY = y;
            if(x > maxX)
            {
                maxX = x;
            }
            if(y > maxY)
            {
                maxY = y;
            }
            if(x < minX)
            {
                minX = x;
            }
            if (y < minY)
            {
                minY = y;
            }
        }
        map1 = new GameObject[maxX - minX + 1, maxY - minY + 1];
        foreach(var cell in cells)
        {
            map1[cell.GetComponent<Cell>().roundedX - minX, cell.GetComponent<Cell>().roundedY - minY] = cell;
            cell.GetComponent<Cell>().indexX = cell.GetComponent<Cell>().roundedX - minX;
            cell.GetComponent<Cell>().indexY = cell.GetComponent<Cell>().roundedY - minY;
        }
        player.GetComponent<Player>().map = map1;
        player.GetComponent<Player>().FastMoveToCell(startCell.transform.position);
        //cellWithPlayer.GetComponent<Cell>().isCurrentPlayerPosition = false;
        startCell.GetComponent<Cell>().isCurrentPlayerPosition = true;
        cellWithPlayer = startCell;


    }

    // Update is called once per frame
    void Update()
    {
        if(Manager.PlayerState == Manager.PlayerGameState.Finishing && finishedEnmiesCount < enemies.Count)
        {
            if (!enemies[finishedEnmiesCount].GetComponent<Enemy>().isStartMoving && !isOnceEnemyMoved)
            {
                enemies[finishedEnmiesCount].GetComponent<Enemy>().MoveToCell(map1);
                isOnceEnemyMoved = true;
            }
            else if (enemies[finishedEnmiesCount].GetComponent<Enemy>().isEndMoving && isOnceEnemyMoved)
            {
                finishedEnmiesCount++;
                isOnceEnemyMoved = false;
            }
        }
        else if(finishedEnmiesCount >= enemies.Count && Manager.PlayerState == Manager.PlayerGameState.Finishing && Manager.CanRollDice)
        {
            finishedEnmiesCount = 0;
            Manager.Finished();
        }

        if (Input.GetMouseButtonDown(0) && Manager.PlayerState != Manager.PlayerGameState.Moving && Manager.PlayerState != Manager.PlayerGameState.Collecting)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            //////////////////////////////////////////////////////
            if (hit.transform != null && hit.transform.gameObject.tag.Equals("Cell") && Manager.PlayerMovesCount > 0)
            {
                var cell = hit.transform.gameObject;
                if (cell.GetComponent<Cell>().isHighLighted)
                {
                    Manager.StartMoving();
                    player.GetComponent<Player>().MoveToCell(cellWithPlayer, cell);
                    cellWithPlayer.GetComponent<Cell>().isCurrentPlayerPosition = false;
                    cell.GetComponent<Cell>().isCurrentPlayerPosition = true;
                    cellWithPlayer = cell;

                    UnHighLightCells();
                }
            }
            ////////////////////////////////////////////////////////////
            if (hit.transform != null && hit.transform.gameObject.tag.Equals("Door") && Manager.PlayerMovesCount > 0
                && Manager.PlayerState == Manager.PlayerGameState.CanEverything && hit.transform.gameObject.GetComponent<Door>().isPlayerNearDoor())
            {
                var door = hit.transform.gameObject;
                var isInteracted = false;
                if (door.GetComponent<Door>().isClosed)
                {
                    if (door.GetComponent<Door>().isClosedOnKey)
                    {
                        if (Inventory.CurrentSelectedItemName != null
                            && Inventory.CurrentSelectedItemName.Split("_")[0].Equals("key")
                            && Inventory.CurrentSelectedItemName.Split("_")[1].Equals("door"))
                        {
                            if (Inventory.CurrentSelectedItemName.Split("_")[2].Equals(door.name.Split("_")[1]))
                            {
                                door.GetComponent<Door>().Open();
                                door.GetComponent<Door>().isClosedOnKey = false;
                                inventory.GetComponent<Inventory>().DeleteItemFromInventory(Inventory.CurrentSelectedItemName);
                                isInteracted = true;
                            }
                        }
                    }
                    else
                    {
                        door.GetComponent<Door>().Open();
                        isInteracted = true;
                    }


                }
                else if (!door.GetComponent<Door>().isClosed)
                {
                    isInteracted = true;
                    door.GetComponent<Door>().Close();
                }
                if (isInteracted)
                {
                    UnHighLightCells();
                    HighLightCells(1);
                    Manager.DecreasePlayerMovesCount();
                    interface1.GetComponent<Interface>().movesCount.GetComponent<Text>().text = Manager.PlayerMovesCount.ToString();
                    if (Manager.PlayerMovesCount <= 0)
                    {
                        UnHighLightCells();
                        Manager.StartFinishing();
                    }
                }

            }


            ///////////////////////////////////////////////////////////////////
            if (hit.transform != null && hit.transform.gameObject.tag.Equals("MainPlayer") && Manager.PlayerMovesCount > 0)
            {
                var isInteracted = false;
                if (Inventory.CurrentSelectedItemName != null && Inventory.CurrentSelectedItemName.Equals("medkit"))
                {
                    if (player.GetComponent<Player>().GetHealth() + player.GetComponent<Player>().GetInfection() != 100)
                    {
                        player.GetComponent<Player>().IncreaseHealth(50);
                        inventory.GetComponent<Inventory>().DeleteItemFromInventory(Inventory.CurrentSelectedItemName);
                        isInteracted = true;
                    }

                }

                if (Inventory.CurrentSelectedItemName != null && Inventory.CurrentSelectedItemName.Equals("antidote"))
                {
                    if (player.GetComponent<Player>().GetInfection() != 0)
                    {
                        player.GetComponent<Player>().DecreaseInfection(50);
                        inventory.GetComponent<Inventory>().DeleteItemFromInventory(Inventory.CurrentSelectedItemName);
                        isInteracted = true;
                    }

                }

                if (isInteracted)
                {
                    Manager.DecreasePlayerMovesCount();
                    interface1.GetComponent<Interface>().movesCount.GetComponent<Text>().text = Manager.PlayerMovesCount.ToString();
                    if (Manager.PlayerMovesCount <= 0)
                    {
                        UnHighLightCells();
                        Manager.StartFinishing();
                    }
                }
            }
            //////////////////////////////////////////////////////////////////////////
            if (hit.transform != null && hit.transform.gameObject.tag.Equals("Card") && Manager.PlayerMovesCount > 0)
            {
                var card = hit.transform.gameObject;
                var canCollect = false;
                foreach (var cell in card.GetComponent<Card>().cellsWherePlayerCanCollectCard)
                {
                    if (cellWithPlayer.GetInstanceID().Equals(cell.GetInstanceID()))
                    {
                        canCollect = true;
                        break;
                    }
                }
                if (canCollect)
                {
                    card.GetComponent<Card>().Collect(this.gameObject, inventory, interface1);
                }
            }
            ///////////////////////////////////////
            if (hit.transform != null && hit.transform.gameObject.tag.Equals("Enemy") && Manager.PlayerMovesCount > 0)
            {
                var isInteracted = false;
                if (Inventory.CurrentSelectedItemName != null && Inventory.CurrentSelectedItemName.Equals("bat"))
                {
                    if (hit.transform.GetComponent<Enemy>().isCanBeAttackedByMeleeWeapon)
                    {
                        player.GetComponent<Player>().Attack(hit.transform.gameObject, Inventory.CurrentSelectedItemName);
                        inventory.GetComponent<Inventory>().DeleteItemFromInventory(Inventory.CurrentSelectedItemName);
                        isInteracted = true;
                    }
                }
                if (Inventory.CurrentSelectedItemName != null && Inventory.CurrentSelectedItemName.Equals("fists"))
                {
                    if (hit.transform.GetComponent<Enemy>().isCanBeAttackedByMeleeWeapon)
                    {
                        player.GetComponent<Player>().Attack(hit.transform.gameObject, Inventory.CurrentSelectedItemName);
                        //inventory.GetComponent<Inventory>().DeleteItemFromInventory(Inventory.CurrentSelectedItemName);
                        isInteracted = true;
                    }
                }
                if (Inventory.CurrentSelectedItemName != null && Inventory.CurrentSelectedItemName.Equals("knife"))
                {
                    if (hit.transform.GetComponent<Enemy>().isCanBeAttackedByMeleeWeapon)
                    {
                        player.GetComponent<Player>().Attack(hit.transform.gameObject, Inventory.CurrentSelectedItemName);
                        inventory.GetComponent<Inventory>().DeleteItemFromInventory(Inventory.CurrentSelectedItemName);
                        isInteracted = true;
                    }
                }
                if (Inventory.CurrentSelectedItemName != null && Inventory.CurrentSelectedItemName.Equals("revolver") && inventory.GetComponent<Inventory>().revolverAmmoCount > 0)
                {
                    if (hit.transform.GetComponent<Enemy>().isCanBeAttackedByRangeWeapon)
                    {
                        player.GetComponent<Player>().Attack(hit.transform.gameObject, Inventory.CurrentSelectedItemName);
                        inventory.GetComponent<Inventory>().DeleteItemFromInventory(Inventory.CurrentSelectedItemName);
                        isInteracted = true;
                    }
                }
                if (isInteracted)
                {
                    Manager.DecreasePlayerMovesCount();
                    interface1.GetComponent<Interface>().movesCount.GetComponent<Text>().text = Manager.PlayerMovesCount.ToString();
                    if (Manager.PlayerMovesCount <= 0)
                    {
                        UnHighLightCells();
                        Manager.StartFinishing();
                    }
                }
            }
        }
    }

    public void HighLightCells(int stepsCount)
    {
        player.GetComponent<Player>().CheckZombies(3, player.GetComponent<Player>().currentCell);
        if (!cellWithPlayer.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Right))
        {
            HighLightCell(stepsCount, map1[cellWithPlayer.GetComponent<Cell>().indexX + 1, cellWithPlayer.GetComponent<Cell>().indexY]);
        }
        if (!cellWithPlayer.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Left))
        {
            HighLightCell(stepsCount, map1[cellWithPlayer.GetComponent<Cell>().indexX - 1, cellWithPlayer.GetComponent<Cell>().indexY]);
        }
        if (!cellWithPlayer.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Up))
        {
            HighLightCell(stepsCount, map1[cellWithPlayer.GetComponent<Cell>().indexX, cellWithPlayer.GetComponent<Cell>().indexY + 1]);
        }
        if (!cellWithPlayer.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Down))
        {
            HighLightCell(stepsCount, map1[cellWithPlayer.GetComponent<Cell>().indexX, cellWithPlayer.GetComponent<Cell>().indexY - 1]);
        }
        //HighLightCell(stepsCount, cellWithPlayer);
        cellWithPlayer.GetComponent<Cell>().UnHighLight();
    }

    public void ClickOnCell(BaseEventData e)
    {
        
    }

    public void UnHighLightCells()
    {
        foreach(var cell in cells)
        {
            cell.GetComponent<Cell>().UnHighLight();
        }
    }

    private void HighLightCell(int stepsCount, GameObject cell)
    {
        if(stepsCount > 1)
        {
            stepsCount--;
            if (!cell.GetComponent<Cell>().isWithCard && !cell.GetComponent<Cell>().isWithZombie)
            {
                cell.GetComponent<Cell>().HighLight();
                if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Right))
                {
                    HighLightCell(stepsCount, map1[cell.GetComponent<Cell>().indexX + 1, cell.GetComponent<Cell>().indexY]);
                }
                if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Left))
                {
                    HighLightCell(stepsCount, map1[cell.GetComponent<Cell>().indexX - 1, cell.GetComponent<Cell>().indexY]);
                }
                if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Up))
                {
                    HighLightCell(stepsCount, map1[cell.GetComponent<Cell>().indexX, cell.GetComponent<Cell>().indexY + 1]);
                }
                if (!cell.GetComponent<Cell>().blockDirections.Contains(Cell.BlockDirection.Down))
                {
                    HighLightCell(stepsCount, map1[cell.GetComponent<Cell>().indexX, cell.GetComponent<Cell>().indexY - 1]);
                }
            }
            if (cell.GetComponent<Cell>().isWithZombie)
            {
                player.GetComponent<Player>().canMeleeAttack = true;
            }
        }
        else
        {
            if (!cell.GetComponent<Cell>().isWithCard && !cell.GetComponent<Cell>().isWithZombie)
            {
                cell.GetComponent<Cell>().HighLight();
            }
            if (cell.GetComponent<Cell>().isWithZombie)
            {
                player.GetComponent<Player>().canMeleeAttack = true;
            }
        }
    }
}
