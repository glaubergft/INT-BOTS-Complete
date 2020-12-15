using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByExpiration : MonoBehaviour
{
    [SerializeField]
    float lifeTime = 5;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(ExpireCheck());
    }

    IEnumerator ExpireCheck()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
