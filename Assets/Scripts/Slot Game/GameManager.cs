using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI moneyText;
    float _playerMoney = 10000;
    string savePath;



    private void Awake()
    {
        savePath=Path.Combine(Application.streamingAssetsPath, "savedData.json");
        _playerMoney = LoadSavedData();
    }
    private void Start()
    {
        moneyText.text = _playerMoney.ToString();

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SaveMoney();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadSavedData();
        }
    }
    public void AddMoney(float money)
    {
        _playerMoney += money;
        moneyText.text = _playerMoney.ToString();
    } 


    public void DecreaseMoney(float money)
    {
        _playerMoney -= money;
        moneyText.text = _playerMoney.ToString();

    }

    public float GetPlayerMoney() => _playerMoney;

    public void SaveMoney()
    {
        MyData data = new MyData();
        data.money = _playerMoney;

        string jsonData = JsonUtility.ToJson(data);


        File.WriteAllText(savePath, jsonData);

        Debug.Log("Data saved to: " + savePath);
    }

    public float LoadSavedData()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);

            MyData loadedData = JsonUtility.FromJson<MyData>(jsonData);

            return loadedData.money;
        }
        else
        {
            Debug.LogWarning("No saved data found at: " + savePath);
            return _playerMoney;
        }        
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}


[System.Serializable]
public class MyData
{
    public float money;
}

