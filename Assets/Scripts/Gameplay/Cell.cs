using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Cell : GameViewElement
{
    [SerializeField] private Button button;

    [SerializeField] private Player player;

    [SerializeField] private LayerMask m_LayerMask;

    [SerializeField] private Vector2Int position;

    [SerializeField] private bool isStart = false;
    [SerializeField] private bool isEnd = false;
    [SerializeField] private bool hasCollectable = false;

    [SerializeField] private RectTransform rect;

    public Vector2Int Position { get => position; set => position = value; }
    public Image Image { get => image; set => image = value; }
    public bool IsStart { get => isStart; private set => isStart = value; }
    public bool IsEnd { get => isEnd; private set => isEnd = value; }

    public bool enableCheck = false;
    public bool HasCollectable { get; set; }
    public RectTransform Rect { get => rect; private set => rect = value; }
    public Player Player { get => player; private set => player = value; }

    public event Action OnCellEnabled;


    public void Init(bool isActve, bool isStartPoint, bool isEndEndPoint, bool hasCollectable = false)
    {
        SetState(isActve ? 1f : 0.1f);
        this.isElementActive = isActve;
        this.IsStart = isStartPoint;
        this.IsEnd = isEndEndPoint;
        this.HasCollectable = hasCollectable;
        button.onClick.AddListener(OnCellClick);
        GameViewController.Instance.OnColorSchemeChange += ChangeColor;
    }

    private void SetState(float state)
    {
        Color imgColor = Image.color;
        imgColor.a = state;
        Image.color = imgColor;
    }

    private void OnCellClick()
    {
        if (isElementActive || IsNotEmpty() || hasCollectable || isEnd)
        {
            return;
        }
        if (GameplayController.Instance.ActivableCellsCount > 0)
        {
            isElementActive = true;
            SetState(1);
            OnCellEnabled?.Invoke();
            Image.transform.DOScale(1f, 0.25f).From(1.1f).SetEase(Ease.OutBack);
        }
    }

    public bool IsNotEmpty()
    {
        return transform.childCount > 0;
    }

    void FixedUpdate()
    {
        if (enableCheck)
        {
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(0.1f, 0.1f, 1f), Quaternion.identity, m_LayerMask);
            if (hitColliders.Length > 0)
            {
                CellPoint point = hitColliders[0].GetComponent<CellPoint>();
                Position = point.Position;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (enableCheck)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(0.1f, 0.1f, 1f));
        }
    }
}
