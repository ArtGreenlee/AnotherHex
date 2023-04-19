using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms;

public class DeathFragmentation : MonoBehaviour
{
    private FragmentObjectPool fragmentObjectPool;
    public static DeathFragmentation instance;
    private Helper helper;
    public float fragmentFadeDelay;
    public float fragmentFadeDuration;

    public Color fragmentColor;
    static class Combinations
    {
        // Enumerate all possible m-size combinations of [0, 1, ..., n-1] array
        // in lexicographic order (first [0, 1, 2, ..., m-1]).
        private static IEnumerable<int[]> CombinationsRosettaWoRecursion(int m, int n)
        {
            int[] result = new int[m];
            Stack<int> stack = new Stack<int>(m);
            stack.Push(0);
            while (stack.Count > 0)
            {
                int index = stack.Count - 1;
                int value = stack.Pop();
                while (value < n)
                {
                    result[index++] = value++;
                    stack.Push(value);
                    if (index != m) continue;
                    yield return (int[])result.Clone(); // thanks to @xanatos
                                                        // yield return result;
                    break;
                }
            }
        }

        public static IEnumerable<T[]> CombinationsRosettaWoRecursion<T>(T[] array, int m)
        {
            if (array.Length < m)
                throw new ArgumentException("Array length can't be less than number of selected elements");
            if (m < 1)
                throw new ArgumentException("Number of selected elements can't be less than 1");
            T[] result = new T[m];
            foreach (int[] j in CombinationsRosettaWoRecursion(m, array.Length))
            {
                for (int i = 0; i < m; i++)
                {
                    result[i] = array[j[i]];
                }
                yield return result;
            }
        }
    }

    private static ushort[] triangleIndices = new ushort[] { 0, 1, 2};
    public Texture2D texture;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        helper = Helper.helper;
        fragmentObjectPool = FragmentObjectPool.fragmentObjectPool;
        //createFragment(new Vector2[] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, 1) }, Vector3.one, Color.red);
    }


    /*void Start()
    {
        Vector2[] vertices = new Vector2[] { new Vector2(1, 1), new Vector2(.05f, 1.3f), new Vector2(1, 2), new Vector2(1.95f, 1.3f), new Vector2(1.58f, 0.2f), new Vector2(.4f, .2f) };
        ushort[] triangles = new ushort[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 1 };
        DrawPolygon2D(vertices, triangles, Color.red);
    }*/
    private List<Fragment> fragmentSprite(SpriteRenderer sr)
    {
        List<Vector2> vertices = sr.sprite.vertices.ToList();//Combinations.CombinationsRosettaWoRecursion<Vector2>(sr.sprite.vertices, 2).ToList();
        Vector2 v = Vector2.zero;
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = sr.transform.TransformVector(vertices[i]);
            v += vertices[i];
        }
        v /= vertices.Count;
        List<Vector2> sorted = vertices.OrderBy(p => -Math.Atan2(p.x - v.x, p.y - v.y)).ToList();

        /* LineRenderer l = LineRendererObjectPool.lineRendererObjectPool.Pool.Get();
         l.positionCount = vertices.Count;
         int counter = 0;
         foreach (Vector2 v2 in sorted)
         {
             l.SetPosition(counter, v2);
             counter++;
         }*/

        List<Fragment> fragments = new List<Fragment>();
        int count = 0;

        for (int i = 0; i < sorted.Count; i++)
        {
            count++;
            fragments.Add(createFragment(new Vector2[] { sorted[i], sorted[(i + 1) % sorted.Count], v }, sr.transform.position));
        }
        //Debug.Log("Vertices length: " + sr.sprite.vertices.Length.ToString());
        //Debug.Log("Drawing: " + count.ToString() + " triangles");
        return fragments;
    }

    public IEnumerator equilateralize(Fragment fragment, Vector2[] vertices, float xDiff, float yDiff)
    {
        /*Vector3 scale = fragment.transform.localScale;
        while (Mathf.Abs(xDiff - yDiff) > .001f)
        {
            if (xDiff > yDiff)
            {
                scale.x = 1 - (xDiff - yDiff * Time.deltaTime);
                xDiff -= Time.deltaTime;
            }
            else
            {
                scale.y = 1 - (yDiff - xDiff * Time.deltaTime);
                yDiff -= Time.deltaTime;
            }
            Debug.Log(scale);
            fragment.transform.localScale = scale;
            yield return new WaitForEndOfFrame();
        }*/
        yield break;
    }
    
    public IEnumerator fadeAndRelease(Fragment fragment)
    {
        helper.fade(fragment.sr, fragmentFadeDuration);
        yield return new WaitForSeconds(fragmentFadeDuration);
        fragmentObjectPool.Pool.Release(fragment);
    }

    private IEnumerator fragmentBehaviorRoutine(Fragment fragment, Vector3 forceDirection, float flashDuration, float delay)
    {
        yield return new WaitForSeconds(delay);
        helper.flashColor(fragment.sr, Color.white, flashDuration);
        yield return new WaitForSeconds(flashDuration);
        helper.changeColorOverTime(fragment.sr, fragmentColor, 2);
        fragment.rb.AddForce(forceDirection / 8 + Random.insideUnitSphere, ForceMode2D.Impulse);
        //fragment.rb.AddTorque(Random.Range(-1, 1), ForceMode2D.Impulse);
        yield break;
    }

    /*public void fragmentEnemy(Entity e)
    {
        //Debug.Log("Fragmenting entity: " + e.gameObject.name);
        foreach (SpriteRenderer sr in e.spriteRenderers)
        {
            //Debug.Log("Fragmenting sprite renderer: " + sr.gameObject.name);
            Vector2 v = Random.insideUnitCircle / 3;
            List<Fragment> fragments = fragmentSprite(sr, v);
            foreach (Fragment fragment in fragments)
            {
                fragment.rb.AddForce((fragment.bary - v).normalized * 2, ForceMode2D.Impulse);
                //fragment.rb.AddTorque(Random.Range(-4, 4), ForceMode2D.Impulse);
                //StartCoroutine(fadeAfterDelay(fragment));
                //Destroy(fragment, fragmentFadeDuration);
            }
        }
    }*/

    public void fragmentEntity(Entity e)
    {
        //Debug.Log(forceDirection);
        //Debug.Log("Fragmenting entity: " + e.gameObject.name);
        //Debug.Log("fragmenting : " + e.spriteRenderers.Count.ToString() + " Sprite renderers");
        foreach (SpriteRenderer sr in e.spriteRenderers)
        {
            //Debug.Log("Fragmenting sprite renderer: " + sr.gameObject.name);
            List<Fragment> fragments = fragmentSprite(sr);

            foreach (Fragment fragment in fragments)
            {
                fragment.sr.color = helper.getColor(sr);
                Vector3 forceDirection = e.transform.position - fragment.transform.position;
                StartCoroutine(fragmentBehaviorRoutine(fragment, forceDirection, Random.Range(.05f, .1f), Random.Range(.02f, .04f)));
                
            }
        }
    }

    public IEnumerator fragmentSqrd(Fragment fragment)
    {
        yield return new WaitForSeconds(1f);
        foreach (Fragment fSqrd in fragmentSprite(fragment.sr)) {
            fSqrd.sr.color = helper.getColor(fragment.sr);
            Vector3 forceDirection = fragment.transform.position - fSqrd.transform.position;
            StartCoroutine(fragmentBehaviorRoutine(fSqrd, forceDirection, Random.Range(.05f, .1f), Random.Range(.02f, .04f)));
        }
        fragmentObjectPool.Pool.Release(fragment);
    }

    public void fragmentEntity(Entity e, Vector3 projectileVelocity, Vector3 projecilePosition)
    {
        //Debug.Log(forceDirection);
        //Debug.Log("Fragmenting entity: " + e.gameObject.name);
        //Debug.Log("fragmenting : " + e.spriteRenderers.Count.ToString() + " Sprite renderers");
        foreach (SpriteRenderer sr in e.spriteRenderers)
        {
            //Debug.Log("Fragmenting sprite renderer: " + sr.gameObject.name);
            List<Fragment> fragments = fragmentSprite(sr);
            
            foreach (Fragment fragment in fragments)
            {
                fragment.sr.color = helper.getColor(sr);
                /*if (fragment.sr.color == Color.white)
                {
                    Debug.Log("why and how");
                    fragmentObjectPool.Pool.Release(fragment);
                    continue;
                }*/
                float delay = (fragment.transform.position - projecilePosition).sqrMagnitude / 5;
                Vector3 forceVelocity = projectileVelocity;
                if (e.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                {
                    forceVelocity += (Vector3)rb.velocity;
                }
                StartCoroutine(fragmentBehaviorRoutine(fragment, forceVelocity, Random.Range(.05f, .1f), delay));
                //fragment.rb.AddTorque(Random.Range(-4, 4), ForceMode2D.Impulse);
                //StartCoroutine(fadeAfterDelay(fragment));
                //Destroy(fragment, fragmentFadeDuration);
            }
        }
    }

    Fragment createFragment(Vector2[] vertices, Vector3 position)
    {
        Fragment fragment = fragmentObjectPool.Pool.Get();

        Vector2 centroid = Vector2.zero;
        float xMin = Mathf.Infinity, yMin = Mathf.Infinity;
        float xMax = -Mathf.Infinity, yMax = -Mathf.Infinity;
        foreach (Vector2 vi in vertices)
        {
            if (vi.x > xMax)
            {
                xMax = vi.x;
            }
            if (vi.y > yMax)
            {
                yMax = vi.y;
            }
           
            if (vi.x < xMin)
            {
                xMin = vi.x;
            }
               
            if (vi.y < yMin)
            {
                yMin = vi.y;
            }
            centroid += vi;
        }

        centroid /= vertices.Length;

        Vector2[] localv = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            localv[i] = vertices[i] - new Vector2(xMin, yMin);
        }
        
        //Debug.Log("num vertices: " + vertices.Length.ToString());
        //Debug.Log("xmax : " + xMax.ToString() + " ymax: " + yMax.ToString());
        //Debug.Log("xMin : " + xMin.ToString() + " yMin: " + yMin.ToString());
        //Debug.Log("v: " + v.ToString());
        float xDiff = xMax - xMin;
        float yDiff = yMax - yMin;
        //Debug.Log("Xdiff: " + xDiff.ToString() + " yDiff: " + yDiff.ToString());
        Vector2 rectOffset = new Vector2(xDiff, yDiff);
        Vector2 colliderOffset = rectOffset / 2;
        //correct = .5f, .33f
        //Debug.Log("colliderOffset: " + colliderOffset.ToString());


        fragment.sr.sprite = Sprite.Create(texture, new Rect(0, 0, xDiff, yDiff), Vector2.zero, 1); //create a sprite with the texture we just created and colored in

        /*for (int i = 0; i < localv.Length; i++)
        {
            Debug.Log("Local: " + localv[i].ToString() + " vertices: " + vertices[i].ToString());
        }*/

        //Debug.Log(Mathf.Sqrt(xDiff * xDiff + yDiff + yDiff));

        fragment.col.radius = centroid.magnitude * 1.2f;
        fragment.col.offset = colliderOffset;
        fragment.col.enabled = true;
        //fragment.sr.color = color;
        fragment.sr.sprite.OverrideGeometry(localv, triangleIndices);
        fragment.transform.position = position + new Vector3(xMin, yMin);
        fragment.transform.localScale = Vector3.one;
        StartCoroutine(equilateralize(fragment, localv, xDiff, yDiff));
        /*if (Mathf.Sqrt(xDiff * xDiff + yDiff + yDiff) > 4)
        {
            Debug.Log("fSqrd");
            StartCoroutine(fragmentSqrd(fragment));
        }*/
        return fragment;
    }
}
