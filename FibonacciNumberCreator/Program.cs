using System;
using System.Linq;
using System.Collections.Generic;

namespace FibonacciNumberCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            // x    = 0 1 2 3 4 5 6  7  8  9 10
            // f(x) = 0 1 1 2 3 5 8 13 21 34 55
            var fx = new [] { 0.0, 1.0, 1.0, 2.0, 3.0, 5.0, 8.0, 13.0, 21.0, 34.0, 55.0 };
            var gp = new GenericProgram(100,fx);
            foreach(var _ in Enumerable.Range(0,50000))
            {
               var val =  gp.Inherit();
            }
            var bestgene = gp.RefreshValues();
            Console.WriteLine("Polish Notation");
            Console.WriteLine(bestgene.Dump());
            foreach (var i in Enumerable.Range(0,11))
                Console.WriteLine($"Actual : f({i}) = {fx[i]} , Result : f({i}) = {bestgene.Calculate(i)}");
        }
    }

    public class Tree
    {
        public string Value{ get; private set; }

        public Tree[] Children{ get; private set; }
        
        /// <summary>
        ///     Output Formula to Polish Notation
        /// </summary>
        /// <returns></returns>
        public string ToFormula()
        {
            switch (Value[0]) 
            {
                case '+':
                    return $"{Children[0].ToFormula()} + {Children[1].ToFormula()}";
                case '-': 
                    return $"{Children[0].ToFormula()} - {Children[1].ToFormula()}";
                case '*': 
                    return $"{Children[0].ToFormula()} * {Children[1].ToFormula()}";
                case '/': 
                    return $"{Children[0].ToFormula()} / {Children[1].ToFormula()}";
                case '^': 
                    return $"{Children[0].ToFormula()} ^ {Children[1].ToFormula()}";
                case 'x': 
                    return $"x";
            }
            return Value;
        }

        /// <summary>
        ///     Output Polish Notation
        /// </summary>
        /// <returns></returns>
        public string Dump()
        {
            var result = Value;
            foreach (var child in Children)
            {
                result += " " + child.Dump();
            }
            return result;
        }

        public double Calculate(double x)
        {
            switch (Value[0]) 
            {
                case '+': return Children[0].Calculate(x) + Children[1].Calculate(x);
                case '-': return Children[0].Calculate(x) - Children[1].Calculate(x);
                case '*': return Children[0].Calculate(x) * Children[1].Calculate(x);
                case '/': return Children[0].Calculate(x) / Children[1].Calculate(x);
                case '^': return Math.Pow(Children[0].Calculate(x), Children[1].Calculate(x));
                case 'x': return x;
            }
            return double.Parse(Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static Tree Make(Random rand, int len)
        {
            var result = new Tree();
            switch(len >= 5 ? 6 : rand.Next(7))
            {
                case 0: 
                    result.Value = "+"; 
                    result.Children = new Tree[2];
                    break;
                case 1: 
                    result.Value = "-";
                    result.Children = new Tree[2];
                    break;
                case 2: 
                    result.Value = "*"; 
                    result.Children = new Tree[2]; 
                    break;
                case 3: 
                    result.Value = "/"; 
                    result.Children = new Tree[2];
                    break;
                case 4:
                    result.Value = "^"; 
                    result.Children = new Tree[2]; 
                    break;
                case 5: 
                    result.Value = "x";
                    result.Children = new Tree[0]; 
                    break;
                default: 
                    result.Value = ((int)(rand.Next(10) + 1)).ToString(); 
                    result.Children = new Tree[0];
                    break;
            }

            for (int i = 0; i < result.Children.Length; i++)
                result.Children[i] = Make(rand, len + 1);
            return result;
        }

        public int CountLeaves()
        {
            int result = 1;
            for (int i = 0; i < Children.Length; i++)
                result += Children[i].CountLeaves();
            return result;
        }
        
        public Tree Copy(int[] leaf, Tree other) 
        {
            leaf[0]--;
            if (leaf[0] == -1)
                return other.Copy(leaf, null);
            Tree result = new Tree();
            result.Value = Value;
            result.Children = new Tree[Children.Length];
            for (int i = 0; i < Children.Length; i++)
                result.Children[i] = Children[i].Copy(leaf, other);
            return result;
        }

        public Tree Select(int[] leaf) 
        {
            leaf[0]--;
            if (leaf[0] == -1)
                return this;
            for (int i = 0; i < Children.Length; i++) {
                Tree result = Children[i].Select(leaf);
                if ( result != null)
                    return result;
            }
            return null;
        }
    }

    /// <summary>
    ///     Generic program.
    /// </summary>
    public class GenericProgram
    {
        public Random Rand { get; private set; }

        public Tree[] Genes { get; private set; }

        public double[] Values { get; private set; }

        public Tree Bestgene { get; private set; }

        public double Bestvalue { get; private set; }

        public double[] Function{ get; private set; }

        public GenericProgram(int genesNum, double[] function)
        {
            Rand = new Random();
            Function = function;
            Genes = new Tree[genesNum];
            foreach (var i in Enumerable.Range(0, Genes.Length))
            {
                Genes[i] = Tree.Make(Rand, 0);
            }
            Values = new double[genesNum];
            Bestgene = null;
            Bestvalue = 0.0;
            RefreshValues();
        }

        /// <summary>
        /// Refreshs the values.
        /// </summary>
        /// <returns>The values.</returns>
        public Tree RefreshValues()
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                Values[i] = 0.0;
                for (int j = 0; j < Function.Length; j++)
                {
                    double v;
                    if (Genes[i].CountLeaves() >= 50)
                        v = 100000000.0;
                    else
                    {
                        v = Genes[i].Calculate((double)j) - Function[j];
                        v *= v;
                    }
                    if (Double.IsNaN(v))
                        v = 100000000.0;
                    Values[i] += v;
                }
                Values[i] = 1.0 / Values[i];
                if (Bestvalue < Values[i])
                {
                    Bestvalue = Values[i];
                    Bestgene = Genes[i];
                }
            }
            return Bestgene;
        }

        /// <summary>
        /// Gets the good gene.
        /// </summary>
        /// <returns>The good gene.</returns>
        public Tree GetGoodGene()
        {
            double totalvalue = 0.0;
            for (int i = 0; i < Genes.Length; i++)
                totalvalue += Values[i];
            var r = Rand.NextDouble() * totalvalue;
            int select = 0;
            while (r >= Values[select])
            {
                r -= Values[select];
                select++;
            }
            return Genes[select];
        }

        /// <summary>
        /// Inherit this instance.
        /// </summary>
        /// <returns>The inherit.</returns>
        public double Inherit()
        {
            Tree[] newgenes = new Tree[Genes.Length];
            for (int i = 0; i < Genes.Length; i++)
            {
                Tree a = GetGoodGene(), b;
                if (Rand.NextDouble() < 0.05)
                    b = Tree.Make(Rand, 0);
                else
                    b = GetGoodGene();
                if (a == b)
                    newgenes[i] = a;
                else
                {
                    int[] leafa = new int[] { Rand.Next(a.CountLeaves()) };
                    int[] leafb = new int[] { Rand.Next(b.CountLeaves()) };
                    newgenes[i] = a.Copy(leafa, b.Select(leafb));
                }
            }
            Genes = newgenes;
            RefreshValues();
            return Bestvalue;
        }
    }
}
