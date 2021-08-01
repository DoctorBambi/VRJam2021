using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisolveManager : MonoBehaviour
{

    public Renderer shader;
    public float targetDissolve = -35f;
    public float targetCondense = 80f;
    public float step = 1f;
    private float currentDissolveValue;

    // Start is called before the first frame update
    void Start()
    {
        currentDissolveValue = targetDissolve;
    }

    IEnumerator condense()
    {
        for (float i = targetDissolve; i <= targetCondense; i = i + step)
        {
            shader.material.SetFloat("_CutoffHeight", i);
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    public void condenseCall()
    {
        StartCoroutine(condense());
    }

    public void dissolve()
    {
        shader.material.SetFloat("_CutoffHeight", targetDissolve);
    }
}
