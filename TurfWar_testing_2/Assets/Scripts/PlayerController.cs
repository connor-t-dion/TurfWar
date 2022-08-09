using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //we set one player to be player 1. They take the lead on rolling for who goes first, shared actions, etc
    public bool isPlayer1;
    public bool isMyTurn = true;
    public bool myTurnToEnd = false;
    public bool isSetup;
    public GameObject OtherPlayer;
    private PlayerController OtherPlayerPC;
    public List<string> PlantOrderFinal;

    //for testing, we allow players to be set publically
    private Plant Plant1;
    public GameObject p1_OBJ;
    private GameObject Plant1_OB;

    private Plant Plant2;
    public GameObject p2_OBJ;
    private GameObject Plant2_OB;

    private Plant Plant3;
    public GameObject p3_OBJ;
    private GameObject Plant3_OB;

    private Plant Plant4;
    public GameObject p4_OBJ;
    private GameObject Plant4_OB;

    public string ident1 = "wm 10 40 15 15 5373452";
    public string ident2 = "wm 10 40 15 15 5373455";
    public string ident3 = "wm 10 40 15 15 5373456";
    public string ident4 = "wm 10 40 15 15 5373457";

    private int Plant1_speed = 0;
    private int Plant2_speed = 1;
    private int Plant3_speed = 2;
    private int Plant4_speed = 3;

    // Start is called before the first frame update
    void Start()
    {
        //we want to only do this once, hence we assign a player 1 to handle it
        if (isPlayer1)
        {
            OtherPlayerPC = OtherPlayer.GetComponent<PlayerController>();

            //determine who goes first via a random roll (can be changed)
            float Roll = Random.Range(0f,1f);
            if ( Roll > 0.0 )
            {
                //P1 turn first, then P2
                isMyTurn = true;
                OtherPlayerPC.isMyTurn = false;
            }
            else
            {
                //P2 turn first, then P1
                isMyTurn = false;
                OtherPlayerPC.SetMyTurn(true);
            }
            
        }

        Vector2 OldPos = new Vector2(0, 0);
        if (p1_OBJ != null)
        {
            Plant1_OB = Instantiate(p1_OBJ, OldPos, Quaternion.identity);
            Plant1_OB.name = ident1;
            Plant1 = Plant1_OB.GetComponent<Plant>();
            Plant1.CreateStats(ident1, isPlayer1);
            Plant1.SetMyTurn(isMyTurn);
            Plant1_speed = Plant1.PMovementSpeed;
        }

        if (p2_OBJ != null)
        {
            Plant2_OB = Instantiate(p2_OBJ, OldPos, Quaternion.identity);
            Plant2_OB.name = ident2;
            Plant2 = Plant1_OB.GetComponent<Plant>();
            Plant2.CreateStats(ident2, isPlayer1);
            Plant2.SetMyTurn(isMyTurn);
            Plant2_speed = Plant2.PMovementSpeed;
        }

        if (p3_OBJ != null)
        {
            Plant3_OB = Instantiate(p3_OBJ, OldPos, Quaternion.identity);
            Plant3_OB.name = ident3;
            Plant3 = Plant1_OB.GetComponent<Plant>();
            Plant3.CreateStats(ident3, isPlayer1);
            Plant3.SetMyTurn(isMyTurn);
            Plant3_speed = Plant3.PMovementSpeed;
        }

        if (p4_OBJ != null)
        {
            Plant4_OB = Instantiate(p4_OBJ, OldPos, Quaternion.identity);
            Plant4_OB.name = ident4;
            Plant4 = Plant1_OB.GetComponent<Plant>();
            Plant4.CreateStats(ident4, isPlayer1);
            Plant4.SetMyTurn(isMyTurn);
            Plant4_speed = Plant4.PMovementSpeed;
        }

        isSetup = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (isSetup)
        {
            //set up turn order
            if (isPlayer1)
            {
                
            }

            isSetup = false;
        }

        if (isMyTurn)
        {


        }

        if (myTurnToEnd)
        {
            //P2 turn first, then P1
            isMyTurn = false;
            OtherPlayerPC.SetMyTurn(true);
            myTurnToEnd = false;
        }

    }

    public void SetMyTurn(bool value)
    {
        //for setting the player controller's isMyTurn once the plant has finished their action
        isMyTurn = value;
    }
}
