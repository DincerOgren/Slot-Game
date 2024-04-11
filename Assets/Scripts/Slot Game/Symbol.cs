using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : MonoBehaviour
{
    public SymbolType symbolType;
    public Color lineColor;
    public float threeMoneyMultiplier;
    public float fourMoneyMultiplier;
    public float fiveMoneyMultiplier;
    
}
public enum SymbolType
{
    Banana,
    Apple,
    Purple,
    Watermelon,
    Orange,
    Grape,
    Pepper,
    Strawberry,
    Bonus,
    Scatter
}