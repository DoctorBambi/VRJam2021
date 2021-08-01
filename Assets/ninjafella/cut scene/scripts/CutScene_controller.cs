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

[System.Serializable]
public class Ship
{
    public GameObject ship;
    public Vector3 endLocation;
    public Vector3 warpAwayLocation;
    private Vector3 startLocation;
    public float warpTime = 5f;

    public IEnumerator warp()
    {
        float startTime = Time.time;
        startLocation = ship.transform.position;
        Vector3 pos = startLocation;
        while (pos != endLocation)
        {
            float t = (Time.time - startTime) / warpTime;
            pos = Vector3.Lerp(startLocation, endLocation, t);
            ship.transform.position = pos;
            yield return null;
        }
    }

    public IEnumerator warpAway()
    {
        float startTime = Time.time;
        startLocation = ship.transform.position;
        Vector3 pos = startLocation;
        while (pos != warpAwayLocation)
        {
            float t = (Time.time - startTime) / warpTime;
            pos = Vector3.Lerp(startLocation, warpAwayLocation, t);
            ship.transform.position = pos;
            yield return null;
        }
        ship.SetActive(false);
    }
}

[System.Serializable]
public class Line
{
    public float LineAnimationLength = 5f;
    public LineRenderer lineRednerer;
    public Vector3 endPosition;
    private Vector3 startPosition;

    public IEnumerator extend()
    {
        float startTime = Time.time;

        startPosition = lineRednerer.GetPosition(0);

        Vector3 pos = startPosition;
        while (pos != endPosition)
        {
            float t = (Time.time - startTime) / LineAnimationLength;
            pos = Vector3.Lerp(startPosition, endPosition, t);
            lineRednerer.SetPosition(1, pos);
            yield return null;
        }
    }

    public IEnumerator retract()
    {
        float startTime = Time.time;
        startPosition = lineRednerer.GetPosition(0);
        Vector3 pos = startPosition;
        while (pos != endPosition)
        {
            float t = (Time.time - startTime) / LineAnimationLength;
            pos = Vector3.Lerp(startPosition, endPosition, t);
            lineRednerer.SetPosition(0, pos);
            yield return null;
        }
    }
}

public class CutScene_controller : MonoBehaviour
{
    public Line line;
    public DisolveManager disolveManager;
    public leEarth earth;
    public GameObject earthTrigger;
    public Ship ship;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(begin());
    }

    private IEnumerator begin()
    {
        StartCoroutine(ship.warp());
        yield return new WaitForSeconds(ship.warpTime + 1f);

        StartCoroutine(line.extend());
        yield return new WaitForSeconds(line.LineAnimationLength);

        disolveManager.condenseCall();
        yield return new WaitForSeconds(5f);

        StartCoroutine(line.retract());
        yield return new WaitForSeconds(line.LineAnimationLength);

        StartCoroutine(earth.shrink());
        yield return new WaitForSeconds(earth.shrinkSpeed);

        disolveManager.dissolve();

        StartCoroutine(ship.warpAway());
        yield return new WaitForSeconds(ship.warpTime);

        earthTrigger.SetActive(true);

    }
}
