namespace Ganbaru;

public class SourceText
{
    private readonly string _text;
    private int _currentLocation = 0;
    private int _start = 0;

    public SourceText(string text)
    {
        _text = text;
    }

    public char Current => Peek(0);
    public char LA => Peek(1);

    public char Peek(int n)
    {
        var index = _currentLocation + n;
        if (index >= _text.Length)
        {
            return '\0';
        }
        return _text[index];
    }

    public bool IsEnd => _currentLocation >= _text.Length;

    public int Length => _text.Length;

    public void Start()
    {
        _start = _currentLocation;
    }

    public void Eat()
    {
        _currentLocation++;
    }

    public string GetText()
    {
        return _text.Substring(_start, _currentLocation - _start);
    }
}
