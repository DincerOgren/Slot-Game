using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    [SerializeField] bool isAcitve = false;
    [SerializeField] GameObject tick;


    public int matxValue;

    public LineHolders line;



    public void TileOnClick()
    {
        if (!isAcitve)
        {
            isAcitve = true;
            tick.SetActive(true);
            line.SetMatrix(matxValue, true,matxValue);

        }
        else
        {
            CloseTile();
        }
    }

    public void CloseTile()
    {
        isAcitve = false;
        tick.SetActive(false);
        line.SetMatrix(matxValue, false,matxValue);

    }
}
