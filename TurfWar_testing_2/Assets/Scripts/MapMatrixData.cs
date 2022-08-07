using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMatrixData : MonoBehaviour
{
    public int MapSize = 10;
    public double[,] BlockHeight;
    private double[,] BlockX;
    private double[,] BlockY;
    public double[,] BlockFeature;
    public string[,] BlockPlantLocation;

    // Start is called before the first frame update
    void Start()
    {
        //we create 3 matrices that span the board. The first entry (0,0) represents
        //the most forward corner of the map. That is designated as "the point of origin
        BlockHeight = new double[MapSize, MapSize];
        BlockX = new double[MapSize, MapSize];
        BlockY = new double[MapSize, MapSize];
        BlockFeature = new double[MapSize, MapSize];
        BlockPlantLocation = new string[MapSize, MapSize];
        for (int i = 0; i < MapSize; i++)
        {
            for (int j = 0; j < MapSize; j++)
            {
                //initialize at a height of 1 
                BlockHeight[i, j] = 0;
                BlockFeature[i, j] = 0;
                BlockX[i, j] = i*.5 + j*(-.5) - 0.5;
                BlockY[i, j] = i*.25 + j*.25  - 1.25;
                BlockPlantLocation[i,j] = "VOID";
            }
        }

        SetSpecialForMap();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 GetBlockCoords(int x,int y, bool includeHeight = false)
    {
        Vector2 pos;
        double Bx = BlockX[x, y];
        double By = BlockY[x, y];

        if (!includeHeight)
        {
           pos = new Vector2((float) Bx, (float) By);
        }
        else
        {
           pos = new Vector2((float) Bx, (float) (By + BlockHeight[x, y]*.5));
        }

        return pos;
    }

    void SetSpecialForMap()
    {
        BlockHeight[3, 2] = 0.5;
        BlockHeight[3, 3] = 0.5;
        BlockHeight[3, 4] = 0.5;
        BlockHeight[4, 1] = 0.5;
        BlockHeight[4, 2] = 0.5;
        BlockHeight[5, 1] = 0.5;
        BlockHeight[6, 1] = 0.5;

        BlockHeight[4, 7] = 0.5;
        BlockHeight[5, 6] = 0.5;
        BlockHeight[6, 6] = 0.5;
        BlockHeight[7, 5] = 0.5;

        /*
         * block feature:
         * = -1 - no block here 
         * ------SLOPE------
         * = 0.125 - slope down to SW
         * = 0.25  - slope down to SE
         * = 0.325 - slope down to NE
         * = 0.5   - sope down to NW
         * -------------------
        */

        BlockFeature[2, 4] = 0.125;
        BlockFeature[3, 5] = -1;
        BlockFeature[4, 5] = -1;
        BlockFeature[4, 4] = -1;
        BlockFeature[4, 3] = -1;
        BlockFeature[2, 5] = -1;
        BlockFeature[2, 6] = -1;
        BlockFeature[5, 3] = -1;
        BlockFeature[5, 2] = -1;
        BlockFeature[6, 2] = -1;

        BlockFeature[5, 7] = -1;
        BlockFeature[5, 8] = -1;
        BlockFeature[6, 7] = -1;
        BlockFeature[7, 7] = -1;
        BlockFeature[8, 6] = -1;
        BlockFeature[7, 1] = -1;
        BlockFeature[7, 2] = -1;
        BlockFeature[7, 6] = -1;
        BlockFeature[8, 5] = -1;
        BlockFeature[4, 8] = -1;
        BlockFeature[2, 7] = -1;
        BlockFeature[8, 8] = -1;

    }

}
