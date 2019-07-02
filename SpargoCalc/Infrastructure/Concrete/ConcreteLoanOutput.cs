using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SpargoCalc.Entities.Calc
{
    public class ConcreteLoanOutput
    {
        [JsonProperty(PropertyName = "monthly payment")]
        public decimal MonthlyPayment { get; set; }

        [JsonProperty(PropertyName = "total interest")]
        public decimal TotalInterest { get; set; }

        [JsonProperty(PropertyName = "total payment")]
        public decimal TotalPayment { get; set; }

    }
}
