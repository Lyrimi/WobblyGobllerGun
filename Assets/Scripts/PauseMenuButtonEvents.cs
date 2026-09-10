using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseMenuButtonEvents : MonoBehaviour
{
    [SerializeField] private GameObject _menuObj;
    [SerializeField] private Volume _globalVolume;
    private DepthOfField _depthOfField;

    [Space]

    [SerializeField] private GameObject _mainMenuObj;
    [SerializeField] private GameObject _settingMenuObj;

    [Header("Animation")]

    [SerializeField] private float _transtionTime;
    [SerializeField] private float _enableTime;
    [SerializeField] private float _startFocalLength;
    [SerializeField] private float _endFocalLength;
    // Update is called once per frame

    void Update()
    {

    }

    void Awake()
    {
        _globalVolume.profile.TryGet<DepthOfField>(out _depthOfField);
      if (_depthOfField == null)
      {
            Debug.LogError("Missing Depth Of Field Object");
      }
    }

    public void EnableMenu()
    {
        StartCoroutine(StartMenuAnim());
    }

    IEnumerator StartMenuAnim() {
        float curTime = 0;

        if (_depthOfField == null)
        {
            Debug.LogError("Missing Depth Of Field Object");
            yield break;
        }

        while (curTime < _transtionTime)
        {
            curTime += Time.deltaTime;

            if (curTime > _enableTime && _menuObj.activeSelf)
            {
                _menuObj.SetActive(true);
            }

            _depthOfField.focusDistance = new(Mathf.Lerp(_startFocalLength, _endFocalLength, curTime / _transtionTime), 0.1f);



            yield return null;

        }

        _depthOfField.focusDistance = new(_endFocalLength, 0.1f);

    }
    
    

    public void SettingsButtonPresed()
    {
        _mainMenuObj.SetActive(false);
        _settingMenuObj.SetActive(true);
    }
    public void KeyBindsButtonPresed()
    {

    }
    public void ExitSettingsButtonPresed()
    {
        _mainMenuObj.SetActive(true); 
        
        _settingMenuObj.SetActive(false);
    }
}
