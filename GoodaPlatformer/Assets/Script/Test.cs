using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TTimer.AddTimer(Test1, 5, "Nice");
        TTimer.AddTimer(CancelTest1, 2, "Cancel");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void Test1()
    {
        Debug.Log("Gooda");
    }

    void CancelTest1()
    {
        TTimer.StopTimer("Nice");
        Debug.Log("Stop");
    }
}
