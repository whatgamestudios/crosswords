// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Text;

using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.ABI;

using Immutable.Passport;
using Immutable.Passport.Model;

using CrossWords.WorcadianWordListV1;
using CrossWords.WorcadianWordListV1.ContractDefinition;

namespace CrossWords {

    public class WordListProcessor : ContractExecution {

        private WorcadianWordListV1Service service;

        public const string WORD_LIST_CONTRACT = "0x6b17A8D0eD442a234df46342B52C49f4247e119d";

        public WordListProcessor() : base(WORD_LIST_CONTRACT) {
            var web3 = new Web3(RPC_URL);
            service = new WorcadianWordListV1Service(web3, contractAddress);
        }

        private static byte[] EncodeWord(string word) {
            return Encoding.UTF8.GetBytes(word);
        }

        private static List<byte[]> EncodeWords(List<string> words) {
            var encoded = new List<byte[]>(words.Count);
            foreach (var word in words) {
                encoded.Add(EncodeWord(word));
            }
            return encoded;
        }

        public async Task<bool> AddWords(List<string> words) {
            var func = new AddWordsFunction() {
                Words = EncodeWords(words),
            };
            var (success, _) = await executeTransaction(func.GetCallData());
            return success;
        }

        public async Task<bool> RemoveWord(string word) {
            var func = new RemoveWordFunction() {
                Word = EncodeWord(word),
            };
            var (success, _) = await executeTransaction(func.GetCallData());
            return success;
        }

        public async Task<bool> InWordList(string word) {
            return await service.InWordListQueryAsync(EncodeWord(word));
        }

        public async Task<List<bool>> InWordListBulk(List<string> words) {
            return await service.InWordListBulkQueryAsync(EncodeWords(words));
        }

    }
}
