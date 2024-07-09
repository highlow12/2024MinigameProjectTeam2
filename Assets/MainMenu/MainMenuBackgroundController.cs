using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackgroundController : MonoBehaviour
{
    Camera main;
    float moveSpeed = -3f;

    List<GameObject> bamboos = new List<GameObject>();
    List<GameObject> others = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        foreach (Transform tf in GetComponentsInChildren<Transform>())
        {
            if (tf.gameObject.name.StartsWith("bamboo"))
            {
                bamboos.Add(tf.gameObject);
            }
            else if (tf.gameObject.name.StartsWith("grass"))
            {
                others.Add(tf.gameObject);
            }
            else if (tf.gameObject.name.StartsWith("dirt"))
            {
                others.Add(tf.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        // transform.position = new Vector3(pos.x + (moveSpeed * Time.deltaTime), pos.y, pos.z);
        foreach (GameObject other in others)
        {
            Vector3 pos = other.transform.position;
            Vector3 _pos = main.WorldToViewportPoint(pos);

            float dX = moveSpeed * Time.deltaTime;

            if (_pos.x < 0)
            {
                other.transform.position = new Vector3(11 + dX, pos.y, pos.z);
            }
            else
            {
                other.transform.position = new Vector3(pos.x + dX, pos.y, pos.z);
            }
        }

        foreach (GameObject bamboo in bamboos)
        {
            // 53.4
            Vector3 pos = bamboo.transform.position;
            Vector3 _pos = main.WorldToViewportPoint(pos);

            float dX = moveSpeed * Time.deltaTime * (pos.z / 9);
            
            if (_pos.x < -2.5)
            {
                bamboo.transform.position = new Vector3(53.4f + dX, pos.y, pos.z);
            }
            else
            {
                bamboo.transform.position = new Vector3(pos.x + dX, pos.y, pos.z);
            }         
        }
    }
}
