using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI textMesh = default;

    public Bubble Construct(string text) {
        textMesh.text = text;
        return this;
    }
}
