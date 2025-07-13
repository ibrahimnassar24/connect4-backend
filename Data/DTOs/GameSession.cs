using System.Collections.Generic;
using System.Linq;

namespace connect4_backend.Data.DTOs
{
    public class GameSession
    {
        public int GameId { get; set; }
        public string FirstPlayerEmail { get; set; } = "";
        public string FirstPlayerId { get; set; } = "";
        public string firstPlayerFormat { get; set; } = "firstPlayer";
        public string SecondPlayerEmail { get; set; } = "";
        public string SecondPlayerId { get; set; } = "";
        public string secondPlayerFormat { get; set; } = "secondPlayer";
        public string Turn { get; set; } = "";
        public string Winner { get; private set; } = "";

        private List<List<Movement?>> _grid;
        private (int P1, int P2) _counter;
        private int _columns;
        private int _rows;

        public GameSession(int columns = 7, int rows = 6)
        {
            _columns = columns;
            _rows = rows;
            _grid = BuildGrid(columns, rows);
            _counter = (0, 0);
        }

        private List<List<Movement?>> BuildGrid(int columns, int rows)
        {
            var grid = new List<List<Movement?>>();
            for (int c = 0; c < columns; c++)
            {
                var column = new List<Movement?>();
                for (int r = 0; r < rows; r++)
                    column.Add(null);
                grid.Add(column);
            }
            return grid;
        }

        public Movement? AddMovement(Movement movement)
        {
            int c = movement.column;
            int r = _grid[c].FindLastIndex(x => x == null);


            if (r >= _rows || r < 0)
                return null;

            movement.format = (movement.player == FirstPlayerEmail)
            ? firstPlayerFormat
            : secondPlayerFormat;
            _grid[c][r] = movement;
            return movement;
        }

        public bool ValidateMovement(Movement movement)
        {
            if (Turn == movement.player)
                return true;
            return false;
        }

        public void ChangeTurn()
        {
            Turn = (Turn == FirstPlayerEmail)
    ? SecondPlayerEmail
    : FirstPlayerEmail;
        }
        private bool AddToCounter(Movement movement)
        {
            if (movement.player == FirstPlayerEmail)
            {
                _counter.P1++;
                _counter.P2 = 0;
            }
            else
            {
                _counter.P2++;
                _counter.P1 = 0;
            }
            return CheckCounter();
        }

        private bool CheckCounter()
        {
            if (_counter.P1 == 4)
            {
                Winner = FirstPlayerEmail;
                return true;
            }
            if (_counter.P2 == 4)
            {
                Winner = SecondPlayerEmail;
                return true;
            }
            return false;
        }

        private void ResetCounter() => _counter = (0, 0);

        public bool ScanForWin()
        {
            return ScanHorizontally() ||
            ScanVertically() ||
            ScanLeftToRight() ||
            ScanRightToLeft();
        }

        private bool ScanVertically()
        {
            for (int i = 0; i < _columns; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    var temp = _grid[i][j];
                    if (temp == null) continue;
                    if (AddToCounter(temp)) return true;
                }
                ResetCounter();
            }
            return false;
        }

        private bool ScanHorizontally()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    var temp = _grid[j][i];
                    if (temp == null) continue;
                    if (AddToCounter(temp)) return true;
                }
                ResetCounter();
            }
            return false;
        }

        private bool ScanRightToLeft()
        {
            for (int i = _rows - 1; i > 2; i--)
            {
                int c = 0;
                int r = i;
                while (c < _columns && r >= 0)
                {
                    var temp = _grid[c][r];
                    r--;
                    c++;
                    if (temp == null) continue;
                    if (AddToCounter(temp)) return true;
                }
                ResetCounter();
            }

            for (int i = 1; i < _columns - 3; i++)
            {
                int c = i;
                int r = _rows - 1;
                while (c < _columns && r >= 0)
                {
                    var temp = _grid[c][r];
                    r--;
                    c++;
                    if (temp == null) continue;
                    if (AddToCounter(temp)) return true;
                }
                ResetCounter();
            }

            return false;
        }

        private bool ScanLeftToRight()
        {
            for (int j = 0; j < _rows - 3; j++)
            {
                int c = 0;
                int r = j;
                while (c < _columns && r < _rows)
                {
                    var temp = _grid[c][r];
                    c++;
                    r++;
                    if (temp == null) continue;
                    if (AddToCounter(temp)) return true;
                }
                ResetCounter();
            }

            for (int i = 1; i < _columns - 3; i++)
            {
                int c = i;
                int r = 0;
                while (c < _columns && r < _rows)
                {
                    var temp = _grid[c][r];
                    c++;
                    r++;
                    if (temp == null) continue;
                    if (AddToCounter(temp)) return true;
                }
                ResetCounter();
            }

            return false;
        }


        public List<List<Movement?>> GetGrid() => _grid;
    }
}
