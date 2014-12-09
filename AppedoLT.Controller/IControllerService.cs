using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace AppedoLTController
{

    [ServiceContract]
    interface IControllerService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml,BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "xmltest")]
        ControllerData GetValueXml();


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat=WebMessageFormat.Xml,  BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "xml")]
        ControllerData GetValue(ControllerData data);
    }
}
