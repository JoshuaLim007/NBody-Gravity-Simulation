using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public GameObject pointer;
    RectTransform rectTransform;
    float timer = 0;
    int _switch = 0;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        var mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //Debug.Log(mousePos);
        if (mousePos.x < 0.4f && mousePos.y > 0.5f)
        {
            if (_switch == 0)
            {
                timer = 0;
                _switch++;
            }

            var pos = rectTransform.anchoredPosition;
            pos.x = Mathf.Lerp(pos.x, -660, timer);
            rectTransform.anchoredPosition = pos;
            pointer.transform.localScale = Vector3.Lerp(pointer.transform.localScale, new Vector2(-1, 1), timer * 0.7f);
            timer += Time.deltaTime;
        }
        else
        {
            if (_switch == 1) {
                timer = 0;
                _switch = 0;
            }
            var pos = rectTransform.anchoredPosition;
            pos.x = Mathf.Lerp(pos.x, -1334, timer);
            rectTransform.anchoredPosition = pos;
            pointer.transform.localScale = Vector3.Lerp(pointer.transform.localScale, new Vector2(1, 1), timer * 0.7f);
            timer += Time.deltaTime;
        }

    }
}
