using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MainMenuGate : MonoBehaviour
{
    SpriteRenderer sr;
    List<Light2D> childrens = new List<Light2D>();
    public GameObject trigger;

    float moveSpeed = -3f;
    bool canMove = false;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        foreach (Transform tf in GetComponentsInChildren<Transform>())
        {
            if (tf.gameObject.name == name) continue;
            var _ = tf.GetComponent<Light2D>();
            if (_ != null) childrens.Add(_);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;
        Vector3 pos = transform.position;

        float dX = moveSpeed * Time.deltaTime;
        transform.position = new Vector3(pos.x + dX, pos.y, pos.z);
    }

    public void ShowGate()
    {
        StartCoroutine("Dissolve");
    }

    IEnumerator Dissolve()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if ((trigger.transform.position.x > 7) && (trigger.transform.position.x < 9))
                break;
        }

        float x = 0f;
        canMove = true;
        while (true)
        {
            x += 0.0085f;
            sr.material.SetFloat("_Delta", x);
            yield return new WaitForSeconds(0.01f);

            if (x > 1f) break;
        }

        foreach (Light2D children in childrens)
        {
            while (true)
            {
                children.intensity -= 0.5f;
                yield return new WaitForSeconds(0.01f);
            
                if (children.intensity < 0f)
                {
                    children.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}
