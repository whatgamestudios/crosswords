using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CrossWords.WorcadianGameV1.ContractDefinition
{
    public partial class Submission : SubmissionBase { }

    public class SubmissionBase 
    {
        [Parameter("address", "player", 1)]
        public virtual string Player { get; set; }
        [Parameter("string", "board", 2)]
        public virtual string Board { get; set; }
    }
}
