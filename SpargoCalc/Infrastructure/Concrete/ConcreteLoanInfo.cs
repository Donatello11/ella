using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using SpargoCalc.Entities.Calc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SpargoCalc.Infrastructure.Concrete
{
    public class ConcreteLoanInfo : LoanInfo<ConcreteLoanResult, string>
    {
        #region classes for input
        /// <summary>
        /// dummy class for scheme generation
        /// </summary>
        private class ConcreteLoanRequest
        {
            [Required]
            public decimal Amount { get; set; }
            [Required]
            public string Interest { get; set; }
            [Required]
            public decimal Downpayment { get; set; }
            [Required]
            public int Term { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        private class ConcreteLoanInput
        {
            [Required]
            public decimal Amount { get; set; }
            [Required]
            [JsonConverter(typeof(PercentageToDoubleConverter))]
            public double Interest { get; set; }
            [Required]
            public decimal Downpayment { get; set; }
            [Required]
            public int Term { get; set; }
        }
        #endregion

        #region jsonCOnverters
        private class PercentageToDoubleConverter : JsonConverter
        {
            public override bool CanRead
            {
                get { return true; }
            }
            public override bool CanConvert(Type objectType)
            {
                return true;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var val = reader.Value.ToString();

                return Convert.ToDouble(val.Substring(0, val.Length - 1));



            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
        private ConcreteLoanOutput concreteLoanOutput;

        public override (ConcreteLoanResult, string) GetInfo(string source)
        {
            var (result, input) = GetLoanInput(source);
            if (result != ConcreteLoanResult.Ok) { return (result, null); }

            if (!IsDataCorrect(input)) return (ConcreteLoanResult.InvalidData, null);

            concreteLoanOutput = GetLoanOutput(input);
            return (ConcreteLoanResult.Ok, GetJsonStringOfOutput(concreteLoanOutput));
        }

        /// <summary>
        /// Helper for tests
        /// </summary>
        /// <returns></returns>
        public ConcreteLoanOutput GetLoanInfoObject() => concreteLoanOutput;

        private (ConcreteLoanResult, ConcreteLoanInput) GetLoanInput(string source)
        {
            var generator = new JSchemaGenerator
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            JSchema schema = generator.Generate(typeof(ConcreteLoanRequest));

            JObject jsonObject;
            try
            {
                jsonObject = JObject.Parse(source);
            }
            catch
            {
                return (ConcreteLoanResult.Error, null);
            }
            if (!jsonObject.IsValid(schema)
                || !Regex.IsMatch(jsonObject["interest"].Value<string>(), @"^[0-9]+(\.[0-9][0-9]?)?\%$"))
            {
                return (ConcreteLoanResult.BadFormat, null);
            }
            return (ConcreteLoanResult.Ok, JsonConvert.DeserializeObject<ConcreteLoanInput>(source));
        }

        /// <summary>
        /// Get a  json representation of LoanOutput
        /// </summary>
        /// <param name="concreteLoanOutput"></param>
        /// <returns></returns>
        private string GetJsonStringOfOutput(ConcreteLoanOutput concreteLoanOutput) => JsonConvert.SerializeObject(concreteLoanOutput);

        /// <summary>
        /// sanity check
        /// </summary>
        /// <param name="concreteCalcInput"></param>
        /// <returns></returns>
        private bool IsDataCorrect(ConcreteLoanInput concreteCalcInput) =>
            concreteCalcInput.Amount > 0
            && concreteCalcInput.Downpayment >= 0
            && concreteCalcInput.Interest > 0
            && concreteCalcInput.Term > 0;

        /// <summary>
        /// Get a resulting loan object
        /// </summary>
        /// <param name="concreteLoanInput"></param>
        /// <returns></returns>
        private ConcreteLoanOutput GetLoanOutput(ConcreteLoanInput concreteLoanInput)
        {
            var loanAmount = Convert.ToDouble(concreteLoanInput.Amount - concreteLoanInput.Downpayment);
            var rateOfInterest = concreteLoanInput.Interest / 1200;
            var numberOfPayments = concreteLoanInput.Term * 12;

            var monthlyPayment = (rateOfInterest * loanAmount) / (1 - Math.Pow(1 + rateOfInterest, numberOfPayments * -1));
            var totalPayment = monthlyPayment * numberOfPayments;
            var totalInterest = totalPayment - loanAmount;
            return new ConcreteLoanOutput()
            {
                MonthlyPayment = decimal.Round(Convert.ToDecimal(monthlyPayment), 2),
                TotalInterest = decimal.Round(Convert.ToDecimal(totalInterest), 2),
                TotalPayment = decimal.Round(Convert.ToDecimal(totalPayment), 2)
            };
        }




    }
}
