using System;
using System.Collections.Generic;
using System.Linq;
using Ganbaru.Text;

namespace Ganbaru.Syntax
{
    public enum SyntaxKind
    {
        BadToken,
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
        ColonColonToken
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

        public SyntaxKind Kind()
        {
            return _kind;
        }

        public string Value()
        {
            return _value;
        }

        public int Length()
        {
            return _value.Length;
        }

        public List<SyntaxTrivia> Leading()
        {
            return _leading;
        }

        public List<SyntaxTrivia> Trailing()
        {
            return _trailing;
        }
    }

    public class Node
    {
        private readonly SyntaxKind _kind;
        private readonly NodeOrToken[] _children;

        private readonly int _length;

        public Node(SyntaxKind kind, NodeOrToken[] children)
        {
            _kind = kind;
            _children = children;
            _length = children.Select(x => x.Length()).Sum();
        }

        public SyntaxKind Kind()
        {
            return _kind;
        }

        public NodeOrToken[] Children()
        {
            return _children;
        }

        public int Length()
        {
            return _length;
        }
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

        public bool IsToken()
        {
            return _token != null;
        }

        public bool IsNode()
        {
            return _node != null;
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

        public SyntaxKind Kind()
        {
            if (_node != null)
            {
                return _node.Kind();
            }
            return _token!.Kind();
        }

        public int Length()
        {
            if (_node != null)
            {
                return _node.Length();
            }
            return _token!.Length();
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

    }

    public class LanguageParser {

        private readonly Lexer _lexer;

        public Node ParseClassDeclaration() {
            throw new NotImplementedException();
        }

        private Node ParseModifiers() {
            throw new NotImplementedException();
            
        }

        private Token Match(SyntaxKind kind) {
            throw new NotImplementedException();
            
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