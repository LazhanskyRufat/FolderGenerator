using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CodeAnalyzer
{
    class Program
    {
        const string text = @"
#define UNITY_EDITOR            

using System;
#if UNITY_EDITOR
            using System.Collections;
#endif
            using System.Linq;
            using System.Text;

            namespace HelloWorld
            {
                class Program
                {
                    static void Main(string[] args)
                    {
                        Console.WriteLine(""Hello, World!"");
                    }
                }
            }";

        static void Main(string[] args)
        {
            var node = (CompilationUnitSyntax)CSharpSyntaxTree.ParseText(text).GetRoot();
            foreach(var usingExpression in node.Usings)
            {
                var text = usingExpression.GetText();
                string length = text.ToString();
                Console.WriteLine(usingExpression.Name.ToString());
            }

            foreach(var member in node.Members)
            {
                Console.WriteLine(member.ToString());
            }
        }
    }
}
