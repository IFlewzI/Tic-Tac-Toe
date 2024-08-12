using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
public class GameFieldCell : MonoBehaviour
{
    [SerializeField] private Sprite _ContentTypeEmptyView;
    [SerializeField] private Sprite _ContentTypeCrossView;
    [SerializeField] private Sprite _ContentTypeZeroView;

    private Image _image;
    private Button _button;

    public RectTransform RectTransform { get; private set; }
    public Vector2Int PositionInGrid { get; private set; }
    public ContentTypes Content { get; private set; }

    public event UnityAction<GameFieldCell> Click;
    public event UnityAction<GameFieldCell> ContentChanged;

    public enum ContentTypes
    {
        Empty,
        Cross,
        Zero,
    }

    private void Start()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick);
        RectTransform = GetComponent<RectTransform>();
    }

    public void Init(Vector2Int positionInGrid)
    {
        PositionInGrid = positionInGrid;
        Content = ContentTypes.Empty;
    }

    public void SetNewContent(ContentTypes newContent)
    {
        Content = newContent;

        switch (Content)
        {
            case ContentTypes.Empty:
                _image.sprite = _ContentTypeEmptyView;
                break;
            case ContentTypes.Cross:
                _image.sprite = _ContentTypeCrossView;
                break;
            case ContentTypes.Zero:
                _image.sprite = _ContentTypeZeroView;
                break;
        }

        ContentChanged?.Invoke(this);
    }

    public void SetNewButtonEnabledValue(bool newValue) => _button.enabled = newValue;

    private void OnButtonClick() => Click?.Invoke(this);
}
