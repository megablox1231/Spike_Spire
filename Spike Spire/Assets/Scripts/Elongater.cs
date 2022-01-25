using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Elongates the spike to become longer and shorter on a defined basis, positions included !!!(Deprecated Script in favor of Elongate)!!!
/// </summary>
/// <remarks>
/// It is assumed that the <code>shortLength</code> is given as the predefined length.
/// Same goes for <code>shortPos</code>.
public class Elongater : MonoBehaviour
{
    public enum scaleAxis { xAxis, yAxis};

    public bool first; //if this spike is the first to go up in a succession
    public float timer = 2;
    public float longLength;
    public float speed = .3f;
    public scaleAxis axis;
    public float longPosX, longPosY;

    bool isElongating;
    bool waiting;
    float shortLength;
    Vector3 longScaleX, longScaleY, longPos, shortScale, shortPos;

    void Start()
    {

        isElongating = true;
        waiting = false;

        if (axis == scaleAxis.yAxis) {
            shortLength = transform.localScale.y;
        }
        else {
            shortLength = transform.localScale.x;
        }

        longScaleY = new Vector3(transform.localScale.x, longLength, transform.localScale.z);
        longScaleX = new Vector3(longLength, transform.localScale.y, transform.localScale.z);
        longPos = new Vector3(longPosX, longPosY, transform.position.z);
        shortScale = transform.localScale;
        shortPos = transform.position;

    }

    void Update()
    {
        updateAction();
    }

    //Corountine that will wait.
    private IEnumerator Wait() {
        waiting = true;
        yield return new WaitForSeconds(timer);
        waiting = false;
    }

    private void updateAction() {
        if (isElongating && !waiting) {
            if (axis == scaleAxis.yAxis) {
                transform.localScale = Vector3.MoveTowards(transform.localScale, longScaleY, speed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, longPos, speed * Time.deltaTime);
                if (transform.localScale.y == longLength && transform.position == longPos) {
                    isElongating = false;
                    StartCoroutine(Wait());
                }
            }
            else {
                transform.localScale = Vector3.MoveTowards(transform.localScale, longScaleX, speed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, longPos, speed * Time.deltaTime);
                if (transform.localScale.x == longLength && transform.position == longPos) {
                    isElongating = false;
                    StartCoroutine(Wait());
                }
            }
        }
        else if (!waiting) {
            if (axis == scaleAxis.yAxis) {
                transform.localScale = Vector3.MoveTowards(transform.localScale, shortScale, speed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, shortPos, speed * Time.deltaTime);
                if (transform.localScale.y == shortLength && transform.position == shortPos) {
                    isElongating = true;
                    StartCoroutine(Wait());
                }
            }
            else {
                transform.localScale = Vector3.MoveTowards(transform.localScale, shortScale, speed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, shortPos, speed * Time.deltaTime);
                if (transform.localScale.x == shortLength && transform.position == shortPos) {
                    isElongating = true;
                    StartCoroutine(Wait());
                }
            }
        }
    }

}
