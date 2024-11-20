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
    public Vector3[,] GridobejctPositins = new Vector3[Column, Row];
    public int[,] Cakespotion = new int[Column, Row];
    public bool[,] IsFilled = new bool[Column, Row];
    public Transform CakesParent;
    public List<GameObject> Cakes = new List<GameObject>();

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
                GridobejctPositins[i, j] = _Child.position;
                IsFilled[i, j] = true;
                Count++;
            }
        }
    }

    bool IsPositionFree(int row, int col)
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


    public void SetupGrid()
    {
        //  CheckFilled();

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
                                // Debug.Log("==>Continue|" + C + "::" + j);
                                continue;
                            }
                            else
                            {
                                //Debug.Log("==>Set up pos|" + C + "::" + j);
                                if (CakeObjects[C, j] != null)
                                {
                                    CakeObjects[C, j].transform.position = GridobejctPositins[i, j];
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

        CheckFilled();


    }

    void CheckFilled()
    {
        for (int i = 0; i < Column; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                //  Debug.LogError("==>" + i + "::" + j + "::" + IsFilled[i, j]);
                if (IsFilled[i, j] != true)
                {
                    CakeObjects[i, j] = Instantiate(Cakes[UnityEngine.Random.Range(0, Cakes.Count)], GridobejctPositins[i, j], Quaternion.identity, CakesParent);
                    IsFilled[i, j] = true;
                }
            }
        }
    }


    public bool IsValidPos(Vector2Int position)
    {
        return position.x >= 0 && position.x < Column && position.y >= 0 && position.y < Row;
    }


}
