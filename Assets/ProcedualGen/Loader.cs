using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public GameObject[] Dots;
    int index = 0;

    public float timeDelay;
    float curTime;

    int count;

    public bool _debugButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        count = Dots.Length;

    }

    // Update is called once per frame
    void Update()
    {
        if (_debugButton)
        {
            _debugButton = !_debugButton;
            StartCoroutine(LoadMyScene());
        }
    }

    IEnumerator LoadMyScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("ProcedualTest", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
    void FixedUpdate()
    {
        if (curTime > timeDelay)
        {
            index++;
            index %= count;
            foreach (var item in Dots)
            {
                item.SetActive(false);
            }
            Dots[index].SetActive(true);
            curTime = 0;
        }
        else
        {
            curTime += Time.fixedDeltaTime;
        }
    }
}
