using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFloatBehavior : MonoBehaviour
{

    public Text floatText;

    private IEnumerator KillOnAnimationEnd()
    {
        yield return new WaitForSeconds(.9f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(KillOnAnimationEnd());
    }
}
