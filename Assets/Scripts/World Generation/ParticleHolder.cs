using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHolder : MonoBehaviour
{
    public float lifespan = 1.0f;

    public bool isRemoving = false;

    private void Update()
    {
        if (transform.childCount > 0)
        {
            if(isRemoving == false)
                 StartCoroutine(ParticleRemover());
        }
    }

    private IEnumerator ParticleRemover()
    {
        isRemoving = true;
        yield return new WaitForSeconds(lifespan);
        Destroy(transform.GetChild(0).gameObject);
        isRemoving = false;
    }
}
