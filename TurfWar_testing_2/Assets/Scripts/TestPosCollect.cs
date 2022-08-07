using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPosCollect : MonoBehaviour
{
    Vector2 currentPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentPos = transform.position;
        if (Input.GetKeyDown("p"))
        {
            print(currentPos.x);
            print(currentPos.y);
        }
        }
}
