using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Navigator : MonoBehaviour
{
    Vector3 scale = Vector3.one;
    // Use this for initialization
    void OnEnable()
    {
        this.transform.localScale = scale;
    }

    void OnDisable()
    {
        //SetDestination(string.Empty);
        this.transform.localScale = scale;
    }

    void Update()
    {
        this.transform.localScale = scale * (1 + Mathf.Sin(Mathf.PI * Time.time) * .2f);
    }
}
