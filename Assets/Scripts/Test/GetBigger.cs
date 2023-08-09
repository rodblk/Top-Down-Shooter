using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBigger : MonoBehaviour
{
    public float scaleIncrement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(scaleIncrement,scaleIncrement,scaleIncrement);
    }
}
