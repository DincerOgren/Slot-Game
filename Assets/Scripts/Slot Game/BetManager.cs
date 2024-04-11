using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    public static BetManager instance;
    public float currentBetAmount = 200;
    public float betIncreaseRate = 200;

    public TextMeshProUGUI text;

    private void Start()
    {
        instance = this;
        currentBetAmount = betIncreaseRate;
        text.text=currentBetAmount.ToString();

        DontDestroyOnLoad(gameObject);
    }
    public void BetIncrease()
    {
        if (GameManager.instance.GetPlayerMoney() >=currentBetAmount+betIncreaseRate)
        {
            currentBetAmount += betIncreaseRate;
            text.text = currentBetAmount.ToString();
        }
        else
        {
            currentBetAmount = GameManager.instance.GetPlayerMoney();
            text.text = currentBetAmount.ToString();

        }
    }
    public void BetDecrease()
    {
        if (currentBetAmount-betIncreaseRate>=betIncreaseRate)
        {
            if (currentBetAmount - betIncreaseRate > GameManager.instance.GetPlayerMoney())
            {
                currentBetAmount = GameManager.instance.GetPlayerMoney() - betIncreaseRate;
            }
            else
                currentBetAmount -= betIncreaseRate;
            
            text.text = currentBetAmount.ToString();

        }
        
    }

    public void MaxBet()
    {
        currentBetAmount = GameManager.instance.GetPlayerMoney();
        text.text = currentBetAmount.ToString();
        
    }

    public void Bet()
    {
        GameManager.instance.DecreaseMoney(currentBetAmount);
    }

    public bool CanBet()
    {
        return GameManager.instance.GetPlayerMoney()-currentBetAmount >= 0;
    }

    public float GetCurrentBet() => currentBetAmount;
}
