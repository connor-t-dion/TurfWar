using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIButton : MonoBehaviour
{
    public Button yourButton;
    private string buttonname;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        buttonname = btn.name;
        btn.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TaskOnClick()
    {
        string plant = GameObject.Find("Player1").GetComponent<PlayerController>().ident_active;
        GameObject PlantGO = GameObject.Find(plant);

        if (PlantGO != null)
        {
            PlantGO.GetComponent<Plant>().SetMoveType(buttonname);
        }
    }
}
