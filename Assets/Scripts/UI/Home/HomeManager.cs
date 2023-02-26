using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    #region Inspector Fields
    [Header("Home Screen Dependencies")]
    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _exitButton;
    
    [Header("Button Texts")]
    [SerializeField] private TextMeshProUGUI _playButtonText;
    [SerializeField] private TextMeshProUGUI _exitButtonText;

    [Header("Titles")] 
    [SerializeField] private TextMeshProUGUI _gameTitle;

    #endregion
    
    #region Unity Events

    private void Awake()
    {
        _playButtonText.text = StringKeywords.Home.HomePlayButtonText;
        _exitButtonText.text = StringKeywords.Home.HomeExitButtonText;
    }

    private void OnEnable()
    {
        _playButton.onClick.AddListener(OnPlayButtonClicked);
        _exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(OnPlayButtonClicked);
        _exitButton.onClick.RemoveListener(OnExitButtonClicked);
    }

    #endregion
    
    #region Private Functions

    /// <summary>
    /// Play button OnClick event handler
    /// </summary>
    private void OnPlayButtonClicked()
    {
        LoadGameScene();
    }
    
    /// <summary>
    /// Exit button OnClick event handler
    /// </summary>
    private void OnExitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    
    /// <summary>
    /// Load game scene asynchronously
    /// </summary>
    private async void LoadGameScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(StringKeywords.Scenes.Game);
        await UniTask.WaitUntil(() => asyncOperation.isDone);
    }

    #endregion
}
