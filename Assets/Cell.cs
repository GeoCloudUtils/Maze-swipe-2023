using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Image Image;
    public bool Enabled = true;
    public TMP_Text debugText;
    public LayerMask m_LayerMask;
    [SerializeField] private Vector2Int position;

    public Vector2Int Position { get => position; private set => position = value; }

    public bool m_Started = false;

    private void Awake()
    {
        debugText = GetComponentInChildren<TMP_Text>();
    }
    private void Update()
    {
        Image.enabled = Enabled;
    }

    void FixedUpdate()
    {
        if (m_Started)
        {
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);
            if (hitColliders.Length > 0)
            {
                Point point = hitColliders[0].GetComponent<Point>();
                Position = point.Position;
                debugText.SetText(position.x + "," + position.y);
            }
            debugText.transform.rotation = Quaternion.Euler(0.0f, 0.0f, gameObject.transform.parent.rotation.z * -1.0f);
        }
    }
}
