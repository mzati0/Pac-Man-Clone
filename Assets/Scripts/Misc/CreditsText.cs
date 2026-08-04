using System;
using UnityEngine;

namespace Misc
{
    public class CreditsText : MonoBehaviour
    {
        public static CreditsText Instance { get; private set; }
        private SpriteRenderer[] _spriteRenderers;

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

        public void ShowCredits()
        {  
                foreach (var sr in _spriteRenderers)
                {
                    sr.enabled = true;
                }
        }
        
        public void HideCredits()
        {
            foreach (var sr in _spriteRenderers)
            {
                sr.enabled = false;
            }
            
        }
    }
}
