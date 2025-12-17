using Cysharp.Threading.Tasks;
using UnityEngine;

public class ElementAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private int idleStateHash;
    private int destroyStateHash;

    private void Awake()
    {
        idleStateHash = Animator.StringToHash("Idle");
        destroyStateHash = Animator.StringToHash("Destroy");
    }

    public void PlayIdle()
    {
        animator.Play(idleStateHash, -1, Random.Range(0f, 1f));
    }

    public async UniTask PlayDestroyAsync()
    {
        animator.Play(destroyStateHash);

        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash == destroyStateHash, cancellationToken: destroyCancellationToken);
        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f, cancellationToken: destroyCancellationToken);
    }
}
