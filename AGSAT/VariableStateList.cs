using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adams_SAT_Solver
{
    public class VariableStateList: Dictionary<VariableExpression, bool>
    {
        public bool OverallEvaluation
        {
            get;
            set;
        }
    }
    public class VariableStateListCollection : List<VariableStateList> { }
}
