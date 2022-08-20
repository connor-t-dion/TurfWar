using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool PlantShotPreview = false;
    private bool IsMyTurn = false;
    private bool FirstRun;
    private bool MoveLocked;
    public bool IsPlayerOneChar;

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

    //plant specific data

    public int PMovementSpeed = 10;
    public int PHealth;
    public int PAttack;
    public int PDefense;
    private int[,] PMovementZone;
    private int[,] PPlacementZone;
    private string identifier;
    public string plantType;
    public string moveType = "notMyTurn";

    //the animator

    private Animator animator;

    
    private bool isShowing; //Create a Boolian for displaying the assigned item

    // Start is called before the first frame update
    public void Start()
    {
        IsMapMade = false;
        PlantToMove = false;
        FirstRun = true;
        MoveLocked = false;
        animator = GetComponent<Animator>();
        moveType = "move";
    }

    // Update is called once per frame
    public void Update()
    {

        if (IsMyTurn && moveType == "setup")
        {
            AllowInitialPlantPlacement();
        }
        if (IsMyTurn)
        {
            GetPlantMovementZone(PMovementSpeed);

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

            RunAttackAnimation(DisplayAttack);

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
            int mpsz = gameObject.GetComponent<MapMatrixData>().MapSize;
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
                            float BlockData = (float)gameObject.GetComponent<MapMatrixData>().BlockFeature[i + x, y + j];

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
        Vector2 OldPos = gameObject.GetComponent<MapMatrixData>().GetBlockCoords(x, y, true);
        Vector2 NewPos = gameObject.GetComponent<MapMatrixData>().GetBlockCoords(xNew, yNew, true);

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
            step = (float)((float)(PMovementSpeed / 8.08135) * Time.deltaTime * (float)(Mathf.Abs(perc) + .3));
        }
        else
        {
            //nice meme
            step = (float)(PMovementSpeed / 8.008135) * Time.deltaTime;
        }
        transform.position = Vector2.MoveTowards(transform.position, NewPos, step);


        if (dist < 0.001)
        {
            PlantToMove = false;
            IsMapMade = false;
            MoveLocked = false;
            //update our matrix space x and y
            x = xNew;
            y = yNew;
            gameObject.GetComponent<MapMatrixData>().BlockPlantLocation[x, y] = identifier;
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

        if (PlantToMove == false)
        {
            animator.SetInteger("Watermellon_Movement", 0);
        }

        // Move Right Up
        else if ((OldPos.x < NewPos.x) && (OldPos.y > NewPos.y))
        {
            animator.SetInteger("Watermellon_Movement", 1);
        }

        // Move Right Down
        else if ((OldPos.x < NewPos.x) && (OldPos.y < NewPos.y))
        {
            animator.SetInteger("Watermellon_Movement", 2);
        }

        // Move Left Down
        else if ((OldPos.x > NewPos.x) && (OldPos.y > NewPos.y))
        {
            animator.SetInteger("Watermellon_Movement", 3);
        }

        // Move Left Up
        else if ((OldPos.x > NewPos.x) && (OldPos.y < NewPos.y))
        {
            animator.SetInteger("Watermellon_Movement", 4);
        }
    }

    //All actions to deal with the left click
    private void LeftClick()
    {
        //make sure PMovementZone has been made
        if (PMovementZone != null && !MoveLocked)
        {
            //get the position of the select cursor, plus the height modifier for the block it is on
            var posInd = GameObject.Find("Select").transform.position;
            double HeightMod = GameObject.Find("Select").GetComponent<MouseControl>().HeightFromBlock;

            //go from the Unity coordinate space to the matrix space (unapplying height mod)
            double x2 = posInd.x + 0.5;
            double y2 = posInd.y + 1.25 - 0.5 * HeightMod;

            double matx = Mathf.Round((float)(x2 + 2 * y2));
            double maty = Mathf.Round((float)(-x2 + 2 * y2));

            int mpsz = gameObject.GetComponent<MapMatrixData>().MapSize;

            //now that we have the coordinate in the matrix space, make sure it is within the matrix (>0)
            if (matx >= 0 && matx <= (double)(mpsz - 1))
            {
                if (maty >= 0 && maty <= (double)(mpsz - 1))
                {
                    int matxin = (int)matx;
                    int matyin = (int)maty;

                    string[,] PlantMapLayout = gameObject.GetComponent<MapMatrixData>().BlockPlantLocation;

                    switch (moveType)
                    {

                        case "move":
                            {
                                //MOVEMENT
                                //if that is an acceptable spot (within our PMoveZone), initiate movement
                                if (PMovementZone[matxin, matyin] == 1)
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
                        case "setup":
                            {
                                //MOVEMENT
                                //if that is an acceptable spot (within our PMoveZone), initiate movement
                                if (PPlacementZone[matxin, matyin] == 1)
                                {
                                    x = matxin;
                                    y = matyin;

                                    Vector2 pos = gameObject.GetComponent<MapMatrixData>().GetBlockCoords(matxin, matyin, true);

                                    transform.position = pos;

                                    //delete the old Movement Zone data
                                    for (int i = 0; i < mpsz; i++)
                                    {
                                        for (int j = 0; j < mpsz; j++)
                                        {
                                            Destroy(PlacementZoneObj[i, j]);
                                        }
                                    }

                                    moveType = "done";
                                    if (IsPlayerOneChar)
                                    {
                                        GameObject.Find("Player1").GetComponent<PlayerController>().EndMyTurn();
                                    }
                                    else
                                    {
                                        GameObject.Find("Player2").GetComponent<PlayerController>().EndMyTurn();
                                    }

                                    return;
                                }
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
        Vector2 OldPos = gameObject.GetComponent<MapMatrixData>().GetBlockCoords(x, y, true);
        Vector2 NewPos = gameObject.GetComponent<MapMatrixData>().GetBlockCoords(x_shot, y_shot, true);

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
                PlantToFire = false;
                MoveLocked = false;
            }
            if (preview)
            {
                PlantShotPreview = false;
            }
            // deal damage
            // base.DealDamage(); ?
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

            int mpsz = gameObject.GetComponent<MapMatrixData>().MapSize;

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

        PMovementSpeed =    int.Parse(array[1]);
        PHealth =           int.Parse(array[2]);
        PAttack =           int.Parse(array[3]);
        PDefense =          int.Parse(array[4]);
        IsPlayerOneChar = player_num;

        identifier = Identifier;
    }

    public void RunAttackAnimation(GameObject AttackAnimation)
    {
        if (Input.GetKeyDown("e")) // if you press the E key
        {
            isShowing = !isShowing;
            AttackAnimation.SetActive(isShowing); // display or not whatever is linked to the gameobject (canvas) (following the state of the bool)
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

    private void AllowInitialPlantPlacement()
    {
        //we set the available area to place. for now, ill make it so you can place anywhere
        // have we made the allowable walking range? If we have, don't make it again
        if (!IsMapMadePlace)
        {
            int mpsz = gameObject.GetComponent<MapMatrixData>().MapSize;
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

                            float BlockData = (float)gameObject.GetComponent<MapMatrixData>().BlockFeature[i + x, y + j];

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
        Vector2 pos = gameObject.GetComponent<MapMatrixData>().GetBlockCoords(x_block, y_block, true);
        pos.y = (float)(pos.y - .175);
        float BlockData = (float)gameObject.GetComponent<MapMatrixData>().BlockFeature[x_block, y_block];

        //if valid spot
        if (BlockData != -1)
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

}

