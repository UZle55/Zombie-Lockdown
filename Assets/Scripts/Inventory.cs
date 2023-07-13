using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject[] inventoryItems;
    public GameObject[] allInventoryItems;
    public static string CurrentSelectedItemName { get; private set; } = null;
    private float t = 0;
    private bool isPickedUp = false;

    private Color defaultColor = Color.white;
    private Color selectedColor = new Color(0.7f, 1, 0f, 1);
    private GameObject currentSelectedItem;
    public GameObject itemsLine1;
    public GameObject itemsLine2;
    private int currentActiveLine = 1;
    public GameObject rotatingPoint;
    private bool isSwitching = false;
    private float angle = 0;
    private bool isActivated = false;
    public int revolverAmmoCount = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach(var item in inventoryItems)
        {
            item.transform.Find("Count").GetComponent<Text>().text = "";
        }
        //PickUpItem("medkit");
    }

    // Update is called once per frame
    void Update()
    {
        t = 0;
        t += Time.deltaTime;
        if (isSwitching)
        {
            angle += t * 180 * 1.5f;
            if (currentActiveLine == 1)
            {
                if (angle >= 270 && !isActivated)
                {
                    itemsLine1.SetActive(true);
                    itemsLine2.SetActive(false);
                    isActivated = true;
                }
                if (angle >= 360)
                {
                    angle = 0;
                    isSwitching = false;
                    isActivated = false;
                }
            }
            else if(currentActiveLine == 2)
            {
                if(angle >= 90 && !isActivated)
                {
                    itemsLine2.SetActive(true);
                    itemsLine1.SetActive(false);
                    isActivated = true;
                }
                if(angle >= 180)
                {
                    angle = 180;
                    isSwitching = false;
                    isActivated = false;
                }
            }
            rotatingPoint.transform.localEulerAngles = new Vector3(angle, 0, 0);
        }
        /*if(t > 5 && !isPickedUp)
        {
            t = 0;
            
            isPickedUp = true;
        }*/
    }

    public void SwitchSlots()
    {
        if (!isSwitching)
        {
            isSwitching = true;
            if (currentSelectedItem != null && !currentSelectedItem.name.Equals("BackPackItem:fists"))
            {
                currentSelectedItem.transform.parent.GetComponent<Image>().color = defaultColor;
                currentSelectedItem = null;
                CurrentSelectedItemName = null;
            }
            if (currentActiveLine == 1)
            {
                currentActiveLine = 2;
            }
            else if (currentActiveLine == 2)
            {
                currentActiveLine = 1;
            }
        }
    }

    public void OnBackPackItemCkick(GameObject item)
    {
        if (currentSelectedItem == null)
        {
            currentSelectedItem = item;
            item.transform.parent.GetComponent<Image>().color = selectedColor;
            CurrentSelectedItemName = item.name.Split(":")[1];
        }
        else if(currentSelectedItem.GetInstanceID().Equals(item.GetInstanceID()))
        {
            currentSelectedItem = null;
            item.transform.parent.GetComponent<Image>().color = defaultColor;
            CurrentSelectedItemName = null;
        }
        else
        {
            currentSelectedItem.transform.parent.GetComponent<Image>().color = defaultColor;
            currentSelectedItem = item;
            item.transform.parent.GetComponent<Image>().color = selectedColor;
            CurrentSelectedItemName = item.name.Split(":")[1];
        }
    }

    public void PickUpItem(string itemName)
    {
        var isInventoryFull = true;
        for(var i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i].name.Equals("BackPackItem:" + itemName))
            {
                if (!itemName.Equals("ammo_revolver"))
                {
                    isInventoryFull = false;
                    var count = int.Parse(inventoryItems[i].transform.Find("Count").GetComponent<Text>().text);
                    count++;
                    inventoryItems[i].transform.Find("Count").GetComponent<Text>().text = count.ToString();
                    break;
                }
                else
                {
                    revolverAmmoCount += 3;
                    UpdateRevolverAmmoCount();
                }
            }
            if (inventoryItems[i].name.Equals("BackPackItem:empty"))
            {
                var isFind = false;
                foreach (var item in allInventoryItems)
                {
                    if (item.name.Equals(itemName))
                    {
                        
                        if (itemName.Equals("bat"))
                        {
                            inventoryItems[i].GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;
                            inventoryItems[i].transform.Find("Count").GetComponent<Text>().text = "7";
                            isFind = true;
                            inventoryItems[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                            break;
                        }
                        else if (itemName.Equals("knife"))
                        {
                            inventoryItems[i].GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;
                            inventoryItems[i].transform.Find("Count").GetComponent<Text>().text = "10";
                            isFind = true;
                            inventoryItems[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                            break;
                        }
                        else if (itemName.Equals("revolver"))
                        {
                            inventoryItems[i].GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;
                            inventoryItems[i].transform.Find("Count").GetComponent<Text>().text = revolverAmmoCount.ToString();
                            isFind = true;
                            inventoryItems[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                            break;
                        }
                        else if (itemName.Equals("ammo_revolver"))
                        {
                            revolverAmmoCount += 3;
                            UpdateRevolverAmmoCount();
                            isFind=true;
                            break;
                        }
                        else
                        {
                            inventoryItems[i].GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;
                            inventoryItems[i].transform.Find("Count").GetComponent<Text>().text = "1";
                            isFind = true;
                            inventoryItems[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                            break;
                        }
                        
                    }
                }
                isInventoryFull = false;
                if (isFind && !itemName.Equals("ammo_revolver"))
                {
                    inventoryItems[i].name = "BackPackItem:" + itemName;
                }
                else if(!itemName.Equals("ammo_revolver"))
                {
                    Debug.Log("Error_AddItemException_ItemNotFound");
                }
                break;
            }
            
        }
        if (isInventoryFull)
        {
            Debug.Log("Inventory is full");
        }
    }

    public void UpdateRevolverAmmoCount()
    {
        foreach (var invItem in inventoryItems)
        {
            if (invItem.name.Equals("BackPackItem:revolver"))
            {
                invItem.transform.Find("Count").GetComponent<Text>().text = revolverAmmoCount.ToString();
            }
        }
    }

    public void DeleteItemFromInventory(string itemName)
    {
        var isInventoryHasThisItem = false;
        if (itemName.Equals("revolver"))
        {
            revolverAmmoCount--;
            UpdateRevolverAmmoCount();
        }
        else
        {
            for (var i = 0; i < inventoryItems.Length; i++)
            {
                if (inventoryItems[i].name.Equals("BackPackItem:" + itemName))
                {
                    if (int.Parse(inventoryItems[i].transform.Find("Count").GetComponent<Text>().text) == 1)
                    {
                        inventoryItems[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                        inventoryItems[i].name = "BackPackItem:empty";
                        //inventoryItems[i].GetComponent<Image>().sprite = null;
                        inventoryItems[i].transform.Find("Count").GetComponent<Text>().text = "";
                        if (currentSelectedItem.GetInstanceID().Equals(inventoryItems[i].GetInstanceID()))
                        {
                            OnBackPackItemCkick(currentSelectedItem);
                            currentSelectedItem = null;
                            inventoryItems[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                            CurrentSelectedItemName = null;
                        }
                    }
                    else
                    {
                        var count = int.Parse(inventoryItems[i].transform.Find("Count").GetComponent<Text>().text);
                        count--;
                        inventoryItems[i].transform.Find("Count").GetComponent<Text>().text = count.ToString();
                    }


                    isInventoryHasThisItem = true;
                }
            }
            if (!isInventoryHasThisItem)
            {
                Debug.Log("Error_DeleteItemException_ItemNotFound");
            }
        }
    }
}
