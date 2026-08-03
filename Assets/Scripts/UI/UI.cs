using System;
using UnityEngine;

namespace UI
{
    public class UI : MonoBehaviour
    {

        private void Start()
        {
            //GameManager.Instance.OnScoreChanged += UpdateScore;
        }

        private void UpdateScore(int newScore)
        {
            // Update the UI with the new score
            Debug.Log($"Score updated: {newScore}");
        }
    }
}
