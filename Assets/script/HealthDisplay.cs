using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthDisplay : MonoBehaviour
{
    [SerializeField] FloatVariable totalHealth;
    [SerializeField] FloatVariable healthVariable;
    float XperHealth;
    RectTransform currentHealth;
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        currentHealth = transform.GetChild(0).GetComponent<RectTransform>();

        float sizeOfBar = currentHealth.sizeDelta.x;
        XperHealth = sizeOfBar / totalHealth.point;
        Debug.Log(XperHealth + " " + sizeOfBar + " " + totalHealth.point);
        currentHealth.anchoredPosition = newHealthBarPos();

    }
    Vector2 newHealthBarPos()
    {
        return new Vector2(XperHealth * -(totalHealth.point - healthVariable.point), currentHealth.anchoredPosition.y);
    }
    void Update()
    {
        if (healthVariable.changed)
        {
            healthVariable.changed = false;
            currentHealth.anchoredPosition = newHealthBarPos();
        }
    }
}