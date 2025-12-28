using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class LogoScene : MonoBehaviour
{

    [SerializeField]
    Image image;
    [SerializeField]
    float logoTime = 2.0f;


    private void Start()
    {
        StartCoroutine(SceneLoadYield());
    }
    IEnumerator SceneLoadYield()
    {
        image.gameObject.SetActive(true);

        Color c = new Color(1f, 1f, 1f, 0f);

        image.color = c;

        yield return new WaitForSeconds(0.5f);

        bool bExit = false;
        for (; ; )
        {
            c.a += (Time.deltaTime * 2.0f);
            if (c.a >= 1)
            {
                c.a = 1;
                bExit = true;
            }
            image.color = c;

            if (bExit)
            {
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(logoTime);

        c.a = 1.0f;
        bExit = false;

        for (; ; )
        {
            c.a -= (Time.deltaTime * 2.0f);
            if(c.a <= 0)
            {
                c.a = 0;
                bExit = true;
            }
            image.color = c;

            if(bExit)
            {
                break;
            }
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
    }

}
