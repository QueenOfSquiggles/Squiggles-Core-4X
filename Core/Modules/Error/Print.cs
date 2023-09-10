using System;
using Godot;

namespace queen.error;

public static class Print
{

    public const int LOG_DEBUG = 0;
    public const int LOG_INFO = 1;
    public const int LOG_WARNING = 2;
    public const int LOG_ERROR = 3;

    private const string ERR_COLOUR = "red";
    private const string WARN_COLOUR = "yellow";
    private const string INFO_COLOUR = "cyan";
    private const string DEBUG_COLOUR = "lightgrey";

    public static int LogLevel { get; set; } =
#if DEBUG
    LOG_DEBUG;
#else
    LOG_WARNING;
#endif

    public static void Error(string msg)
    {
        DisplayMessage(new Msg(msg, false, true).SetLevel(LOG_ERROR).Color(ERR_COLOUR).Bold().SetType("ERROR"));
    }

    public static void Warn(string msg)
    {
        DisplayMessage(new Msg(msg, true, false).SetLevel(LOG_WARNING).Color(WARN_COLOUR).SetType("WARNING"));
    }
    public static void Info(string msg)
    {
        DisplayMessage(new Msg(msg).SetLevel(LOG_INFO).Color(INFO_COLOUR).Italics().SetType("INFO"));
    }

    public static void Debug(string msg)
    {
#if DEBUG
        // Nice little QOL so I don't forget my random print statements
        DisplayMessage(new Msg(msg).SetLevel(LOG_DEBUG).Color(DEBUG_COLOUR).Italics().SetType("DEBUG"));
#endif
    }

    private static void DisplayMessage(Msg message)
    {
        if (OS.HasFeature("editor")) DisplayEditor(message);
        else DisplayRelease(message);

        DisplayLogFile(message);
    }

    private static void DisplayRelease(Msg msg)
    {
        if (msg.LogLevel < LogLevel) return; // skip lower level messages for clarity. Defaults to WARNING in non-debug release
        if (msg.IsError) GD.PushError(msg.Text);
        else if (msg.IsWarning) GD.PushWarning(msg.Text);

        var raw = $"{msg.MsgType} ({msg.LogLevel}) {msg.Text}";
        if (OS.HasFeature("windows")) GD.Print(raw); // Rich Text doesn't work well in the windows terminal
        else GD.PrintRich(msg.WrapFormatting(raw));

    }

    private static void DisplayEditor(Msg msg)
    {
        if (msg.IsError) GD.PushError(msg.Text);
        else if (msg.IsWarning) GD.PushWarning(msg.Text);


        GD.PrintRich(msg.WrapFormatting($"{msg.MsgType}"));
    }

    private static void DisplayLogFile(Msg msg)
    {
        var _ = $"({DateTime.Now.ToLongTimeString()})[{msg.MsgType}] {msg.Text}";
        // TODO append to a log file
    }

}