using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuGate : MonoBehaviour
{
    SpriteRenderer sr;

    public GameObject trigger;

    float moveSpeed = -3f;
    bool canMove = false;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
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
            x += 0.01f;
            sr.material.SetFloat("_Delta", x);
            yield return new WaitForSeconds(0.01f);

            if (x > 1f) break;
        }
    }
}
