using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LineHolders : MonoBehaviour
{
    [SerializeField] Tiles[] tiles;

    const int LineLength = 3;

    public TileMatrix[] tileMatrix;

    private void Awake()
    {
        tileMatrix = new TileMatrix[LineLength];
        for (int i = 0; i < LineLength; i++)
        {
            tileMatrix[i] = new TileMatrix();
        }
    }


    public void OnTileSelect()
    {
        foreach (var tile in tiles)
        {
            tile.CloseTile();
        }
    }

    public void SetMatrix(int matxValue, bool isActive,int value)
    {
        tileMatrix[matxValue].isActive = isActive;
        tileMatrix[matxValue].tileValue= value;
    }

    [System.Serializable]
    public class TileMatrix
    {
        public bool isActive;
        public int tileValue;
    }

    public void CloseTiles()
    {
        foreach (var item in tiles)
        {
            item.CloseTile();
        }
    }
    public void SetScreen(int payline)
    {
        foreach (var item in tiles)
        {
            if (item.matxValue==payline)
            {
                item.TileOnClick();
            }
        }
    }
    public int GetActiveValue()
    {
        foreach(var tile in tileMatrix)
        {
            if (tile.isActive)
            {
                return tile.tileValue;
            }
        }

        return 0;
    }
}
