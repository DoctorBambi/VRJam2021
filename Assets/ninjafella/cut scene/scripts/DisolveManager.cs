using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisolveManager : MonoBehaviour
{

    public Renderer shader;
    public float targetDissolve = 0.84f;
    public float targetCondense = 0f;
    public float step = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        shader.material.SetFloat("_Threshold", targetDissolve);
    }

    public IEnumerator condense()
    {
        Debug.Log("Condense");
        for (float i = targetDissolve; i >= targetCondense; i = i - step)
        {
            Debug.Log(i);
            shader.material.SetFloat("_Threshold", i);
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    public void dissolve()
    {
        shader.material.SetFloat("_Threshold", targetDissolve);
    }
}
