using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

public class NonRectangularButtonHitDetector : MonoBehaviour
{
    Image theButton;

    // Use this for initialization
    void Start()
    {
        theButton = GetComponent<Image>();
        theButton.alphaHitTestMinimumThreshold = 0.9f;
    }
}