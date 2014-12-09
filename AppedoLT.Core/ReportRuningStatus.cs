using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppedoLT.Core
{
    public class ReportRuningStatus
    {

        private double totalSecounts = 0;
        private double competedSecounts = 0;

        public double TotalSecounts
        {
            get { return totalSecounts; }
            set { totalSecounts = value; }
        }
        public double CompetedSecounts
        {
            get { return competedSecounts; }
            set { competedSecounts = value; }
        }
        public double Percentage
        {
            get
            {
                {
                    return (competedSecounts / totalSecounts) * 100;
                }

            }
            private set { }
        }

        public ReportRuningStatus(DateTime minData, DateTime maxDate)
        {
            TimeSpan? variable = maxDate - minData;
            if (variable.HasValue == true) totalSecounts = variable.Value.TotalSeconds;

        }
    }
}
