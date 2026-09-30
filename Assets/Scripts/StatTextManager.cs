using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatTextManager : MonoBehaviour
{
    [SerializeField]
    GameObject _mainText;
    List<GameObject> _copyTexts = new();

    List<TMP_Text> _textElements;

    [SerializeField] RectTransform _statsPanel;

    [SerializeField]
    StatDisplay[] TempstatDisplays;

    Camera mainCam;
    [SerializeField]
    bool _debugButton;

    float _sPannelHeight;

    RectTransform canvasRect;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sPannelHeight = _statsPanel.rect.height;
        canvasRect = GetComponent<RectTransform>();
        mainCam = GetComponentInParent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_debugButton)
        {
            _debugButton = false;
            SetText(TempstatDisplays);
        }
    }

    public void SetPostion(Vector2 pos)
    {
        Vector2 ScreenPos = mainCam.WorldToScreenPoint(pos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, ScreenPos, mainCam, out Vector2 truePos);
        truePos.y -= 250;
        _statsPanel.anchoredPosition = truePos;
    }

    public void SetText(StatDisplay[] statDisplays)
    {
        _statsPanel.gameObject.SetActive(true);
        ResetText();
        for (int i = 0; i < statDisplays.Length - 1; i++)
        {

            GameObject obj = Instantiate(_mainText, _mainText.transform.parent);
            RectTransform rTransform = obj.GetComponent<RectTransform>();
            rTransform.anchoredPosition = new(rTransform.anchoredPosition.x, rTransform.anchoredPosition.y - 20 * (i + 1));
            _textElements.Add(obj.GetComponent<TMP_Text>());
            _copyTexts.Add(obj);
            _statsPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _statsPanel.rect.height + 20);
        }

        for (int i = 0; i < statDisplays.Length; i++)
        {
            StatDisplay stat = statDisplays[i];
            TMP_Text text = _textElements[i];

            text.text = stat.Text;
            text.color = stat.TextColor;
        }
        hasReset = false;

    }
    bool hasReset = false;
    public void ClearText()
    {
        _statsPanel.gameObject.SetActive(false);
    }

    void ResetText()
    {
        hasReset = true;
        _statsPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _sPannelHeight);
        foreach (var item in _copyTexts)
        {
            Destroy(item);
        }
        _textElements = new()
        {
            _mainText.GetComponent<TMP_Text>()
        };
        _copyTexts = new();
    }
}
