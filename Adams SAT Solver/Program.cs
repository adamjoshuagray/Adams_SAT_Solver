using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adams_SAT_Solver
{
    class Program
    {
        static Random _ent = new Random();
        static void Main(string[] args)
        {
            VariableExpression a = true;
            VariableExpression b = false;
            VariableExpression c = false;
            VariableExpression d = false;
            VariableExpression e = true;
            VariableExpression f = false;
            VariableExpression g = false;
            Expression exp = ((a & !c) & !(a & (!b & c))) & (( f & (!d & !(b & !e))) & !e);
            exp = GenerateRandomExpression(3, 70, 0.95);
            //Console.WriteLine(exp.Evaluate().ToString());
            VariableStateListCollection vslc = SAT(exp);
            int count = 0;
            Console.WriteLine(exp.ToString());
            foreach (VariableStateList vsl in vslc)
            {
                if (vsl.OverallEvaluation)
                {
                    count++;
                    Console.WriteLine("---");
                    foreach (KeyValuePair<VariableExpression, bool> kvp in vsl)
                    {
                        Console.WriteLine(kvp.Key.GetHashCode().ToString() + " : " + kvp.Value.ToString());
                    }
                }
            }
            Console.WriteLine(count.ToString());
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
        static Expression GenerateRandomExpression(int variables, int ands, double notp)
        {
            ANDExpression aexp = GenerateRandomAndTree(ands);
            AddRandomNots(aexp, notp);
            VariableExpression[] vexps = new VariableExpression[variables];
            for (int i = 0; i < variables; i++)
            {
                vexps[i] = new VariableExpression();
            }
            AddRanomVariables(aexp, vexps);
            return aexp;
        }
        static ANDExpression GenerateRandomAndTree(int nodes)
        {
            int sides = 0;
            if (nodes >= 2)
            {
                sides = _ent.Next(0, 3);
            }
            else if (nodes == 1)
            {
                sides = _ent.Next(0, 2);
            }
            else
            {
                // There are no more nodes left.
                return null;
            }
            ANDExpression aexp = new ANDExpression();
            if (sides == 0 || sides == 2)
            {
                aexp.TermA = GenerateRandomAndTree(sides == 2 ? nodes - 2 : nodes - 1);
            }
            if (sides == 1 || sides == 2)
            {
                aexp.TermB = GenerateRandomAndTree(sides == 2 ? nodes - 2 : nodes - 1);
            }
            return aexp;
        }
        static void AddRandomNots(ANDExpression aexp, double notp)
        {
            double y = _ent.NextDouble();
            if (y < notp)
            {
                ANDExpression e = aexp.TermA as ANDExpression;
                aexp.TermA = new NOTExpression();
                (aexp.TermA as NOTExpression).Term = e;
                if (e != null)
                {
                    AddRandomNots(e, notp);
                }
            }
            y = _ent.NextDouble();
            if (y < notp)
            {
                ANDExpression e = aexp.TermB as ANDExpression;
                aexp.TermB = new NOTExpression();
                (aexp.TermB as NOTExpression).Term = e;
                if (e != null)
                {
                    AddRandomNots(e, notp);
                }
            }
        }
        static void AddRanomVariables(Expression e, VariableExpression[] vars)
        {
            if (e is ANDExpression)
            {
                ANDExpression aexp = e as ANDExpression;
                if (aexp.TermA == null)
                {
                    aexp.TermA = vars[_ent.Next(0, vars.Length)];
                }
                else
                {
                    AddRanomVariables(aexp.TermA, vars);
                }
                if (aexp.TermB == null)
                {
                    aexp.TermB = vars[_ent.Next(0, vars.Length)];
                }
                else
                {
                    AddRanomVariables(aexp.TermB, vars);
                }
            }
            if (e is NOTExpression)
            {
                NOTExpression nexp = e as NOTExpression;
                if (nexp.Term == null)
                {
                    nexp.Term = vars[_ent.Next(0, vars.Length)];
                }
                else
                {
                    AddRanomVariables(nexp.Term, vars);
                }
            }
        }
    }
}
