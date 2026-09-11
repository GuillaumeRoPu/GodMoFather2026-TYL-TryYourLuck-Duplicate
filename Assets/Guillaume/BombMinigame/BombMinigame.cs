using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BombMinigame : MonoBehaviour
{
    [SerializeField] private GameObject _round1Cell; //UI for rounds
    [SerializeField] private GameObject _round2Cell; //UI for rounds
    [SerializeField] private GameObject _round3Cell; //UI for rounds
    [SerializeField] private PlayerSwitch _switchPlayersUI; //UI to switch between player 1 and player 2
    [SerializeField] private GameObject _wheel; //Wheel object
    [SerializeField] private Sprite _wheel5Sprite;
    [SerializeField] private Sprite _wheel10Sprite;
    [SerializeField] private Sprite _wheel15Sprite;
    [SerializeField] private float _additionalRotation5;
    [SerializeField] private float _additionalRotation10;
    [SerializeField] private float _additionalRotation15;
    [SerializeField] private BalloonAnims _balloonAnims;
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private Vector2 _minMax5;
    [SerializeField] private Vector2 _minMax10;
    [SerializeField] private Vector2 _minMax15;
    private Vector2 _minMaxNumber; //Min and max numbers for the random number chosen
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
    [SerializeField] private int[] _wheel5Options;
    [SerializeField] private int[] _wheel10Options;
    [SerializeField] private int[] _wheel15Options;
    private int[] _spinOptions; //Array that keeps track of the number of the wheel
    private int _roundsToWin; //Number of rounds a player needs to win to win

    [SerializeField] private UnityEvent _gameStarted;
    [SerializeField] private UnityEvent _startSpin;
    [SerializeField] private UnityEvent _stopSpin;
    [SerializeField] private UnityEvent _roundWon;
    [SerializeField] private UnityEvent _gameWon;
    [SerializeField] private UnityEvent _openCurtains;
    [SerializeField] private UnityEvent _closeCurtains;
    [SerializeField] private UnityEvent _gameEnded;

    private enum GameStateBalloon
    {
        Start,
        WaitingForSpin,
        Spinning,
        RespinToMiddle,
        PumpingBalloon,
        WaitingForNewRound,
        Ended,
        CloseCurtains,
        None,
    }

    private enum End
    {
        none,
        close,
        open,
        closeagain,
        done,
    }

    private GameStateBalloon _gameState;
    private End _endState;
    [SerializeField] private GameWinnerView _gmView;
    [SerializeField] private GameObject _vicScreen;
    private int _playerWon;

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
        _gameStarted.Invoke();
        StartNewRound();
    }

    private void StartNewRound()
    {
        _round += 1;
        if (_round % 2 == 0)
        {
            _player1Turn = false;
            _switchPlayersUI.AnimSwitch(false);
        }
        else
        {
            _player1Turn = true;
            _switchPlayersUI.AnimSwitch(true);
        }
        _gameState = GameStateBalloon.Start;
        _timer = 0;
        _totalNumberOfPumps = 0;
        _goalNumberOfPumps = 0;
        if (_round == 1)
        {
            _wheel.GetComponent<Image>().sprite = _wheel5Sprite;
            _spinOptions = _wheel5Options;
        }
        else if (_round == 2)
        {
            _wheel.GetComponent<Image>().sprite = _wheel10Sprite;
            _spinOptions = _wheel10Options;
        }
        else
        {
            _wheel.GetComponent<Image>().sprite = _wheel15Sprite;
            _spinOptions = _wheel15Options;
        }
        ChooseRandomNumber();
        _balloonAnims.Base();
    }

    private void EndGame()
    {
        _space.action.started -= RedeemAnswerInput;
        _space.action.Disable();
        _roundsToWin = 0;
        _round = 0;
        _gameEnded.Invoke();
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
                float additionalRotation;
                if (_round == 1)
                {
                    additionalRotation = _additionalRotation5;
                }
                else if (_round == 2)
                {
                    additionalRotation = _additionalRotation10;
                }
                else
                {
                    additionalRotation = _additionalRotation15;
                }
                _wheel.transform.rotation = Quaternion.Euler(0,0,0);
                _wheel.transform.Rotate(new Vector3(0,0, _spinCurve.Evaluate(_timer / _spinTime) * (_spinRotation + 90 + additionalRotation)));
                if (_timer > _spinTime)
                {
                    _timer = 0;
                    ChangeGameState(GameStateBalloon.RespinToMiddle);
                }
                break;
            case GameStateBalloon.RespinToMiddle:
                _timer += Time.deltaTime;
                _wheel.transform.rotation = Quaternion.Euler(0, 0, 0);
                float additionalRotation2;
                if (_round == 1)
                {
                    additionalRotation2 = _additionalRotation5;
                }
                else if (_round == 2)
                {
                    additionalRotation2 = _additionalRotation10;
                }
                else
                {
                    additionalRotation2 = _additionalRotation15;
                }
                float recalculatedAngle = (((_spinResult * 360) / _spinOptions.Length) + ((0.5f * 360) / _spinOptions.Length)) + 90 + additionalRotation2 + 1800;
                _wheel.transform.Rotate(new Vector3(0, 0, Mathf.Lerp(_spinRotation + 90 + additionalRotation2, recalculatedAngle, _spinCurve.Evaluate(_timer / _respinTime))));
                if (_timer > _respinTime)
                {
                    _timer = 0;
                    ChangeGameState(GameStateBalloon.PumpingBalloon);
                }
                break;
            case GameStateBalloon.PumpingBalloon:
                _timer += Time.deltaTime;
                if (_totalNumberOfPumps >= _goalNumberOfPumps)
                {
                    _totalNumberOfPumps = _goalNumberOfPumps;
                    _player1Turn = !_player1Turn;
                    if (_player1Turn)
                    {
                        SetInfoText("Player 1 spins");
                        _switchPlayersUI.AnimSwitch(true);
                    }
                    else
                    {
                        SetInfoText("Player 2 spins");
                        _switchPlayersUI.AnimSwitch(false);
                    }
                    ChangeGameState(GameStateBalloon.WaitingForSpin);
                }
                break;
            case GameStateBalloon.WaitingForNewRound:
                _timer += Time.deltaTime;
                if (_timer > _timeBetweenRounds)
                {
                    _timer = 0;
                    _player1Turn = !_player1Turn;
                    if (_player1Turn)
                    {
                        _switchPlayersUI.AnimSwitch(true);
                        SetInfoText("Player 1 spins");
                    }
                    else
                    {
                        _switchPlayersUI.AnimSwitch(false);
                        SetInfoText("Player 2 spins");
                    }
                    StartNewRound();
                }
                break;
            case GameStateBalloon.Ended:
                _timer += Time.deltaTime;
                if (_timer >= 3)
                {
                    _timer = 0;
                    ChangeGameState(GameStateBalloon.CloseCurtains);
                    _closeCurtains.Invoke();
                    break;
                }
                break;
            case GameStateBalloon.CloseCurtains:
                _timer += Time.deltaTime;
                if (_timer >= 2)
                {
                    EndGame();
                    ChangeGameState(End.close);
                    ChangeGameState(GameStateBalloon.None);
                }
                break;
            case GameStateBalloon.None:
                break;
            default:
                Debug.LogError("default activated in switch");
                break;
        }

        switch (_endState)
        {
            case End.none:
                break;
            case End.close:
                _timer += Time.deltaTime;
                if (_timer > 0.5f)
                {
                    ChangeGameState(End.open);
                }
                break;
            case End.open:
                _timer += Time.deltaTime;
                if (_timer > 3)
                {
                    ChangeGameState(End.closeagain);
                }
                break;
            case End.closeagain:
                _timer += Time.deltaTime;
                if (_timer > 2)
                {
                    ChangeGameState(End.done);
                }
                break;
            case End.done:
                break;
        }
    }

    private void ChooseRandomNumber()
    {
        if (_round == 1)
        {
            _minMaxNumber = _minMax5;
        }
        else if (_round == 2)
        {
            _minMaxNumber = _minMax10;
        }
        else
        {
            _minMaxNumber = _minMax15;
        }
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
                }
            }
            _spinRotation += 1800;
            ChangeGameState(GameStateBalloon.Spinning);
        }
        else if (_gameState == GameStateBalloon.PumpingBalloon && _timer > _timeBetweenPumps)
        {
            _totalNumberOfPumps++;
            _timer = 0;
            if (_totalNumberOfPumps >= _randomNumberResult)
            {
                _balloonAnims.BlowUp();
                if (_player1Turn)
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
                else
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
                        SetInfoText("PLAYER 1 WON THE ROUND !");
                        _playerWon = 0;
                    }
                    else
                    {
                        SetInfoText("PLAYER 2 WON THE ROUND !");
                        _playerWon = 1;
                    }
                    ChangeGameState(GameStateBalloon.Ended);
                }
            }
            else
            {
                _balloonAnims.Pump();
                if (_totalNumberOfPumps >= _goalNumberOfPumps)
                {
                    Debug.Log("Went here");
                    _totalNumberOfPumps = _goalNumberOfPumps;
                    _player1Turn = !_player1Turn;
                    if (_player1Turn)
                    {
                        SetInfoText("Player 1 spins");
                        _switchPlayersUI.AnimSwitch(true);
                    }
                    else
                    {
                        SetInfoText("Player 2 spins");
                        _switchPlayersUI.AnimSwitch(false);
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

    private void ChangeGameState(End state)
    {
        _timer = 0;
        _endState = state;
        if (state == End.close)
        {
            _vicScreen.SetActive(true);
            _gmView.SetWinnerPlayerIndex(_playerWon);
        }
        if (state == End.open)
        {
            _openCurtains.Invoke();
        }
        if (state == End.closeagain)
        {
            _closeCurtains.Invoke();
        }
        if (state == End.done)
        {
            SceneManager.LoadScene(0);
        }
    }
}