using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    DamageTextObjectPool damageTextObjectPool;
    public TextMeshProUGUI textMeshPro;
    public float awakeDuration;
    public float fadeMagnitude;
    public float fadeDuration;
    public float surgeMagnitude;
    public float surgeDuration;
    public float curveMagnitude;
    public float curveFrequency;
    public float curveDuration;
    Helper helper;
    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        helper = Helper.helper;
        damageTextObjectPool = DamageTextObjectPool.damageTextObjectPool;
    }

    public void showDamageText(Vector3 position, float damage, Color color)
    {   
        transform.position = position + Random.insideUnitSphere / 2;
        textMeshPro.text = Mathf.Abs(damage).ToString();
        /*for (int i = 0; i < damageTextColorThreshold.Count; i++)
        {
            if (damage >= damageTextColorThreshold[i])
            {
                textMeshPro.color = damageTextColor[i];
            }
        }*/
        textMeshPro.color = color;
        transform.localScale = Vector3.one;
        StartCoroutine(damageText());
    }

    public void showDamageText(Vector3 position, float damage)
    {
        transform.position = position + Random.insideUnitSphere / 2;
        textMeshPro.text = Mathf.Abs(damage).ToString();
        /*for (int i = 0; i < damageTextColorThreshold.Count; i++)
        {
            if (damage >= damageTextColorThreshold[i])
            {
                textMeshPro.color = damageTextColor[i];
            }
        }*/
        textMeshPro.color = Color.white;
        transform.localScale = Vector3.one;
        StartCoroutine(damageText());
    }

    private IEnumerator damageText()
    {
        //StartCoroutine(curve());
        StartCoroutine(swellTextScale(textMeshPro.rectTransform, surgeMagnitude, surgeDuration));
        StartCoroutine(swellTextColor());
        //StartCoroutine(curve());
        yield return new WaitForSeconds(awakeDuration);
        damageTextObjectPool.Pool.Release(this);
    }

    private void Update()
    {
        textMeshPro.transform.position += Vector3.up * Time.deltaTime;
    }

    private IEnumerator swellTextScale(RectTransform t, float change, float duration)
    {
        Vector3 startScale = t.localScale;
        Vector3 endScale = new Vector3(startScale.x + change, startScale.y + change, startScale.z);
        float timer = 0;
        while (timer < duration)
        {
            t.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        t.localScale = endScale;
        timer = 0;
        while (timer < duration)
        {
            t.localScale = Vector3.Lerp(endScale, startScale, timer / duration);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        t.localScale = startScale;
    }

    private IEnumerator swellTextColor()
    {
        Color color = Color.white;
        float duration = .1f;
        duration /= 2;
        Color startColor = textMeshPro.color;
        float timer = 0;
        while (timer < duration)
        {
            textMeshPro.color = Color.Lerp(startColor, color, timer / duration);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        textMeshPro.color = color;
        timer = 0;
        while (timer < duration)
        {
            textMeshPro.color = Color.Lerp(color, startColor, timer / duration);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        textMeshPro.color = startColor;
    }

    /*private IEnumerator curve()
    {
        Vector3 diff = Vector3.zero;
        Vector3 startPos = textMeshPro.transform.position;
        float start = Time.deltaTime;
        float direction = 1f;
        if (Random.value > .5f)
        {
            direction = -1f;
        }
        float duration = 0;
        while (duration < curveDuration)
        {
            duration += Time.deltaTime;
            diff.x = (duration - start) * curveFrequency;
            diff.y = curveMagnitude * Mathf.Sin(diff.x);
            textMeshPro.transform.position = startPos + diff;
            yield return new WaitForEndOfFrame();
        }
    }
    /*private IEnumerator surge()
    {

    }

    private IEnumerator fade()
    {

    }*/
}
