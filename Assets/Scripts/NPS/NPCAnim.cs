using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnim : MonoBehaviour
{

    public Animator animator;
    public string parameter;
    public int value;

    public Vector2Int range;
    public bool rand;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        if(rand)
        {
            value = Random.Range(range.x, range.y);
        }
        yield return new WaitForSeconds(Random.Range(0f, 3f));
        animator.SetInteger(parameter, value);

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + Random.Range(-20, 20), 0);
    }

   
}
