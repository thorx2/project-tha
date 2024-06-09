using UnityEngine;

namespace ProjTha
{
    public class VisualController : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer visualSprite;
        [SerializeField]
        private Animator animator;

        private const int IS_MOVING = -1;


        public void ToggleMovement(bool isMoving)
        {

        }

        public void FlipSprite(bool flip)
        {
            visualSprite.flipX = flip;
        }
    }
}