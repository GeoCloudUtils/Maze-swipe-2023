using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Cell : MonoBehaviour
{
    public Image Image;
    public bool Enabled = true;
    private void Update()
    {
        Image.enabled = Enabled;
    }
}
