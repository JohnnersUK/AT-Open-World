using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed;
    public int camSpeed;

    public float minAngle;
    public float maxAngle;

    private float xAxisRotation;
    private float yAxisRotation;


	// Use this for initialization
	void Start ()
    {
        xAxisRotation = transform.localRotation.x;
        yAxisRotation = transform.localRotation.y;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + (transform.forward * (speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + (transform.forward * (-speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + (transform.right * (speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + (transform.right * (-speed * Time.deltaTime));
        }

        xAxisRotation += Input.GetAxis("Mouse X") * camSpeed;
        yAxisRotation += Input.GetAxis("Mouse Y") * -camSpeed;
        yAxisRotation = Mathf.Clamp(yAxisRotation, minAngle, maxAngle);

        transform.rotation = Quaternion.Euler(yAxisRotation, xAxisRotation, 0);
    }
}
