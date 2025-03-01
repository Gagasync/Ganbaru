using System;
using System.Collections.Generic;
using System.Linq;
using Ganbaru.Text;

namespace Ganbaru.Syntax
{
    public enum SyntaxKind
    {
        BadToken,
        ListKind,
        EofToken,

        PlusToken,
        MinusToken,
        PublicKeyword,
        PrivateKeyword,
        InternalKeyword,
        ProtectedKeyword,
        AbstractKeyword,
        StaticKeyword,
        VirtualKeyword,
        SealedKeyword,
        ClassKeyword,
        InterfaceKeyword,
        VoidKeyword,
        IntKeyword,
        LongKeyword,
        StringKeyword,

        LParenToken,
        RParenToken,
        LBraceToken,
        RBraceToken,
        LBracketToken,
        RBracketToken,
        SemicolonToken,
        ColonToken,
        CommaToken,
        DotToken,
        ColonColonToken,
        IdentifierToken,
        ClassDeclaration,
        MethodDeclaration
    }



    public class SyntaxTrivia {

    }

    public class Token
    {
        private readonly SyntaxKind _kind;
        private readonly string _value;
        private readonly List<SyntaxTrivia> _leading;
        private readonly List<SyntaxTrivia> _trailing;

        public Token(SyntaxKind kind, string value)
        {
            _kind = kind;
            _value = value;
            _leading = new List<SyntaxTrivia>();
            _trailing = new List<SyntaxTrivia>();
        }
        public Token(SyntaxKind kind, string value, List<SyntaxTrivia> leading, List<SyntaxTrivia> trailing)
        {
            _kind = kind;
            _value = value;
            _leading = leading;
            _trailing = trailing;
        }

        public SyntaxKind Kind => _kind;

        public string Value => _value;

        public int Length => _value.Length;

        public List<SyntaxTrivia> Leading => _leading;

        public List<SyntaxTrivia> Trailing => _trailing;
    }

    public class Node
    {
        private readonly SyntaxKind _kind;
        private readonly List<NodeOrToken> _children;

        private readonly int _length;

        public Node(SyntaxKind kind, List<NodeOrToken> children)
        {
            _kind = kind;
            _children = children;
            _length = children.Select(x => x.Length).Sum();
        }

        public SyntaxKind Kind => _kind;

        public List<NodeOrToken> Children => _children;

        public int Length => _length;
    }

    public class NodeOrToken
    {
        private readonly Node? _node;
        private readonly Token? _token;

        public NodeOrToken(Node node)
        {
            _node = node;
            _token = null;
        }
        public NodeOrToken(Token token)
        {
            _node = null;
            _token = token;
        }

        public bool IsToken => _token != null;

        public bool IsNode => _node != null;

        public SyntaxKind Kind
        {
            get
            {
                if (_node != null)
                {
                    return _node.Kind;
                }
                return _token!.Kind;
            }
        }

        public int Length
        {
            get
            {
                if (_node != null)
                {
                    return _node.Length;
                }
                return _token!.Length;
            }
        }


        public Node GetRequiredNode()
        {
            if (_node == null)
            {
                throw new Exception("Node is null");
            }
            return _node;
        }

        public Token GetRequiredToken()
        {
            if (_token == null)
            {
                throw new Exception("Token is null");
            }
            return _token;
        }

        public Node? GetNode()
        {
            return _node;
        }

        public Token? GetToken()
        {
            return _token;
        }
    }

    public class SyntaxFactory {
        public static Node ClassDeclaration(Node modifiers, Token classKeyword, Token identifier, Token leftBrace, Token rightBrace) {
            return new Node(SyntaxKind.ClassDeclaration, [
                new NodeOrToken(modifiers),
                new NodeOrToken(classKeyword),
                new NodeOrToken(identifier),
                new NodeOrToken(leftBrace),
                new NodeOrToken(rightBrace)
            ]);
        }

        public static Node MethodDeclaration(Node modifiers, Token returnType, Token identifier, Token leftParen, Token rightParen, Token leftBrace, Token rightBrace) {
            return new Node(SyntaxKind.MethodDeclaration, [
                new NodeOrToken(modifiers),
                new NodeOrToken(returnType),
                new NodeOrToken(identifier),
                new NodeOrToken(leftParen),
                new NodeOrToken(rightParen),
                new NodeOrToken(leftBrace),
                new NodeOrToken(rightBrace)
            ]);
        }
    }
}

namespace Ganbaru.Text 
{
    public class SourceText
    {
        private readonly string _text;
        private int _currentLocation = 0;

        public SourceText(string text)
        {
            _text = text;
        }

        public char Current => Peek(0);
        public char LA => Peek(1);

        public char Peek(int n) {
            var index = _currentLocation + n;
            if (index >= _text.Length) {
                return '\0';
            }
            return _text[index];
        }

        public bool IsEnd() => _currentLocation >= _text.Length;

        public int Length()
        {
            return _text.Length;
        }
    }
}

namespace Ganbaru.Parser 
{
    using Ganbaru.Text;
    using Ganbaru.Syntax;

    internal struct TokenInfo {
        public SyntaxKind Kind;
        public string Value;
    }

    public class Lexer {
        private readonly SourceText _source;
    
        public Lexer(SourceText source)
        {
            _source = source;
        }

        public Token Next() {
            List<SyntaxTrivia> leading = new List<SyntaxTrivia>();
            List<SyntaxTrivia> trailing = new List<SyntaxTrivia>();

            LexTrivia(false, ref leading);
            var tokenInfo = LexToken();
            LexTrivia(true, ref trailing);

            return new Token(tokenInfo.Kind, tokenInfo.Value, leading, trailing);
        }

        private TokenInfo LexToken() {
            if (_source.IsEnd()) {
                return new TokenInfo { Kind = SyntaxKind.EofToken, Value = string.Empty };
            }

            throw new NotImplementedException();
        }

        private void LexTrivia(bool isTrailing, ref List<SyntaxTrivia> trivias) {

        }
    }

    public class LexerLookAheadQueue {
        private readonly Lexer _lexer;
        private readonly Token[] _tokens;
        private int _position;

        public Token Current {get;}
        public Token LA {get;}

        internal Token EatToken()
        {
            throw new NotImplementedException();
        }
    }

    public class LanguageParser {

        private readonly LexerLookAheadQueue _lexer;

        public Node ParseClassDeclaration() {
            var modifiers = ParseModifiers();
            var classKeyword = Match(SyntaxKind.ClassKeyword);
            var identifier = Match(SyntaxKind.IdentifierToken);
            var leftBrace = Match(SyntaxKind.LBraceToken);

            var rightBrace = Match(SyntaxKind.RBraceToken);

            return SyntaxFactory.ClassDeclaration(modifiers, classKeyword, identifier, leftBrace, rightBrace);
        }

        private Node ParseModifiers() {
            List<NodeOrToken> list = [];
            bool foundModifier = true;
            while(foundModifier)
            {
                switch(_lexer.Current.Kind) {
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

        private Token Match(SyntaxKind kind) {
            if (_lexer.Current.Kind == kind) {
                return _lexer.EatToken();
            }
            return new Token(kind, string.Empty);
        }
    }
}

namespace Ganbaru
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}