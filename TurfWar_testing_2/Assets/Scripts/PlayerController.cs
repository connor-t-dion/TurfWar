using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //we set one player to be player 1. They take the lead on rolling for who goes first, shared actions, etc
    public bool isPlayer1;
    public bool isMyTurn = false;
    public bool myTurnToEnd = false;
    public bool isSetup;
    public GameObject OtherPlayer;
    private PlayerController OtherPlayerPC;
    public List<string> PlantOrderFinal = new List<string> { };

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

    private float Plant1_speed = 0;
    private float Plant2_speed = 1;
    private float Plant3_speed = 2;
    private float Plant4_speed = 3;

    public int it = 0;

    // Start is called before the first frame update
    void Start()
    {
        OtherPlayerPC = OtherPlayer.GetComponent<PlayerController>();

        //set wayyyy off screen (will adjust when its our turn to place)
        Vector2 OldPos = new Vector2(50, 50);
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

        //proud of this one. adding a random decimal allows us to work around speed ties and give each number a unique value
        Plant1_speed += Random.Range(0f, 1f);
        Plant2_speed += Random.Range(0f, 1f);
        Plant3_speed += Random.Range(0f, 1f);
        Plant4_speed += Random.Range(0f, 1f);

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
                setupTurns();
            }
            isSetup = false;
        }

        if (isMyTurn)
        {


        }

        if (myTurnToEnd)
        {
            isMyTurn = false;
            //end plants turn

            myTurnToEnd = false;
            OtherPlayerPC.it += 1;
            it += 1;
            //reset if too large
            if (it > 7)
            {
                OtherPlayerPC.it = 0;
                it = 0;
            }

            //get next available plant in order. It's their turn now
            GameObject NextPlant = GameObject.Find(PlantOrderFinal[it]);
            if (NextPlant != null)
            {
                if (NextPlant.GetComponent<Plant>().IsPlayerOneChar == isPlayer1)
                {
                    isMyTurn = true;
                }
                else
                {
                    OtherPlayerPC.isMyTurn = true;
                }
                NextPlant.GetComponent<Plant>().SetMyTurn(true);
            }


        }

    }

    public void SetMyTurn(bool value)
    {
        //for setting the player controller's isMyTurn once the plant has finished their action
        isMyTurn = value;
    }

    public void EndMyTurn()
    {
        myTurnToEnd = true;
    }

    private void setupTurns()
    {
        var PlantSpeeds = new List<float> { Plant1_speed, Plant2_speed, Plant3_speed, Plant4_speed, OtherPlayerPC.Plant1_speed, OtherPlayerPC.Plant2_speed, OtherPlayerPC.Plant3_speed, OtherPlayerPC.Plant4_speed };
        var PlantID = new List<string> { ident1, ident2, ident3, ident4, OtherPlayerPC.ident1, OtherPlayerPC.ident2, OtherPlayerPC.ident3, OtherPlayerPC.ident4 };
        var arrPlantSpeeds = PlantSpeeds.OrderByDescending(x => x);
        List<float> arrPlantSpeeds2 = new List<float>();
        foreach (float x in arrPlantSpeeds)
        {
            arrPlantSpeeds2.Add(x);
            int i = PlantSpeeds.FindIndex(a => a == x);
            PlantOrderFinal.Add(PlantID[i]);
            OtherPlayerPC.PlantOrderFinal.Add(PlantID[i]);
        }

        GameObject FirstPlant = GameObject.Find(PlantID[0]);
        if (FirstPlant != null)
        {
            if (FirstPlant.GetComponent<Plant>().IsPlayerOneChar == isPlayer1)
            {
                isMyTurn = true;
                FirstPlant.GetComponent<Plant>().SetMyTurn(true);
                FirstPlant.GetComponent<Plant>().isPlantSetup();
            }
        }
    }
}
