using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scriptGameTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Untagged")
        {
            SceneManager.LoadScene("_scene3Main", LoadSceneMode.Single);
        }
    }
}
