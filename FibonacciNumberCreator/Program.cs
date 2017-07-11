using System;
using System.Collections.Generic;

namespace FibonacciNumberCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = Tree.Make(new Random(), 0);
            //変数
            var x = 1.0;
            Console.WriteLine("四則演算のルールを無視して左から順に計算します。");
            Console.WriteLine(tree.ToFormula());
            Console.WriteLine("= " + ((double)tree.Calculate(x)).ToString());
        }
    }

    public class Tree
    {
        public string Value{get;set;}

        public Tree[] Children{get;set;}
        
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
    }
}
