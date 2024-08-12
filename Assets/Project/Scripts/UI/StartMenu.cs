using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [Header("Interactable Elements")]
    [SerializeField] private Slider _fieldHeightSlider;
    [SerializeField] private Slider _fieldWidthSlider;
    [SerializeField] private Slider _lineLengthForWinningSlider;
    [SerializeField] private Toggle _isCrossesControlledByAI;
    [SerializeField] private Toggle _isZerosControlledByAI;
    [SerializeField] private Button _startGameButton;

    [Header("Other")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameManager _gameManager;

    private static string FadeAnimationTriggerName = "Fade";
    private static string UnfadeAnimationTriggerName = "Unfade";

    private void Start()
    {
        _fieldHeightSlider.onValueChanged.AddListener(OnFieldSizeSliderValueChanged);
        _fieldWidthSlider.onValueChanged.AddListener(OnFieldSizeSliderValueChanged);
        _startGameButton.onClick.AddListener(OnStartGameButtonClick);
        _gameManager.WinningSideDefined += OnWinningSideDefined;
        _lineLengthForWinningSlider.maxValue = Mathf.Min(_fieldHeightSlider.value, _fieldWidthSlider.value);
    }

    private void OnFieldSizeSliderValueChanged(float newValue) => _lineLengthForWinningSlider.maxValue = Mathf.Min(_fieldHeightSlider.value, _fieldWidthSlider.value);

    private void OnWinningSideDefined(GameFieldCell.ContentTypes winningSide) => _animator.SetTrigger(UnfadeAnimationTriggerName);
    
    private void OnStartGameButtonClick()
    {
        Vector2Int fieldSize = new Vector2Int((int)_fieldHeightSlider.value, (int)_fieldWidthSlider.value);
        int lineLengthForWinning = (int)_lineLengthForWinningSlider.value;
        _gameManager.StartGame(fieldSize, lineLengthForWinning, !_isCrossesControlledByAI.isOn, !_isZerosControlledByAI.isOn);
        _animator.SetTrigger(FadeAnimationTriggerName);
    }
}
