using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class CheckingElement : MonoBehaviour
{
    public static CheckingElement Instance;
    public GameObject LinerendrerObejct;
    bool IsDragging = false;
    string Tag = string.Empty;
    public List<GameObject> ConnectedObjects = new List<GameObject>();

    public Vector2Int CurrentGripos, OldGridPOs;
    private List<Vector3> points = new List<Vector3>();
    LineRenderer lineRenderer;
    int Lenth = 3; //user Can Collect 3 or more
    public GameObject StackObejct;
    public float Delay = 0;
    float offsetY = 0.07f; //For Stacking the Cake 
    int Totaltarget = 20;
    int PresentTarget = 0;
    public Image TargetImage;
    public Text TargetText;
    public GameObject TargetObject;

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
    void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {



#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            points.Clear();
            ConnectedObjects.Clear();
            _positions.Clear();
            lineRenderer.positionCount = 0;
            Delay = 0f;

            GameObject HitObject = CheckrayCasting(Input.mousePosition);

            if (HitObject != null && HitObject.layer.Equals(6))
            {
                //Debug.Log("==>Detected|" + HitObject.name);
                if (!ConnectedObjects.Contains(HitObject))
                {
                    OldGridPOs = GridManager.Instance.GetGridPos(HitObject);
                    CurrentGripos = OldGridPOs;
                    _positions.Add(CurrentGripos);
                    ConnectedObjects.Add(HitObject);
                }
                Tag = HitObject.tag;
                IsDragging = true;
            }
        }
        else if (IsDragging && Input.GetMouseButton(0))
        {

            Vector3 mousePos = CameraMain.ScreenToWorldPoint(Input.mousePosition);
            mousePos.y = 15;

            // if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], mousePos) > 0.1f)
            // {
            //     points.Add(mousePos);
            //     lineRenderer.positionCount = points.Count;
            //     lineRenderer.SetPosition(points.Count - 1, mousePos);
            // }

            GameObject HitObject = CheckrayCasting(Input.mousePosition);
            if (HitObject != null && !ConnectedObjects.Contains(HitObject) &&
            HitObject.layer.Equals(6) && HitObject.gameObject.tag.Equals(Tag))
            {
                CurrentGripos = GridManager.Instance.GetGridPos(HitObject);
                if (checkvalidpos(OldGridPOs, CurrentGripos))
                {
                    _positions.Add(CurrentGripos);
                    OldGridPOs = CurrentGripos;
                    ConnectedObjects.Add(HitObject);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {

            if (ConnectedObjects.Count >= Lenth)
            {

                StackObejct = ConnectedObjects[0];
                for (int i = 1; i < ConnectedObjects.Count; i++)
                {
                    ConnectedObjects[i].transform.GetComponent<CurveAnimation>().enabled = true;
                    Delay += offsetY;
                    //   Debug.Log("==>|" + Delay);
                }
                PresentTarget += ConnectedObjects.Count;
                UpdateGrid();
                SetAnimation();
            }
            Tag = string.Empty;
            ConnectedObjects.Clear();
            _positions.Clear();
            points.Clear();
            lineRenderer.positionCount = 0;
        }

#else

        if (Input.touchCount > 0)
        {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    points.Clear();
                ConnectedObjects.Clear();
                _positions.Clear();
                lineRenderer.positionCount = 0;
                Delay = 0f;

                GameObject HitObject = CheckrayCasting(touch.position);

                    if (HitObject != null && HitObject.layer.Equals(6))
                    {
                        //Debug.Log("==>Detected|" + HitObject.name);
                        if (!ConnectedObjects.Contains(HitObject))
                        {
                            OldGridPOs = GridManager.Instance.GetGridPos(HitObject);
                            CurrentGripos = OldGridPOs;
                            _positions.Add(CurrentGripos);
                            ConnectedObjects.Add(HitObject);
                        }
                        Tag = HitObject.tag;
                        IsDragging = true;
                    }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                    Vector3 mousePos =CameraMain.ScreenToWorldPoint(touch.position);
                    mousePos.y = 15;

                    // if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], mousePos) > 0.1f)
                    // {
                    //     points.Add(mousePos);
                    //     lineRenderer.positionCount = points.Count;
                    //     lineRenderer.SetPosition(points.Count - 1, mousePos);
                    // }

                    GameObject HitObject = CheckrayCasting(touch.position);
                    if (HitObject != null && !ConnectedObjects.Contains(HitObject) &&
                    HitObject.layer.Equals(6) && HitObject.gameObject.tag.Equals(Tag))
                    {
                        CurrentGripos = GridManager.Instance.GetGridPos(HitObject);
                        if (checkvalidpos(OldGridPOs, CurrentGripos))
                        {
                            _positions.Add(CurrentGripos);
                            OldGridPOs = CurrentGripos;
                            ConnectedObjects.Add(HitObject);
                        }
                    }
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                
                if (ConnectedObjects.Count >= Lenth)
                {

                    StackObejct = ConnectedObjects[0];
                    for (int i = 1; i < ConnectedObjects.Count; i++)
                    {
                        ConnectedObjects[i].transform.GetComponent<CurveAnimation>().enabled = true;
                        Delay += offsetY;
                        //   Debug.Log("==>|" + Delay);
                    }
                    PresentTarget += ConnectedObjects.Count;
                    UpdateGrid();
                    SetAnimation();
                }
                Tag = string.Empty;
                ConnectedObjects.Clear();
                _positions.Clear();
                points.Clear();
                lineRenderer.positionCount = 0;
            }

        }
#endif
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

    [SerializeField] Vector3 offset;
    void SetAnimation()
    {

        RectTransform rectTransform = TargetObject.GetComponent<RectTransform>();
        // Transform transform = (Transform)rectTransform;

        Vector3 worldPosition = CameraMain.ScreenToWorldPoint(rectTransform.localPosition) + offset;
       // Debug.LogError("==>Wordl Position|" + worldPosition);
        // Vector3 Dest = transform.TransformPoint(TargetObject.transform.position);

        StackObejct.transform.DOScale(new Vector3(0.6f, 1f, 1.2f), 0.2f).SetDelay(0.8f).OnComplete(() =>
        {
            StackObejct.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuart);
            StackObejct.transform.DOMove(worldPosition, 0.5f).SetEase(Ease.InOutQuart).OnComplete(() =>
            {
                float fillAmount = 1 / ((float)Totaltarget / (float)PresentTarget);
                TargetImage.DOFillAmount(fillAmount, 1f).SetEase(Ease.InOutQuart);
                if (PresentTarget > Totaltarget)
                {
                    PresentTarget = Totaltarget;
                }
                TargetText.text = PresentTarget.ToString() + "/" + Totaltarget.ToString();
                GridManager.Instance.SetupGrid1();
            });
        });

    }

    GameObject CheckrayCasting(Vector3 Position)
    {
        Ray ray = CameraMain.ScreenPointToRay(Position);
        RaycastHit Hit;
        if (Physics.Raycast(ray, out Hit))
        {
            if (Hit.collider.gameObject.layer.Equals(6))
            {
                return Hit.collider.gameObject;
            }
        }
        return null;
    }


    bool checkvalidpos(Vector2Int OldGridpos, Vector2Int CurrentGridPos)
    {
        // Debug.LogError("Current|" + CurrentGridPos + "::" + OldGridpos);
        if ((OldGridpos.x == CurrentGridPos.x && (OldGridpos.y == CurrentGridPos.y + 1 || OldGridpos.y == CurrentGridPos.y - 1))
        || (OldGridpos.y == CurrentGridPos.y && (OldGridpos.x == CurrentGridPos.x - 1 || OldGridpos.x == CurrentGridPos.x + 1))
        || (OldGridpos.x == CurrentGridPos.x + 1 && (OldGridpos.y == CurrentGridPos.y - 1 || OldGridpos.y == CurrentGridPos.y + 1))
        || (OldGridpos.x == CurrentGridPos.x - 1 && (OldGridpos.y == CurrentGridPos.y - 1 || OldGridpos.y == CurrentGridPos.y + 1)))
        {
            return true;
        }
        return false;
    }

}
