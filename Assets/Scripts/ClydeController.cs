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
        if(!ghostPathing.house && !ghostPathing.dead && !GhostManager.instance.frightened && !GhostManager.instance.scatter){
            print(Vector2.Distance(pacTarget.position, transform.position));
            if(Vector2.Distance(pacTarget.position, transform.position) < 8f){
                ghostPathing.targetPac = scatterTarget;
            } else {
                ghostPathing.targetPac = pacTarget;
            }
        }
    }
}
