using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapBox : MonoBehaviour
{
    bool m_Started;
    public LayerMask m_LayerMask;

    void Start()
    {
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        m_Started = true;
    }



}
