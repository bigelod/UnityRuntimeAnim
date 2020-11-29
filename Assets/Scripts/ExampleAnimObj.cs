using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleAnimObj : MonoBehaviour
{
    [SerializeField]
    private LoadAnim animCtrl;
    [SerializeField]
    private int targetSlot = 1; //Which slot is this assigned to?

    private bool registered = false;

    // Update is called once per frame
    void Update()
    {
        if (animCtrl != null)
        {
            if (!registered)
            {
                animCtrl.myAnim.AssignObj(gameObject, targetSlot);
                registered = true;
            }
            else if (transform.parent == null) 
            {
                registered = false;
            }
        }
    }
}
