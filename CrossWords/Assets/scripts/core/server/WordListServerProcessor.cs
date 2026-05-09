// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace CrossWords {

    public class AddWordsResult {
        public string[] Added;
        public string[] AlreadyExists;
    }

    public class WordListServerProcessor {

        /// <summary>
        /// Check whether words are in the wordlist. Returns a map of uppercase word to bool.
        /// </summary>
        public async Task<Dictionary<string, bool>> CheckWords(string[] words) {
            var parameters = new JObject {
                ["words"] = new JArray(words)
            };
            JObject result = await ServerClient.SendAsync("check", parameters);

            var map = new Dictionary<string, bool>();
            foreach (var prop in result.Properties()) {
                map[prop.Name] = (bool)prop.Value;
            }
            return map;
        }

        /// <summary>
        /// Add words to the wordlist. Returns which were newly added vs already present.
        /// </summary>
        public async Task<AddWordsResult> AddWords(string[] words) {
            var parameters = new JObject {
                ["words"] = new JArray(words)
            };
            JObject result = await ServerClient.SendAsync("add", parameters);

            return new AddWordsResult {
                Added = result["added"]?.ToObject<string[]>() ?? Array.Empty<string>(),
                AlreadyExists = result["already_exists"]?.ToObject<string[]>() ?? Array.Empty<string>()
            };
        }
    }
}
