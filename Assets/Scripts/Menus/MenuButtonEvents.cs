using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonEvents : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Escape))
        {
            InitializeExitProcess();
            Application.Quit();
        }
    }

    private void InitializeExitProcess()
    {
        WorldDetailIdentity[] detectors = FindObjectsOfType<WorldDetailIdentity>();
        foreach (WorldDetailIdentity detector in detectors)
        {
            detector.allowDetection = false;
        }
    }
}
