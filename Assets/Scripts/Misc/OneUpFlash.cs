using System;
using UnityEngine;

namespace Misc
{
    public class OneUpFlash : MonoBehaviour
    {
        public static OneUpFlash Instance { get; private set; }
        private SpriteRenderer[] _spriteRenderers;
        [SerializeField] private float flashInterval = 0.5f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        public void StartFlash()
        {
            StopAllCoroutines();
            StartCoroutine(FlashCoroutine());
        }
        
        public void StopFlash()
        {
            StopCoroutine(FlashCoroutine());
            foreach (var sr in _spriteRenderers)
            {
                sr.enabled = true;
            }
        }
        
        private System.Collections.IEnumerator FlashCoroutine()
        {
            while (true)
            {
                foreach (var sr in _spriteRenderers)
                {
                    sr.enabled = !sr.enabled;
                }
                yield return new WaitForSecondsRealtime(flashInterval);
            }
        }
        
    }
}
