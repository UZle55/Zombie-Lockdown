using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static GameObject player;
    public GameObject map;
    public GameObject interface1;

    public enum PlayerGameState
    {
        Pause,
        CanNothing,
        Moving,
        CanEverything,
        Collecting,
        Finishing
    }
    public static PlayerGameState PlayerState { get; private set; } = PlayerGameState.CanEverything;
    private static PlayerGameState previousPlayerState = PlayerGameState.CanEverything;
    public static bool CanRollDice = true;
    public static int PlayerMovesCount;
    private bool isInfectionIncreased = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if(PlayerState == PlayerGameState.Finishing && !isInfectionIncreased) {
            var infection = int.Parse(map.GetComponent<Map>().cellWithPlayer.transform.Find("InfectionNumberCanvas")
                .transform.Find("InfectionNumber_Cell").gameObject.GetComponent<Text>().text);
            player.GetComponent<Player>().IncreaseInfection(infection);
            interface1.GetComponent<Interface>().SetInfection(player.GetComponent<Player>().GetInfection());
            interface1.GetComponent<Interface>().SetHealth(player.GetComponent<Player>().GetHealth());
            isInfectionIncreased = true;
            //Finished();
        }
        if(PlayerState == PlayerGameState.CanEverything && isInfectionIncreased)
        {
            isInfectionIncreased = false;
        }*/
    }

    public static void StartPause()
    {
        previousPlayerState = PlayerState;
        PlayerState = PlayerGameState.Pause;
    }

    public static void EndPause()
    {
        PlayerState = previousPlayerState;
    }

    public static void StartMoving()
    {
        previousPlayerState = PlayerState;
        PlayerState = PlayerGameState.Moving;
    }

    public static void EndMoving()
    {
        if(PlayerMovesCount > 0)
        {
            PlayerState = PlayerGameState.CanEverything;
        }
        else
        {
            PlayerState = PlayerGameState.Finishing;
        }
    }

    public static void EndInteracting()
    {
        if (PlayerMovesCount > 0)
        {
            PlayerState = PlayerGameState.CanEverything;
        }
        else
        {
            PlayerState = PlayerGameState.Finishing;
        }
    }

    public static void StartCollecting()
    {
        PlayerState = PlayerGameState.Collecting;
    }

    public static void EndCollecting()
    {
        if (PlayerMovesCount > 0)
        {
            PlayerState = PlayerGameState.CanEverything;
        }
        else
        {
            PlayerState = PlayerGameState.Finishing;
        }
    }

    public static void StartFinishing()
    {
        PlayerState = PlayerGameState.Finishing;
    }

    public static void Finished()
    {
        PlayerState = PlayerGameState.CanEverything;
        CanRollDice = true;
        
    }

    public static void DecreasePlayerMovesCount()
    {
        PlayerMovesCount--;
        //player.GetComponent<Player>().CheckZombies(3, player.GetComponent<Player>().currentCell);
    }

    public static void IncreasePlayerMovesCount(int value)
    {
        PlayerMovesCount = value;
        //player.GetComponent<Player>().CheckZombies(3, player.GetComponent<Player>().currentCell);
    }
}
