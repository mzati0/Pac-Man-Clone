using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Misc
{
    public class TilemapFlash : MonoBehaviour
    {
        public GameObject doars;
        public static TilemapFlash Instance;
        private Tilemap _tilemap;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            _tilemap = GetComponent<Tilemap>();
        }
        public void Flash()
        {
            StartCoroutine(FlashCoroutine());
        }
        
        private IEnumerator FlashCoroutine()
        {
            doars.SetActive(false);
            var baseColor = Instance._tilemap.color;
            _tilemap.color = Color.white;
            yield return new WaitForSecondsRealtime(0.25f);
            _tilemap.color = baseColor;
            yield return new WaitForSecondsRealtime(0.25f);
            _tilemap.color = Color.white;
            yield return new WaitForSecondsRealtime(0.25f);
            _tilemap.color = baseColor;
            yield return new WaitForSecondsRealtime(0.25f);
            _tilemap.color = Color.white;
            yield return new WaitForSecondsRealtime(0.25f);
            _tilemap.color = baseColor;
            yield return new WaitForSecondsRealtime(0.25f);
            _tilemap.color = Color.white;
            yield return new WaitForSecondsRealtime(0.25f);
            _tilemap.color = baseColor;
            doars.SetActive(true);
        }

    }
}
