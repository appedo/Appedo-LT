using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AppedoLT.Core
{ 
   public class LicenseInfo
    {
        public string LicenseId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string PhoneNo { get; set; }
        public string EmailAddress { get; set; }
        public string MacId { get; set; }
        public string ProductId { get; set; }
        public string ProductVersion { get; set; }
        public string CreatedDate { get; set; }
        public string Mode { get; set; }
        public string Note { get; set; }
        public bool IsExpired { get; set; }
        public DateTime ExpiredData = new DateTime();
        public int NoOfUsers { get; set; }

        public LicenseInfo()
        {
        }
        public LicenseInfo(XmlDocument doc)
        {
            this.MacId = doc.ChildNodes[1].Attributes["mac_id"].Value;
            this.Mode = doc.ChildNodes[1].Attributes["mode"].Value;
            this.ProductVersion = doc.ChildNodes[1].Attributes["product_version"].Value;
            this.LicenseId = doc.ChildNodes[1].Attributes["spl_id"].Value;
            this.IsExpired = Convert.ToBoolean(doc.ChildNodes[1].Attributes["isExpired"].Value);
            this.ExpiredData = Convert.ToDateTime(doc.ChildNodes[1].Attributes["expiredate"].Value);
            this.NoOfUsers = Convert.ToInt32(doc.ChildNodes[1].Attributes["note"].Value.Split(':')[1]);
        }
        public string GetInfoToGetLicense()
        {
            StringBuilder str = new StringBuilder();
            str.Append("{");
            str.Append("\"").Append("firstName").Append("\":").Append("\"").Append(this.FirstName).Append("\"").Append(",");
            str.Append("\"").Append("lastName").Append("\":").Append("\"").Append(this.LastName).Append("\"").Append(",");
            str.Append("\"").Append("emailId").Append("\":").Append("\"").Append(this.EmailAddress).Append("\"").Append(",");
            str.Append("\"").Append("country").Append("\":").Append("\"").Append(this.Country).Append("\"").Append(",");
            str.Append("\"").Append("phoneNo").Append("\":").Append("\"").Append(this.PhoneNo).Append("\"").Append(",");
            str.Append("\"").Append("macId").Append("\":").Append("\"").Append(this.MacId).Append("\"").Append(",");
            str.Append("\"").Append("productId").Append("\":").Append("\"").Append(this.ProductId).Append("\"").Append(",");
            str.Append("\"").Append("productVersion").Append("\":").Append("\"").Append(this.ProductVersion).Append("\"").Append(",");
            str.Append("\"").Append("mode").Append("\":").Append("\"").Append(this.Mode).Append("\"");
            str.Append("}");
            return str.ToString();

        }
        
    }
}