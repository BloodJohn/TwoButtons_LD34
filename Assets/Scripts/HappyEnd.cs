using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HappyEnd : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        GoogleAnalyticsV3.instance.LogScreen("Level " + SceneManager.GetActiveScene().buildIndex);
        #endif

        StartCoroutine(WaitAndRestart(1f));
    }

    IEnumerator WaitAndRestart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            if (Input.anyKeyDown || Input.touchCount > 0)
            {
                SceneManager.LoadScene(0);
                yield break;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
}
