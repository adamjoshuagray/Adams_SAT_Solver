using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adams_SAT_Solver
{
    abstract class Expression
    {
        public abstract bool Evaluate();
        public void Reset(int setlevel)
        {
            if (this is ANDExpression)
            {
                ANDExpression xor = this as ANDExpression;
                xor.TermA.Reset(setlevel);
                xor.TermB.Reset(setlevel);
            }
            if (this is NOTExpression)
            {
                NOTExpression not = this as NOTExpression;
                not.Term.Reset(setlevel);
            }
            if (this is VariableExpression)
            {
                VariableExpression var = this as VariableExpression;
                if (var.SetLevel >= setlevel)
                {
                    var.Reset();
                }
            }
        }
        public static ANDExpression operator &(Expression a, Expression b)
        {
            ANDExpression exp = new ANDExpression();
            exp.TermA = a;
            exp.TermB = b;
            return exp;
        }
        public static NOTExpression operator !(Expression a)
        {
            NOTExpression exp = new NOTExpression();
            exp.Term = a;
            return exp;
        }
    }
    class ANDExpression : Expression
    {
        public Expression TermA
        {
            get;
            set;
        }
        public Expression TermB
        {
            get;
            set;
        }
        public override bool Evaluate()
        {
            if (TermA != null & TermB != null)
            {
                return TermA.Evaluate() ^ TermB.Evaluate();
            }
            else
            {
                throw new InvalidOperationException("XOR expression must have both sub terms defined.");
            }
        }
    }
    class NOTExpression : Expression
    {
        public Expression Term
        {
            get;
            set;
        }
        public override bool Evaluate()
        {
            return !Term.Evaluate();
        }
    }
    class VariableExpression : Expression
    {
        private bool _Value;
        public bool IsSet
        {
            get;
            private set;
        }
        public void Reset()
        {
            IsSet = false;
        }
        public int SetLevel
        {
            get;
            private set;
        }
        public void Set(bool value, int setlevel)
        {
            if (!IsSet)
            {
                _Value = value;
                IsSet = true;
                SetLevel = setlevel;
            }
            else
            {
                throw new InvalidOperationException("Variable has already been set. To change the value, call Reset and then Set.");
            }
        }
        public VariableExpression()
        {
            SetLevel = 0;
            IsSet = false;
            _Value = false;
        }
        public override bool Evaluate()
        {
            if (IsSet)
            {
                return _Value;
            }
            else
            {
                throw new InvalidOperationException("Variable is not yet set. Call Set to set the value.");
            }
        }
        public static implicit operator VariableExpression(bool a)
        {
            VariableExpression exp = new VariableExpression();
            exp.Set(a, 0);
            return exp;
        }
    }
}
