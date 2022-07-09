using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        KeyCode keyCode = GetInput.GetKeyCode();
        if (keyCode != KeyCode.None)
        {
            Debug.Log(keyCode);
        }
    }


}


public static class GetInput
{
    private static System.Array keyCodes = System.Enum.GetValues(typeof(KeyCode));

     public static KeyCode GetKeyCode()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in keyCodes)
            {
                if (Input.GetKey(keyCode))
                {
                    return keyCode;
                }
            }
        }
        return KeyCode.None;
    }
}