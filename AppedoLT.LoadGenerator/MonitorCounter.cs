using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppedoLTLoadGenerator
{
    [DataContract]
    public class MonitorCounter
    {
        [DataMember(Name = "counter_id")]
        public int CounterId { get; set; }
        [DataMember(Name = "counter_name")]
        public string CounterName { get; set; }
        [DataMember(Name = "category_name")]
        public string CategoryName { get; set; }
        [DataMember(Name = "has_instance")]
        public bool HasInstance { get; set; }
        [DataMember(Name = "instance_name")]
        public string InstanceName { get; set; }
    }
}
