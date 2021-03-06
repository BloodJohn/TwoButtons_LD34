﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HappyEnd : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitAndRestart(0.5f));
    }

    IEnumerator WaitAndRestart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            if (Input.anyKeyDown || Input.touchCount > 0)
            {
                SceneManager.LoadScene(6);
                yield break;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
}
