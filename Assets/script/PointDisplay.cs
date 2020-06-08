using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PointDisplay : MonoBehaviour
{
    [SerializeField] IntVariable pointCollection;
    Text pointText;
    // Start is called before the first frame update
    void Awake()
    {
        pointText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pointCollection.changed)
        {
            pointCollection.changed = false;
            pointText.text = "Collectible: " + pointCollection.point;
        }
    }
}
