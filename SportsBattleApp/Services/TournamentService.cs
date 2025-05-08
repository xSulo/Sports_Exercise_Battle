using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsBattleApp.DTOs;
using SportsBattleApp.Repositories;
using SportsBattleApp.Tcp;

namespace SportsBattleApp.Services
{
    public class TournamentService
    {
        private List<UserStatsTournamentDTO> _lastParticipants = new();
        private readonly UserRepository _userRepository;

        public TournamentService(UserRepository userRepository)
        {
            _userRepository = userRepository;
            TournamentState.Instance.OnTournamentFinished += OnTournamentFinished;
        }

        private void OnTournamentFinished(List<UserStatsTournamentDTO> finalEntries)
        {
            _ = Task.Run(async () =>
            {
                int maxCount = finalEntries.Max(x => x.Count);
                var candidates = finalEntries.Where(x => x.Count == maxCount).ToList();
                int minDuration = candidates.Min(x => x.Duration);
                var winners = candidates.Where(x => x.Duration == minDuration).ToList();
                bool isTie = winners.Count > 1;

                foreach (var entry in finalEntries)
                {
                    if (winners.Any(x => x.UserId == entry.UserId))
                    {
                        await _userRepository.UpdateEloTournament(entry.UserId, isTie ? 1 : 2);
                    }
                    else
                    {
                        await _userRepository.UpdateEloTournament(entry.UserId, -2);
                    }
                }
            });
        }

        public Task<TournamentGetStatusDTO> GetTournamentStatusAsync()
        {
            var status = TournamentState.Instance.GetTournamentStatus();
            return Task.FromResult(status);
        }

        public Task AddOrUpdateStatsAsync(UserStatsTournamentDTO stats)
        {
            TournamentState.Instance.AddOrUpdateEntry(stats);
            return Task.CompletedTask;
        }
    }
}
