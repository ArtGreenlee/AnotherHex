using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public SpriteRenderer redBox;
    public SpriteRenderer blackBox;
    public SpriteRenderer whiteBox;
    public SpriteRenderer greenBox;
    private float currentPercentage = 1;
    private float bufferPercentage = 1;
    public float bufferWait;
    private float bufferTimer = 0;
    public float bufferSpeed;
    private static float healthLineAmount = 10;
    LineRenderer lineRenderer;
    public float transformOffset;

    private void Start()
    {
        Vector3 temp = transform.localPosition;
        temp.y = transformOffset;
        transform.localPosition = temp;
        lineRenderer = GetComponent<LineRenderer>();
    }
    /*public void registerHealthLines(float maxHealth)
    {
        int numLines = Mathf.RoundToInt(maxHealth / healthLineAmount);
        int numLinePositions = numLines + numLines - 1;
        int linePositionCount = 0;
        lineRenderer.positionCount = numLinePositions;
        for (int i = 0; i < numLines; i++)
        {
            lineRenderer.SetPosition(linePositionCount, new Vector3(0, )
        }
    }*/

    public virtual void registerChange(float percentage)
    {
        currentPercentage = percentage;

    }

    private void setBoxSize(SpriteRenderer box, float percentage)
    {
        if (box != null) {
            box.transform.localScale = new Vector2(percentage, 1);
            box.transform.localPosition = new Vector2(-(.5f - percentage / 2), 0);
        }   
    }

    private void Update()
    {
        if (bufferPercentage > currentPercentage)
        {
            setBoxSize(redBox, currentPercentage);
            setBoxSize(greenBox, currentPercentage);
            if (Time.time > bufferTimer)
            {
                bufferPercentage -= Time.deltaTime * bufferSpeed;
                if (bufferPercentage < currentPercentage)
                {
                    bufferPercentage = currentPercentage;
                }
                setBoxSize(whiteBox, bufferPercentage);
            }
        }
        else if (bufferPercentage < currentPercentage)
        {
            setBoxSize(greenBox, currentPercentage);
            setBoxSize(whiteBox, currentPercentage);
            if (Time.time > bufferTimer)
            {
                bufferPercentage += Time.deltaTime * bufferSpeed;
                if (bufferPercentage > currentPercentage)
                {
                    bufferPercentage = currentPercentage;
                }
                setBoxSize(redBox, currentPercentage);
            }
        }
    }

    /*private void Start()
    {
        // ---- ---- ---- ---- --
        health = GetComponent<Health>();
        float maxHealth = health.maxHealth;
        int numBoxes = Mathf.RoundToInt(maxHealth / boxSplitSize);
        float remainderBoxWidth = 0;
        float remainder = maxHealth % boxSplitSize;
        if (remainder != 0)
        {
            remainderBoxWidth = 
            numBoxes++;
        }
        float boxWidth = (1 - (numBoxes - 1) * boxSpacing) / boxSplitSize;
        for (int i = 0; i < numBoxes; i++)
        {

        }
    }*/

    /*public void registerChange(float amount)
    {
        float percentageChange = currentPercentage - (health.curHealth / health.maxHealth);
        if (!bufferActive)
        {
            if (percentageChange > 0)
            {
                //healing
                bufferPercentage = percentageChange;
                StartCoroutine(healingBuffer());
            }
        }

    }

    public void registerChange(float remainingPercentage)
    {
        float damage = currentPercentage - remainingPercentage;
        if (!bufferActive)
        {
            bufferActive = true;
            bufferDamage = damage;
            StartCoroutine(shrinkBuffer());
        }
        else
        {
            bufferDamage += damage;
        }
        currentPercentage = remainingPercentage;
        if (redBox != null && whiteBox != null)
        {
            redBox.transform.sizeDelta = new Vector2(-.5f + remainingPercentage, 1);
            whiteBox.transform.sizeDelta = new Vector2(-.5f + remainingPercentage + bufferDamage, 1);
        }
    }

    private IEnumerator healingBuffer()
    {
        float temp;
        do
        {
            temp = bufferPercentage;
            yield return new WaitForSeconds(bufferWait);
        }
        while (temp != bufferPercentage);
        yield return null;
    }

    private IEnumerator shrinkBuffer()
    {
        float temp;
        do
        {
            temp = bufferDamage;
            yield return new WaitForSeconds(bufferWait);
        }
        while (temp != bufferDamage);
        while (bufferDamage > 0 && whiteBox != null)
        {
            bufferDamage -= Time.deltaTime * bufferSpeedFactor;
            if (bufferDamage < 0)
            {
                bufferDamage = 0;
            }
            whiteBox.transform.sizeDelta = new Vector2(-.5f + currentPercentage + bufferDamage, 1);
            yield return new WaitForEndOfFrame();
        }
        bufferActive = false;
    }*/
}
