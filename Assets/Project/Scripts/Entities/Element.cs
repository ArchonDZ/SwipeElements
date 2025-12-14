using UnityEngine;

namespace Elements.Entities
{
    public class Element : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void SetSortingOrder(int value)
        {
            spriteRenderer.sortingOrder = value;
        }
    }
}