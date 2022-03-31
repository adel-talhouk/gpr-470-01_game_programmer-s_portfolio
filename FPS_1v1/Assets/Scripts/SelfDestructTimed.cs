using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructTimed : MonoBehaviour
{
    public float lifeTime = 0.25f;
    Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = FindObjectOfType<PlayerController>().transform;
        StartCoroutine(TimedSelfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position - (playerTransform.position - transform.position));
    }

    IEnumerator TimedSelfDestruct()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }
}
