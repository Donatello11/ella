using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpargoCalc.Infrastructure.Concrete;

namespace SpargoCalcTests
{
    [TestClass]
    public class UnitTest
    {
        ConcreteLoanInfo concreteLoanInfo;

        [TestInitialize()]
        public void Initialize() {
            concreteLoanInfo = new ConcreteLoanInfo();
        }

        [TestMethod]
        public void GetBadFormatForMalformedJson()
        {
            // look the interest value
            var (result,_) = concreteLoanInfo.GetInfo("{\"amount\":100000, \"interest\":\"1%11\",\"downpayment\":20000,\"term\":30}");
            Assert.AreEqual(ConcreteLoanResult.BadFormat, result);
        }

        [TestMethod]
        public void GetOkForValidJson()
        {
            var (result, _) = concreteLoanInfo.GetInfo("{\"amount\":100000, \"interest\":\"5.5%\",\"downpayment\":20000,\"term\":30}");
            Assert.AreEqual(ConcreteLoanResult.Ok, result);
        }

        [TestMethod]
        public void GetInvalidDataForValidJson()
        {
            // the amount is lower than 0
            var (result, _) = concreteLoanInfo.GetInfo("{\"amount\":-100000, \"interest\":\"5.5%\",\"downpayment\":20000,\"term\":30}");
            Assert.AreEqual(ConcreteLoanResult.InvalidData, result);
        }

        [TestMethod]
        public void ValidateDataForExample()
        {
            var _ = concreteLoanInfo.GetInfo("{\"amount\":100000, \"interest\":\"5.5%\",\"downpayment\":20000,\"term\":30}");
            var t = concreteLoanInfo.GetLoanInfoObject();
            Assert.AreEqual(454.23m, t.MonthlyPayment);
            Assert.AreEqual(83523.23m, t.TotalInterest);
            Assert.AreEqual(163523.23m, t.TotalPayment);
        }
    }
}
