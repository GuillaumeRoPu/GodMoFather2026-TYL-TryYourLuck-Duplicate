using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ColdHotMinigame : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputNumberField;
    [SerializeField] private Image _downArrow;
    [SerializeField] private Image _upArrow;
    [SerializeField] private TMP_Text _numberResultText;
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private Vector2 _minMaxNumber;
    private int _randomNumberResult;
    private bool _player1Turn;
    private bool _initializdAnswerTurn;
    private float _timer;
    [SerializeField] private float _maxTimerAnwser;

    private enum GameStateCH
    {
        Start,
        WaitingForAnswer,
        Answered,
        Ended,
    }

    private GameStateCH _gameState;

    private void Awake()
    {
        _player1Turn = true;
        _gameState = GameStateCH.Start;
        _timer = 0;
    }

    private void Start()
    {
        _downArrow.color = new Color(_downArrow.color.r, _downArrow.color.g, _downArrow.color.b, 0);
        _upArrow.color = new Color(_downArrow.color.r, _downArrow.color.g, _downArrow.color.b, 0);
        _numberResultText.color = Color.HSVToRGB(0, 0, 0);
    }

    private void OnDestroy()
    {

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

    public void ClickedButton()
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
