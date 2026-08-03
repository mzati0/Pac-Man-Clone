using System;
using UnityEngine;

public class ClydeController : MonoBehaviour
{
    [SerializeField] private Transform scatterTarget;
    [SerializeField] private Transform pacTarget;
    GhostPathing ghostPathing;
    void Start()
    {
        ghostPathing = gameObject.GetComponent<GhostPathing>();
    }
    void Update()
    {
       if(Vector2.Distance(pacTarget.position, transform.position) < 8f){
            ghostPathing.targetPac = scatterTarget;

        } else {
            ghostPathing.targetPac = pacTarget;
        }
    }
}
