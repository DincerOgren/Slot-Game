using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

public class Reel : MonoBehaviour
{
    public bool withTranslate = false;
    public bool shouldTriggerMatches = false;

    public const int ROWS = 3;

    public float speed = 5f;
    public float symbolPosRate = 1.5f;

    public float correctionSpeed = 2f;
    Vector3 _startPos;

    float _currentSpeed;

    public float timeForAutoStop = 3f;

    float _spinTimer = 0;

    public float scatterChance = 100f;
    public bool shouldScatterEnd = false;

    public float symbolAmount = 100;

    public List<Symbol> objectList;

    public GameObject[] spawnedObjects;

    public List<BonusSymbol> bonusObjects;

    public const float totalWeight = 1;
    float _biggestDropChance = 0;

    bool _isTriggeredAction = false;


    public Symbol[] winningSymbols = new Symbol[ROWS];
    private void Start()
    {
        _startPos = transform.position;
        _currentSpeed = speed;
        spawnedObjects = new GameObject[Mathf.FloorToInt(symbolAmount)];
        CalculateBonusDrops();




        for (int i = 0; i < symbolAmount; i++)
        {
            float randomWeight = Random.Range(0f, totalWeight);

            if (randomWeight <= _biggestDropChance)
            {
                float cumulativeWeight = 0f;
                foreach (var bonusObject in bonusObjects)
                {
                    cumulativeWeight += bonusObject.dropChance;
                    if (randomWeight <= cumulativeWeight)
                    {
                        spawnedObjects[i] = Instantiate(bonusObject.prefab, transform);
                        break;
                    }
                }
            }
            else
            {
                spawnedObjects[i] = Instantiate(objectList[Random.Range(0, objectList.Count)].gameObject, transform);
            }
            spawnedObjects[i].transform.localPosition = new Vector3(0, (symbolPosRate * i) - 1.5f, 0);
        }

    }

    private void CalculateBonusDrops()
    {
        bonusObjects.Sort((a, b) => a.dropChance.CompareTo(b.dropChance));
        foreach (var item in bonusObjects)
        {
            _biggestDropChance = _biggestDropChance < item.dropChance ? item.dropChance : _biggestDropChance;
        }
    }

    public void Spin()
    {

        float randomSpeed = 0;
        float randomDelay = 0;
        _isTriggeredAction = false;
        _spinTimer = 0;
        _currentSpeed = speed;
        StartCoroutine(SpinReel(randomSpeed + _currentSpeed, randomDelay));

    }

    public void Spin(float spinSpeed, float delay)
    {
        _isTriggeredAction = false;
        _spinTimer = 0;
        _currentSpeed = speed;
        StartCoroutine(SpinReel(_currentSpeed + spinSpeed, delay));
    }

    IEnumerator SpinReel(float totalSpeed, float delay)
    {
        shouldScatterEnd = Random.Range(0, 100) < scatterChance;

        if (delay > 0) yield return new WaitForSeconds(delay);
        while (timeForAutoStop >= _spinTimer)
        {
            if (transform.position.y < -symbolPosRate * (symbolAmount - ROWS))
            {
                transform.position = _startPos;
            }

            //if (_spinTimer >= timeForAutoStop - 1f)
            //{
            //    //scatteR?
            //    //scatter olayýný nasýl yapacaðýmý çözemedim hocam :)
            //}


            if (withTranslate)
                transform.Translate(new Vector3(0, -1 * ((totalSpeed) * Time.deltaTime), 0));
            else
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(_startPos.x, -1 * symbolPosRate * symbolAmount, 0), (speed + _currentSpeed) * Time.deltaTime);


            _spinTimer += Time.deltaTime;
            yield return null;

        }

        StartCoroutine(AllignSymbols());
    }

    IEnumerator AllignSymbols()
    {
        float currentYValue = transform.position.y;
        float divide = currentYValue / symbolPosRate;
        float remaining = currentYValue % symbolPosRate;
        float result;
        if (Mathf.Abs(remaining) < Mathf.Abs(symbolPosRate) / 2f)
        {

            result = Mathf.CeilToInt(divide) * symbolPosRate;
        }
        else
        {
            result = Mathf.FloorToInt(divide) * symbolPosRate;
        }


        while (transform.position.y != result)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(transform.position.x, result, transform.position.z), correctionSpeed * Time.deltaTime);

            yield return null;

        }
        //int winningObject0 = -Mathf.FloorToInt(result / symbolPosRate);

        int winningObject0 = Mathf.FloorToInt(Mathf.Abs(transform.position.y) / symbolPosRate);

        for (int i = 0; i < ROWS; i++)
        {
            winningSymbols[i] = spawnedObjects[winningObject0 + i].GetComponent<Symbol>();
                                                
        }

        yield return null;

        if (shouldTriggerMatches && !_isTriggeredAction)
        {
            _isTriggeredAction = true;
            //For last ending
            yield return new WaitForSeconds(.25f);
            StartCoroutine(SpinManager.instance.FillBoard());
        }
    }

    public Symbol[] GetFinalResults()
    {

        return winningSymbols;
    }

}


[System.Serializable]
public class BonusSymbol
{
    public float dropChance;
    public GameObject prefab;
}


