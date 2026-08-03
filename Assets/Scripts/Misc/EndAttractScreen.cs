using UnityEngine;

namespace Misc
{
    public class EndAttractScreen : MonoBehaviour
    {
        public void EndAttract()
        {
            FindAnyObjectByType<AttractScreen>().EndAttractScreen();
        }
    }
}
