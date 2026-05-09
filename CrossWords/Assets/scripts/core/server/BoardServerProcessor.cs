// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace CrossWords {

    public class BoardAnalysisResult {
        public int Score;
        public string[] Words;
        public bool[] InDictionary;
    }

    public class BoardSubmission {
        public string Player;
        public string Board;
    }

    public class BoardSubmitResult {
        public string Status;
        public int SubmittedScore;
        public int CalculatedScore;
        public string[] Words;
        public bool[] InDictionary;
    }

    public class BoardImportResult {
        public string Status;
        public int GameDay;
        public string Player;
        public int CalculatedScore;
        public string[] Words;
        public bool[] InDictionary;
    }

    public class BoardResultsResult {
        public int NumSubmissions;
        public int? BestScore;
        public BoardSubmission[] Submissions;
    }

    public class BoardSubmissionsResult {
        public int Total;
        public BoardSubmission[] Submissions;
    }

    public class BoardServerProcessor {

        /// <summary>
        /// Discover all connected words on a board starting from the centre cell.
        /// Does not check the dictionary or calculate a score.
        /// </summary>
        public async Task<string[]> AnalyseBoard(string board) {
            var parameters = new JObject { ["board"] = board };
            JObject result = await ServerClient.SendAsync("analyse", parameters);
            return result["words"]?.ToObject<string[]>() ?? Array.Empty<string>();
        }

        /// <summary>
        /// Full analysis: word discovery + dictionary lookup + score calculation.
        /// </summary>
        public async Task<BoardAnalysisResult> FullAnalyseBoard(string board) {
            var parameters = new JObject { ["board"] = board };
            JObject result = await ServerClient.SendAsync("board.analyse", parameters);
            return new BoardAnalysisResult {
                Score = (int)result["score"],
                Words = result["words"]?.ToObject<string[]>() ?? Array.Empty<string>(),
                InDictionary = result["in_dictionary"]?.ToObject<bool[]>() ?? Array.Empty<bool>()
            };
        }

        /// <summary>
        /// Calculate the score for an already-known word list and dictionary flags.
        /// </summary>
        public async Task<int> CalculateScore(string[] words, bool[] inDictionary) {
            var parameters = new JObject {
                ["words"] = new JArray(words),
                ["in_dictionary"] = new JArray(inDictionary)
            };
            JObject result = await ServerClient.SendAsync("score", parameters);
            return (int)result["score"];
        }

        /// <summary>
        /// Submit a completed board for a game day. Lower score is better; 0 is optimal.
        /// </summary>
        public async Task<BoardSubmitResult> SubmitBoard(int gameDay, int score, string board, string player) {
            var parameters = new JObject {
                ["game_day"] = gameDay,
                ["score"] = score,
                ["board"] = board,
                ["player"] = player
            };
            JObject result = await ServerClient.SendAsync("board.submit", parameters);
            return new BoardSubmitResult {
                Status = (string)result["status"],
                SubmittedScore = (int)(result["submitted_score"] ?? 0),
                CalculatedScore = (int)(result["calculated_score"] ?? 0),
                Words = result["words"]?.ToObject<string[]>() ?? Array.Empty<string>(),
                InDictionary = result["in_dictionary"]?.ToObject<bool[]>() ?? Array.Empty<bool>()
            };
        }

        /// <summary>
        /// Import a historical submission without validating game day range or seed word position.
        /// Idempotent — same (game_day, player, board) triple is only stored once.
        /// </summary>
        public async Task<BoardImportResult> ImportBoard(int gameDay, string board, string player) {
            var parameters = new JObject {
                ["game_day"] = gameDay,
                ["board"] = board,
                ["player"] = player
            };
            JObject result = await ServerClient.SendAsync("board.import", parameters);
            return new BoardImportResult {
                Status = (string)result["status"],
                GameDay = (int)(result["game_day"] ?? gameDay),
                Player = (string)(result["player"] ?? player),
                CalculatedScore = (int)(result["calculated_score"] ?? 0),
                Words = result["words"]?.ToObject<string[]>() ?? Array.Empty<string>(),
                InDictionary = result["in_dictionary"]?.ToObject<bool[]>() ?? Array.Empty<bool>()
            };
        }

        /// <summary>
        /// Return total submission count, best score, and all submissions at the best score for a game day.
        /// </summary>
        public async Task<BoardResultsResult> GetResults(int gameDay) {
            var parameters = new JObject { ["game_day"] = gameDay };
            JObject result = await ServerClient.SendAsync("board.results", parameters);

            int? bestScore = null;
            if (result["best_score"] != null && result["best_score"].Type != JTokenType.Null) {
                bestScore = (int)result["best_score"];
            }

            return new BoardResultsResult {
                NumSubmissions = (int)result["num_submissions"],
                BestScore = bestScore,
                Submissions = ParseSubmissions(result["submissions"] as JArray)
            };
        }

        /// <summary>
        /// Return a paginated slice of submissions at a specific score tier.
        /// </summary>
        public async Task<BoardSubmissionsResult> GetSubmissions(int gameDay, int score, int startIndex, int count) {
            var parameters = new JObject {
                ["game_day"] = gameDay,
                ["score"] = score,
                ["start_index"] = startIndex,
                ["count"] = count
            };
            JObject result = await ServerClient.SendAsync("board.submissions", parameters);
            return new BoardSubmissionsResult {
                Total = (int)result["total"],
                Submissions = ParseSubmissions(result["submissions"] as JArray)
            };
        }

        private static BoardSubmission[] ParseSubmissions(JArray arr) {
            if (arr == null) return Array.Empty<BoardSubmission>();
            var list = new List<BoardSubmission>();
            foreach (JToken item in arr) {
                list.Add(new BoardSubmission {
                    Player = (string)item["player"],
                    Board = (string)item["board"]
                });
            }
            return list.ToArray();
        }
    }
}
