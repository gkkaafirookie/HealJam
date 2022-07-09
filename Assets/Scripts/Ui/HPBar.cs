using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HPBar : MonoBehaviour
{
    public Image fill;

    public Color Color;

    public void Fill(bool value) => fill.gameObject.SetActive(value);

    private void Start()
    {
        fill.color = Color;
    }
}
