using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static GameFieldCell;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameField _gameField;
    [SerializeField] private MovesGenerator _movesGenerator;
    [SerializeField] private LineRenderer _winningCellsLineRenderer;
    [SerializeField] private Button _stopGameButton;
    [SerializeField] private float _pauseBeforeAIMove;
    [SerializeField] private float _pauseBetweenWinningLinePositionsRender;
    [SerializeField] private float _pauseAfterEndOfGame;

    private Coroutine _executeGameModeInJob;
    private GameFieldCell _lastClickedCell;

    public event UnityAction<ContentTypes> WinningSideDefined;

    private void Start()
    {
        _gameField.GridCellClick += OnGameFieldCellClick;
        _stopGameButton.onClick.AddListener(OnStopGameButtonClick);
    }

    public void StartGame(Vector2Int fieldSize, int lineLengthForWinning, bool isCrossesControlledByPlayer, bool isZerosControlledByPlayer)
    {
        if (_executeGameModeInJob != null)
            StopCoroutine(_executeGameModeInJob);

        _winningCellsLineRenderer.positionCount = 0;
        _gameField.Init(fieldSize, lineLengthForWinning);
        _executeGameModeInJob = StartCoroutine(ExecuteGameMode(isCrossesControlledByPlayer, isZerosControlledByPlayer, lineLengthForWinning));
    }

    private IEnumerator ExecuteGameMode(bool isCrossesControlledByPlayer, bool isZerosControlledByPlayer, int lineLengthForWinning)
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        WaitForSeconds waitForPauseBeforeAIMove = new WaitForSeconds(_pauseBeforeAIMove);
        WaitForSeconds waitForPauseBetweenLinePositionsRender = new WaitForSeconds(_pauseBetweenWinningLinePositionsRender);
        WaitForSeconds waitForPauseAfterEndOfGame = new WaitForSeconds(_pauseAfterEndOfGame);
        List<GameFieldCell> winningCells;
        ContentTypes winningSide = ContentTypes.Empty;
        Vector3 lineRendererPositionsOffset = new Vector3(0, 0, -1);
        bool isRunning = true;
        bool isCrossesTurn = true;
        yield return waitForEndOfFrame;

        while (isRunning)
        {
            if ((isCrossesTurn && isCrossesControlledByPlayer) || ((isCrossesTurn == false) && isZerosControlledByPlayer))
            {
                while (_lastClickedCell == null)
                    yield return waitForEndOfFrame;
            }
            else
            {
                _gameField.SetNewIsActiveValue(false);
                yield return waitForPauseBeforeAIMove;

                if (isCrossesTurn)
                    _gameField.TryGetCellByPosition(_movesGenerator.Generate(ContentTypes.Cross, ContentTypes.Zero), out _lastClickedCell);
                else
                    _gameField.TryGetCellByPosition(_movesGenerator.Generate(ContentTypes.Zero, ContentTypes.Cross), out _lastClickedCell);

                _gameField.SetNewIsActiveValue(true);
            }

            if (isCrossesTurn)
                _lastClickedCell.SetNewContent(ContentTypes.Cross);
            else
                _lastClickedCell.SetNewContent(ContentTypes.Zero);

            winningSide = _gameField.CheckIsAnySideWinning(out winningCells);

            if (winningSide != ContentTypes.Empty)
            {
                isRunning = false;
                _gameField.SetNewIsActiveValue(false);
                _winningCellsLineRenderer.positionCount = winningCells.Count;

                for (int i = 0; i < winningCells.Count; i++)
                {
                    for (int j = i; j < winningCells.Count; j++)
                        _winningCellsLineRenderer.SetPosition(j, winningCells[i].RectTransform.localPosition + lineRendererPositionsOffset);
                    
                    yield return waitForPauseBetweenLinePositionsRender;
                }
            }
            else if (_gameField.CheckIsDraw())
                isRunning = false;

            isCrossesTurn = !isCrossesTurn;
            _lastClickedCell = null;
        }

        yield return waitForPauseAfterEndOfGame;
        _winningCellsLineRenderer.positionCount = 0;
        WinningSideDefined?.Invoke(winningSide);
    }

    private void OnGameFieldCellClick(GameFieldCell clickedCell) => _lastClickedCell = clickedCell;

    private void OnStopGameButtonClick()
    {
        if (_executeGameModeInJob != null)
            StopCoroutine(_executeGameModeInJob);

        WinningSideDefined?.Invoke(ContentTypes.Empty);
    }
}
