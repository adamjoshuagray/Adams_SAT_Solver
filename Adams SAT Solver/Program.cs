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
            Expression exp = a & b & c | d;
            //exp = GenerateRandomExpression(3, 70, 0.95);
            //Console.WriteLine(exp.Evaluate().ToString());
            VariableStateListCollection vslc = AGSAT.SAT(exp);
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
    }
}
