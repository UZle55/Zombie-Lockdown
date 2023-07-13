using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public enum CardWith
    {
        Item,
        Enemy
    }
    public string ItemName;
    public string EnemyName;
    public CardWith cardWith = CardWith.Item;
    public GameObject cellWithThisCard;
    public GameObject[] cellsWherePlayerCanCollectCard;
    private GameObject map;
    private GameObject inventory;
    private GameObject interface1;
    private bool isCollecting = false;
    public GameObject cardItemPanel;
    public GameObject[] allCardItems;
    private GameObject cardItemImage;
    private GameObject cardItemText;
    private float t = 0;
    private bool isShowed = false;
    private static bool canCloseCard = false;
    private bool isPickedUp = false;
    public GameObject enemyExample;
    private GameObject cardItemDescription;
    // Start is called before the first frame update
    void Start()
    {
        cardItemImage = cardItemPanel.transform.Find("CardItemImage").gameObject;
        cardItemText = cardItemPanel.transform.Find("CardItemText").gameObject;
        cardItemDescription = cardItemPanel.transform.Find("CardItemDescription").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCollecting)
        {
            t = 0;
            t += Time.deltaTime;
            if (!isShowed && cardItemPanel.GetComponent<Image>().color.a < 1)
            {
                cardItemPanel.GetComponent<Image>().color += new Color(0, 0, 0, t * 2);
                cardItemImage.GetComponent<Image>().color += new Color(0, 0, 0, t * 2);
                cardItemText.GetComponent<Text>().color += new Color(0, 0, 0, t * 2);
                cardItemDescription.GetComponent<Text>().color += new Color(0, 0, 0, t * 2);
                if (cardItemPanel.GetComponent<Image>().color.a >= 1)
                {
                    isShowed = true;
                }
            }
            else if (isShowed && cardItemPanel.GetComponent<Image>().color.a > 0 && canCloseCard)
            {
                if (cardWith == CardWith.Item && !isPickedUp)
                {
                    inventory.GetComponent<Inventory>().PickUpItem(ItemName);
                    isPickedUp = true;
                }
                if (cardWith == CardWith.Enemy && !isPickedUp)
                {
                    var newEnemy = GameObject.Instantiate(enemyExample) as GameObject;
                    newEnemy.GetComponent<Enemy>().isExample = false;
                    newEnemy.GetComponent<Enemy>().currentCell = cellWithThisCard;
                    newEnemy.transform.position = cellWithThisCard.transform.position;
                    cellWithThisCard.GetComponent<Cell>().isWithZombie = true;
                    cellWithThisCard.GetComponent<Cell>().zombieOnThatCell = newEnemy;
                    newEnemy.transform.position += new Vector3(0, 0, -1);
                    map.GetComponent<Map>().enemies.Add(newEnemy);
                    isPickedUp = true;
                }
                cardItemPanel.GetComponent<Image>().color -= new Color(0, 0, 0, t * 2);
                cardItemImage.GetComponent<Image>().color -= new Color(0, 0, 0, t * 2);
                cardItemText.GetComponent<Text>().color -= new Color(0, 0, 0, t * 2);
                cardItemDescription.GetComponent<Text>().color -= new Color(0, 0, 0, t * 2);
            }
            else if(isShowed && cardItemPanel.GetComponent<Image>().color.a <= 0 && canCloseCard)
            {
                isCollecting = false;
                isShowed = false;
                cardItemPanel.SetActive(false);
                cellWithThisCard.GetComponent<Cell>().isWithCard = false;
                isPickedUp = false;
                Manager.EndCollecting();
                Manager.DecreasePlayerMovesCount();
                canCloseCard = false;
                interface1.GetComponent<Interface>().movesCount.GetComponent<Text>().text = Manager.PlayerMovesCount.ToString();
                if (Manager.PlayerMovesCount <= 0)
                {
                    map.GetComponent<Map>().UnHighLightCells();
                    Manager.StartFinishing();
                }
                else
                {
                    map.GetComponent<Map>().UnHighLightCells();
                    map.GetComponent<Map>().HighLightCells(1);
                }
                
            }
        }
    }

    public void OnScreenClick()
    {
        canCloseCard = true;
    }

    public void Collect(GameObject m, GameObject i, GameObject inter)
    {
        map = m;
        inventory = i;
        interface1 = inter;
        var isFind = false;
        isShowed = false;
        foreach (var item in allCardItems)
        {
            if (item.name.Equals(ItemName) || item.name.Equals(EnemyName))
            {
                cardItemImage.GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;
                cardItemText.GetComponent<Text>().text = item.name;
                if (item.name.Equals("antidote"))
                {
                    cardItemDescription.GetComponent<Text>().text = "reduces infection \nby 50 units";
                }
                else if (item.name.Equals("medkit"))
                {
                    cardItemDescription.GetComponent<Text>().text = "heals health \nby 50 units";
                }
                else if (item.name.Equals("bat"))
                {
                    cardItemDescription.GetComponent<Text>().text = "damage 8\ndistance: 1\n";
                }
                else if (item.name.Equals("knife"))
                {
                    cardItemDescription.GetComponent<Text>().text = "damage 4\ndistance: 1\n";
                }
                else if (item.name.Equals("revolver"))
                {
                    cardItemDescription.GetComponent<Text>().text = "damage: 16\ndistance: 3\nuses ammunition";
                }
                else if (item.name.Equals("ammo_revolver"))
                {
                    cardItemDescription.GetComponent<Text>().text = "";
                }
                else if (item.name.Equals("zombie_1"))
                {
                    cardItemDescription.GetComponent<Text>().text = "health: 15\ndamage: 10\nmovement range: 2";
                }
                else if (item.name.Equals("zombie_2"))
                {
                    cardItemDescription.GetComponent<Text>().text = "health: 10\ndamage: 5\nmovement range: 3";
                }
                else if (item.name.Equals("zombie_3"))
                {
                    cardItemDescription.GetComponent<Text>().text = "health: 30\ndamage: 25\nmovement range: 1";
                }
                else
                {
                    cardItemDescription.GetComponent<Text>().text = "";
                }
                cardItemPanel.GetComponent<Image>().color -= new Color(0, 0, 0, 1);
                cardItemImage.GetComponent<Image>().color -= new Color(0, 0, 0, 1);
                cardItemText.GetComponent<Text>().color -= new Color(0, 0, 0, 1);
                cardItemDescription.GetComponent<Text>().color -= new Color(0, 0, 0, 1);
                cardItemPanel.SetActive(true);
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            Debug.Log("Error_CollectingCardException_ItemNotFound");
        }
        else
        {
            Manager.StartCollecting();

            GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            GetComponent<BoxCollider2D>().enabled = false;
            isCollecting = true;
        }

       
    }
}
