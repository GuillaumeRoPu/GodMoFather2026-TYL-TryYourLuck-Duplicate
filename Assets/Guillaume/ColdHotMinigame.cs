using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ColdHotMinigame : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputNumberField; //Input Field for the players to answer their number
    [SerializeField] private Image _downArrow; //image for down arrow
    [SerializeField] private Image _upArrow; //image for up arrow
    [SerializeField] private TMP_Text _numberResultText; //Text to display the number at the end of the game
    [SerializeField] private TMP_Text _infoText; //Text to display information of the game
    [SerializeField] private Vector2 _minMaxNumber; //Min and max numbers for the random number chosen
    [SerializeField] private InputActionReference _enter; //Enter input for the input field
    private int _randomNumberResult; //Random number chosen at the start of the game
    private bool _player1Turn; //Boolean to know wether it's the 1st player's turn or the 2nd one's
    private float _timer; //Timer used during the update for stuff
    [SerializeField] private float _maxTimerAnwser; //max number for the timer. If timer goes beyong it goes on to the next state (here from answered to waitingforanswer)

    private enum GameStateCH
    {
        Start,
        WaitingForAnswer,
        Answered,
        Ended,
    }

    private GameStateCH _gameState;

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        _downArrow.color = new Color(_downArrow.color.r, _downArrow.color.g, _downArrow.color.b, 0);
        _upArrow.color = new Color(_downArrow.color.r, _downArrow.color.g, _downArrow.color.b, 0);
        _numberResultText.color = Color.HSVToRGB(0, 0, 0);
        _enter.action.Enable();
        _enter.action.started += RedeemAnswerInput;
        _player1Turn = true;
        _gameState = GameStateCH.Start;
        _timer = 0;
        SetInfoText("1st Player can choose a number to answer");
    }

    private void OnDestroy()
    {
        _enter.action.started -= RedeemAnswerInput;
        _enter.action.Disable();
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
                    _downArrow.color = new Color(_downArrow.color.r, _downArrow.color.g, _downArrow.color.b, 0);
                    _upArrow.color = new Color(_downArrow.color.r, _downArrow.color.g, _downArrow.color.b, 0);
                    if (_player1Turn)
                    {
                        SetInfoText("1st Player can choose a number to answer");
                    }
                    else
                    {
                        SetInfoText("2nd Player can choose a number to answer");
                    }
                    ChangeGameState(GameStateCH.WaitingForAnswer);
                }
                break;
            case GameStateCH.Ended:
                _timer += Time.deltaTime * 0.3f;
                if (_timer >= 1)
                {
                    _timer -= 1;
                }
                _numberResultText.color = Color.HSVToRGB(_timer, 1, 1);
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
        RedeemAnswer();
    }

    public void RedeemAnswer()
    {
        if (_gameState == GameStateCH.WaitingForAnswer)
        {
            int answer;
            if (!int.TryParse(_inputNumberField.text, out answer))
            {
                SetInfoText("Your answer isn't a whole number");
                ClearInputField();
                return;
            }
            answer = int.Parse(_inputNumberField.text);
            if (answer < 0 || answer > 100)
            {
                SetInfoText("You didn't answer a correct number");
            }
            else if (answer > _randomNumberResult)
            {
                SetInfoText("The number is lower than : " + answer);
                ChangeGameState(GameStateCH.Answered);
                _downArrow.color = new Color(_downArrow.color.r, _downArrow.color.g, _downArrow.color.b, 1);
            }
            else if (answer < _randomNumberResult)
            {
                SetInfoText("The number is higher than : " + answer);
                ChangeGameState(GameStateCH.Answered);
                _upArrow.color = new Color(_downArrow.color.r, _downArrow.color.g, _downArrow.color.b, 1);
            }
            else if (answer == _randomNumberResult)
            {
                if (_player1Turn)
                {
                    SetInfoText("WIN FOR PLAYER 1 !!!!");
                }
                else
                {
                    SetInfoText("WIN FOR PLAYER 2 !!!!");
                }
                _numberResultText.text = _randomNumberResult.ToString();
                ChangeGameState(GameStateCH.Ended);
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
        _infoText.text = text;
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
