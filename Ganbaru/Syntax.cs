using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganbaru;

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
    MethodDeclaration,

    WhitespaceSyntaxTrivia,
    EndOfLineSyntaxTrivia,
    SingleLineCommentTrivia,
    MultiLineCommentTrivia,

    NumberLiteralToken,
}

public class SyntaxFacts
{
    public static bool TryGetKeywordKind(string text, out SyntaxKind kind)
    {
        switch (text)
        {
            case "public":
                kind = SyntaxKind.PublicKeyword;
                return true;
            case "private":
                kind = SyntaxKind.PrivateKeyword;
                return true;
            case "internal":
                kind = SyntaxKind.InternalKeyword;
                return true;
            case "protected":
                kind = SyntaxKind.ProtectedKeyword;
                return true;
            case "abstract":
                kind = SyntaxKind.AbstractKeyword;
                return true;
            case "static":
                kind = SyntaxKind.StaticKeyword;
                return true;
            case "virtual":
                kind = SyntaxKind.VirtualKeyword;
                return true;
            case "sealed":
                kind = SyntaxKind.SealedKeyword;
                return true;
            case "class":
                kind = SyntaxKind.ClassKeyword;
                return true;
            case "interface":
                kind = SyntaxKind.InterfaceKeyword;
                return true;
            case "void":
                kind = SyntaxKind.VoidKeyword;
                return true;
            case "int":
                kind = SyntaxKind.IntKeyword;
                return true;
            case "long":
                kind = SyntaxKind.LongKeyword;
                return true;
            case "string":
                kind = SyntaxKind.StringKeyword;
                return true;
            default:
                kind = SyntaxKind.BadToken;
                return false;
        }
    }
}

public class SyntaxTrivia
{
    private readonly SyntaxKind _kind;
    private readonly string _value;

    public SyntaxTrivia(SyntaxKind kind, string value)
    {
        _kind = kind;
        _value = value;
    }

    public SyntaxKind Kind => _kind;
    public string Value => _value;
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

public class SyntaxFactory
{
    public static Node ClassDeclaration(Node modifiers, Token classKeyword, Token identifier, Token leftBrace, Token rightBrace)
    {
        return new Node(SyntaxKind.ClassDeclaration, [
            new NodeOrToken(modifiers),
                new NodeOrToken(classKeyword),
                new NodeOrToken(identifier),
                new NodeOrToken(leftBrace),
                new NodeOrToken(rightBrace)
        ]);
    }

    public static Node MethodDeclaration(Node modifiers, Token returnType, Token identifier, Token leftParen, Token rightParen, Token leftBrace, Token rightBrace)
    {
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
