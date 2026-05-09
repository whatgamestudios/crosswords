// Copyright (c) Whatgame Studios 2024 - 2026
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace CrossWords {

    public class GameDayRange {
        public int MinDay;
        public int MaxDay;
    }

    public class GameDayCheckResult {
        public bool Valid;
        public int RequestedDay;
        public int MinDay;
        public int MaxDay;
    }

    public class GameDayServerProcessor {

        /// <summary>
        /// Return the current valid game day range (min and max day visible across all timezones).
        /// </summary>
        public async Task<GameDayRange> GetCurrentGameDay() {
            JObject result = await ServerClient.SendAsync("gameday.current", new JObject());
            return new GameDayRange {
                MinDay = (int)result["min_day"],
                MaxDay = (int)result["max_day"]
            };
        }

        /// <summary>
        /// Check whether a specific game day is currently valid.
        /// </summary>
        public async Task<GameDayCheckResult> CheckGameDay(int day) {
            var parameters = new JObject { ["day"] = day };
            JObject result = await ServerClient.SendAsync("gameday.check", parameters);
            return new GameDayCheckResult {
                Valid = (bool)result["valid"],
                RequestedDay = (int)result["requested_day"],
                MinDay = (int)result["min_day"],
                MaxDay = (int)result["max_day"]
            };
        }

        /// <summary>
        /// Return the seed word for each requested game day. Null entries mean no seed word configured for that day.
        /// </summary>
        public async Task<Dictionary<int, string>> GetSeedWords(int[] days) {
            var parameters = new JObject {
                ["days"] = new JArray(days)
            };
            JObject result = await ServerClient.SendAsync("seedwords.check", parameters);

            var map = new Dictionary<int, string>();
            foreach (var prop in result.Properties()) {
                string word = prop.Value.Type == JTokenType.Null ? null : (string)prop.Value;
                map[int.Parse(prop.Name)] = word;
            }
            return map;
        }
    }
}
