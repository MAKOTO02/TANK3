using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    void Example()
    {
        print(Application.persistentDataPath);
    }

    private void Start()
    {
        Example();
    }
}