using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPointIndicatorTEMP : MonoBehaviour
{
    public float lifeTime = 4f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimedDeath());
    }

    IEnumerator TimedDeath()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }
}
