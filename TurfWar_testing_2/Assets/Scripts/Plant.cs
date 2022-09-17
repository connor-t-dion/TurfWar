using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plant : MonoBehaviour
{
    public int x = 1;
    public int y = 1;
    private int x_move;
    private int y_move;
    public int x_shot;
    public int y_shot;
    private float time_init;
    private float time_between;
    private int incr = 0;

    //boolean vars

    public bool IsMapMade;
    public bool IsMapMadePlace;
    public bool PlantToMove;
    public bool PlantToFire; //ranged attacks
    public bool PlantToMelee = false;
    private bool PlantShotPreview = false;
    private bool IsMyTurn = false;
    private bool FirstRun;
    private bool MoveLocked;
    public bool IsPlayerOneChar;
    public bool HasBeenSetup = false;
    private bool IsUnderground = false;
    public bool TriggerDamText = false;

    //GameObjs

    public GameObject moveTile;
    public GameObject moveTileSW;
    private GameObject[,] MovementZoneObj;
    private GameObject[,] PlacementZoneObj;
    private GameObject[] ShotPreview = null;
    private GameObject fired_seed;
    public GameObject preview_square;
    public GameObject SelectedEnemy;
    private GameObject DisplayAttack;
    private GameObject HitByAttack;
    public GameObject Grid;
    public GameObject DamageTxt;

    //plant specific data

    public int PMovement = 10;
    public int PHealth;
    public int PAttack;
    public int PDefense;
    public int PSpeed;
    public int PSpAttack;
    public int PSpDefense;
    private int[,] PMovementZone;
    private int[,] PPlacementZone;
    private string identifier;
    public string plantType;
    public string moveType = "notMyTurn";

    //the animator

    private Animator animator;

    
    private bool isShowing = false; //Create a Boolian for displaying the assigned item

    // Start is called before the first frame update
    public void Start()
    {
        IsMapMade = false;
        PlantToMove = false;
        FirstRun = true;
        MoveLocked = false;
        animator = GetComponent<Animator>();
        moveType = "move";
        Grid = GameObject.Find("Grid");
        DamageTxt = GameObject.Find("DamgText");
    }

    // Update is called once per frame
    public void Update()
    {

        if (IsMyTurn && moveType == "setup" && !IsMapMadePlace)
        {
            AllowInitialPlantPlacement();
        }
        if (IsMyTurn)
        {
            if (moveType == "move")
                GetPlantMovementZone(PMovement);
            else if (moveType == "melee")
                GetPlantMovementZone(6);

            if (moveType == "range" && PlantToFire != true)
            {
                ShowProjectilePath();
            }

            if (PlantShotPreview)
            {
                FireProjectilePath(preview_square, "arc", true);
            }
                
            //left click
            if (Input.GetMouseButtonDown(0))
            {
                LeftClick();
            }

            AnimationUpdate(x_move, y_move);

            if (PlantToMove == true)
            {
                PlantMoveToSpot(x_move, y_move);
            }

            if (PlantToMelee)
            {
                if (!isShowing)
                    RunAttackAnimation(DisplayAttack);
                else
                {
                    if (Time.time - time_init > 1.32)
                    {
                        RunAttackAnimation(DisplayAttack);
                        CalcDamage();
                        SetMoveType("done");
                        PlantToMelee = false;
                    }
                }
                
            }

            CheckIfDone();

        }

        

    }

    //public function to get the plant coordinates in the matrix space
    public Vector2 GetPlantPos()
    {
        Vector2 Pos = new Vector2(x, y);
        return Pos;
    }

    //public function to set that it is/ isn't our turn
    public void SetMyTurn(bool value)
    {
        IsMyTurn = value;
    }

    //use this function to create the acceptable movement grid (blue squares) for our plant
    private void GetPlantMovementZone(float radius)
    {
        // have we made the allowable walking range? If we have, don't make it again
        if (!IsMapMade)
        {
            int mpsz = Grid.GetComponent<MapMatrixData>().MapSize;
            int tiles = (int)Mathf.Round((float)(radius / 5));
            PMovementZone = new int[mpsz, mpsz];

            for (int i = 0; i < mpsz; i++)
            {
                for (int j = 0; j < mpsz; j++)
                {
                    PMovementZone[i, j] = 0;
                }
            }

            // make a new movement object to contain our sprites
            // (might not need to make new every time?)
            MovementZoneObj = new GameObject[mpsz, mpsz];

            for (int i = -tiles; i <= tiles; i++)
            {
                for (int j = -tiles; j <= tiles; j++)
                {
                    //make sure we aren't trying to add outside the matrix range (-1, mpsz)
                    if ((i + x) >= 0 && (i + x) <= mpsz - 1 && (j + y) >= 0 && (j + y) <= mpsz - 1)
                    {
                        //only make the diamond
                        if (Mathf.Abs(((float)i) + Mathf.Abs((float)j)) <= tiles && -Mathf.Abs(((float)i) - Mathf.Abs((float)j)) >= -tiles)
                        {
                            float BlockData = (float)Grid.GetComponent<MapMatrixData>().BlockFeature[i + x, y + j];

                            if (BlockData != -1)
                            {
                                AddBlockToMoveRange(x + i, y + j);
                            }
                            //If we aren't a real block
                            else
                            {
                                //add the block above it to the range 
                                if ((i + x + 1) >= 0 && (i + x + 1) <= mpsz - 1 &&
                                    (j + y + 1) >= 0 && (j + y + 1) <= mpsz - 1)
                                {
                                    //(IF WE DONT ALREADY HAVE THAT IN OUR DIAMOND)
                                    if ((Mathf.Abs(((float)i + 1) + Mathf.Abs((float)j + 1)) > tiles ||
                                        -Mathf.Abs(((float)i + 1) - Mathf.Abs((float)j + 1)) < -tiles))

                                    {
                                        AddBlockToMoveRange(x + i + 1, y + j + 1);
                                    }
                                }
                                //and add below it
                                if ((i + x - 1) >= 0 && (i + x - 1) <= mpsz - 1 &&
                                    (j + y - 1) >= 0 && (j + y - 1) <= mpsz - 1)
                                {
                                    //(IF WE DONT ALREADY HAVE THAT IN OUR DIAMOND)
                                    if ((Mathf.Abs(((float)i - 1) + Mathf.Abs((float)j - 1)) > tiles ||
                                        -Mathf.Abs(((float)i - 1) - Mathf.Abs((float)j - 1)) < -tiles))

                                    {
                                        AddBlockToMoveRange(x + i - 1, y + j - 1);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            IsMapMade = true;
            RigorousMovementZone();
        }
    }

    //send the plant in the direction of a coordinate
    private void PlantMoveToSpot(int xNew, int yNew)
    {
        //move our plant to it's new spot
        Vector2 OldPos = Grid.GetComponent<MapMatrixData>().GetBlockCoords(x, y, true);
        Vector2 NewPos = Grid.GetComponent<MapMatrixData>().GetBlockCoords(xNew, yNew, true);

        //extra garbage for when we animate
        NewPos.y = (float)(NewPos.y + 0.18);
        float dist_full = Vector2.Distance(OldPos, NewPos);
        float dist = Vector2.Distance(transform.position, NewPos);
        float perc;

        if (dist < dist_full)
            perc = (dist_full - dist) / dist_full;
        else
            perc = (dist - dist_full) / dist;
        float step;


        //if in the first 20%, ramp up speed
        if (Mathf.Abs(perc) <= .2)
        {
            step = (float)((float)(PMovement / 8.08135) * Time.deltaTime * (float)(Mathf.Abs(perc) + .3));
        }
        else
        {
            //nice meme
            step = (float)(PMovement / 8.008135) * Time.deltaTime;
        }
        transform.position = Vector2.MoveTowards(transform.position, NewPos, step);


        if (dist < 0.001)
        {
            PlantToMove = false;
            IsMapMade = false;
            MoveLocked = false;
            //update our matrix space x and y
            Grid.GetComponent<MapMatrixData>().SetBlockPlantLocation(x, y, "VOID");
            x = xNew;
            y = yNew;
            Grid.GetComponent<MapMatrixData>().SetBlockPlantLocation(x, y, identifier);
        }
    }

    //Make the sprite for the movement zone, if it is a valid spot
    private void AddBlockToMoveRange(int x_block, int y_block)
    {
        Vector2 pos = gameObject.GetComponent<MapMatrixData>().GetBlockCoords(x_block, y_block, true);
        pos.y = (float)(pos.y - .175);
        float BlockData = (float)gameObject.GetComponent<MapMatrixData>().BlockFeature[x_block, y_block];

        //if valid spot
        if (BlockData != -1)
        {
            //if it'snot a sw slope (will alter for more features)
            if (BlockData != 0.125)
                MovementZoneObj[x_block, y_block] = Instantiate(moveTile, pos, Quaternion.identity);
            else
            {
                //use sw slope sprite
                pos.y = (float)(pos.y + 0.5);
                MovementZoneObj[x_block, y_block] = Instantiate(moveTileSW, pos, Quaternion.identity);
            }
            //mark that it is indeed a movement tile
            PMovementZone[x_block, y_block] = 1;
        }
    }

    //Here we clean up the movement zone using some tricks to see pathways to that block
    private void RigorousMovementZone()
    {
        // need to rework unfortunately
    }

    //Update our animations based on motion, action, etc
    private void AnimationUpdate(int xNew, int yNew)
    {
        // Blakes shotty attempt to add animation to movment
        Vector2 OldPos = gameObject.GetComponent<MapMatrixData>().GetBlockCoords(x, y, true);
        Vector2 NewPos = gameObject.GetComponent<MapMatrixData>().GetBlockCoords(xNew, yNew, true);

        if (plantType == "wm")
        {
            if (PlantToMove == false)
            {
                animator.SetInteger("Watermellon_Movement", 0);
            }

            // Move Right Up
            else if ((OldPos.x <= NewPos.x) && (OldPos.y >= NewPos.y))
            {
                animator.SetInteger("Watermellon_Movement", 1);
            }

            // Move Right Down
            else if ((OldPos.x <= NewPos.x) && (OldPos.y <= NewPos.y))
            {
                animator.SetInteger("Watermellon_Movement", 2);
            }

            // Move Left Down
            else if ((OldPos.x >= NewPos.x) && (OldPos.y >= NewPos.y))
            {
                animator.SetInteger("Watermellon_Movement", 3);
            }

            // Move Left Up
            else if ((OldPos.x >= NewPos.x) && (OldPos.y <= NewPos.y))
            {
                animator.SetInteger("Watermellon_Movement", 4);
            }
        }

        if (plantType == "cr")
        {
            if (PlantToMove == false || (xNew == x && yNew == y))
            {
                animator.SetInteger("Carrot_Is_Moving", 5);
                animator.SetInteger("Carrot_Start_Movement", 0);
                IsUnderground = false;
            }

            // Move Right Up
            else if ((OldPos.x <= NewPos.x) && (OldPos.y >= NewPos.y))
            {
                if (!IsUnderground)
                {
                    animator.SetInteger("Carrot_Start_Movement", 1);
                    IsUnderground = true;
                }
                else
                {
                    animator.SetInteger("Carrot_Is_Moving", 1);
                }
                
            }

            // Move Right Down
            else if ((OldPos.x <= NewPos.x) && (OldPos.y <= NewPos.y))
            {
                if (!IsUnderground)
                {
                    animator.SetInteger("Carrot_Start_Movement", 2);
                    IsUnderground = true;
                }
                else
                {
                    animator.SetInteger("Carrot_Is_Moving", 2);
                }
            }

            // Move Left Down
            else if ((OldPos.x >= NewPos.x) && (OldPos.y >= NewPos.y))
            {
                if (!IsUnderground)
                {
                    animator.SetInteger("Carrot_Start_Movement", 3);
                    IsUnderground = true;
                }
                else
                {
                    animator.SetInteger("Carrot_Is_Moving", 3);
                }
            }

            // Move Left Up
            else if ((OldPos.x >= NewPos.x) && (OldPos.y <= NewPos.y))
            {
                if (!IsUnderground)
                {
                    animator.SetInteger("Carrot_Start_Movement", 4);
                    IsUnderground = true;
                }
                else
                {
                    animator.SetInteger("Carrot_Is_Moving", 4);
                }
            }
        }

    }

    //All actions to deal with the left click
    private void LeftClick()
    {
        //make sure PMovementZone has been made
        if (!MoveLocked)
        {
            //get the position of the select cursor, plus the height modifier for the block it is on
            var posInd = GameObject.Find("Select").transform.position;
            double HeightMod = GameObject.Find("Select").GetComponent<MouseControl>().HeightFromBlock;

            //go from the Unity coordinate space to the matrix space (unapplying height mod)
            double x2 = posInd.x + 0.5;
            double y2 = posInd.y + 1.25 - 0.5 * HeightMod;

            double matx = Mathf.Round((float)(x2 + 2 * y2));
            double maty = Mathf.Round((float)(-x2 + 2 * y2));

            int mpsz = Grid.GetComponent<MapMatrixData>().MapSize;

            //now that we have the coordinate in the matrix space, make sure it is within the matrix (>0)
            if (matx >= 0 && matx <= (double)(mpsz - 1))
            {
                if (maty >= 0 && maty <= (double)(mpsz - 1))
                {
                    int matxin = (int)matx;
                    int matyin = (int)maty;

                    string[,] PlantMapLayout = Grid.GetComponent<MapMatrixData>().BlockPlantLocation;

                    switch (moveType)
                    {

                        case "move":
                            {
                                //MOVEMENT
                                //if that is an acceptable spot (within our PMoveZone), initiate movement
                                if (PMovementZone[matxin, matyin] == 1 && PlantMapLayout[matxin, matyin] == "VOID")
                                {
                                    x_move = matxin;
                                    y_move = matyin;
                                    PlantToMove = true;
                                    MoveLocked = true;

                                    //delete the old Movement Zone data
                                    for (int i = 0; i < mpsz; i++)
                                    {
                                        for (int j = 0; j < mpsz; j++)
                                        {
                                            Destroy(MovementZoneObj[i, j]);
                                        }
                                    }
                                    return;
                                }
                                break;
                            }

                        case "range":
                            {
                                //RANGE ATTACK
                                if (PlantMapLayout[matxin, matyin] != "VOID" && PMovementZone[matxin, matyin] == 1)
                                {
                                    //check to see that it's within range and on the enemy team
                                    SelectedEnemy = GameObject.Find(PlantMapLayout[matxin, matyin]);
                                    if (SelectedEnemy != null)
                                    {
                                        //if it's not on our team...
                                        if (SelectedEnemy.GetComponent<Plant>().IsPlayerOneChar != IsPlayerOneChar)
                                        {
                                            //ready to fire!
                                            x_shot = matxin;
                                            y_shot = matyin;
                                            PlantToFire = true;
                                            IsMapMade = false;
                                            FirstRun = true;
                                            MoveLocked = true;

                                            //delete the old Movement Zone data
                                            for (int i = 0; i < mpsz; i++)
                                            {
                                                for (int j = 0; j < mpsz; j++)
                                                {
                                                    Destroy(MovementZoneObj[i, j]);
                                                }
                                            }

                                            if (ShotPreview != null)
                                            {
                                                for (int i = 0; i < 50; i++)
                                                    Destroy(ShotPreview[i]);
                                            }
                                        }
                                    }

                                    return;
                                }
                                break;
                            }
                        case "melee":
                            {
                                //RANGE ATTACK
                                if (PlantMapLayout[matxin, matyin] != "VOID" && PMovementZone[matxin, matyin] == 1)
                                {
                                    //check to see that it's within range and on the enemy team
                                    SelectedEnemy = GameObject.Find(PlantMapLayout[matxin, matyin]);
                                    if (SelectedEnemy != null)
                                    {
                                        //if it's not on our team...
                                        if (SelectedEnemy.GetComponent<Plant>().IsPlayerOneChar != IsPlayerOneChar)
                                        {
                                            //ready to melee!
                                            IsMapMade = false;
                                            MoveLocked = true;
                                            PlantToMelee = true;

                                            //delete the old Movement Zone data
                                            for (int i = 0; i < mpsz; i++)
                                            {
                                                for (int j = 0; j < mpsz; j++)
                                                {
                                                    Destroy(MovementZoneObj[i, j]);
                                                }
                                            }

                                        }
                                    }
                                    return;
                                }
                                break;
                            }
                        case "setup":
                            {
                                //MOVEMENT
                                //if that is an acceptable spot (within our PMoveZone), initiate movement
                                if (PPlacementZone[matxin, matyin] == 1 && PlantMapLayout[matxin, matyin] == "VOID")
                                {
                                    x = matxin;
                                    y = matyin;

                                    Vector2 pos = Grid.GetComponent<MapMatrixData>().GetBlockCoords(matxin, matyin, true);
                                    pos.y += + (float)0.18;

                                    transform.position = pos;

                                    //delete the old Movement Zone data
                                    for (int i = 0; i < mpsz; i++)
                                    {
                                        for (int j = 0; j < mpsz; j++)
                                        {
                                            Destroy(PlacementZoneObj[i, j]);
                                        }
                                    }
                                    Grid.GetComponent<MapMatrixData>().SetBlockPlantLocation(x, y,identifier);
                                    moveType = "done";
                                    HasBeenSetup = true;
                                    return;
                                }
                                break;
                            }
                        case "stats":
                            {
                                //we don't do anything special? 
                                break;
                            }

                    }

                }

            }
            
        }
    }

    //User has chosen to attack with projectile, so make the pretty arc
    public void FireProjectilePath(GameObject seed, string ShotType = "arc", bool preview = false)
    {
        //here we fire the projectile and do ze math to make that arc look pretty

        //get firing locations
        Vector2 OldPos = Grid.GetComponent<MapMatrixData>().GetBlockCoords(x, y, true);
        Vector2 NewPos = Grid.GetComponent<MapMatrixData>().GetBlockCoords(x_shot, y_shot, true);

        //calibrate for plant height
        OldPos.y += .4f;
        NewPos.y += .4f;

        float dist = Vector2.Distance(OldPos, NewPos);
        float t;

        if (ShotType == "arc")
            t = .8f * Mathf.Sqrt(dist); // how long the shot will last
        else
            t = .05f * Mathf.Sqrt(dist);

        float a = -9.81f / 1.2f;

        //calculate initial velocities
        float v_xyo = (NewPos.x - OldPos.x) / (t);
        float v_zo = (NewPos.y - OldPos.y - .5f * a * t * t) / t;

        //if this is our first time calling this function for the shot, make the seed
        if (FirstRun == true && !preview)
        {
            fired_seed = Instantiate(seed, OldPos, Quaternion.identity);
            FirstRun = false;
            time_init = Time.time;
        }

        if (FirstRun == true && preview)
        {
            //fired_seed = Instantiate(seed, OldPos, Quaternion.identity);
            FirstRun = false;
            time_init = Time.time;
            time_between = Time.time;
        }

        float t_prime = 0;

        if (!preview)
        {
            t_prime = Time.time - time_init;

            float X = OldPos.x + (v_xyo * t_prime);
            float Y = OldPos.y + v_zo * t_prime + .5f * a * t_prime * t_prime;

            Vector2 NewPos2 = new Vector2(X, Y);
            fired_seed.transform.position = NewPos2; 
        }
        else if (preview && Time.time - time_between > .15f && incr < 50)
        {
            t_prime = Time.time - time_init;

            float X = OldPos.x + (v_xyo * t_prime);
            float Y = OldPos.y + v_zo * t_prime + .5f * a * t_prime * t_prime;

            Vector2 NewPos2 = new Vector2(X, Y);

            ShotPreview[incr] = Instantiate(seed, NewPos2, Quaternion.identity);
            incr += 1;
            time_between = Time.time;
        }

        if (t_prime >= t)
        {
            if (!preview)
            {
                Destroy(fired_seed);
                CalcDamage();
                PlantToFire = false;
                MoveLocked = false;
            }
            if (preview)
            {
                PlantShotPreview = false;
            }
            FirstRun = true;
            
        }

    }

    private void ShowProjectilePath()
    {
        if (PMovementZone != null)
        {
            //get the position of the select cursor, plus the height modifier for the block it is on
            var posInd = GameObject.Find("Select").transform.position;
            double HeightMod = GameObject.Find("Select").GetComponent<MouseControl>().HeightFromBlock;

            //go from the Unity coordinate space to the matrix space (unapplying height mod)
            double x = posInd.x + 0.5;
            double y = posInd.y + 1.25 - 0.5 * HeightMod;

            double matx = Mathf.Round((float)(x + 2 * y));
            double maty = Mathf.Round((float)(-x + 2 * y));

            int mpsz = Grid.GetComponent<MapMatrixData>().MapSize;

            //now that we have the coordinate in the matrix space, make sure it is within the matrix (>0)
            if (matx >= 0 && matx <= (double)(mpsz - 1))
            {
                if (maty >= 0 && maty <= (double)(mpsz - 1))
                {
                    int matxin = (int)matx;
                    int matyin = (int)maty;
                    if (PMovementZone[matxin, matyin] == 1 && matxin != x_shot || matyin != y_shot)
                    {
                        if (ShotPreview != null)
                        {
                            for (int i = 0; i < 50; i++)
                                Destroy(ShotPreview[i]);
                        }
                        ShotPreview = new GameObject[50];
                        x_shot = matxin;
                        y_shot = matyin;
                        PlantShotPreview = true;
                    }
                 }
            }
        }
    }

    public void CreateStats(string Identifier, bool player_num)
    {
        string[] array = Identifier.Split(' ');
        plantType = array[0];

        PMovement =         int.Parse(array[1]);
        PHealth =           int.Parse(array[2]);
        PAttack =           int.Parse(array[3]);
        PDefense =          int.Parse(array[4]);
        PSpeed =            int.Parse(array[5]);
        PSpAttack =         int.Parse(array[6]);
        PSpDefense =        int.Parse(array[7]);
        IsPlayerOneChar = player_num;

        identifier = Identifier;
    }

    public void RunAttackAnimation(GameObject AttackAnimation)
    {
        if (AttackAnimation != null)
        {
            isShowing = !isShowing;
            AttackAnimation.SetActive(isShowing); // display or not whatever is linked to the gameobject (canvas) (following the state of the bool)
            if (SelectedEnemy != null)
            {
                GameObject EnemyAttackAnimation = SelectedEnemy.GetComponent<Plant>().HitByAttack;
                if (EnemyAttackAnimation != null)
                    EnemyAttackAnimation.SetActive(isShowing); // display or not whatever is linked to the gameobject (canvas) (following the state of the bool)
            }
            time_init = Time.time;
        }  

    }

    public void SetDisplayAttack(GameObject AttackAnimation)
    {
        DisplayAttack = AttackAnimation;
    }

    public void SetHitByAttack(GameObject AttackAnimation)
    {
        HitByAttack = AttackAnimation;
    }

    public void isPlantSetup()
    {
        moveType = "setup";
    }

    public void SetMoveType(string type)
    {
        string PrevType = moveType;
        moveType = type;
        int mpsz = Grid.GetComponent<MapMatrixData>().MapSize;
        //for now, we delete movement zone
        if (MovementZoneObj != null)
        {
            for (int i = 0; i < mpsz; i++)
            {
                for (int j = 0; j < mpsz; j++)
                {
                    Destroy(MovementZoneObj[i, j]);
                }
            }

            if (ShotPreview != null)
            {
                for (int i = 0; i < 50; i++)
                    Destroy(ShotPreview[i]);
            }

            IsMapMade = false;
        }
    }

    private void AllowInitialPlantPlacement()
    {
        //we set the available area to place. for now, ill make it so you can place anywhere
        // have we made the allowable walking range? If we have, don't make it again
        if (!IsMapMadePlace)
        {
            int mpsz = Grid.GetComponent<MapMatrixData>().MapSize;
            int radius = (int)Mathf.Round((float) (mpsz/2));
            x = radius;
            y = radius;
            radius *= 2;
            int tiles = (int)Mathf.Round((float)(radius / 5));
            PPlacementZone = new int[mpsz, mpsz];

            for (int i = 0; i < mpsz; i++)
            {
                for (int j = 0; j < mpsz; j++)
                {
                    PPlacementZone[i, j] = 0;
                }
            }

            // make a new placement object to contain our sprites
            // (might not need to make new every time?)
            PlacementZoneObj = new GameObject[mpsz, mpsz];

            for (int i = -tiles; i <= tiles; i++)
            {
                for (int j = -tiles; j <= tiles; j++)
                {
                    //make sure we aren't trying to add outside the matrix range (-1, mpsz)
                    if ((i + x) >= 0 && (i + x) <= mpsz - 1 && (j + y) >= 0 && (j + y) <= mpsz - 1)
                    {

                            float BlockData = (float)Grid.GetComponent<MapMatrixData>().BlockFeature[i + x, y + j];

                            if (BlockData != -1)
                            {
                                AddBlockToPlaceRange(x + i, y + j);
                            }
                            //If we aren't a real block
                            else
                            {
                                //add the block above it to the range 
                                if ((i + x + 1) >= 0 && (i + x + 1) <= mpsz - 1 &&
                                    (j + y + 1) >= 0 && (j + y + 1) <= mpsz - 1)
                                {
                                    //(IF WE DONT ALREADY HAVE THAT IN OUR DIAMOND)
                                    if ((Mathf.Abs(((float)i + 1) + Mathf.Abs((float)j + 1)) > tiles ||
                                        -Mathf.Abs(((float)i + 1) - Mathf.Abs((float)j + 1)) < -tiles))

                                    {
                                        AddBlockToPlaceRange(x + i + 1, y + j + 1);
                                    }
                                }
                                //and add below it
                                if ((i + x - 1) >= 0 && (i + x - 1) <= mpsz - 1 &&
                                    (j + y - 1) >= 0 && (j + y - 1) <= mpsz - 1)
                                {
                                    //(IF WE DONT ALREADY HAVE THAT IN OUR DIAMOND)
                                    if ((Mathf.Abs(((float)i - 1) + Mathf.Abs((float)j - 1)) > tiles ||
                                        -Mathf.Abs(((float)i - 1) - Mathf.Abs((float)j - 1)) < -tiles))

                                    {
                                        AddBlockToPlaceRange(x + i - 1, y + j - 1);
                                    }
                                }
                            }

                    }
                }
            }
            IsMapMadePlace = true;
        }
    }

    //Make the sprite for the movement zone, if it is a valid spot
    private void AddBlockToPlaceRange(int x_block, int y_block)
    {
        Vector2 pos = Grid.GetComponent<MapMatrixData>().GetBlockCoords(x_block, y_block, true);
        pos.y = (float)(pos.y - .175);
        float BlockData = (float)Grid.GetComponent<MapMatrixData>().BlockFeature[x_block, y_block];

        //if valid spot
        if (BlockData != -1 && PlacementZoneObj[x_block, y_block] == null)
        {
            //if it'snot a sw slope (will alter for more features)
            if (BlockData != 0.125)
                PlacementZoneObj[x_block, y_block] = Instantiate(moveTile, pos, Quaternion.identity);
            else
            {
                //use sw slope sprite
                pos.y = (float)(pos.y + 0.5);
                PlacementZoneObj[x_block, y_block] = Instantiate(moveTileSW, pos, Quaternion.identity);
            }
            //mark that it is indeed a movement tile
            PPlacementZone[x_block, y_block] = 1;
        }
    }

    public bool GetPlayerNum()
    {
        return IsPlayerOneChar;
    }

    public void CheckIfDone()
    {
        if (moveType == "done")
        {
            int mpsz = Grid.GetComponent<MapMatrixData>().MapSize;
            IsMyTurn = false;
            if (IsPlayerOneChar)
            {
                GameObject.Find("Player1").GetComponent<PlayerController>().EndMyTurn();
            }
            else
            {
                GameObject.Find("Player2").GetComponent<PlayerController>().EndMyTurn();
            }

            if (PMovementZone != null)
            {
                PMovementZone = null;
            }

            if (MovementZoneObj != null)
            {
                for (int i = 0; i < mpsz; i++)
                {
                    for (int j = 0; j < mpsz; j++)
                    {
                        Destroy(MovementZoneObj[i, j]);
                    }
                }
            }

            if (ShotPreview != null)
            {
                for (int i = 0; i < 50; i++)
                    Destroy(ShotPreview[i]);
            }

            PlantToMove = false;
            IsMapMade = false;
            MoveLocked = false;

        }
    }

    public void CalcDamage(string attType = "melee")
    {
        //you can probably guess what we do here...
        //the damage dealer always does the calc for the damage taker
        if (SelectedEnemy != null)
        {
            int EnHP = SelectedEnemy.GetComponent < Plant>().PHealth;
            int EnDef = SelectedEnemy.GetComponent < Plant >().PDefense;
            int EnSpDef = SelectedEnemy.GetComponent < Plant >().PSpDefense;

            int DAM_Att = (PAttack - EnDef);
            int DAM_SpAtt = (PSpAttack - EnSpDef);

            if (DAM_Att < 0)
                DAM_Att = 0;

            if (DAM_SpAtt < 0)
                DAM_SpAtt = 0;

            int DAM_tot = (int)((DAM_Att + DAM_SpAtt) * (1 - .5*Random.Range(0f, 1f)));
            
            float CritChance = Random.Range(0f, 1f);
            bool Crit = false;

            //  1/16 chance of crit
            if (CritChance > 0.9375f)
            {
                //critical hit!
                DAM_tot *=  2;
                Crit = true;
            }
                if (DAM_tot < 1)
                DAM_tot = 1;

            if (DAM_tot > EnHP)
            {
                //ya dead, kid
                //probably more to come here, I would think
            }
            SelectedEnemy.GetComponent < Plant >().PHealth = EnHP - DAM_tot;

            Vector2 pos = Grid.GetComponent<MapMatrixData>().GetBlockCoords(SelectedEnemy.GetComponent<Plant>().x, SelectedEnemy.GetComponent<Plant>().y, true);
            pos.y = (float)(pos.y + .6);
            pos.x = (float)(pos.x);

            Vector2 fin = new Vector2(pos.x,pos.y+.4f);

            DamageTxt.GetComponent<TMPro.TextMeshProUGUI>().text = DAM_tot.ToString();
            GameObject DamText = Instantiate(DamageTxt, pos, Quaternion.identity);
            DamText.GetComponent<TextBehavior>().isDamText = true;
            DamText.GetComponent<TextBehavior>().FinalPos = fin;

            if (Crit == true)
            {
                //add a little crit text
                DamageTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "crit!";
                pos.y = (float)(pos.y + .3);
                fin.y = pos.y + .4f;
                GameObject CritText = Instantiate(DamageTxt, pos, Quaternion.identity);
                CritText.GetComponent<TextBehavior>().isDamText = true;
                CritText.GetComponent<TextBehavior>().FinalPos = fin;

            }

        }

    }

}

