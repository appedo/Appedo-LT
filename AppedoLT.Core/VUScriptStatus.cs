using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AppedoLT.Core
{
    [DataContract]
    public class VUScriptStatus
    {
        [DataMember(Name = "scriptname")]
        public string ScriptName { get; set; }

        [DataMember(Name = "scriptid")]
        public string ScriptId { get; set; }

        [DataMember(Name = "totaltwohundredstatuscodecount")]
        public int TotalTwoHundredStatusCodeCount { get; set; }

        [DataMember(Name = "totalthreehundredstatuscodecount")]
        public int TotalThreeHundredStatusCodeCount { get; set; }

        [DataMember(Name = "totalfourhundredstatuscodecount")]
        public int TotalFourHundredStatusCodeCount { get; set; }

        [DataMember(Name = "totalfivehundredstatuscodecount")]
        public int TotalFiveHundredStatusCodeCount { get; set; }

        [DataMember(Name = "totalerrorcount")]
        public int TotalErrorCount { get; set; }

        [DataMember(Name = "totalvusercreated")]
        public int TotalVUserCreated { get; set; }

        [DataMember(Name = "totalvusercompleted")]
        public int TotalVUserCompleted { get; set; }
    }
}
