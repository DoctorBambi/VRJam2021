﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Earth_trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Untagged")
        {
            SceneManager.LoadScene(2, LoadSceneMode.Single);
        }
    }
}
