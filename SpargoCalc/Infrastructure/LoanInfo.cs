using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpargoCalc.Infrastructure
{
    /// <summary>
    /// asbtract class for getting loaninfo objects
    /// </summary>
    /// <typeparam name="TResultEnum"></typeparam>
    /// <typeparam name="TResultObj"></typeparam>
    public abstract class LoanInfo<TResultEnum, TResultObj> 
        where TResultEnum:Enum
        where TResultObj:class
    {
       public abstract  (TResultEnum, TResultObj) GetInfo(string source);
    }
}
