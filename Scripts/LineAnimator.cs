using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LineAnimator : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float drawSpeed = 0.05f;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;   // 트레일 유지 시간
    public AnimationCurve fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);

    Coroutine currentRoutine;

    class PointData
    {
        public Vector3 pos;
        public float time;
    }

    List<PointData> points = new List<PointData>();

    void Update()
    {
        RemoveOldPoints();
    }

    // 새로 그리기
    public void DrawPath(List<Vector3> path)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        points.Clear();
        lineRenderer.positionCount = 0;

        currentRoutine = StartCoroutine(AnimatePath(path));
    }

    // 기존 라인 뒤에 추가
    public void AddPath(List<Vector3> path)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(AnimateAddPath(path));
    }

    IEnumerator AnimatePath(List<Vector3> path)
    {
        lineRenderer.positionCount = 0;
        points.Clear();

        for (int i = 0; i < path.Count; i++)
        {
            AddPoint(path[i]);
            yield return new WaitForSeconds(drawSpeed);
        }

        currentRoutine = null;
    }

    IEnumerator AnimateAddPath(List<Vector3> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            AddPoint(path[i]);
            yield return new WaitForSeconds(drawSpeed);
        }

        currentRoutine = null;
    }

    void AddPoint(Vector3 pos)
    {
        points.Add(new PointData
        {
            pos = pos,
            time = Time.time
        });

        lineRenderer.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i].pos);
        }
    }

    void RemoveOldPoints()
    {
        if (points.Count == 0) return;

        float now = Time.time;

        while (points.Count > 0)
        {
            float age = now - points[0].time;

            if (age > fadeDuration)
            {
                points.RemoveAt(0);
                lineRenderer.positionCount = points.Count;

                // 전체 재세팅 (인덱스 밀림 방지)
                for (int i = 0; i < points.Count; i++)
                {
                    lineRenderer.SetPosition(i, points[i].pos);
                }
            }
            else
            {
                break;
            }
        }
    }
}