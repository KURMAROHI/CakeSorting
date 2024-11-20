using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance;
    private const int Cake_LayerValue = 6;
    private Camera _cameraMain;
    private bool _isDragging = false;
    private int _minObjectsUserNeedToConnect = 3;
    private Vector2Int _oldGridPOs, _currentGridPos;
    private string _tag { get; set; }
    
    [SerializeField] private List<GameObject> _connectedObjects = new List<GameObject>();
    [SerializeField] private List<Vector2Int> _connectedObjectsPositions = new List<Vector2Int>();


    public event EventHandler<Transform> OnSelect_EachObject;  // call on if each Object Selected
    public event EventHandler<OnCorrectMatch> OnCOrrect_Match; //  Call on if user conncetd _minObjectsUserNeedToConnect or more
    public event EventHandler<List<GameObject>> OnWrong_Match;  // call on if user connected less than _minObjectsUserNeedToConnect

    public class OnCorrectMatch : EventArgs
    {
        public List<GameObject> ConnectedObjects;
        public List<Vector2Int> ConnectedObjectsPositions;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Already one Instance is Avilable");
        }
        Instance = this;
        _cameraMain = Camera.main;
    }

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            _connectedObjects.Clear();
            _connectedObjectsPositions.Clear();

            GameObject HitObject = CheckRayCasting(Input.mousePosition);
            if (HitObject != null)
            {
                if (!_connectedObjects.Contains(HitObject))
                {
                    _currentGridPos = GridManager.Instance.GetGridPos(HitObject);
                    _tag = HitObject.tag;
                    OnSelect_EachObject?.Invoke(this, HitObject.transform);

                    _oldGridPOs = _currentGridPos;
                    AddObjectPositionInList(HitObject, _currentGridPos);

                }
                _isDragging = true;
            }
        }
        else if (_isDragging && Input.GetMouseButton(0))
        {
            Vector3 mousePos = _cameraMain.ScreenToWorldPoint(Input.mousePosition);
            GameObject HitObject = CheckRayCasting(Input.mousePosition);
            if (HitObject != null && !_connectedObjects.Contains(HitObject) && HitObject.CompareTag(_tag))
            {
                _currentGridPos = GridManager.Instance.GetGridPos(HitObject);
                if (CheckValidPos(ref _oldGridPOs, ref _currentGridPos))
                {
                    OnSelect_EachObject?.Invoke(this, HitObject.transform);
                    AddObjectPositionInList(HitObject, _currentGridPos);
                }
            }


        }
        else if (Input.GetMouseButtonUp(0))
        {

            if (_connectedObjects.Count >= _minObjectsUserNeedToConnect)
            {
                Debug.Log("Connected 3 of more Objects");
                OnCOrrect_Match?.Invoke(this, new OnCorrectMatch { ConnectedObjects = this._connectedObjects, ConnectedObjectsPositions = this._connectedObjectsPositions });
            }
            else
            {
                OnWrong_Match?.Invoke(this, _connectedObjects);
            }
        }
#endif
    }


    private GameObject CheckRayCasting(Vector3 Position)
    {
        RaycastHit Hit;
        Ray ray = _cameraMain.ScreenPointToRay(Position);

        if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject.layer.Equals(Cake_LayerValue))
        {
            //Debug.LogError("its....... Hitting Mama:");
            return Hit.collider.gameObject;
        }
        return null;
    }

    private bool CheckValidPos(ref Vector2Int OldGridpos, ref Vector2Int CurrentGridPos)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int newGridPos = OldGridpos + new Vector2Int(i, j);
                //   Debug.Log("newGridPos:" + CurrentGridPos + "::" + new Vector2Int(i, j) + "::" + newGridPos);
                if (newGridPos == CurrentGridPos)
                {
                    OldGridpos = CurrentGridPos;
                    return GridManager.Instance.IsValidPos(CurrentGridPos);
                }
                else
                {
                    continue;
                }
            }
        }

        return false;
    }

    private void AddObjectPositionInList(GameObject hitObject, Vector2Int gridPos)
    {
        _connectedObjects.Add(hitObject);
        _connectedObjectsPositions.Add(gridPos);
    }


}
