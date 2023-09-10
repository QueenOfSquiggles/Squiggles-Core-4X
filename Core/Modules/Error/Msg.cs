using System;
using Godot;

namespace queen.error;

public class Msg
{
    public string Text { get; private set; } = "";
    public string ColourModifiers { get; private set; } = "{0}";
    public string MsgType { get; private set; } = "MISC";
    public int LogLevel { get; private set; } = 0;
    public bool IsWarning { get; private set; } = false;
    public bool IsError { get; private set; } = false;


    public Msg(string text, bool isWarn = false, bool isErr = false)
    {
        Text = text;
        IsWarning = isWarn;
        IsError = isErr;
    }

    public Msg SetLevel(int level)
    {
        LogLevel = level;
        return this;
    }

    public Msg SetType(string type)
    {
        MsgType = type;
        return this;
    }

    ///  See Godot.Colors for names (There's a ton of them)
    public Msg Color(string col)
    {
        ColourModifiers = $"[color={col}]{Text}[/color]";
        return this;
    }

    public Msg Bold()
    {
        ColourModifiers = $"[b]{ColourModifiers}[/b]";
        return this;
    }
    public Msg Italics()
    {
        ColourModifiers = $"[i]{ColourModifiers}[/i]";
        return this;
    }

    public Msg Underlined()
    {
        ColourModifiers = $"[u]{ColourModifiers}[/u]";
        return this;
    }

    public Msg Monospaced()
    {
        ColourModifiers = $"[code]{ColourModifiers}[/code]";
        return this;
    }

    public Msg BGColour(string colour)
    {
        ColourModifiers = $"[bgcolor={colour}]{ColourModifiers}[/bgcolor]";
        return this;
    }
    public Msg FGColour(string colour)
    {
        ColourModifiers = $"[fgcolor={colour}]{ColourModifiers}[/fgcolor]";
        return this;
    }
    public Msg AlignCenter()
    {
        ColourModifiers = $"[center]{ColourModifiers}[/center]";
        return this;
    }
    public Msg AlignRight()
    {
        ColourModifiers = $"[right]{ColourModifiers}[/right]";
        return this;
    }
    public Msg Indent()
    {
        ColourModifiers = $"[indent]{ColourModifiers}[/indent]";
        return this;
    }

    public string WrapFormatting(string input)
    {
        return string.Format(ColourModifiers, input);
    }


}