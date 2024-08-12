using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class GameField : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private GameFieldCell _fieldCellPrefab;

    private RectTransform _rectTransform;
    private GameFieldCell[][] _grid;
    private List<GameFieldCell> _crosses;
    private List<GameFieldCell> _zeros;
    private int _lineLengthForWinning;

    public Vector2Int GridSize { get; private set; }

    public event UnityAction<GameFieldCell> GridCellClick;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Init(Vector2Int gridSize, int lineLengthForWinning)
    {
        if (gridSize.x > 1 && gridSize.y > 1)
        {
            GridSize = gridSize;
            CreateGrid();
        }

        if (lineLengthForWinning > 0)
        {
            _lineLengthForWinning = lineLengthForWinning;
        }
    }

    public List<GameFieldCell> GetGridAsList()
    {
        List<GameFieldCell> listForReturning = new List<GameFieldCell>();

        for (int x = 0; x < _grid.Length; x++)
        {
            for (int y = 0; y < _grid[x].Length; y++)
                listForReturning.Add(_grid[x][y]);
        }

        return listForReturning;
    }

    public bool TryGetCellByPosition(Vector2Int position, out GameFieldCell cell)
    {
        bool isSuccess;

        if (position.x >= 0 && position.y >= 0 && GridSize.x > position.x && GridSize.y > position.y)
        {
            isSuccess = true;
            cell = _grid[position.x][position.y];
        }
        else
        {
            isSuccess = false;
            cell = null;
        }

        return isSuccess;
    }

    public bool CheckIsDraw()
    {
        bool isDraw = true;

        foreach (var cell in GetGridAsList())
        {
            if (cell.Content == GameFieldCell.ContentTypes.Empty)
                isDraw = false;
        }

        return isDraw;
    }

    public GameFieldCell.ContentTypes CheckIsAnySideWinning(out List<GameFieldCell> winningCells)
    {
        winningCells = new List<GameFieldCell>();
        Vector2Int cellPosition;
        GameFieldCell cellInPosition;
        GameFieldCell.ContentTypes winningSide = GameFieldCell.ContentTypes.Empty;
        GameFieldCell.ContentTypes lastCellContent;
        int sameContentLineLength;
        bool isCheckingHorizontalLines;
        int xMax;
        int yMax;

        for (int i = 0; i <= 1; i++) // Horizontal & Vertical Lines
        {
            isCheckingHorizontalLines = Convert.ToBoolean(i);

            if (isCheckingHorizontalLines)
            {
                xMax = GridSize.x;
                yMax = GridSize.y;
            }
            else
            {
                xMax = GridSize.y;
                yMax = GridSize.x;
            }

            for (int y = 0; y < yMax; y++)
            {
                lastCellContent = GameFieldCell.ContentTypes.Empty;
                sameContentLineLength = 0;

                for (int x = 0; x < xMax; x++)
                {
                    if (isCheckingHorizontalLines)
                        cellInPosition = _grid[x][y];
                    else
                        cellInPosition = _grid[y][x];

                    if (lastCellContent == cellInPosition.Content)
                    {
                        sameContentLineLength++;
                        winningCells.Add(cellInPosition);

                        if (sameContentLineLength >= _lineLengthForWinning && cellInPosition.Content != GameFieldCell.ContentTypes.Empty)
                        {
                            winningSide = cellInPosition.Content;
                            break;
                        }
                    }
                    else
                    {
                        sameContentLineLength = 1;
                        lastCellContent = cellInPosition.Content;
                        winningCells = new List<GameFieldCell>() { cellInPosition };
                    }
                }

                if (winningSide != GameFieldCell.ContentTypes.Empty)
                    break;
            }

            if (winningSide != GameFieldCell.ContentTypes.Empty)
                break;
        }

        for (int x = 0; x < GridSize.x; x++) // Diagonals
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                if (x == 0 || y == 0) // Checking diagonals that look like '/'
                {
                    cellPosition = new Vector2Int(x, y);
                    lastCellContent = GameFieldCell.ContentTypes.Empty;
                    sameContentLineLength = 0;

                    while (TryGetCellByPosition(cellPosition, out cellInPosition) && winningSide == GameFieldCell.ContentTypes.Empty)
                    {
                        if (lastCellContent == cellInPosition.Content)
                        {
                            sameContentLineLength++;
                            winningCells.Add(cellInPosition);
                        }
                        else
                        {
                            sameContentLineLength = 1;
                            winningCells = new List<GameFieldCell>() { cellInPosition };
                        }

                        if (sameContentLineLength >= _lineLengthForWinning && cellInPosition.Content != GameFieldCell.ContentTypes.Empty)
                            winningSide = cellInPosition.Content;

                        cellPosition += Vector2Int.one;
                        lastCellContent = cellInPosition.Content;
                    }
                }

                if (x == 0 || y == GridSize.y - 1) // Checking diagonals that look like '\'
                {
                    cellPosition = new Vector2Int(x, y);
                    lastCellContent = GameFieldCell.ContentTypes.Empty;
                    sameContentLineLength = 0;

                    while (TryGetCellByPosition(cellPosition, out cellInPosition) && winningSide == GameFieldCell.ContentTypes.Empty)
                    {
                        if (lastCellContent == cellInPosition.Content)
                        {
                            sameContentLineLength++;
                            winningCells.Add(cellInPosition);
                        }
                        else
                        {
                            sameContentLineLength = 1;
                            winningCells = new List<GameFieldCell>() { cellInPosition };
                        }

                        if (sameContentLineLength >= _lineLengthForWinning && cellInPosition.Content != GameFieldCell.ContentTypes.Empty)
                            winningSide = cellInPosition.Content;

                        cellPosition += new Vector2Int(1, -1);
                        lastCellContent = cellInPosition.Content;
                    }
                }
            }
        }

        return winningSide;
    }

    public void SetNewIsActiveValue(bool isActive)
    {
        for (int i = 0; i < _grid.Length; i++)
        {
            for (int j = 0; j < _grid.Length; j++)
            {
                if (isActive == false || _grid[i][j].Content == GameFieldCell.ContentTypes.Empty)
                    _grid[i][j].SetNewButtonEnabledValue(isActive);
            }
        }
    }

    private void DestroyGrid()
    {
        if (_grid != null)
        {
            for (int x = 0; x < _grid.Length; x++)
            {
                for (int y = 0; y < _grid[x].Length; y++)
                {
                    _grid[x][y].Click -= OnGridCellClick;
                    _grid[x][y].ContentChanged -= OnGridCellContentChanged;
                    Destroy(_grid[x][y].gameObject);
                }
            }
        }

        _grid = null;
        _crosses = new List<GameFieldCell>();
        _zeros = new List<GameFieldCell>();
    }

    private void CreateGrid()
    {
        DestroyGrid();
        GameFieldCell newGridCell;
        _grid = new GameFieldCell[GridSize.x][];
        _gridLayoutGroup.cellSize = new Vector2(_rectTransform.rect.size.x / GridSize.x, _rectTransform.rect.size.y / GridSize.y);

        for (int x = 0; x < _grid.Length; x++)
        {
            _grid[x] = new GameFieldCell[GridSize.y];

            for (int y = 0; y < _grid[x].Length; y++)
            {
                newGridCell = Instantiate(_fieldCellPrefab, _gridLayoutGroup.transform);
                _grid[x][y] = newGridCell;

                newGridCell.Init(new Vector2Int(x, y));
                newGridCell.Click += OnGridCellClick;
                newGridCell.ContentChanged += OnGridCellContentChanged;
            }
        }
    }

    private void OnGridCellClick(GameFieldCell clickedCell)
    {
        clickedCell.SetNewButtonEnabledValue(false);
        GridCellClick?.Invoke(clickedCell);
    }

    private void OnGridCellContentChanged(GameFieldCell gridCell)
    {
        if (gridCell.Content == GameFieldCell.ContentTypes.Cross)
            _crosses.Add(gridCell);
        else if (gridCell.Content == GameFieldCell.ContentTypes.Zero)
            _zeros.Add(gridCell);
    }
}
