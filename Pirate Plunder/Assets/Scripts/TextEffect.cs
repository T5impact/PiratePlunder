using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEffect : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 0.25f;
    [SerializeField] private float fadeTime = 2f;
    [SerializeField] private TMP_Text textElement;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Fade());
    }

    public IEnumerator Fade()
    {
        float timer = fadeTime;
        Color c;
        while (timer > 0)
        {
            c = textElement.color;
            c.a = timer / fadeTime;
            textElement.color = c;
            timer -= Time.deltaTime;

            transform.position += Vector3.up * riseSpeed * Time.deltaTime;

            yield return null;
        }

        c = textElement.color;
        c.a = 0;
        textElement.color = c;
    }

}
