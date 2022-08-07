using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watermelon : Plant
{
    //Call Game Object
    public GameObject seed;
    public GameObject DisplayAttack; 
    public string identifier2 = "wtrmln";

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        //We use this for inidcating the first run of FireProjPath
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (base.PlantToFire == true)
        {
            base.FireProjectilePath(seed);
        }

        base.RunAttackAnimation(DisplayAttack);
        


    }

    
}
