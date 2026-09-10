using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSwitch : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Animator _animator;
    [SerializeField] private Sprite _player1Sprite;
    [SerializeField] private Sprite _player2Sprite;
    [SerializeField] private Image _image;
    private bool _player1On = true;

    public void PullUp()
    {
        _animator.SetTrigger("PullUp");
    }

    public void PullDown()
    {
        _animator.SetTrigger("PullDown");
    }

    public void AnimSwitch(bool player1On)
    {
        if (_player1On != player1On)
        {
            _player1On = player1On;
            PullUp();
        }
    }
    public void Switch()
    {
        if (_player1On)
        {
            SwitchToPlayer1();
        }
        else 
        {
            SwitchToPlayer2();
        }
        PullDown();
    }

    private void SwitchToPlayer1()
    {
        _text.text = "PLAYER 01";
        _image.sprite = _player1Sprite;
    }

    private void SwitchToPlayer2()
    {
        _text.text = "PLAYER 02";
        _image.sprite = _player2Sprite;
    }
}
