using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class ElementAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private int idleStateHash;
    private int destroyStateHash;

    public void InitializeStates(string idleStateName, string destroyStateName)
    {
        idleStateHash = Animator.StringToHash(idleStateName);
        destroyStateHash = Animator.StringToHash(destroyStateName);
    }

    public void PlayIdle()
    {
        animator.Play(idleStateHash, -1, Random.Range(0f, 1f));
    }

    public async UniTask PlayDestroyAsync(CancellationToken externalToken)
    {
        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken, destroyCancellationToken);

        animator.Play(destroyStateHash);

        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash == destroyStateHash, cancellationToken: linkedCts.Token);
        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f, cancellationToken: linkedCts.Token);
    }
}
