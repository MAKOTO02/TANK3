using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    IEnumerator LoadStage(int num)
    {   
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("stage" +  num.ToString(), LoadSceneMode.Additive);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadStage(1));
    }
}
