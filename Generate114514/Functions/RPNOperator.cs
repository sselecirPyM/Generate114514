using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generate114514.Functions
{
    partial class Algorithms
    {
        enum RPNOperator
        {
            Push,
            Add,
            Sub,
            Mul,
            Div,
        }

        static IReadOnlyDictionary<RPNOperator, int> operatorPriority = new Dictionary<RPNOperator, int>()
        {
            { RPNOperator.Add, 1 },
            { RPNOperator.Sub, 1 },
            { RPNOperator.Mul, 2 },
            { RPNOperator.Div, 2 }
        };

        static IReadOnlyDictionary<RPNOperator, string> operatorSymbol = new Dictionary<RPNOperator, string>()
        {
            { RPNOperator.Add, "+" },
            { RPNOperator.Sub, "-" },
            { RPNOperator.Mul, "*" },
            { RPNOperator.Div, "/" }
        };
    }
}
