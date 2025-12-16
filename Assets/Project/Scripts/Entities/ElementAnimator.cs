using UnityEngine;

public class ElementAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private int destroyStateHash;

    private void Awake()
    {
        destroyStateHash = Animator.StringToHash("Destroy");
    }

    private void Start()
    {
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, Random.Range(0f, 1f));
    }

    public void PlayDestroy()
    {
        animator.Play(destroyStateHash);
    }
}
