using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAnimator : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float drawSpeed = 0.05f;

    public void DrawPath(List<Vector3> path)
    {
        StopAllCoroutines();
        StartCoroutine(AnimatePath(path));
    }

    IEnumerator AnimatePath(List<Vector3> path)
    {
        lineRenderer.positionCount = 0;

        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.positionCount++;

            lineRenderer.SetPosition(i, path[i]);

            yield return new WaitForSeconds(drawSpeed);
        }
    }
}