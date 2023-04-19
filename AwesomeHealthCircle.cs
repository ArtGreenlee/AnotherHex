using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwesomeHealthCircle : HealthBar
{
    /*public Transform redCircleTransform;
    public Transform greenCircleTransform;
    public Transform whiteCircleTransform;
    public float currentPercentage = 1;
    public float bufferPercentage = 1;
    public float bufferWait;
    private float bufferTimer = 0;
    public float bufferSpeed;
    public override void registerChange(float percentage)
    {
        currentPercentage = percentage;
        bufferTimer = Time.time + bufferWait;
    }

    private void Update()
    {
        if (bufferPercentage > currentPercentage)
        {
            setScale(redCircleTransform, currentPercentage);
            setScale(greenCircleTransform, currentPercentage);
            if (Time.time > bufferTimer)
            {
                bufferPercentage -= Time.deltaTime * bufferSpeed;
                if (bufferPercentage < currentPercentage)
                {
                    bufferPercentage = currentPercentage;
                }
                setScale(whiteCircleTransform, bufferPercentage);
            }
        }
        else if (bufferPercentage < currentPercentage)
        {
            setScale(greenCircleTransform, currentPercentage);
            setScale(whiteCircleTransform, currentPercentage);
            //healing
            if (Time.time > bufferTimer)
            {
                bufferPercentage += Time.deltaTime * bufferSpeed;
                if (bufferPercentage > currentPercentage)
                {
                    bufferPercentage = currentPercentage;
                }
                
                setScale(redCircleTransform, bufferPercentage);
            }
        }
    }

    public void setScale(Transform t, float scale)
    {
        t.localScale = new Vector3(scale, scale, scale);
    }*/
}
