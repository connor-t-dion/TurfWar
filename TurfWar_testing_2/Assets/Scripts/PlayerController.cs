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
    public bool UIisUp = false;
    public GameObject OtherPlayer;
    public GameObject UI;
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

    //                    type,mv,hp,at,df,sp,sa,sd 
    public string ident1 = "wm 10 40 15 15 30 00 10 5373452";
    public string ident2 = "wm 10 40 15 15 30 00 10 5373455";
    public string ident3 = "wm 10 40 15 15 30 00 10 5373456";
    public string ident4 = "wm 10 40 15 15 30 00 10 5373457";
    public string ident_active = "";

    private float Plant1_speed = 0;
    private float Plant2_speed = 1;
    private float Plant3_speed = 2;
    private float Plant4_speed = 3;

    public int it = 0;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        OtherPlayerPC = OtherPlayer.GetComponent<PlayerController>();

        //set wayyyy off screen (will adjust when its our turn to place)
        Vector2 OldPos = new Vector2(50, 50);
        if (p1_OBJ != null && Plant1_OB == null)
        {
            Plant1_OB = Instantiate(p1_OBJ, OldPos, Quaternion.identity);
            Plant1_OB.name = ident1;
            Plant1 = Plant1_OB.GetComponent<Plant>();
            Plant1.CreateStats(ident1, isPlayer1);
            Plant1.SetMyTurn(false);
            Plant1_speed = Plant1.PSpeed;
        }

        if (p2_OBJ != null && Plant2_OB == null)
        {
            Plant2_OB = Instantiate(p2_OBJ, OldPos, Quaternion.identity);
            Plant2_OB.name = ident2;
            Plant2 = Plant2_OB.GetComponent<Plant>();
            Plant2.CreateStats(ident2, isPlayer1);
            Plant2.SetMyTurn(false);
            Plant2_speed = Plant2.PSpeed;
        }

        if (p3_OBJ != null && Plant3_OB == null)
        {
            Plant3_OB = Instantiate(p3_OBJ, OldPos, Quaternion.identity);
            Plant3_OB.name = ident3;
            Plant3 = Plant3_OB.GetComponent<Plant>();
            Plant3.CreateStats(ident3, isPlayer1);
            Plant3.SetMyTurn(false);
            Plant3_speed = Plant3.PSpeed;
        }

        if (p4_OBJ != null && Plant4_OB == null)
        {
            Plant4_OB = Instantiate(p4_OBJ, OldPos, Quaternion.identity);
            Plant4_OB.name = ident4;
            Plant4 = Plant3_OB.GetComponent<Plant>();
            Plant4.CreateStats(ident4, isPlayer1);
            Plant4.SetMyTurn(false);
            Plant4_speed = Plant4.PSpeed;
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
            if (!UIisUp && isPlayer1)
            {
                Vector2 OldPos = new Vector2(.4f, 2.5f);
                UI.SetActive(true);
                UIisUp = true;
            }

            if (!isPlayer1 && !isSetup)
            {
                //activate AI
                CPU_AI();
                //for now, we wait five seconds and swap back to another player
            }

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

            bool rep = true; //this is in case we encounter a null. got to the next in line

            while(rep)
            {
                //get next available plant in order. It's their turn now
                ident_active = PlantOrderFinal[it];
                OtherPlayerPC.ident_active = PlantOrderFinal[it];
                GameObject NextPlant = GameObject.Find(PlantOrderFinal[it]);
                if (NextPlant != null)
                {
                    NextPlant.GetComponent<Plant>().SetMoveType("null");
                    if (NextPlant.GetComponent<Plant>().GetPlayerNum() == isPlayer1)
                    {
                        isMyTurn = true;
                    }
                    else
                    {
                        OtherPlayerPC.isMyTurn = true;
                        if (UI.active != false)
                            UI.SetActive(false);
                        UIisUp = false;
                        timer = Time.time;
                    }
                    NextPlant.GetComponent<Plant>().SetMyTurn(true);
                    if (NextPlant.GetComponent<Plant>().HasBeenSetup != true)
                        NextPlant.GetComponent<Plant>().isPlantSetup();

                    rep = false;
                }
                else
                {
                    OtherPlayerPC.it += 1;
                    it += 1;
                    if (it > 7)
                    {
                        OtherPlayerPC.it = 0;
                        it = 0;
                    }
                }
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

        GameObject FirstPlant = GameObject.Find(PlantOrderFinal[0]);
        if (FirstPlant != null)
        {
            if (FirstPlant.GetComponent<Plant>().GetPlayerNum() == isPlayer1)
            {
                isMyTurn = true;
                FirstPlant.GetComponent<Plant>().SetMyTurn(true);
                FirstPlant.GetComponent<Plant>().isPlantSetup();
            }
        }
    }

    private void CPU_AI()
    {
        if(Time.time-timer > 5)
        {
            EndMyTurn();
        }
    }
}
