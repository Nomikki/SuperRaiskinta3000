using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class hitpointTextController : MonoBehaviour
{
    public TextMeshProUGUI hitText;
    float lifetime = 1.0f;
    float opacity = 1.0f;
    Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(-1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        lifetime -= delta;
        opacity -= delta;
        if (opacity < 0) opacity = 0;

        Color color = Color.white;
        color.a = opacity;
        hitText.color = color;

        
        
        if (lifetime < 0)    
        {
            Object.Destroy(gameObject);
        }
    }

    public void setText(string text, Vector3 direction)
    {
        hitText.text = text;
        targetPosition = direction;
        transform.LookAt(targetPosition);
    }
}
