using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_DestroyOnClick : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    transform.GetComponent<Identity>().isNeeded = false;
                    Destroy(gameObject);
                }
            }
        }
    }
}
