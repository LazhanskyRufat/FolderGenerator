using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using DG.Tweening;


public class EdgeDetectorTest : MonoBehaviour
{
    private const bool AdjustBorder = true;

    //private readonly int[,] matrix =
    //{
    //    {1,1,1,1,0,0,0,0,0,0},
    //    {1,1,1,1,0,0,0,0,0,0},
    //    {1,1,1,1,0,0,1,1,0,0},
    //    {0,0,0,0,0,0,1,1,1,0},
    //    {0,0,1,1,1,1,1,1,1,0},
    //    {0,0,1,1,1,1,1,1,1,0},
    //    {0,0,1,1,1,1,1,1,1,0},
    //};

    private readonly int[,] matrix =
    {
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1},
        {0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1},
        {0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1},
        {0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0},
    };

    private Vector3 _brushPosition;
    private float[,] _edgeMatrix;
    private int[,] _enumeratedEdges;


    void Start()
    {
        _edgeMatrix = SimpleEdgeDetector.Prewitt(matrix, AdjustBorder);
        _enumeratedEdges = EnumerateEdges(_edgeMatrix);
        MoveBrushPosition(_enumeratedEdges);
    }

    private void OnDrawGizmos()
    {
        int h = matrix.GetLength(0);
        int w = matrix.GetLength(1);

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                if (matrix[i, j] == 1)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.grey;
                }

                Gizmos.DrawWireCube(new Vector3(j, -i, 0), Vector3.one);
            }
        }

        if (_edgeMatrix != null)
        {
            Gizmos.color = Color.black;

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    //if (_edgeMatrix[i, j] > 0)
                    //{
                    //    if (!AdjustBorder)
                    //    {
                    //        Gizmos.color = Color.Lerp(Color.grey, Color.black, (_edgeMatrix[i, j] - 1) / 10f);
                    //    }
                    //    Gizmos.DrawCube(new Vector3(j, -i, 0), 0.9f * Vector3.one);
                    //}

                    if(_enumeratedEdges[i, j] > 0)
                    {
                        Gizmos.color = Color.Lerp(Color.white, Color.black, (_enumeratedEdges[i, j] - 1) / 1f);
                        Gizmos.DrawCube(new Vector3(j, -i, 0), 0.9f * Vector3.one);
                    }                    
                }
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_brushPosition, 1f);
    }

    private void MoveBrushPosition(int[,] edgeMatrix)
    {
        List<Vector3> edgePoints = new List<Vector3>();
        Stack<Vector3> points = new Stack<Vector3>();
        int h = matrix.GetLength(0);
        int w = matrix.GetLength(1);

        float[,] converterd = new float[h, w];

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                converterd[i, j] = edgeMatrix[i, j];
            }
        }

        MoveBrushPosition(converterd);
    }

    private void MoveBrushPosition(float[,] edgeMatrix)
    {
        List<Vector3> edgePoints = new List<Vector3>();
        List<Vector3> shuffledPoints = new List<Vector3>();
        Stack<Vector3> points = new Stack<Vector3>();
        
        int h = matrix.GetLength(0);
        int w = matrix.GetLength(1);

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                if(edgeMatrix[i,j] > 0)
                {
                    edgePoints.Add(new Vector3(j, -i, 0));
                }
            }
        }

        //for (int i = 0, c = edgePoints.Count; i < c; i++)
        //{
        //    int count = edgePoints.Count;
        //    int randomIndex = UnityEngine.Random.Range(0, count);

        //    shuffledPoints.Add(edgePoints[randomIndex]);
        //    edgePoints.RemoveAt(randomIndex);
        //}

        //foreach (Vector3 point in shuffledPoints.OrderBy(value => edgeMatrix[-(int)value.y, (int)value.x]))
        //{
        //    points.Push(point);
        //}

        foreach (Vector3 point in edgePoints.OrderBy(value => edgeMatrix[-(int)value.y, (int)value.x]).ThenBy(value => value.x))
        {
            points.Push(point);
        }

        NextMove(points);
    }

    private void NextMove(Stack<Vector3> points)
    {
        if (points.Count > 0)
        {
            Vector3 nextPoint = points.Peek();

            while (points.Count > 0 && _brushPosition.x == points.Peek().x)
            {
                nextPoint = points.Pop();
            }

            float distance = (nextPoint - _brushPosition).magnitude;
            float speed = 20f;
            float time = distance / speed;

            DG.Tweening.Core.DOGetter<Vector3> getter = () => _brushPosition;
            DG.Tweening.Core.DOSetter<Vector3> setter = (value) => _brushPosition = value;
            DOTween.To(getter, setter, nextPoint, time).OnComplete(() => NextMove(points));
        }
    }

    private int[,] EnumerateEdges(float[,] edgeMatrix)
    {
        int h = edgeMatrix.GetLength(0);
        int w = edgeMatrix.GetLength(1);

        int[,] result = new int[edgeMatrix.GetLength(0), edgeMatrix.GetLength(1)];
        List<Vector2> temp = new List<Vector2>();

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                if (edgeMatrix[i, j] > 0)
                {
                    temp.Add(new Vector2(i, j));
                }
            }
        }

        int counter = 0;

        while(temp.Count > 0)
        {
            bool canContinue = false;

            for (int i = 0, c = temp.Count; i < c; i++)
            {
                Vector2 point = temp[i];
                int x = (int)point.x;
                int y = (int)point.y;

                for (int k = -1; k <= 1; k++)
                {
                    for (int m = -1; m <= 1; m++)
                    {
                        int dx = x + k;
                        int dy = y + m;

                        if (dx >= 0 && dx < h && dy >= 0 && dy < w)
                        {
                            int val = result[dx, dy];

                            if (val > 0)
                            {
                                temp.RemoveAt(i);
                                result[x, y] = val;
                                canContinue = true;
                                break;
                            }
                        }
                    }

                    if(canContinue)
                    {
                        break;
                    }
                }
                if (canContinue)
                {
                    break;
                }
            }

            if(canContinue)
            {
                continue;
            }

            counter++;
            Vector2 firstPoint = temp[0];
            int fx = (int)firstPoint.x;
            int fy = (int)firstPoint.y;
            temp.RemoveAt(0);
            result[fx, fy] = counter;
        }

        return result;
    }
}
