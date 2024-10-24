using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CurveAnimation : MonoBehaviour
{
    public Transform targetObject;
    public Transform stackObject;
    float Delay;
    public float duration = 2f;

    void OnEnable()
    {
        targetObject = transform;
        Delay = CheckingElement.Instance.Delay;
    }

    void Start()
    {
        stackObject = CheckingElement.Instance.StackObejct.transform;
        Vector3 midpoint = (targetObject.position + stackObject.position) / 2;
        //Vector3 Finalpoint = new Vector3(stackObject.position.x, stackobejctPositionY, stackObject.position.z);
        Vector3[] path = new Vector3[]
        {
            targetObject.position, // Start position
            midpoint=new Vector3(midpoint.x, midpoint.y+5, midpoint.z),
          // Finalpoint,
            stackObject.position // End position
        };
       // Debug.Log("==>Delay|" + Delay);
        targetObject.DOPath(path, duration, PathType.Linear).SetDelay(Delay).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.localScale = Vector3.zero;
            Destroy(transform.gameObject);
        
        });
    }
}