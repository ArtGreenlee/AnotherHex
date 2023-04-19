using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static Helper helper;
    public enum enemyType
    {
        shield,
        swarm,
        rush,
        heal,
        dodge,
        gun
    };
    public static Vector3 mousePosition = Vector3.zero;
    static Plane XZPlane = new Plane(Vector3.forward, Vector3.zero);

    private void Awake()
    {
        if (helper != null)
        {
            Debug.LogError("two helpers");
            return;
        }
        helper = this; 
    }

    public static Vector3 GetMousePosition() {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (XZPlane.Raycast(ray, out distance)) {
            Vector3 hitPoint = ray.GetPoint(distance);
            //Just double check to ensure the y position is exactly zero
            hitPoint.z = 0;
            return hitPoint;
        }
        return Vector3.zero;
    }

    public void Update() {
        mousePosition = GetMousePosition();
    }

    public bool getNearestComponent<T>(Vector3 point, float range, LayerMask layerMask, out T t) where T : Component
    {
        t = null;
        Collider2D[] hits = Physics2D.OverlapCircleAll(point, range, layerMask);
        float min = float.MaxValue;
        foreach (Collider2D hit in hits)
        {
            float distance = (hit.gameObject.transform.position - point).sqrMagnitude;
            if (hit.gameObject.TryGetComponent<T>(out T component) &&
                distance < min)
            {
                t = component;
                min = distance;
            }
        }
        return t != null;
    }

    public bool getNearestComponentByTag<T>(Vector3 point, float range, string tag, out T t) where T : Component
    {
        t = null;
        Collider2D[] hits = Physics2D.OverlapCircleAll(point, range);
        float min = float.MaxValue;
        foreach (Collider2D hit in hits)
        {
            float distance = (hit.gameObject.transform.position - point).sqrMagnitude;
            if (hit.gameObject.TryGetComponent<T>(out T component) &&
                distance < min &&
                hit.gameObject.CompareTag(tag))
            {
                t = component;
                min = distance;
            }
        }
        return t != null;
    }

    public static Vector3 CalculateInterceptCourse(Vector3 aTargetPos, Vector3 aTargetSpeed, Vector3 aInterceptorPos, float aInterceptorSpeed)
    {
        Vector3 targetDir = aTargetPos - aInterceptorPos;
        float iSpeed2 = aInterceptorSpeed * aInterceptorSpeed;
        float tSpeed2 = aTargetSpeed.sqrMagnitude;
        float fDot1 = Vector3.Dot(targetDir, aTargetSpeed);
        float targetDist2 = targetDir.sqrMagnitude;
        float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
        if (d < 0.1f)  // negative == no possible course because the interceptor isn't fast enough
            return Vector3.zero;
        float sqrt = Mathf.Sqrt(d);
        float S1 = (-fDot1 - sqrt) / targetDist2;
        float S2 = (-fDot1 + sqrt) / targetDist2;
        if (S1 < 0.0001f)
        {
            if (S2 < 0.0001f)
                return Vector3.zero;
            else
                return (S2) * targetDir + aTargetSpeed;
        }
        else if (S2 < 0.0001f)
            return (S1) * targetDir + aTargetSpeed;
        else if (S1 < S2)
            return (S2) * targetDir + aTargetSpeed;
        else
            return (S1) * targetDir + aTargetSpeed;
    }

    public struct ColorRoutineData
    {
        public Color originalColor;
        public Color endColor;
        public float startTime;
        public IEnumerator routine;
    }

    public Dictionary<SpriteRenderer, ColorRoutineData> colorChangeManager = new Dictionary<SpriteRenderer, ColorRoutineData>();
    public Dictionary<SpriteRenderer, Tuple<Color, IEnumerator>> transformSwellManager = new Dictionary<SpriteRenderer, Tuple<Color, IEnumerator>>();
    public Dictionary<SpriteRenderer, Color> colorChangeOverride = new Dictionary<SpriteRenderer, Color>();

    public void overrideColor(SpriteRenderer s, Color color)
    {
        colorChangeOverride.Add(s, s.color);
        s.color = color;
    }

    public void removeOverride(SpriteRenderer s)
    {
        s.color = colorChangeOverride[s];
        colorChangeOverride.Remove(s);
    }

    public static IEnumerator flash(SpriteRenderer spriteRenderer, Color flashColor, Color originalColor, float duration)
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }

    public void fade(SpriteRenderer s, float duration)
    {
        StartCoroutine(fadeRoutine(s, duration));
    }

    public Color getColor(SpriteRenderer s)
    {
        if (colorChangeOverride.ContainsKey(s))
        {
            return colorChangeOverride[s];
        }
        if (colorChangeManager.ContainsKey(s))
        {
            return colorChangeManager[s].originalColor;
        }
        return s.color;
    }

    private IEnumerator fadeRoutine(SpriteRenderer s, float duration)
    {
        Color c = s.color;
        float startAlpha = c.a;
        float timer = 0f;
        while (timer < duration)
        {
            c.a = Mathf.Lerp(startAlpha, 0, timer / duration);
            s.color = c;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void colorSwell(SpriteRenderer s, Color swellColor, float duration)
    {
        if (colorChangeOverride.ContainsKey(s))
        {
            return;
        }
        if (colorChangeManager.ContainsKey(s))
        {
            ColorRoutineData data = colorChangeManager[s];
            data.endColor = swellColor;
            StopCoroutine(data.routine);
            data.routine = colorSwellRoutine(s, swellColor, data.originalColor, duration);
            colorChangeManager[s] = data;
        }
        else
        {
            ColorRoutineData data = new ColorRoutineData();
            data.originalColor = s.color; 
            data.endColor = swellColor;
            data.startTime = Time.time;
            data.routine = colorSwellRoutine(s, swellColor, s.color, duration);
            colorChangeManager[s] = data;
        }
        StartCoroutine(colorChangeManager[s].routine);
    }

    public void changeColorOverTime(SpriteRenderer s, Color endColor, float duration) 
    {
        StartCoroutine(changeColorOverTimeRoutine(s, endColor, duration));
    }

    public IEnumerator changeColorOverTimeRoutine(SpriteRenderer s, Color endColor, float duration)
    {
        float timer = 0;
        Color initialColor = s.color;
        while (timer < duration)
        {
            if (s == null)
            {
                colorChangeManager.Remove(s);
                yield break;
            }
            s.color = Color.Lerp(initialColor, endColor, timer / duration);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        s.color = endColor;
        colorChangeManager.Remove(s);
    }

    public void colorSwellKeepAlpha(SpriteRenderer s, Color swellColor, float duration)
    {
        swellColor.a = s.color.a;
        colorSwell(s, swellColor, duration);
    }

    public void flashColor(SpriteRenderer s, Color newColor, float duration)
    {
        if (colorChangeManager.ContainsKey(s))
        {
            ColorRoutineData data = colorChangeManager[s];
            data.endColor = newColor;
            StopCoroutine(data.routine);
            data.routine = colorChangeRoutine(s, newColor, data.originalColor, duration);
            colorChangeManager[s] = data;
        }
        else
        {
            ColorRoutineData data = new ColorRoutineData();
            data.originalColor = s.color;
            data.endColor = newColor;
            data.startTime = Time.time;
            data.routine = colorChangeRoutine(s, newColor, s.color, duration);
            colorChangeManager[s] = data;
        }
        StartCoroutine(colorChangeManager[s].routine);
    }

    private IEnumerator colorChangeRoutine(SpriteRenderer s, Color newColor, Color originalColor, float duration)
    {
        s.color = newColor;
        yield return new WaitForSeconds(duration);
        if (s != null)
        {
            s.color = originalColor;
        }
        colorChangeManager.Remove(s);
    }

    private IEnumerator colorSwellRoutine(SpriteRenderer s, Color swellColor, Color originalColor, float duration)
    {
        duration /= 2;
        float timer = 0;
        Color initialColor = s.color;
        while (timer < duration)
        {
            if (s == null)
            {
                colorChangeManager.Remove(s);
                yield break;
            }
            s.color = Color.Lerp(initialColor, swellColor, timer / duration);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        if (s == null)
        {
            colorChangeManager.Remove(s);
            yield break;
        }
        s.color = swellColor;
        timer = 0;
        while (timer < duration)
        {
            if (s == null)
            {
                colorChangeManager.Remove(s);
                yield break;
            }
            s.color = Color.Lerp(swellColor, originalColor, timer / duration);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        s.color = originalColor;
        colorChangeManager.Remove(s);
    }


    private IEnumerator transformSwellRoutine(Transform t, float change, float duration)
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
}
