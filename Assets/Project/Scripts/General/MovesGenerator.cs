using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovesGenerator : MonoBehaviour
{
    [SerializeField] private GameField _gameField;
    [SerializeField] private float _sameSideCellsLinesValue;
    [SerializeField] private float _oppositeSideCellsLinesValue;
    [SerializeField] private float _repeatingSameSideCellsLineModifier;
    [SerializeField] private float _repeatingOppositeSideCellsLineModifier;
    [SerializeField] private float _sameSideCellsApproachValueModifier;
    [SerializeField] private float _oppositeSideCellsApproachValueModifier;

    private static List<Vector2Int> DirectionsForCheck = new List<Vector2Int>()
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.left, Vector2Int.down, Vector2Int.one, new Vector2Int(-1, -1),
        new Vector2Int(-1, 1), new Vector2Int(1, -1)
    };

    public Vector2Int Generate(GameFieldCell.ContentTypes movingSide, GameFieldCell.ContentTypes oppositeSide)
    {
        List<Vector2Int> sameSideCellsPositions = new List<Vector2Int>();
        List<Vector2Int> oppositeSideCellsPositions = new List<Vector2Int>();
        List<Vector2Int> availableForMovingPositions = new List<Vector2Int>();
        List<Vector2Int> bestForMovingPositions = new List<Vector2Int>();
        List<PositionWorth> positionsWorths = new List<PositionWorth>();
        PositionWorth newPositionWorth;
        Vector2Int cellPosition;
        Vector2Int positionDelta;
        GameFieldCell cellInPosition;
        GameFieldCell.ContentTypes defaultContent = GameFieldCell.ContentTypes.Empty;
        GameFieldCell.ContentTypes lastCellContent;
        GameFieldCell.ContentTypes firstMetContent;
        int sameSideCellsLineLength;
        float maxWorth = float.MinValue;
        bool isCycleRunning;

        foreach (var cell in _gameField.GetGridAsList())
        {
            if (cell.Content == movingSide)
                sameSideCellsPositions.Add(cell.PositionInGrid);
            else if (cell.Content == oppositeSide)
                oppositeSideCellsPositions.Add(cell.PositionInGrid);
            else
                availableForMovingPositions.Add(cell.PositionInGrid);
        }

        foreach (var availableForMovingPosition in availableForMovingPositions)
        {
            newPositionWorth = new PositionWorth() { Position = availableForMovingPosition, Worth = 0 };

            foreach (var directionForCheck in DirectionsForCheck)
            {
                cellPosition = availableForMovingPosition + directionForCheck;
                lastCellContent = defaultContent;
                firstMetContent = defaultContent;
                sameSideCellsLineLength = 0;
                isCycleRunning = true;

                while (_gameField.TryGetCellByPosition(cellPosition, out cellInPosition) && isCycleRunning)
                {
                    positionDelta = new Vector2Int(Mathf.Abs(newPositionWorth.Position.x - cellPosition.x), Mathf.Abs(newPositionWorth.Position.y - cellPosition.y));

                    if (cellInPosition.Content != defaultContent && firstMetContent == defaultContent)
                        firstMetContent = cellInPosition.Content;

                    if (cellInPosition.Content == lastCellContent)
                    {
                        sameSideCellsLineLength++;
                    }
                    else
                    {
                        sameSideCellsLineLength = 1;
                        lastCellContent = cellInPosition.Content;
                    }

                    if (cellInPosition.Content == movingSide)
                        newPositionWorth.Worth += _sameSideCellsLinesValue + Mathf.Pow(_repeatingSameSideCellsLineModifier, sameSideCellsLineLength);
                    else if (cellInPosition.Content == oppositeSide)
                        newPositionWorth.Worth += _oppositeSideCellsLinesValue + Mathf.Pow(_repeatingOppositeSideCellsLineModifier, sameSideCellsLineLength);

                    cellPosition += directionForCheck;
                    isCycleRunning = firstMetContent == cellInPosition.Content;
                }
            }

            foreach (var position in sameSideCellsPositions.Union(oppositeSideCellsPositions))
            {
                positionDelta = new Vector2Int(Mathf.Abs(availableForMovingPosition.x - position.x), Mathf.Abs(availableForMovingPosition.y - position.y));

                if (sameSideCellsPositions.Contains(position))
                    newPositionWorth.Worth -= Mathf.Max(positionDelta.x, positionDelta.y) * _sameSideCellsApproachValueModifier;
                else
                    newPositionWorth.Worth -= Mathf.Max(positionDelta.x, positionDelta.y) * _oppositeSideCellsApproachValueModifier;
            }

            positionsWorths.Add(newPositionWorth);
        }

        foreach (var positionWorth in positionsWorths)
        {
            if (positionWorth.Worth > maxWorth)
            {
                bestForMovingPositions = new List<Vector2Int>() { positionWorth.Position };
                maxWorth = positionWorth.Worth;
            }
            else if (positionWorth.Worth == maxWorth)
            {
                bestForMovingPositions.Add(positionWorth.Position);
            }
        }

        return bestForMovingPositions[Random.Range(0, bestForMovingPositions.Count)];
    }

    private struct PositionWorth
    {
        public Vector2Int Position;
        public float Worth;
    }
}
