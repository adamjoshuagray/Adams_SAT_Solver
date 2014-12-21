using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adams_SAT_Solver
{
    /// <summary>
    /// A simple exact SAT solver.
    /// </summary>
    public class AGSAT
    {
        /// <summary>
        /// Gives a variable state list of solutions.
        /// </summary>
        /// <param name="e">The expression check the satisfiability of.</param>
        /// <returns></returns>
        public static VariableStateListCollection SAT(Expression e)
        {
            if (e is NOTExpression)
            {
                NOTExpression not = e as NOTExpression;
                VariableStateListCollection vslc = SAT(not.Term);
                foreach (VariableStateList vsl in vslc)
                {
                    vsl.OverallEvaluation = !vsl.OverallEvaluation;
                }
                return vslc;
            }
            else if (e is BinaryExpression)
            {
                BinaryExpression bin = e as BinaryExpression;
                VariableStateListCollection vslca = SAT(bin.TermA);
                VariableStateListCollection vslcb = SAT(bin.TermB);
                if (bin is ANDExpression)
                {
                    return Cross(vslca, vslcb, (a, b) => a & b);
                }
                else if (bin is ORExpression)
                {
                    return Cross(vslca, vslcb, (a, b) => a | b);
                }
                else if (bin is XORExpression)
                {
                    return Cross(vslca, vslcb, (a, b) => a ^ b);
                }
                else
                {
                    throw new InvalidCastException("Cannot cast BinaryExpression to one of ORExpression, ANDExpression or XORExpression.");
                }
            }
            else if (e is VariableExpression)
            {
                VariableExpression var = e as VariableExpression;
                VariableStateListCollection vslc = new VariableStateListCollection();
                VariableStateList vsla = new VariableStateList();
                vsla.Add(var, true);
                vsla.OverallEvaluation = true;
                VariableStateList vslb = new VariableStateList();
                vslb.Add(var, false);
                vslb.OverallEvaluation = false;
                vslc.Add(vsla);
                vslc.Add(vslb);
                return vslc;
            }
            else
            {
                throw new InvalidCastException("Cannot cast Expression to one of BinaryExpression, NOTExpression or VariableExpression.");
            }

        }
        /// <summary>
        /// Runs a not transformation of the variable state lists.
        /// </summary>
        /// <param name="vsla">The VariableStateListCollection to transform.</param>
        /// <returns></returns>
        static VariableStateListCollection Not(VariableStateListCollection vsla)
        {
            VariableStateListCollection vslc = new VariableStateListCollection();
            foreach (VariableStateList vsl in vsla)
            {
                foreach (KeyValuePair<VariableExpression, bool> kvpi in vsl)
                {
                    VariableStateList vsln = new VariableStateList();
                    foreach (KeyValuePair<VariableExpression, bool> kvpj in vsl)
                    {
                        vsln[kvpj.Key] = kvpj.Value;
                    }
                    vsln[kvpi.Key] = !vsln[kvpi.Key];
                    vslc.Add(vsln);
                }
            }
            return vslc;
        }
        /// <summary>
        /// Performs a binary cross of VariableStateListCollection.
        /// </summary>
        /// <param name="vsla">One of the VariableStateListCollections</param>
        /// <param name="vslb">One of the VariableStateListCollections</param>
        /// <param name="op">The operation to use on the cross.</param>
        /// <returns>The variable state list collection that results from the cross.</returns>
        static VariableStateListCollection Cross(VariableStateListCollection vsla,
                                                 VariableStateListCollection vslb,
                                                 Func<bool, bool, bool> op)
        {
            VariableStateListCollection vslc = new VariableStateListCollection();
            foreach (VariableStateList vsli in vsla)
            {
                foreach (VariableStateList vslj in vslb)
                {
                    bool ok = true;
                    foreach (KeyValuePair<VariableExpression, bool> kvp in vsli)
                    {
                        if (vslj.ContainsKey(kvp.Key))
                        {
                            if (vslj[kvp.Key] != kvp.Value)
                            {
                                ok = false;
                                break;
                            }
                        }
                    }
                    if (ok)
                    {
                        VariableStateList vsl = new VariableStateList();
                        foreach (KeyValuePair<VariableExpression, bool> kvp in vsli)
                        {
                            vsl[kvp.Key] = kvp.Value;
                        }
                        foreach (KeyValuePair<VariableExpression, bool> kvp in vslj)
                        {
                            vsl[kvp.Key] = kvp.Value;
                        }
                        vsl.OverallEvaluation = op(vsli.OverallEvaluation, vslj.OverallEvaluation);
                        vslc.Add(vsl);
                    }
                }
            }
            return vslc;
        }
    }
}
