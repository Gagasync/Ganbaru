using System;
using System.Linq;

namespace Ganbaru;

class Program
{
    static void Main(string[] args)
    {
        var lexer = new Lexer(new SourceText("1+2.0class   1.2   interface"));
        var lexerQueue = new LexerLookAheadQueue(lexer);
        var tok = lexerQueue.EatToken();
        while (tok.Kind != SyntaxKind.EofToken)
        {
            if (tok.Leading.Any())
            {
                foreach (var leading in tok.Leading)
                {
                    Console.WriteLine($"Leading: {leading.Kind}");
                }
            }
            Console.WriteLine($"    {tok.Kind}, {tok.Value}");
            if (tok.Trailing.Any())
            {
                foreach (var trailing in tok.Trailing)
                {
                    Console.WriteLine($"Trailing: {trailing.Kind}");
                }
            }
            tok = lexerQueue.EatToken();
        }
    }
}
