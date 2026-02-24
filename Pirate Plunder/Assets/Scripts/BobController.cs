using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobController : MonoBehaviour
{
    private float startPosY;
    [SerializeField] private float bobSpeed = 2;
    [SerializeField] private float bobHeight = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        startPosY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float bobDistance = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, startPosY + bobDistance, transform.position.z);
    }
}
