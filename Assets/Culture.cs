using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Culture : MonoBehaviour
{
    void Start()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
    }


}
