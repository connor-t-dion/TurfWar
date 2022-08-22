using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : Plant
{
    //Call Game Object
    public GameObject seed;
    public GameObject DisplayAtk;
    public GameObject HitByAtk;
    public string identifier2 = "carrot";

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        if (DisplayAtk != null)
            base.SetDisplayAttack(DisplayAtk);
        if (HitByAtk != null)
            base.SetHitByAttack(HitByAtk);
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
    }

}
