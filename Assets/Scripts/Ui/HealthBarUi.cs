using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarUi : MonoBehaviour
{

    public List<HPBar> fills;

    public GameObject Content;

    public TMPro.TMP_Text nameText;

    public int totalHealth = 8;

    private void Start()
    {
       
    }

    public void ShowBar(bool value)
    {
        Content.SetActive(value);
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void UpdateUI(int currentHP)
    {
        for(int i = 0;i <totalHealth;i++)
        {
            if(i >= currentHP)
            {
                fills[i].Fill(false);
                continue;
            }
            
            fills[i].Fill(true);
                
           
        }
    }
}


public class HPBars
{
    public Image fill;
    
}