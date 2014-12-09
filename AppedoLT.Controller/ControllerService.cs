using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;

namespace AppedoLTController
{
    [DataContract]
    class ControllerData
    {
        [DataMember]
        public string Name { get; set; }

        //[DataMember]
        //public XmlNode Test { get; set; }
    }

    class ControllerService: IControllerService
    {
        public ControllerData GetValue(ControllerData data)
        {
            return data;
        }

        public ControllerData GetValueXml()
        {
            ControllerData data=new ControllerData();
            data.Name="test";
            return data;
        }
    }
}
