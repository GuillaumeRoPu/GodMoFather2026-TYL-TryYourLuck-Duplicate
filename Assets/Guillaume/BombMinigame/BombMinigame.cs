using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BombMinigame : MonoBehaviour
{
    [SerializeField] private GameObject _roundCell; //Prefab for the round ui
    [SerializeField] private Transform _roundCellsParent; //Grid layout parent to display the rounds
    private GameObject[] _roundCellsList; //List of all the round objects used as ui in the grid layout above
    [SerializeField] private TMP_Text _infoText; //Text to display information of the game
    [SerializeField] private GameObject _wheel; //Wheel object
    [SerializeField] private GameObject _balloon; //balloon object
    [SerializeField] private Vector2 _minMaxNumber; //Min and max numbers for the random number chosen
    [SerializeField] private InputActionReference _space; //Enter input for the input field
    private int _randomNumberResult; //Random number chosen at the start of the game
    private int _totalNumberOfPumps; //Total number of pumps done in the round
    private int _goalNumberOfPumps; //Number of (total) pumps required for the next turn to start
    private bool _player1Turn; //Boolean to know wether it's the 1st player's turn or the 2nd one's
    private float _timer; //Timer used during the update for stuff
    private int _round; //Current round number
    private int _player1Points; //Current number of rounds won for player 1
    [SerializeField] private float _spinTime; //max number for the timer. If timer goes beyong it goes on to the next state (here for spin)
    [SerializeField] private float _respinTime; //max number for the timer. If timer goes beyong it goes on to the next state (here for respin)
    [SerializeField] private AnimationCurve _spinCurve; //Curve to know how the wheel will spin
    private float _spinRotation; //Chosen rotation for the result of the spin
    private int _spinResult; //Chosen result of the spin for the array _spinOptions
    [SerializeField] private float _timeBetweenPumps; //max number for the timer. If timer goes beyond you can pump again with space
    [SerializeField] private float _timeBetweenRounds; //max number for the timer. If timer goes beyong it goes on to the next state (here for waitingfornextround)
    [SerializeField] private int[] _spinOptions; //Array that keeps track of the number of the wheel
    private int _roundsToWin; //Number of rounds a player needs to win to win

    private enum GameStateBalloon
    {
        Start,
        WaitingForSpin,
        Spinning,
        RespinToMiddle,
        PumpingBalloon,
        WaitingForNewRound,
        Ended,
    }

    private GameStateBalloon _gameState;

    private void Start()
    {
        BeginGame(2);
    }

    public void BeginGame(int numberOfRoundsToWin)
    {
        _space.action.Enable();
        _space.action.started += RedeemAnswerInput;
        _round = 0;
        _roundsToWin = numberOfRoundsToWin;
        _roundCellsList = new GameObject[_roundsToWin * 2 - 1];
        for (int i = 0; i < _roundsToWin * 2 - 1; i++)
        {
            _roundCellsList[i] = Instantiate(_roundCell, _roundCellsParent);
        }
        StartNewRound();
    }

    private void StartNewRound()
    {
        _round += 1;
        if (_round % 2 == 0)
        {
            _player1Turn = false;
            SetInfoText("2nd Player spins");
        }
        else
        {
            _player1Turn = true;
            SetInfoText("1st Player spins");
        }
        _gameState = GameStateBalloon.Start;
        _timer = 0;
        _totalNumberOfPumps = 0;
        _goalNumberOfPumps = 0;
        _balloon.transform.localScale = new Vector3(1,1,1);
    }

    private void EndGame()
    {
        _space.action.started -= RedeemAnswerInput;
        _space.action.Disable();
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
            case GameStateBalloon.Start:
                ChooseRandomNumber();
                ChangeGameState(GameStateBalloon.WaitingForSpin);
                break;
            case GameStateBalloon.WaitingForSpin:

                break;
            case GameStateBalloon.Spinning:
                _timer += Time.deltaTime;
                _wheel.transform.rotation = Quaternion.Euler(0,0,0);
                _wheel.transform.Rotate(new Vector3(0,0, _spinCurve.Evaluate(_timer / _spinTime) * (_spinRotation + 90)));
                if (_timer > _spinTime)
                {
                    _timer = 0;
                    ChangeGameState(GameStateBalloon.RespinToMiddle);
                }
                break;
            case GameStateBalloon.RespinToMiddle:
                _timer += Time.deltaTime;
                _wheel.transform.rotation = Quaternion.Euler(0, 0, 0);
                float recalculatedAngle = (((_spinResult * 360) / _spinOptions.Length) + ((0.5f * 360) / _spinOptions.Length)) + 90 + 1800;
                _wheel.transform.Rotate(new Vector3(0, 0, Mathf.Lerp(_spinRotation + 90, recalculatedAngle, _spinCurve.Evaluate(_timer / _respinTime))));
                if (_timer > _respinTime)
                {
                    _timer = 0;
                    ChangeGameState(GameStateBalloon.PumpingBalloon);
                }
                break;
            case GameStateBalloon.PumpingBalloon:
                _timer += Time.deltaTime;
                break;
            case GameStateBalloon.WaitingForNewRound:
                _timer += Time.deltaTime;
                if (_timer > _timeBetweenRounds)
                {
                    _timer = 0;
                    _player1Turn = !_player1Turn;
                    if (_player1Turn)
                    {
                        SetInfoText("Player 1 spins");
                    }
                    else
                    {
                        SetInfoText("Player 2 spins");
                    }
                    StartNewRound();
                }
                break;
            case GameStateBalloon.Ended:
                _timer += Time.deltaTime * 0.3f;
                if (_timer >= 1)
                {
                    _timer -= 1;
                    Debug.Log("true");
                    break;
                }
                break;
            default:
                Debug.LogError("default activated in switch");
                break;
        }
    }

    private void ChooseRandomNumber()
    {
        _randomNumberResult = Random.Range((int)_minMaxNumber.x, (int)_minMaxNumber.y);
    }

    private void RedeemAnswerInput(InputAction.CallbackContext ctxt)
    {
        SpinTheWheel();
    }

    public void SpinTheWheel()
    {
        if (_gameState == GameStateBalloon.WaitingForSpin)
        {
            _timer = 0;
            SetInfoText("");
            _spinRotation = Random.Range(0, 360);
            for (int i = 0; i < _spinOptions.Length; i++)
            {
                if (_spinRotation / 360 < ((float)i + 1f)/_spinOptions.Length && _spinRotation / 360 >= (float)i/_spinOptions.Length)
                {
                    _goalNumberOfPumps += _spinOptions[i];
                    _spinResult = i;
                    Debug.Log("Number of pumps added : " + _spinOptions[i]);
                    Debug.Log("Rot : " + _spinRotation / 360 + " Length : " + i / _spinOptions.Length + " length + 1 : " + (i + 1) / _spinOptions.Length);
                }
            }
            _spinRotation += 1800;
            ChangeGameState(GameStateBalloon.Spinning);
        }
        else if (_gameState == GameStateBalloon.PumpingBalloon && _timer > _timeBetweenPumps)
        {
            _timer = 0;
            if (_totalNumberOfPumps >= _randomNumberResult)
            {
                if (_player1Turn)
                {
                    _roundCellsList[_round - 1].GetComponent<Image>().color = new Color(255, 0, 0, 1);
                }
                else
                {
                    _player1Points++;
                    _roundCellsList[_round - 1].GetComponent<Image>().color = new Color(0, 180, 255, 1);
                }
                if (_player1Points < _roundsToWin && _roundsToWin > _round - _player1Points)
                {
                    if (_player1Turn)
                    {
                        SetInfoText("PLAYER 2 WON THE ROUND !");
                    }
                    else
                    {
                        SetInfoText("PLAYER 1 WON THE ROUND !");
                    }
                    ChangeGameState(GameStateBalloon.WaitingForNewRound);
                }
                else
                {
                    Debug.Log("Went there, done that");
                    if (_player1Points >= _roundsToWin)
                    {
                        SetInfoText("PLAYER 1 WON !!!!");
                    }
                    else
                    {
                        SetInfoText("PLAYER 2 WON !!!!");
                    }
                    ChangeGameState(GameStateBalloon.Ended);
                }
            }
            else
            {
                _totalNumberOfPumps++;
                _balloon.transform.localScale = new Vector3(_balloon.transform.localScale.x + 0.2f, _balloon.transform.localScale.y + 0.2f, _balloon.transform.localScale.z);
                if (_totalNumberOfPumps >= _goalNumberOfPumps)
                {
                    Debug.Log("Went here");
                    _totalNumberOfPumps = _goalNumberOfPumps;
                    _player1Turn = !_player1Turn;
                    if (_player1Turn)
                    {
                        SetInfoText("Player 1 spins");
                    }
                    else
                    {
                        SetInfoText("Player 2 spins");
                    }
                    ChangeGameState(GameStateBalloon.WaitingForSpin);
                }
                else
                {
                    int pumpsLeft = _goalNumberOfPumps - _totalNumberOfPumps;
                    if (_player1Turn)
                    {
                        string text = "Player 1 PUMPS " + pumpsLeft + " TIMES !";
                        if (pumpsLeft == 1)
                        {
                            text = "Player 1 PUMPS " + pumpsLeft + " TIME !";
                        }
                        SetInfoText(text);
                    }
                    else
                    {
                        string text = "Player 2 PUMPS " + pumpsLeft + " TIMES !";
                        if (pumpsLeft == 1)
                        {
                            text = "Player 2 PUMPS " + pumpsLeft + " TIME !";
                        }
                        SetInfoText(text);
                    }
                }
            }
        }
    }

    private void SetInfoText(string text)
    {
        _infoText.text = text;
    }

    private void ChangeGameState(GameStateBalloon state)
    {
        _gameState = state;
        if (state == GameStateBalloon.Spinning)
        {
            _timer = 0;
        }
        if (state == GameStateBalloon.PumpingBalloon)
        {
            if (_player1Turn)
            {
                string text = "Player 1 PUMPS " + _spinOptions[_spinResult] + " TIMES !";
                if (_spinOptions[_spinResult] == 1)
                {
                    text = "Player 1 PUMPS " + _spinOptions[_spinResult] + " TIME !";
                }
                SetInfoText(text);
            }
            else
            {
                string text = "Player 2 PUMPS " + _spinOptions[_spinResult] + " TIMES !";
                if (_spinOptions[_spinResult] == 1)
                {
                    text = "Player 2 PUMPS " + _spinOptions[_spinResult] + " TIME !";
                }
                SetInfoText(text);
            }
        }
    }
}