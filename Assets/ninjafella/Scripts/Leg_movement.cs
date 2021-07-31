using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_movement : MonoBehaviour
{
    public Animator m_animator;
    private float number;
    private Vector3 velocity;
    private Vector3 previous;
    private float divNum = 0.57272f;

    float getRidOfTrailingValues(float number)
    {
        int tempInt = Mathf.RoundToInt(number * 10);
        float returnVal = (float)tempInt;
        return returnVal / 10;
    }

    void Start()
    {
        previous = transform.position;
        Debug.Log("divNum:" + divNum);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = (transform.position - previous) / Time.deltaTime;
        previous = transform.position;

        float x = getRidOfTrailingValues(Mathf.Abs(velocity.x));
        float y = getRidOfTrailingValues(Mathf.Abs(velocity.y));
        float z = getRidOfTrailingValues(Mathf.Abs(velocity.z));
        if (x == 0 && y == 0 && z == 0)
        {
            number = 0f;
            m_animator.SetFloat("idk", number);
        }
        else
        {
            if (x < y && x < z)
            {
                Debug.Log("x" + x);
                number = ((x / divNum) / 11);
                number = getRidOfTrailingValues(number);
                m_animator.SetFloat("idk", number);
            }
            else if (y < x && y < z)
            {
                Debug.Log("y" + y);
                number = ((y / divNum) / 11);
                number = getRidOfTrailingValues(number);
                m_animator.SetFloat("idk", number);
            }
            else if (z < x && z < y)
            {
                Debug.Log("z" + z);
                number = ((z / divNum) / 11);
                number = getRidOfTrailingValues(number);
                m_animator.SetFloat("idk", number);
            }
        }
        Debug.Log("number:" + number);
    }
}
