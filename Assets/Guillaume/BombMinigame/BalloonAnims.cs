using UnityEngine;

public class BalloonAnims : MonoBehaviour
{

    [SerializeField] private Animator _animator;

    public void Pump()
    {
        _animator.SetTrigger("Pump");
    }

    public void BlowUp()
    {
        _animator.SetTrigger("BlowUp");
    }

    public void Base()
    {
        _animator.SetTrigger("Base");
    }
}
