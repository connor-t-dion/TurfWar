using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    Vector3 worldPosition;
    public double HeightFromBlock;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        double x = worldPosition.x + 0.5;
        double y = worldPosition.y + 1.25;

        double matx = Mathf.Round((float)( x + 2 * y));
        double maty = Mathf.Round((float)(-x + 2 * y));

        int mpsz = gameObject.GetComponent<MapMatrixData>().MapSize;

        if ((matx >= 0 && matx <= (double) (mpsz-1)) && (maty >= 0 && maty <= (double)(mpsz - 1)))
        {
            int matxin = (int)matx;
            int matyin = (int)maty;
            if (gameObject.GetComponent<MapMatrixData>().BlockFeature[matxin, matyin] != -1)
            {
                Vector2 pos = gameObject.GetComponent<MapMatrixData>().GetBlockCoords(matxin, matyin, true);
                transform.position = pos;
                HeightFromBlock = gameObject.GetComponent<MapMatrixData>().BlockHeight[matxin, matyin];
            }
        }
        else
        {
            transform.position = new Vector2((float)-10, (float)-10);
        }

    }

}
