using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //we set one player to be player 1. They take the lead on rolling for who goes first, shared actions, etc
    public bool isPlayer1;
    public bool isMyTurn = true;
    public bool myTurnToEnd = false;
    public GameObject OtherPlayer;
    private PlayerController OtherPlayerPC;
    public List<string> myOrder;

    //for testing, we allow players to be set publically
    //public GameObject Plant1_OB = null;
    //public GameObject Plant2_OB = null;
    //public GameObject Plant3_OB = null;
    //public GameObject Plant4_OB = null;
    private Plant Plant1;

    public string ident1 = "wm 10 40 15 15 5373452";
    public GameObject p1_OBJ;
    private GameObject Plant1_OB;

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
        }
        


    }

    // Update is called once per frame
    void Update()
    {
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
