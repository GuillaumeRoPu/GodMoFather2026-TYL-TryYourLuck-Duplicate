using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ColdHotMinigame : MonoBehaviour
{
    [SerializeField] private GameObject _round1Cell; //UI for rounds
    [SerializeField] private GameObject _round2Cell; //UI for rounds
    [SerializeField] private GameObject _round3Cell; //UI for rounds
    [SerializeField] private PlayerSwitch _switchPlayers;
    private GameObject[] _roundCellsList; //List of all the round objects used as ui in the grid layout above
    [SerializeField] private TMP_InputField _inputNumberField; //Input Field for the players to answer their number
    [SerializeField] private TMP_Text _textBoardPlusMinus;
    [SerializeField] private TMP_Text _numberResultText; //Text to display the number at the end of the game
    [SerializeField] private TMP_Text _infoText; //Text to display information of the game
    [SerializeField] private Vector2 _minMaxNumber; //Min and max numbers for the random number chosen
    [SerializeField] private InputActionReference _enter; //Enter input for the input field
    private int _randomNumberResult; //Random number chosen at the start of the game
    private bool _player1Turn; //Boolean to know wether it's the 1st player's turn or the 2nd one's
    private float _timer; //Timer used during the update for stuff
    private int _round; //Current round number
    private int _player1Points; //Current number of rounds won for player 1
    [SerializeField] private float _maxTimerAnwser; //max number for the timer. If timer goes beyong it goes on to the next state (here from answered to waitingforanswer)
    [SerializeField] private float _maxTimerWinRound; //max number for the timer. If timer goes beyong it goes on to the next state (here from answered to waitingfornextround)
     private int _roundsToWin; //Number of rounds a player needs to win to win

    private enum GameStateCH
    {
        Start,
        WaitingForAnswer,
        Answered,
        WaitingForNewRound,
        Ended,
    }

    private GameStateCH _gameState;

    private void Start()
    {
        BeginGame(2);
    }

    public void BeginGame(int numberOfRoundsToWin)
    {
        _enter.action.Enable();
        _enter.action.started += RedeemAnswerInput;
        _round = 0;
        _roundsToWin = numberOfRoundsToWin;
        _roundCellsList = new GameObject[_roundsToWin * 2 - 1];
        Debug.Log(_roundCellsList.Length);
        for (int i = 0; i < _roundsToWin * 2 - 1; i++)
        {
            //_roundCellsList[i] = Instantiate(_roundCell, _roundCellsParent); à compléter
        }
        StartNewRound();
    }

    private void StartNewRound()
    {
        _round += 1;
        _textBoardPlusMinus.text = "";
        //_numberResultText.color = Color.HSVToRGB(0, 0, 0);
        if (_round % 2 == 0)
        {
            _player1Turn = false;
            _switchPlayers.AnimSwitch(false);
        }
        else if (_round != 1)
        {
            _player1Turn = true;
            _switchPlayers.AnimSwitch(true);
        }
        _gameState = GameStateCH.Start;
        _timer = 0;
        //_numberResultText.text = "?";
    }

    private void EndGame()
    {
        _enter.action.started -= RedeemAnswerInput;
        _enter.action.Disable();
        _roundsToWin = 0;
        _round = 0;
        foreach (GameObject obj in _roundCellsList)
        {
            Destroy(obj);
        }
        _roundCellsList = null;
    }

    private void Update()
    {
        switch (_gameState) 
        {
            case GameStateCH.Start:
                ChooseRandomNumber();
                ChangeGameState(GameStateCH.WaitingForAnswer);
                break;
            case GameStateCH.WaitingForAnswer:

                break;
            case GameStateCH.Answered:
                _timer += Time.deltaTime;
                if (_timer > _maxTimerAnwser)
                {
                    _timer = 0;
                    _player1Turn = !_player1Turn;
                    _textBoardPlusMinus.text = "";
                    ChangeGameState(GameStateCH.WaitingForAnswer);
                    Debug.Log(_round);
                }
                break;
            case GameStateCH.WaitingForNewRound:
                _timer += Time.deltaTime;
                if (_timer > _maxTimerWinRound)
                {
                    _timer = 0;
                    _player1Turn = !_player1Turn;
                    _textBoardPlusMinus.text = "";
                    StartNewRound();
                }
                break;
            case GameStateCH.Ended:
                _timer += Time.deltaTime * 0.3f;
                if (_timer >= 1)
                {
                    _timer -= 1;
                    Debug.Log("true");
                    break;
                }
                //_numberResultText.color = Color.HSVToRGB(_timer, 1, 1);
                break;
            default:
                Debug.LogError("default activated in switch");
                break;
        }
    }

    private void ChooseRandomNumber()
    {
        _randomNumberResult = Random.Range((int)_minMaxNumber.x, (int)_minMaxNumber.y + 1);
    }

    private void RedeemAnswerInput(InputAction.CallbackContext ctxt)
    {
        RedeemAnswer();
    }

    public void RedeemAnswer()
    {
        if (_gameState == GameStateCH.WaitingForAnswer)
        {
            int answer;
            if (!int.TryParse(_inputNumberField.text, out answer))
            {
                ClearInputField();
                return;
            }
            answer = int.Parse(_inputNumberField.text);
            if (answer < _minMaxNumber.x || answer > _minMaxNumber.y)
            {
                ClearInputField();
            }
            else if (answer > _randomNumberResult)
            {
                ChangeGameState(GameStateCH.Answered);
                _textBoardPlusMinus.text = "-";
                if (_player1Turn)
                {
                    _switchPlayers.AnimSwitch(true);
                }
                else
                {
                    _switchPlayers.AnimSwitch(false);
                }
            }
            else if (answer < _randomNumberResult)
            {
                SetInfoText("The number is higher");
                ChangeGameState(GameStateCH.Answered);
                _textBoardPlusMinus.text = "+";
                if (_player1Turn)
                {
                    _switchPlayers.AnimSwitch(true);
                }
                else
                {
                    _switchPlayers.AnimSwitch(false);
                }
            }
            else if (answer == _randomNumberResult)
            {
                if (_player1Turn)
                {
                    _player1Points++;
                    if (_round == 1)
                    {
                        _round1Cell.GetComponent<Image>().color = new Color(0, 0, 255, 1);
                    }
                    else if (_round == 2)
                    {
                        _round2Cell.GetComponent<Image>().color = new Color(0, 0, 255, 1);
                    }
                    else
                    {
                        _round3Cell.GetComponent<Image>().color = new Color(0, 0, 255, 1);
                    }
                }
                else
                {
                    if (_round == 1)
                    {
                        _round1Cell.GetComponent<Image>().color = new Color(255, 0, 0, 1);
                    }
                    else if (_round == 2)
                    {
                        _round2Cell.GetComponent<Image>().color = new Color(255, 0, 0, 1);
                    }
                    else
                    {
                        _round3Cell.GetComponent<Image>().color = new Color(255, 0, 0, 1);
                    }
                }
                if (_player1Points < _roundsToWin && _roundsToWin > _round - _player1Points)
                {
                    if (_player1Turn)
                    {
                        //SetInfoText("PLAYER 1 WON THE ROUND !");
                    }
                    else
                    {
                        //SetInfoText("PLAYER 2 WON THE ROUND !");
                    }
                    //_numberResultText.text = _randomNumberResult.ToString();
                    ChangeGameState(GameStateCH.WaitingForNewRound);
                }
                else
                {
                    Debug.Log("Went there, done that");
                    if (_player1Points >= _roundsToWin)
                    {
                        //SetInfoText("PLAYER 1 WON !!!!");
                    }
                    else
                    {
                        //SetInfoText("PLAYER 2 WON !!!!");
                    }
                    //_numberResultText.text = _randomNumberResult.ToString();
                    ChangeGameState(GameStateCH.Ended);
                }
            }
        }
        ClearInputField();
    }

    private void ClearInputField()
    {
        _inputNumberField.text = string.Empty;
    }

    private void SetInfoText(string text)
    {
        //_infoText.text = text;
    }

    private void ChangeGameState(GameStateCH state)
    {
        _gameState = state;
        if (state == GameStateCH.Answered)
        {
            _timer = 0;
        }
    }
}
