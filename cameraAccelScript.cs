using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraAccelScript : MonoBehaviour {

    float xValue;
    float xValueMinMax = 5.0f;

    float yValue;
    float yValueMinMax = 5.0f;

    float cameraSpeed = 20.0f;
    Vector3 accelerometerSmoothValue;

    // Use this for initialization
    void Start () {
        Vector3 rot = transform.localRotation.eulerAngles;
        var rotY = rot.y;
        var rotX = rot.x;
	}
	
	// Update is called once per frame
	void Update () {
        moveCameraWithAccelerometer();
	}

    void moveCameraWithAccelerometer()
    {
        //Set X Min Max
        if (xValue < -xValueMinMax)
            xValue = -xValueMinMax;

        if (xValue > xValueMinMax)
            xValue = xValueMinMax;

        //Set Y Min Max
        if (yValue < -yValueMinMax)
            yValue = -yValueMinMax;

        if (yValue > yValueMinMax)
            yValue = yValueMinMax;

        accelerometerSmoothValue = lowpass();

        xValue += accelerometerSmoothValue.x;
        yValue += accelerometerSmoothValue.y;

        this.transform.rotation = new Quaternion(yValue, xValue, 0, cameraSpeed);
    }

    //Smooth Accelerometer
    public float AccelerometerUpdateInterval = 1.0f / 100.0f;
    public float LowPassKernelWidthInSeconds = 0.001f;
    public Vector3 lowPassValue = Vector3.zero;
    Vector3 lowpass()
    {
        float LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds;//tweakable
        lowPassValue = Vector3.Lerp(lowPassValue, Input.acceleration, LowPassFilterFactor);
        return lowPassValue;
    }

}
