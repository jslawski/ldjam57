using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyObject : MonoBehaviour
{
    private float _timeUntilDestruction = 1.0f;    

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(this._timeUntilDestruction);

        Destroy(this.gameObject);
    }
}
