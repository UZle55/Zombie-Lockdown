using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    enum phase
    {
        fast,
        medium,
        slow,
        result,
        showingNumbers
    }
    public Sprite[] diceSprites;
    private bool isRolling = false;
    private float t = 0;
    private float t1 = 0;
    private int currentNum = 0;
    private phase rollPhase = phase.result;
    private int rollsCount = 1111111;
    private bool isNumberShowed = false;
    public int minDiceIndex;
    public int maxDiceIndex;
    public static int DiceTotalRollCount = 0;
    public static int DicePreviousTotalRollCount = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (rollPhase == phase.fast && rollsCount < 6 && t > 0.1f)
        {
            t = 0;
            rollsCount++;
            if(rollsCount == 6)
            {
                rollPhase = phase.medium; 
                rollsCount = 0;
            }
            while (true)
            {
                var nextNum = Random.Range(0, 6);
                if(nextNum !=  currentNum)
                {
                    currentNum = nextNum;
                    break;
                }
            }
            GetComponent<Image>().sprite = diceSprites[currentNum];
        }

        if (rollPhase == phase.medium && rollsCount < 3 && t > 0.175f)
        {
            t = 0;
            rollsCount++;
            if (rollsCount == 3)
            {
                rollPhase = phase.slow;
                rollsCount = 0;
            }
            while (true)
            {
                var nextNum = Random.Range(0, 5);
                if (nextNum != currentNum)
                {
                    currentNum = nextNum;
                    break;
                }
            }
            GetComponent<Image>().sprite = diceSprites[currentNum];
        }

        if (rollPhase == phase.slow && rollsCount < 2 && t > 0.25f)
        {
            t = 0;
            rollsCount++;
            if (rollsCount == 2)
            {
                rollPhase = phase.result;
                rollsCount = 0;
            }
            while (true)
            {
                var nextNum = Random.Range(0, 5);
                if (nextNum != currentNum)
                {
                    currentNum = nextNum;
                    break;
                }
            }
            GetComponent<Image>().sprite = diceSprites[currentNum];
        }

        if (rollPhase == phase.result && rollsCount < 1 && t > 0.33f)
        {
            t = 0;
            rollsCount++;
            while (true)
            {
                var nextNum = Random.Range(0, 5);
                if (nextNum != currentNum)
                {
                    currentNum = nextNum;
                    rollPhase = phase.showingNumbers;
                    break;
                }
            }
            GetComponent<Image>().sprite = diceSprites[currentNum];
            isRolling = false;
            transform.parent.GetComponent<Interface>().movesCount.GetComponent<Text>().text = (currentNum + 1).ToString();
            transform.parent.GetComponent<Interface>().movesCount.GetComponent<Text>().color = new Color(0, 0, 0, 0);
            //transform.parent.GetComponent<Interface>().movesCount.SetActive(true);
            t1 = 0;
        }
        
        if(rollPhase == phase.showingNumbers && !isNumberShowed && t > 0.5f)
        {
            t1 += Time.deltaTime;
            if (GetComponent<Image>().color.a > 0)
            {
                GetComponent<Image>().color -= new Color(0, 0, 0, t1 * 2);
                t1 = 0;
            }
            else if(transform.parent.GetComponent<Interface>().movesCount.GetComponent<Text>().color.a < 1)
            {
                transform.parent.GetComponent<Interface>().movesCount.GetComponent<Text>().color += new Color(0, 0, 0, t1 * 2);
                t1 = 0;
            }
            else
            {
                GameObject.Find("Player").GetComponent<Player>().SetMovesCount(currentNum + 1);
                isNumberShowed = true;
                DicePreviousTotalRollCount = DiceTotalRollCount;
                DiceTotalRollCount++;
            }


            
        }

        if(Manager.PlayerState == Manager.PlayerGameState.Finishing)
        {
            t1 = 0;
            t1 += Time.deltaTime;
            
            if (transform.parent.GetComponent<Interface>().movesCount.GetComponent<Text>().color.a > 0)
            {
                transform.parent.GetComponent<Interface>().movesCount.GetComponent<Text>().color -= new Color(0, 0, 0, t1 * 2);
            }
            else if (GetComponent<Image>().color.a < 1)
            {
                GetComponent<Image>().color += new Color(0, 0, 0, t1 * 2);
            }
            else
            {
                //GameObject.Find("Player").GetComponent<Player>().SetMovesCount(currentNum + 1);
                
                Manager.CanRollDice = true;
                //Manager.Finished();
            }
        }
    }

    public void Roll()
    {
        if (!isRolling && Manager.CanRollDice)
        {
            Manager.CanRollDice = false;
            StartRolling();

        }
    }

    private void StartRolling()
    {
        isRolling = true;
        rollPhase = phase.fast;
        rollsCount = 0;
        isNumberShowed = false;
        t1 = 0;
    }
}
