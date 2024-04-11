using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinManager : MonoBehaviour
{
    public static SpinManager instance;

    public Reel[] reels;
    [SerializeField]
    SortingLayer _lineLayer;
    [SerializeField]
    Transform _lineParent;

    public float delayBetweenReelsSpins = .5f;
    public const float autoStopTimeForFastSpin = 1f;
    public Symbol[] matchSymbols = new Symbol[25];


    public List<GameObject> _matchLines;

    Symbol[,] board = new Symbol[5, 3];

    GameObject[,] tempList = new GameObject[25, 4];


    int[,] _paylineArray = new int[25, 5];
    int[,] _zeroStarters;
    int[,] _oneStarters;
    int[,] _twoStarters;
    int _zeroCount, _oneCount, _twoCount;

    public Material lineMaterial;

    public float[] winAmounts = new float[25];
    public int fullLine = 0;

    public float lineChangeInterval = 1f;
    bool _isSpinning = false;
    bool _callCoroutine;
    public bool fastSpin;

    [SerializeField]
    float _generalMutiplierForWins = .2f;
    private float _spinBetAmount;

    [Header("CanvasRef")]
    public Button betDecreaseButton;
    public Button betIncreaseButton;
    public Button spinButton;
    public TextMeshProUGUI winAmountText;
    public TextMeshProUGUI lastWinText;
    public GameObject fastSpinVisual;
    private void Start()
    {
        instance = this;
        ReadPaylines();
        AdjustPaylines();
        DontDestroyOnLoad(gameObject);

    }


    public void SpinAllReels()
    {
        if (!BetManager.instance.CanBet())
        {
            return;
        }
        BetManager.instance.Bet();
        _spinBetAmount = BetManager.instance.GetCurrentBet();
        StopAllCoroutines();

        StartCoroutine(ResetSpin());
        StartCoroutine(SpinReels());
       
        _isSpinning = true;
        _callCoroutine = false;
        spinButton.interactable = false;
    }

    IEnumerator SpinReels()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            float randomSpeed = UnityEngine.Random.Range(0, 10);
            if (fastSpin)
            {
                reels[i].timeForAutoStop = autoStopTimeForFastSpin;
                reels[i].Spin(randomSpeed, i * .1f);

            }
            else
            {
                reels[i].timeForAutoStop = 3f;

                reels[i].Spin(randomSpeed, i * delayBetweenReelsSpins);
            }
            yield return null;
        }
    }
    public void FastSpin()
    {
        fastSpin = !fastSpin;
        fastSpinVisual.SetActive(fastSpin);

    }
    private IEnumerator ResetSpin()
    {
        yield return null;
        for (int i = 0; i < _lineParent.childCount; i++)
        {
            Destroy(_lineParent.transform.GetChild(i).gameObject);
        }
        _matchLines.Clear();

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                board[i, j] = null;
            }
        }
        fullLine = 0;
        winAmountText.text = 0.ToString();

        for (int i = 0; i < winAmounts.Length; i++)
        {
            winAmounts[i] = 0;
        }

        for (int i = 0; i < matchSymbols.Length; i++)
        {
            matchSymbols[i] = null;
        }
        yield return null;
    }
    private void Update()
    {
        if (!_isSpinning && fullLine > 0 && !_callCoroutine)
        {
            _callCoroutine = true;
            StartCoroutine(OpenLinesByOne());
        }

        if (!_isSpinning)
        {
            betDecreaseButton.interactable = true;
            betIncreaseButton.interactable = true;
            spinButton.interactable = true;

        }

    }

    IEnumerator OpenLinesByOne()
    {
        GameObject[] temp = new GameObject[tempList.GetLength(1)];
        for (int i = 0; i < tempList.GetLength(0); i++)
        {
            winAmountText.text = winAmounts[i].ToString();
            for (int j = 0; j < tempList.GetLength(1); j++)
            {
                temp[j] = tempList[i, j];
                if (temp[j] == null)
                {
                    break;
                }

                temp[j].SetActive(true);

            }

            yield return new WaitForSecondsRealtime(lineChangeInterval);

            for (int l = 0; l < temp.Length; l++)
            {
                if (temp[l] == null)
                {
                    break;
                }
                temp[l].SetActive(false);
            }

            if (tempList[i, 0] == null)
            {
                break;
            }
        }
        _callCoroutine = false;
    }



    void ReadPaylines()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "test.txt");

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] numbers = lines[i].Split(',');

                for (int j = 0; j < numbers.Length; j++)
                {
                    _paylineArray[i, j] = int.Parse(numbers[j].Trim());
                }
            }

        }
        else
        {
            Debug.LogError("Dosya bulunamadý: " + filePath);
        }
    }

    void AdjustPaylines()
    {
        for (int i = 0; i < _paylineArray.GetLength(0); i++)
        {
            if (_paylineArray[i, 0] == 0)
            {
                _zeroCount++;
            }
            else if (_paylineArray[i, 0] == 1)
            {
                _oneCount++;
            }
            else if (_paylineArray[i, 0] == 2)
            {
                _twoCount++;
            }
        }

        _zeroStarters = new int[_zeroCount, _paylineArray.GetLength(1)];
        _oneStarters = new int[_oneCount, _paylineArray.GetLength(1)];
        _twoStarters = new int[_twoCount, _paylineArray.GetLength(1)];

        int zero = 0;
        int one = 0;
        int two = 0;

        for (int i = 0; i < _paylineArray.GetLength(0); i++)
        {
            if (_paylineArray[i, 0] == 0)
            {
                for (int j = 0; j < _paylineArray.GetLength(1); j++)
                {
                    _zeroStarters[zero, j] = _paylineArray[i, j];
                }
                zero++;
            }
            else if (_paylineArray[i, 0] == 1)
            {
                for (int j = 0; j < _paylineArray.GetLength(1); j++)
                {
                    _oneStarters[one, j] = _paylineArray[i, j];
                }

                one++;
            }
            else if (_paylineArray[i, 0] == 2)
            {
                for (int j = 0; j < _paylineArray.GetLength(1); j++)
                {
                    _twoStarters[two, j] = _paylineArray[i, j];
                }

                two++;
            }

        }
    }

    public IEnumerator FillBoard()
    {
        for (int i = 0; i < board.GetLength(0); i++)
        { 
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (reels[i].winningSymbols[j] == null)
                    Debug.LogWarning("NULLLL " + reels[i].name + " S: " + i + " l:" + j);
                

                board[i, j] = reels[i].winningSymbols[j];

            }
        }
        yield return null;

        CheckMatches();
    }

    void PrintBoard()
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {

            for (int j = 0; j < board.GetLength(1); j++)
            {
                print(board[i, j].symbolType + " pos: " + board[i, j].transform.localPosition.y+"  i: "+i+" j: "+j);
            }
        }
    }
    public void CheckMatches()
    {
        PrintBoard();

        int k = 0;
        Symbol temp;
        int _successMatch = 0;
        int tempMatch = 0;
        int totalMatch = 0;
        int whileCounter = 0;
        while (k < board.GetLength(1))
        {
            print(" While triggered " + whileCounter+" times");
            whileCounter++;
            temp = board[0, k];
            matchSymbols[0] = temp;
            _successMatch = 0;
            tempMatch = _successMatch;
            switch (k)
            {
                case 0:
                    for (int l = 0; l < _zeroStarters.GetLength(0); l++)
                    {
                        tempMatch = 0;
                        _successMatch = 0;
                        for (int i = 1; i < _zeroStarters.GetLength(1); i++)
                        {
                            for (int j = 0; j < board.GetLength(1); j++)
                            {

                                if (j == _zeroStarters[l, i] && (temp.symbolType == board[i, j].symbolType || board[i, j].symbolType == SymbolType.Bonus))
                                {

                                    _successMatch++;
                                    matchSymbols[_successMatch] = board[i, j];
                                    print("Matches Added: "+_successMatch+" :  " + board[i, j].symbolType + board[i, j].transform.localPosition.y);

                                    if (_successMatch == 2)
                                    {
                                        for (int u = 0; u < _successMatch; u++)
                                        {
                                            DrawLine(matchSymbols[u].transform.position, matchSymbols[u + 1].transform.position, temp.lineColor);
                                            totalMatch++;

                                        }
                                    }
                                    else if (_successMatch >= 3)
                                    {
                                        DrawLine(matchSymbols[_successMatch - 1].transform.position, matchSymbols[_successMatch].transform.position, temp.lineColor);
                                        totalMatch++;

                                    }
                                    break;
                                }




                            }
                            if (_successMatch < 1)
                            {
                                break;
                            }
                            if (_successMatch >= 4)
                            {
                                for (int a = 0; a < tempList.GetLength(1); a++)
                                {
                                    tempList[fullLine, a] = _matchLines[totalMatch - _successMatch + a];

                                }
                                winAmounts[fullLine] = temp.fiveMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;
                                fullLine++;
                            }

                            if (tempMatch == _successMatch)
                            {

                                if (_successMatch >= 2)
                                {
                                    for (int a = 0; a < _successMatch; a++)
                                    {

                                        tempList[fullLine, a] = _matchLines[totalMatch - _successMatch + a];

                                    }
                                    if (_successMatch + 1 == 3)
                                    {

                                        winAmounts[fullLine] = temp.threeMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;


                                    }
                                    else if (_successMatch + 1 == 4)
                                    {
                                        winAmounts[fullLine] = temp.fourMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;

                                    }
                                    else if (_successMatch + 1 == 5)
                                    {
                                        winAmounts[fullLine] = temp.fiveMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;

                                    }

                                    fullLine++;
                                }

                                break;
                            }
                            else
                                tempMatch = _successMatch;
                        }


                    }
                    break;
                case 1:
                    for (int l = 0; l < _oneStarters.GetLength(0); l++)
                    {
                        tempMatch = 0;
                        _successMatch = 0;
                        for (int i = 1; i < _oneStarters.GetLength(1); i++)
                        {
                            for (int j = 0; j < board.GetLength(1); j++)
                            {

                                if (j == _oneStarters[l, i] && (temp.symbolType == board[i, j].symbolType || board[i, j].symbolType == SymbolType.Bonus))
                                {
                                    _successMatch++;

                                    matchSymbols[_successMatch] = board[i, j];
                                    print("Matches Added: " + _successMatch + " :  " + board[i, j].symbolType + board[i, j].transform.localPosition.y);

                                    if (_successMatch == 2)
                                    {
                                        for (int u = 0; u < _successMatch; u++)
                                        {
                                            DrawLine(matchSymbols[u].transform.position, matchSymbols[u + 1].transform.position, temp.lineColor);
                                            totalMatch++;

                                        }
                                    }
                                    else if (_successMatch >= 3)
                                    {
                                        totalMatch++;

                                        DrawLine(matchSymbols[_successMatch - 1].transform.position, matchSymbols[_successMatch].transform.position, temp.lineColor);

                                    }
                                    break;
                                }


                            }
                            if (_successMatch < 1)
                            {
                                break;
                            }

                            if (_successMatch >= 4)
                            {
                                for (int a = 0; a < tempList.GetLength(1); a++)
                                {
                                    tempList[fullLine, a] = _matchLines[totalMatch - _successMatch + a];

                                }
                                winAmounts[fullLine] = temp.fiveMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;

                                fullLine++;
                            }

                            if (tempMatch == _successMatch)
                            {

                                if (_successMatch >= 2)
                                {
                                    for (int a = 0; a < _successMatch; a++)
                                    {

                                        tempList[fullLine, a] = _matchLines[totalMatch - _successMatch + a];

                                    }
                                    if (_successMatch + 1 == 3)
                                    {

                                        winAmounts[fullLine] = temp.threeMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;

                                    }
                                    else if (_successMatch + 1 == 4)
                                    {
                                        winAmounts[fullLine] = temp.fourMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;

                                    }
                                    else if (_successMatch + 1 == 5)
                                    {
                                        winAmounts[fullLine] = temp.fiveMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;

                                    }
                                    fullLine++;

                                }


                                break;
                            }
                            else
                                tempMatch = _successMatch;
                        }


                    }
                    break;
                case 2:
                    for (int l = 0; l < _twoStarters.GetLength(0); l++)
                    {
                        tempMatch = 0;
                        _successMatch = 0;
                        for (int i = 1; i < _twoStarters.GetLength(1); i++)
                        {
                            for (int j = 0; j < board.GetLength(1); j++)
                            {
                                if (j == _twoStarters[l, i] && (temp.symbolType == board[i, j].symbolType || board[i, j].symbolType == SymbolType.Bonus))
                                {
                                    _successMatch++;

                                    matchSymbols[_successMatch] = board[i, j];
                                    print("Matches Added: " + _successMatch + " :  " + board[i, j].symbolType + board[i, j].transform.localPosition.y);

                                    if (_successMatch == 2)
                                    {
                                        for (int u = 0; u < _successMatch; u++)
                                        {
                                            DrawLine(matchSymbols[u].transform.position, matchSymbols[u + 1].transform.position, temp.lineColor);

                                            totalMatch++;
                                        }
                                    }
                                    else if (_successMatch >= 3)
                                    {

                                        totalMatch++;
                                        DrawLine(matchSymbols[_successMatch - 1].transform.position, matchSymbols[_successMatch].transform.position, temp.lineColor);

                                    }
                                    break;
                                }


                            }
                            if (_successMatch < 1)
                            {
                                break;
                            }

                            if (_successMatch >= 4)
                            {
                                for (int a = 0; a < tempList.GetLength(1); a++)
                                {
                                    tempList[fullLine, a] = _matchLines[totalMatch - _successMatch + a];

                                }
                                winAmounts[fullLine] = temp.fiveMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;

                                fullLine++;
                            }
                            if (tempMatch == _successMatch)
                            {

                                if (_successMatch >= 2)
                                {
                                    for (int a = 0; a < _successMatch; a++)
                                    {

                                        tempList[fullLine, a] = _matchLines[totalMatch - _successMatch + a];

                                    }
                                    if (_successMatch + 1 == 3)
                                    {

                                        winAmounts[fullLine] = temp.threeMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;


                                    }
                                    else if (_successMatch + 1 == 4)
                                    {
                                        winAmounts[fullLine] = temp.fourMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;


                                    }
                                    else if (_successMatch + 1 == 5)
                                    {
                                        winAmounts[fullLine] = temp.fiveMoneyMultiplier * _spinBetAmount * _generalMutiplierForWins;


                                    }
                                    fullLine++;
                                }

                                break;
                            }
                            else
                                tempMatch = _successMatch;
                        }


                    }
                    break;
                default:
                    break;
            }


            k++;
        }


        _isSpinning = false;
        float winAmount = 0;
        for (int i = 0; i < winAmounts.Length; i++)
        {
            winAmount += winAmounts[i];
        }

        if (winAmount > 0)
        {
            lastWinText.text = winAmount.ToString();

            GameManager.instance.AddMoney(winAmount);
        }


        GameManager.instance.SaveMoney();

    }

    private void DrawLine(Vector3 start, Vector3 end, Color lineColor)
    {
        GameObject myLine = new();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.startWidth = .1f;
        lr.endWidth = .1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.material = lineMaterial;
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.sortingLayerName = "Lines";
        lr.sortingOrder = 1;
        lr.gameObject.SetActive(false);
        _matchLines.Add(myLine);
        myLine.transform.parent = _lineParent;
    }
}

