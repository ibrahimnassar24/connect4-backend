using System.Collections.Generic;
using System.Linq;
using System.Timers;
using connect4_backend.Services;

namespace connect4_backend.Data.DTOs;

public class GameSession
{

    private readonly IGameHub _gameHub;

    private List<List<Movement?>> _grid;
    private readonly System.Timers.Timer _timer;
    private readonly Dictionary<string, int> _timerCounter;
    private (int P1, int P2) _counter;
    private int _columns;
    private int _rows;
    public event EventHandler GameEnded;

    public GameSession(
IGameHub gameHub,
        int columns = 7, int rows = 6)
    {
        _gameHub = gameHub;
        _columns = columns;
        _rows = rows;
        _grid = BuildGrid(columns, rows);
        _counter = (0, 0);
        _timerCounter = new Dictionary<string, int>();

        // initialize the timer object and start it.
        _timer = new System.Timers.Timer(45 * 1000);
        _timer.AutoReset = false;
        _timer.Elapsed += onElapsed;
        _timer.Start();
    }


    public required string MatchId { get; init; }
    public required string FirstPlayerEmail { get; init; }
    public string FirstPlayerConnectionId { get; set; } = "";
    public string firstPlayerFormat { get; set; } = "firstPlayer";
    public required string SecondPlayerEmail { get; init; }
    public string SecondPlayerConnectionId { get; set; } = "";
    public string secondPlayerFormat { get; set; } = "secondPlayer";
    public required string Turn { get; set; }
    public string Winner { get; private set; } = "";

    private async void onElapsed(object? sender, ElapsedEventArgs e)
    {
        // increase the timer counter
        AddToTimerCounter();

        // check if the player has messed 3 times in a row.
        if (CheckTimerCounter())
        {
            Winner = (Turn == FirstPlayerEmail)
            ? SecondPlayerEmail
            : FirstPlayerEmail;

            await GameOver();
            return;
        }

        // change turn.
        // notify players
        // start the timer again
        await SwitchTurn();
    }

    protected void OnGameEnded()
    {
        GameEnded?.Invoke(this, EventArgs.Empty);
    }

    private void AddToTimerCounter()
    {
        if (_timerCounter.ContainsKey(Turn))
            _timerCounter[Turn] += 1;
        else
            _timerCounter[Turn] = 1;
    }

    private void ResetTimerCounter()
    {
        if (_timerCounter.ContainsKey(Turn))
            _timerCounter[Turn] = 0;
}

    private bool CheckTimerCounter()
    {
        if (_timerCounter.ContainsKey(Turn) && _timerCounter[Turn] == 2)
            return true;
        return false;
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

    private Movement? AddMovement(Movement movement)
    {
        int c = movement.column;
        int r = _grid[c].FindIndex(x => x == null);


        if (r >= _rows || r < 0)
            return null;

        movement.format = (movement.player == FirstPlayerEmail)
        ? firstPlayerFormat
        : secondPlayerFormat;
        _grid[c][r] = movement;
        return movement;
    }

    private bool ValidateMovement(Movement movement)
    {
        if (Turn == movement.player)
            return true;
        return false;
    }

    public async Task<bool> RegisterMovement(Movement movement)
    {

        // check if movement was made in the correct turn.
        if (!ValidateMovement(movement))
            return false;

        // add to the specified column if there is empty place
        if (AddMovement(movement) is null)
            return false;


        await _gameHub.SendMovement(FirstPlayerConnectionId, MatchId, movement);
        await _gameHub.SendMovement(SecondPlayerConnectionId, MatchId, movement);

        return true;
    }

    public async Task SwitchTurn()
    {
        // stop the timer.
        _timer.Stop();

        // switch turns
        var turn = (Turn == FirstPlayerEmail)
    ? SecondPlayerEmail
    : FirstPlayerEmail;

        // register the new turn into the session
        Turn = turn;

        // notifiy all players
        await _gameHub.SendSwitchTurnNotification(FirstPlayerConnectionId, MatchId, turn);
        await _gameHub.SendSwitchTurnNotification(SecondPlayerConnectionId, MatchId, turn);

        // restart the timer.
        _timer.Start();
    }

    public async Task GameOver()
    {
        if (Winner == FirstPlayerEmail)
        {
            await _gameHub.SendMatchWonNotification(FirstPlayerConnectionId, MatchId);
            await _gameHub.SendMatchLostNotification(SecondPlayerConnectionId, MatchId);
        }
        else if (Winner == SecondPlayerEmail)
        {
            await _gameHub.SendMatchWonNotification(SecondPlayerConnectionId, MatchId);
            await _gameHub.SendMatchLostNotification(FirstPlayerConnectionId, MatchId);
        }

        OnGameEnded();
    }


    public async Task Forfit(string connectionId)
    {
        if (connectionId == FirstPlayerConnectionId)
        {
            Winner = SecondPlayerEmail;
            await _gameHub.SendMatchForfittedNotification(SecondPlayerConnectionId, MatchId);
        }
        else if (connectionId == FirstPlayerConnectionId)
        {
            Winner = FirstPlayerEmail;
            await _gameHub.SendMatchForfittedNotification(FirstPlayerConnectionId, MatchId);
        }

        OnGameEnded();
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

    public void Dispose()
    {
        _timer.Dispose();
    }
}
