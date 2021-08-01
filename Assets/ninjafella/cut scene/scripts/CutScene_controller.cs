using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class leEarth
{
    public GameObject Earth;
    public Vector3 smallSize;
    private Vector3 originalSize;
    public float shrinkSpeed = 5f;

    public IEnumerator shrink()
    {
        float startTime = Time.time;
        originalSize = Earth.transform.localScale;
        Vector3 scale = originalSize;

        while (scale != smallSize)
        {
            float t = (Time.time - startTime) / shrinkSpeed;
            scale = Vector3.Lerp(originalSize, smallSize, t);
            Earth.transform.localScale = scale;
            yield return null;

        }

    }
}

public class CutScene_controller : MonoBehaviour
{

    public float LineAnimationLength = 5f;
    public LineRenderer lineRednerer;
    public DisolveManager disolveManager;
    public leEarth earth;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AnimateLine());
        //disolveManager.condenseCall();
        //StartCoroutine(earth.shrink());
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
        disolveManager.condenseCall();
        yield return new WaitForSeconds(5f);
        //lineRednerer.enabled = false;
        startTime = Time.time;
        pos = startPosition;
        while (pos != endPosition)
        {
            float t = (Time.time - startTime) / LineAnimationLength;
            pos = Vector3.Lerp(startPosition, endPosition, t);
            lineRednerer.SetPosition(0, pos);
            yield return null;
        }
        StartCoroutine(earth.shrink());
        yield return new WaitForSeconds(5f);
        disolveManager.dissolve(); 
    }
}
