using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBehavior : MonoBehaviour
{
    private float Timer;
    public bool isDamText;
    public TMPro.TextMeshProUGUI text;
    public Vector2 FinalPos;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        Vector3 scale = new Vector3(.00854701f, .00854701f, 1);
        transform.localScale = scale;
        transform.SetParent(GameObject.Find("Texts").transform);
        Timer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDamText)
        {
            float dt = Time.time - Timer;
            transform.position = Vector2.MoveTowards(transform.position, FinalPos, .0001f);
            if (dt < .5f)
            {
                text.color = new Color (1,1,1,dt * 2);
            }
            else if (dt < 3f)
            {
                text.color = new Color (1,1,1,1-((dt-.5f)*1/2.5f ));
            }
            else
            {
                isDamText = false;
                Object.Destroy(this.gameObject);
            }
                
        }
    }
}
