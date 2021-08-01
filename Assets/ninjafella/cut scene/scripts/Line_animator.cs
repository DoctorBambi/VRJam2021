using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_animator : MonoBehaviour
{

    public float LineAnimationLength = 5f;
    public LineRenderer lineRednerer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AnimateLine());
    }

    private IEnumerator AnimateLine()
    {
        float startTime = Time.time;

        Vector3 startPosition = lineRednerer.GetPosition(0);
        Vector3 endPosition = lineRednerer.GetPosition(1);

        Vector3 pos = startPosition;
        while (pos != endPosition)
        {
            float t = (Time.time - startTime) / LineAnimationLength;
            pos = Vector3.Lerp(startPosition, endPosition, t);
            lineRednerer.SetPosition(1, pos);
            yield return null;

        }
    }
}
