using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    [SerializeField]
    private LoadAnim animCtrl;
    [SerializeField]
    private Light myLight;

    private void Start()
    {
        if (myLight == null) myLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animCtrl != null && myLight != null)
        {
            if (animCtrl.myAnim.getVariable("LightsOn") > 0)
            {
                myLight.enabled = true;
            }
            else
            {
                myLight.enabled = false;
            }
        }
    }
}
