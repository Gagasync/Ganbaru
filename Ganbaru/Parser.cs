using System;
using System.Collections.Generic;

namespace Ganbaru;

internal struct TokenInfo
{
    public SyntaxKind Kind;
    public string Value;
}

public class Lexer
{
    private readonly SourceText _source;

    public Lexer(SourceText source)
    {
        _source = source;
    }

    public Token Lex()
    {
        List<SyntaxTrivia> leading = [];
        List<SyntaxTrivia> trailing = [];

        LexTrivia(false, ref leading);
        var tokenInfo = LexToken();
        LexTrivia(true, ref trailing);

        return new Token(tokenInfo.Kind, tokenInfo.Value, leading, trailing);
    }

    private TokenInfo LexToken()
    {
        if (_source.IsEnd)
        {
            return new TokenInfo { Kind = SyntaxKind.EofToken, Value = string.Empty };
        }

        _source.Start();

        if (char.IsDigit(_source.Current))
        {
            return LexNumber();
        }

        if (char.IsLetter(_source.Current) || _source.Current == '_')
        {
            return LexIdentifier();
        }

        switch (_source.Current)
        {
            case '+':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.PlusToken, Value = "+" };
            case '-':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.MinusToken, Value = "-" };
            case '(':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.LParenToken, Value = "(" };
            case ')':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.RParenToken, Value = ")" };
            case '{':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.LBraceToken, Value = "{" };
            case '}':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.RBraceToken, Value = "}" };
            case '[':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.LBracketToken, Value = "[" };
            case ']':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.RBracketToken, Value = "]" };
            case ';':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.SemicolonToken, Value = ";" };
            case ':':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.ColonToken, Value = ":" };
            case ',':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.CommaToken, Value = "," };
            case '.':
                _source.Eat();
                return new TokenInfo { Kind = SyntaxKind.DotToken, Value = "." };
            default:
                _source.Eat();
                var text = _source.GetText();
                return new TokenInfo { Kind = SyntaxKind.BadToken, Value = text };
        }
    }

    private TokenInfo LexIdentifier()
    {
        while (char.IsLetterOrDigit(_source.Current) || _source.Current == '_')
        {
            _source.Eat();
        }
        var text = _source.GetText();

        if (SyntaxFacts.TryGetKeywordKind(text, out var kind))
        {
            return new TokenInfo { Kind = kind, Value = text };
        }

        return new TokenInfo { Kind = SyntaxKind.IdentifierToken, Value = text };
    }

    private TokenInfo LexNumber()
    {
        while (char.IsDigit(_source.Current))
        {
            _source.Eat();
        }
        if (_source.Current == '.' && char.IsDigit(_source.LA))
        {
            _source.Eat();
            while (char.IsDigit(_source.Current))
            {
                _source.Eat();
            }
        }
        return new TokenInfo { Kind = SyntaxKind.NumberLiteralToken, Value = _source.GetText() };
    }

    private void LexTrivia(bool isTrailing, ref List<SyntaxTrivia> trivias)
    {
        _source.Start();
        if (char.IsWhiteSpace(_source.Current))
        {

            while (char.IsWhiteSpace(_source.Current))
            {
                _source.Eat();
            }

            trivias.Add(new SyntaxTrivia(SyntaxKind.WhitespaceSyntaxTrivia, _source.GetText()));
        }
    }
}

public class LexerLookAheadQueue
{
    private readonly Lexer _lexer;
    private readonly List<Token> _tokens = new List<Token>();
    private int _position;

    public LexerLookAheadQueue(Lexer lexer)
    {
        _lexer = lexer;
    }

    public Token Current => Peek(0);
    public Token LA => Peek(1);

    private Token Peek(int n)
    {
        while (_position + n >= _tokens.Count)
        {
            _tokens.Add(_lexer.Lex());
        }
        return _tokens[_position + n];
    }

    internal Token EatToken()
    {
        var tok = Current;
        _position++;
        return tok;
    }
}

public class LanguageParser
{

    private readonly LexerLookAheadQueue _lexer;

    public Node ParseClassDeclaration()
    {
        var modifiers = ParseModifiers();
        var classKeyword = Match(SyntaxKind.ClassKeyword);
        var identifier = Match(SyntaxKind.IdentifierToken);
        var leftBrace = Match(SyntaxKind.LBraceToken);

        var rightBrace = Match(SyntaxKind.RBraceToken);

        return SyntaxFactory.ClassDeclaration(modifiers, classKeyword, identifier, leftBrace, rightBrace);
    }

    private Node ParseModifiers()
    {
        List<NodeOrToken> list = [];
        bool foundModifier = true;
        while (foundModifier)
        {
            switch (_lexer.Current.Kind)
            {
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.PrivateKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.AbstractKeyword:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.VirtualKeyword:
                case SyntaxKind.SealedKeyword:
                    list.Add(new NodeOrToken(_lexer.EatToken()));
                    foundModifier = true;
                    break;
                default:
                    foundModifier = false;
                    break;
            }
        }

        return new Node(SyntaxKind.ListKind, list);
    }

    private Token Match(SyntaxKind kind)
    {
        if (_lexer.Current.Kind == kind)
        {
            return _lexer.EatToken();
        }
        return new Token(kind, string.Empty);
    }
}
