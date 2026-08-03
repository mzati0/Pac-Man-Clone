using UnityEngine;

namespace Misc
{
    public class PacManAnimatorBridge : MonoBehaviour
    {
        [Header("Animated by Timeline/Animation")]
        public float x;
        public float y;
        public bool death;

        private Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            _animator.SetFloat("X", x);
            _animator.SetFloat("Y", y);

            if (death)
            {
                _animator.SetTrigger("Death");
                death = false;
            }
        }
    }
}
