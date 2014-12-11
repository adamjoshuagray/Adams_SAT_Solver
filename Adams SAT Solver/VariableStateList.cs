using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adams_SAT_Solver
{
    class VariableStateList: Dictionary<VariableExpression, bool>
    {
        public bool OverallEvaluation
        {
            get;
            set;
        }
    }
    class VariableStateListCollection : List<VariableStateList> { }
}
