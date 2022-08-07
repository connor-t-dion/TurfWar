using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    Rigidbody2D rbody;
    public float m_XPosition;
    public float m_YPosition;
    Vector2 m_NewPosition;
    Vector2 currentPos;

    // Start is called before the first frame update
    void Start()
    {
        m_NewPosition = new Vector2 (0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 currentPos = rbody.position;
        if (Input.GetKeyDown("a"))
        {
            currentPos = transform.position;
            print(currentPos.x);
            print(currentPos.y);
            m_NewPosition = currentPos + new Vector2 (-.5f,.25f);
            transform.position = m_NewPosition;
        }
        if (Input.GetKeyDown("d"))
        {
            currentPos = transform.position;
            print(currentPos.x);
            print(currentPos.y);
            m_NewPosition = currentPos + new Vector2(.5f, -.25f);
            transform.position = m_NewPosition;
        }
        if (Input.GetKeyDown("w"))
        {
            currentPos = transform.position;
            print(currentPos.x);
            print(currentPos.y);
            m_NewPosition = currentPos + new Vector2(.5f, .25f);
            transform.position = m_NewPosition;
        }
        if (Input.GetKeyDown("s"))
        {
            currentPos = transform.position;
            print(currentPos.x);
            print(currentPos.y);
            m_NewPosition = currentPos + new Vector2(-.5f, -.25f);
            transform.position = m_NewPosition;
        }
    }
}
