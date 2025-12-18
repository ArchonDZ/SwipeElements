using DG.Tweening;
using UnityEngine;

namespace Elements.Configs
{
    [CreateAssetMenu(fileName = nameof(ElementConfig), menuName = "Configs/" + nameof(ElementConfig))]
    public class ElementConfig : ScriptableObject
    {
        [SerializeField] private float swapDuration = 0.2f;
        [SerializeField] private Ease swapEase = Ease.Linear;
        [SerializeField] private float dropDuration = 0.4f;
        [SerializeField] private Ease dropEase = Ease.InExpo;
        [SerializeField] private float destroyDuration = 1f;

        [Header("Animator")]
        [SerializeField] private string idleStateName = "Idle";
        [SerializeField] private string destroyStateName = "Destroy";

        public float SwapDuration => swapDuration;
        public Ease SwapEase => swapEase;
        public float DropDuration => dropDuration;
        public Ease DropEase => dropEase;
        public float DestroyDuration => destroyDuration;
        public string IdleStateName => idleStateName;
        public string DestroyStateName => destroyStateName;
    }
}