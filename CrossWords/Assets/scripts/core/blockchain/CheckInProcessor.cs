// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using System;
using System.Collections;
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

using CrossWords.WorcadianCheckInV1;
using CrossWords.WorcadianCheckInV1.ContractDefinition;

namespace CrossWords {

    public class CheckInProcessor : ContractExecution {

        private WorcadianCheckInV1Service service;

        public const string CHECK_IN_CONTRACT = "0x7e70b51a3090753593aaafedf26536ed2cbc26e8";

        public CheckInProcessor() : base(CHECK_IN_CONTRACT) {
            var web3 = new Web3(RPC_URL);
            service = new WorcadianCheckInV1Service(web3, contractAddress);
        }


        public async Task<bool> SubmitCheckIn(uint gameDay) {
            var func = new CheckInFunction() {
                GameDay = gameDay,
            };
            var (success, _) = await executeTransaction(func.GetCallData());
            return success;
        }

    }
}
