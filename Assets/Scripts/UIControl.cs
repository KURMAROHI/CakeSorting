using System;
using System.Collections.Generic;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    private float _highLightScale = 1.1f;


    private void Start()
    {
        GameInput.Instance.OnSelect_EachObject += OnSelect_EachObject;
        GameInput.Instance.OnCOrrect_Match += OnCOrrect_Match;
        GameInput.Instance.OnWrong_Match += OnWrong_Match;
    }


    private void OnDisable()
    {
        GameInput.Instance.OnSelect_EachObject -= OnSelect_EachObject;
        GameInput.Instance.OnCOrrect_Match -= OnCOrrect_Match;
        GameInput.Instance.OnWrong_Match -= OnWrong_Match;
    }



    private void OnWrong_Match(object sender, List<GameObject> connectedObjects)
    {
        for (int i = 0; i < connectedObjects.Count; i++)
        {
            connectedObjects[i].transform.localScale = Vector3.one;
        }
    }

    private void OnCOrrect_Match(object sender, GameInput.OnCorrectMatch data)
    {
        Debug.LogError("Destroying Something :");

        // for (int i = 0; i < data.ConnectedObjects.Count; i++)
        // {

        // }
    }

    

    private void OnSelect_EachObject(object sender, Transform selectedObject)
    {
        selectedObject.localScale = selectedObject.localScale * _highLightScale;
    }
}
