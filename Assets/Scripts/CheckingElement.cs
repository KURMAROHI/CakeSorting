using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class CheckingElement : MonoBehaviour
{
    // [Serili]
    public static CheckingElement Instance;
    public GameObject LinerendrerObejct;
    LineRenderer lineRenderer;
    public List<Vector2Int> _positions = new List<Vector2Int>();
    Camera CameraMain;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    void Start()
    {
        CameraMain = Camera.main;
        GameObject _Object = Instantiate(LinerendrerObejct, transform.position, Quaternion.identity);
        lineRenderer = _Object.GetComponent<LineRenderer>();
    }
   

   

    // void OnDrawGizmos()
    // {
    //     Vector2 MousePos =CameraMain.ScreenToWorldPoint(Input.mousePosition);
    //     Gizmos.DrawLine(MousePos, MousePos.normalized);
    // }

    void UpdateGrid()
    {
        for (int i = 0; i < _positions.Count; i++)
        {
            GridManager.Instance.IsFilled[_positions[i].x, _positions[i].y] = false;
        }
    }

}
