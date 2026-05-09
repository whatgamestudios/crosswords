// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace CrossWords {

    public class CheckInResult {
        public int GameDay;
        public string Player;
        public int DaysPlayed;
        public bool IsNewDay;
    }

    public class PlayersResult {
        public int Total;
        public string[] Players;
    }

    public class CheckInServerProcessor {

        /// <summary>
        /// Record a player session. Increments session counter every call; unique-player and days_played
        /// counters only increment on the first check-in of a given day.
        /// </summary>
        public async Task<CheckInResult> CheckIn(int gameDay, string player) {
            var parameters = new JObject {
                ["game_day"] = gameDay,
                ["player"] = player
            };
            JObject result = await ServerClient.SendAsync("checkin.checkin", parameters);
            return new CheckInResult {
                GameDay = (int)result["game_day"],
                Player = (string)result["player"],
                DaysPlayed = (int)result["days_played"],
                IsNewDay = (bool)result["is_new_day"]
            };
        }

        /// <summary>
        /// Return the total number of unique days a player has checked in.
        /// Returns 0 for unknown players.
        /// </summary>
        public async Task<int> GetDaysPlayed(string player) {
            var parameters = new JObject { ["player"] = player };
            JObject result = await ServerClient.SendAsync("checkin.days_played", parameters);
            return (int)result["days_played"];
        }

        /// <summary>
        /// Return the unique player count for a range of consecutive game days.
        /// Array is indexed from startGameDay; days with no check-ins return 0.
        /// </summary>
        public async Task<int[]> GetNumPlayers(int startGameDay, int numDays) {
            var parameters = new JObject {
                ["start_game_day"] = startGameDay,
                ["num_days"] = numDays
            };
            JObject result = await ServerClient.SendAsync("checkin.num_players", parameters);
            return result["players"]?.ToObject<int[]>() ?? Array.Empty<int>();
        }

        /// <summary>
        /// Return the session count for a range of consecutive game days.
        /// Array is indexed from startGameDay; days with no sessions return 0.
        /// </summary>
        public async Task<int[]> GetNumSessions(int startGameDay, int numDays) {
            var parameters = new JObject {
                ["start_game_day"] = startGameDay,
                ["num_days"] = numDays
            };
            JObject result = await ServerClient.SendAsync("checkin.num_sessions", parameters);
            return result["sessions"]?.ToObject<int[]>() ?? Array.Empty<int>();
        }

        /// <summary>
        /// Return the total number of unique players who have ever checked in.
        /// </summary>
        public async Task<int> GetTotalPlayers() {
            JObject result = await ServerClient.SendAsync("checkin.total_players", new JObject());
            return (int)result["total"];
        }

        /// <summary>
        /// Return a paginated slice of all-time players in order of first check-in.
        /// </summary>
        public async Task<PlayersResult> GetPlayers(int startIndex, int count) {
            var parameters = new JObject {
                ["start_index"] = startIndex,
                ["count"] = count
            };
            JObject result = await ServerClient.SendAsync("checkin.players", parameters);
            return new PlayersResult {
                Total = (int)result["total"],
                Players = result["players"]?.ToObject<string[]>() ?? Array.Empty<string>()
            };
        }
    }
}
