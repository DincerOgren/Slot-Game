using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using TMPro;
using UnityEngine;

public class PaylineGenerator : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    [SerializeField] LineHolders[] lineHolders;


    public int[,] paylines = new int[25, 5];

    public int paylineValue = 0;

    public void PaylineSave()
    {

        for (int i = 0; i < lineHolders.Length; i++)
        {
            paylines[paylineValue, i] = lineHolders[i].GetActiveValue();

        }

        WriteArray();
    }

    public void SetSavedValues()
    {
        for (int i = 0; i < lineHolders.Length; i++)
        {
            lineHolders[i].CloseTiles();
            lineHolders[i].SetScreen(paylines[paylineValue, i]);

        }

    }
    public void ChangeTextR()
    {
        if (paylineValue + 1 > paylines.GetLength(0))
        {
            paylineValue = paylines.GetLength(0);
        }
        else
        {
            paylineValue++;
            valueText.text = paylineValue.ToString();
        }
        SetSavedValues();

    }
    public void ChangeTextL()
    {
        if (paylineValue - 1 < 0)
        {
            paylineValue = 0;
        }
        else
        {
            paylineValue--;
            valueText.text = paylineValue.ToString();
        }
        SetSavedValues();

    }

    public void WriteArray()
    {
        for (int i = 0; i < paylines.GetLength(0); i++)
        {
            // Print the slot number
            Debug.Log(i + ". slot: " + string.Join(", ", GetRowValues(paylines, i)));
        }
    }

    // Generic method to get row values as a string
    private string[] GetRowValues<T>(T[,] array, int rowIndex)
    {
        int cols = array.GetLength(1);
        string[] rowValues = new string[cols];
        for (int j = 0; j < cols; j++)
        {
            rowValues[j] = array[rowIndex, j].ToString();
        }
        return rowValues;
    }
    public void SaveTXT()
    {
        string path =Path.Combine(Application.streamingAssetsPath ,"test.txt"); // Platform-independent path
        StreamWriter writer = new StreamWriter(path, false); // Append mode (true)
   
            for (int i = 0; i < paylines.GetLength(0); i++)
            {
                writer.WriteLine(string.Join(", ", GetRowValues(paylines, i)));
            }

        

        writer.Close();

        StreamReader reader = new StreamReader(path);

        // Print the text from the file
        Debug.Log(reader.ReadToEnd());
        Debug.Log("Dosya yolu: " + path);

        reader.Close();
    }
}
