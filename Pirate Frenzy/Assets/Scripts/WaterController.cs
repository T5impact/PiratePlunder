using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    private float startOffsetX;
    private float startPosX, startPosY, length;

    private float currentPosX;

    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float bobSpeed = 2;
    [SerializeField] private float bobHeight = 0.25f;


    // Start is called before the first frame update
    void Start()
    {
        startPosX = transform.position.x;
        //startPosX = 0;
        startPosY = transform.position.y;

        currentPosX = 0;

        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = moveSpeed * Time.fixedDeltaTime;
        currentPosX += distance;

        float bobDistance = Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.position = new Vector3(startPosX + currentPosX, startPosY + bobDistance, transform.position.z);

        if (transform.position.x > length)
        {
            //startPosX -= length;
            currentPosX -= length * 3;
            startOffsetX = 0;
        }
        /*else if (transform.position.x < startPosX - length)
        {
            startPosX += length;
        }*/
    }
}
