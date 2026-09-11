using UnityEngine;

public class CurtainOpenClose : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void PullDown()
    {
        _animator.SetTrigger("PullDown");
    }
}
