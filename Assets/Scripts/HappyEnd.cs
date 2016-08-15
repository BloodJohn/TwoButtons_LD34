using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HappyEnd : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        StartCoroutine(WaitAndRestart(1f));
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
