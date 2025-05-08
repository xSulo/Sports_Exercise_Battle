using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsBattleApp.DTOs;

namespace SportsBattleApp.Tcp
{
    public class TournamentState
    {
        private static readonly TournamentState _instance = new();
        public static TournamentState Instance => _instance;

        private readonly Dictionary<int, UserStatsTournamentDTO> _entries = new();
        private readonly List<string> _eventLog = new();
        public event Action<string>? OnEventBroadcast;

        private readonly object _lock = new();

        public bool IsTournamentRunning = false;
        private TournamentGetStatusDTO? _lastTournamentResult = null;
        private DateTime? _tournamentStart;
        private Timer? _tournamentTimer;
        public event Action<List<UserStatsTournamentDTO>>? OnTournamentFinished;

        private TournamentState() { }


        public TournamentGetStatusDTO GetTournamentStatus()
        {
            lock (_lock)
            {
                if (IsTournamentRunning)
                {
                    int maxCount = _entries.Values.Max(x => x.Count);
                    var participants = _entries.Values.ToList();

                    var leaders = participants
                        .Where(x => x.Count == maxCount)
                        .ToList();

                    int minDuration = leaders.Min(x => x.Duration);
                    leaders = [.. leaders.Where(x => x.Duration == minDuration)];

                    int participantsCount = participants.Count;
                    string leader = string.Empty;

                    if (leaders.Count > 1)
                    {
                        leader = string.Join(", ", leaders.Select(x => x.Username));
                    }
                    else
                    {
                        leader = leaders[0].Username;
                    }

                    return new TournamentGetStatusDTO
                    {
                        IsTournamentRunning = IsTournamentRunning,
                        Participants = participantsCount,
                        leader = leader,
                        TournamentStart = _tournamentStart?.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                }
                else if (_lastTournamentResult != null)
                {
                    return _lastTournamentResult;
                }
                else
                {
                    return new TournamentGetStatusDTO
                    {
                        IsTournamentRunning = IsTournamentRunning,
                    };
                }
            }
        }

        public void AddOrUpdateEntry(UserStatsTournamentDTO newEntry)
        {
            lock (_lock)
            {
                if (!IsTournamentRunning)
                {
                    IsTournamentRunning = true;
                    _tournamentStart = DateTime.UtcNow;
                    _tournamentTimer = new Timer(OnTournamentEnd, null, TimeSpan.FromMinutes(2), Timeout.InfiniteTimeSpan);

                    var eventMessege = "\nTournament started!\n";
                    _eventLog.Add(eventMessege);
                    OnEventBroadcast?.Invoke(eventMessege);
                }

                if (_entries.ContainsKey(newEntry.UserId))
                {
                    var existingData = _entries[newEntry.UserId];
                    existingData.Count += newEntry.Count;
                    existingData.Duration += newEntry.Duration;

                    if (existingData.Username != newEntry.Username)
                    {
                        existingData.Username = newEntry.Username;
                    }

                    if (existingData.WinningSpeech != newEntry.WinningSpeech)
                    {
                        existingData.WinningSpeech = newEntry.WinningSpeech;
                    }

                    // For the tournament live view
                    var eventMessege = ($"{newEntry.Username} performed {newEntry.Count} Push Ups in {newEntry.Duration} seconds.");
                    _eventLog.Add(eventMessege);
                    OnEventBroadcast?.Invoke(eventMessege);


                }
                else
                {
                    _entries[newEntry.UserId] = newEntry;

                    // For the tournament live view
                    var eventMessege = ($"\n{newEntry.Username} joined the tournament.");
                    _eventLog.Add(eventMessege);
                    OnEventBroadcast?.Invoke(eventMessege);

                    eventMessege = ($"{newEntry.Username} performed {newEntry.Count} Push Ups in {newEntry.Duration} seconds.");
                    _eventLog.Add(eventMessege);
                    OnEventBroadcast?.Invoke(eventMessege);
                }
            }
        }

        public void OnTournamentEnd(object? state)
        {
            lock (_lock)
            {
                int maxCount = _entries.Values.Max(x => x.Count);

                var candidates = _entries
                    .Where(x => x.Value.Count == maxCount)
                    .Select(x => (UserId: x.Key, Data: x.Value))
                    .ToList();

                UserStatsTournamentDTO winner;

                if (candidates.Count > 1)
                {
                    int minDuration = candidates.Min(x => x.Data.Duration);
                    candidates = candidates
                        .Where(x => x.Data.Duration == minDuration)
                        .ToList();

                    if (candidates.Count > 1)
                    {
                        string winnerUsernames = string.Join(", ", candidates.Select(x => x.Data.Username));

                        OnEventBroadcast?.Invoke($"\n\nTournament ended! \nIt's a tie between: {winnerUsernames}, both performed {maxCount} Push Ups in {minDuration} seconds.");

                        _lastTournamentResult = new TournamentGetStatusDTO
                        {
                            IsTournamentRunning = false,
                            Participants = _entries.Count,
                            leader = winnerUsernames,
                            TournamentStart = _tournamentStart?.ToString("yyyy-MM-dd HH:mm:ss")
                        };
                    }
                }
                else
                {
                    winner = candidates[0].Data;
                    OnEventBroadcast?.Invoke($"\n\nTournament ended! \nThe winner is {winner.Username} with {winner.Count} Push Ups in {winner.Duration} seconds.");

                    if (winner.WinningSpeech != "")
                    {
                        OnEventBroadcast?.Invoke($"{winner.Username}: {winner.WinningSpeech}");
                    }

                    _lastTournamentResult = new TournamentGetStatusDTO
                    {
                        IsTournamentRunning = false,
                        Participants = _entries.Count,
                        leader = winner.Username,
                        TournamentStart = _tournamentStart?.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                }

                _tournamentStart = null;
                IsTournamentRunning = false;
                OnTournamentFinished?.Invoke(_entries.Values.ToList());
                _entries.Clear();
                _eventLog.Clear();
            }
        }

        public List<UserStatsTournamentDTO> GetAll()
        {
            lock (_lock)
            {
                return _entries.Values.ToList();
            }
        }

        public List<string> GetEventLog()
        {
            lock (_lock)
            {
                return new List<string>(_eventLog);
            }
        }

        public void StartTournament() => IsTournamentRunning = true;
        public void EndTournament() => IsTournamentRunning = false;
    }
}