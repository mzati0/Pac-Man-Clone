using UnityEngine;

namespace Misc
{
    public class GhostAnimatorBridge : MonoBehaviour
    {
        [Header("Animated by Timeline/Animation")]
        public float x;
        public float y;

        public bool frightened;
        public bool dead;   

        private Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            _animator.SetFloat("X", x);
            _animator.SetFloat("Y", y);

            _animator.SetBool("Frightened", frightened);
            _animator.SetBool("Dead", dead);
        }
    }
}
