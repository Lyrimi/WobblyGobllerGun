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
    [SerializeField]
    bool _debugButton;

    float _sPannelHeight;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sPannelHeight = _statsPanel.rect.height;
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

    public void SetText(StatDisplay[] statDisplays)
    {
        _statsPanel.gameObject.SetActive(true);
        ResetText();
        for (int i = 0; i < statDisplays.Length - 1; i++)
        {

            GameObject obj = Instantiate(_mainText, _mainText.transform.parent);
            RectTransform rTransform = obj.GetComponent<RectTransform>();
            rTransform.anchoredPosition = new(rTransform.anchoredPosition.x, rTransform.anchoredPosition.y - rTransform.rect.height * (i + 1));
            _textElements.Add(obj.GetComponent<TMP_Text>());
            _copyTexts.Add(obj);
            _statsPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _statsPanel.rect.height + rTransform.rect.height);
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
        Debug.Log(_statsPanel.gameObject.name);
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
