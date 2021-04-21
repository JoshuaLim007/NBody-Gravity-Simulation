using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiContainer : MonoBehaviour
{
    public GameObject pointer;
    RectTransform rectTransform;
    float timer = 0;
    int _switch = 0;
    [SerializeField] Vector2 XMinMax;
    [SerializeField] Vector2 YMinMax;

    [SerializeField] Vector2 Position;
    [SerializeField] Vector2 Size;

    [SerializeField] float XOffset = -1000;
    float initialXPos;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialXPos = rectTransform.anchoredPosition.x;
    }
    bool CheckCursorInside()
    {
        float pw = Camera.main.pixelWidth;
        float ph = Camera.main.pixelHeight;
        Rect rect = new Rect(Position.x * pw, Position.y * ph, Size.x * pw, Size.y * ph);
        return rect.Contains(Input.mousePosition);
    }
    // Update is called once per frame
    void Update()
    {
        var mousePos = Input.mousePosition;
        //Debug.Log(mousePos);
        if (CheckCursorInside())
        {
            if (_switch == 0)
            {
                timer = 0;
                _switch++;
            }

            var pos = rectTransform.anchoredPosition;
            pos.x = Mathf.Lerp(pos.x, initialXPos, timer);
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
            pos.x = Mathf.Lerp(pos.x, initialXPos + XOffset, timer);
            rectTransform.anchoredPosition = pos;
            pointer.transform.localScale = Vector3.Lerp(pointer.transform.localScale, new Vector2(1, 1), timer * 0.7f);
            timer += Time.deltaTime;
        }
    }
    private void OnGUI()
    {

    }
}
