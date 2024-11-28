using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public static int Row = 5, Column = 6;
    public GameObject[,] CakeObjects = new GameObject[Column, Row];
    public Vector3[,] GridobejctPositions = new Vector3[Column, Row];
    public bool[,] IsFilled = new bool[Column, Row];
    public Transform CakesParent;
    public List<GameObject> Cakes = new List<GameObject>();

    private const string Destroy_Element = "Destroy";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    private void Start()
    {
        int Count = 0;
        for (int i = 0; i < Column; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                Transform _Child = CakesParent.transform.GetChild(Count);
                CakeObjects[i, j] = _Child.gameObject;
                GridobejctPositions[i, j] = _Child.position;
                IsFilled[i, j] = true;
                Count++;
            }
        }

        GameInput.Instance.OnCOrrect_Match += OnCOrrect_Match;
    }

    private void OnDisable()
    {
        GameInput.Instance.OnCOrrect_Match -= OnCOrrect_Match;
    }


    private void OnCOrrect_Match(object sender, GameInput.OnCorrectMatch data)
    {
        Debug.Log("Grid Manager OnCOrrect_Match");
        for (int i = 0; i < data.ConnectedObjectsPositions.Count; i++)
        {
            Vector2Int position = data.ConnectedObjectsPositions[i];
            IsFilled[position.x, position.y] = false;
            CakeObjects[position.x, position.y] = null;
            Destroy(data.ConnectedObjects[i]);
            // data.ConnectedObjects[i].transform.Find("Parent").GetComponent<Animator>().SetTrigger(Animator.StringToHash(Destroy_Element));
            // FillDataOnSpawn(position);
        }

        // SetupGrid();
        // FillTheEmptySlots();
        StartCoroutine(FillTheEmptySlots());
    }



    private bool IsPositionFree(int row, int col)
    {
        return IsFilled[row, col];
    }

    public Vector2Int GetGridPos(GameObject ThisgameObject)
    {
        for (int i = 0; i < Column; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                if (CakeObjects[i, j] != null && CakeObjects[i, j].gameObject == ThisgameObject)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }
    public GameObject GetGridObejct(int _Column, int row)
    {
        if (Column >= 0 && _Column < Column && row >= 0 && row < Row)
            return null;

        return CakeObjects[_Column + 1, row].gameObject;
    }

    #region  MovingGrid
    public void SetupGrid()
    {

        for (int i = 0; i < Column; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                try
                {
                    //   Debug.Log("==>" + i + "::" + j + "::" + IsFilled[i, j]);
                    if (IsFilled[i, j] != false)
                    {
                        continue;
                    }
                    else
                    {
                        for (int C = i + 1; C < Column; C++)
                        {
                            if (IsFilled[C, j] != true)
                            {
                                continue;
                            }
                            else
                            {
                                //Debug.Log("==>Set up pos|" + C + "::" + j);
                                if (CakeObjects[C, j] != null)
                                {
                                    CakeObjects[C, j].transform.position = GridobejctPositions[i, j];
                                    IsFilled[i, j] = true;
                                    CakeObjects[i, j] = CakeObjects[C, j].gameObject;
                                    CakeObjects[C, j] = null;
                                    IsFilled[C, j] = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("==>Catch" + i + "::" + j + "::" + e.Message);
                    //Debug.Log("==>Catch|" + e.Message);
                }
            }
        }

        // CheckFilled();


    }


    private IEnumerator FillTheEmptySlots()
    {
        for (int i = 0; i < Column; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                if (IsFilled[i, j] == true)
                {
                    continue;
                }
                else
                {
                    for (int m = i + 1; m < Column; m++)
                    {
                        if (IsFilled[m, j] == true)
                        {
                            yield return new WaitForSeconds(0.03f);
                            StartCoroutine(AnimateCakes(CakeObjects[m, j].transform, GridobejctPositions[i, j]));
                            IsFilled[i, j] = true;
                            CakeObjects[i, j] = CakeObjects[m, j].gameObject;
                            CakeObjects[m, j] = null;
                            IsFilled[m, j] = false;
                            break;
                        }

                        // if (CakeObjects[C, j] != null)
                        // {
                        //     CakeObjects[C, j].transform.position = GridobejctPositions[i, j];
                        //     IsFilled[i, j] = true;
                        //     CakeObjects[i, j] = CakeObjects[C, j].gameObject;
                        //     CakeObjects[C, j] = null;
                        //     IsFilled[C, j] = false;
                        //     break;
                        // }
                    }
                }

            }
        }
    }

    private IEnumerator AnimateCakes(Transform currentObject, Vector3 destPosition)
    {
        float duration = 0.25f;
        // Debug.Log("Dist:" + Vector3.Distance(currentObject.position, destPosition));
        duration = Vector3.Distance(currentObject.position, destPosition) / 15;
        // Debug.Log("duration:" + duration);
        Vector3 startPosition = currentObject.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            currentObject.transform.position = Vector3.Lerp(startPosition, destPosition, Mathf.Clamp01(elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currentObject.transform.position = destPosition;
    }
    #endregion



    void CheckFilled()
    {
        for (int i = 0; i < Column; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                //  Debug.LogError("==>" + i + "::" + j + "::" + IsFilled[i, j]);
                if (IsFilled[i, j] != true)
                {
                    CakeObjects[i, j] = Instantiate(Cakes[UnityEngine.Random.Range(0, Cakes.Count)], GridobejctPositions[i, j], Quaternion.identity, CakesParent);
                    IsFilled[i, j] = true;
                }
            }
        }
    }


    public bool IsValidPos(Vector2Int position)
    {
        return position.x >= 0 && position.x < Column && position.y >= 0 && position.y < Row;
    }

    public void FillDataOnSpawn(Vector2Int position)
    {
        CakeObjects[position.x, position.y] = Instantiate(Cakes[UnityEngine.Random.Range(0, Cakes.Count)], GridobejctPositions[position.x, position.y], Quaternion.identity, CakesParent);
        IsFilled[position.x, position.y] = true;
    }


}
