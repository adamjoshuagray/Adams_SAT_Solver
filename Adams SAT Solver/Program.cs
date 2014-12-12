using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adams_SAT_Solver
{
    class Program
    {
        static void Main(string[] args)
        {
            VariableExpression a = true;
            VariableExpression b = false;
            VariableExpression c = false;
            Expression exp = ((a & !c) & !(a & (!b & c))) & !a;
            Console.WriteLine(exp.Evaluate().ToString());
            VariableStateListCollection vslc = SAT(exp);
            foreach (VariableStateList vsl in vslc)
            {
                if (vsl.OverallEvaluation)
                {
                    Console.WriteLine("---");
                    foreach (KeyValuePair<VariableExpression, bool> kvp in vsl)
                    {
                        Console.WriteLine(kvp.Key.GetHashCode().ToString() + " : " + kvp.Value.ToString());
                    }
                    Console.WriteLine("---");
                }
            }
            Console.WriteLine(" a : " + a.GetHashCode().ToString() 
                            + " b : " + b.GetHashCode().ToString() 
                            + " c : " + c.GetHashCode().ToString());
            Console.Read();
        }
        static VariableStateListCollection SAT(Expression e)
        {
            if (e is ANDExpression)
            {
                ANDExpression xor = e as ANDExpression;
                VariableStateListCollection vsla = SAT(xor.TermA);
                VariableStateListCollection vslb = SAT(xor.TermB);
                return ANDCross(vsla, vslb);
            }
            else if (e is NOTExpression)
            {
                NOTExpression not = e as NOTExpression;
                VariableStateListCollection vslc = SAT(not.Term);
                foreach (VariableStateList vsl in vslc)
                {
                    vsl.OverallEvaluation = !vsl.OverallEvaluation;
                }
                return vslc;
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
                throw new InvalidCastException("Cannot cast expression to one of Expression, NOTExpression or VariableExpression");
            }
            
        }
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
        static VariableStateListCollection ANDCross(VariableStateListCollection vsla, 
                                                 VariableStateListCollection vslb)
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
                        foreach(KeyValuePair<VariableExpression, bool> kvp in vsli)
                        {
                            vsl[kvp.Key] = kvp.Value;
                        }
                        foreach (KeyValuePair<VariableExpression, bool> kvp in vslj)
                        {
                            vsl[kvp.Key] = kvp.Value;
                        }
                        vsl.OverallEvaluation = (vsli.OverallEvaluation && vslj.OverallEvaluation);
                        vslc.Add(vsl);
                    }
                }
            }
            return vslc;
        }
    }
}
