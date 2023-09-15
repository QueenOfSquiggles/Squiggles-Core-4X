namespace Squiggles.Core.Error;

using System;
using System.Diagnostics;
using Godot;

public static class Print {

  public const string LOG_FILE = "user://log.txt";

  public const int LOG_DEBUG = 0;
  public const int LOG_INFO = 1;
  public const int LOG_WARNING = 2;
  public const int LOG_ERROR = 3;

  private const string ERR_COLOUR = "red";
  private const string WARN_COLOUR = "yellow";
  private const string INFO_COLOUR = "cyan";
  private const string DEBUG_COLOUR = "gray";

  public static int LogLevel { get; set; } =
#if DEBUG
  LOG_DEBUG;
#else
    LOG_WARNING;
#endif


  // Class Name Acquisition (funnily enough, borrowing some styling from Unity)
  public static void Error<T>(string msg, T inst) => Error(msg, inst.GetType().FullName);

  public static void Warn<T>(string msg, T inst) => DisplayMessage(new Msg(msg, inst.GetType().FullName, true, false).SetLevel(LOG_WARNING).Color(WARN_COLOUR).SetType("WARNING"));
  public static void Info<T>(string msg, T inst) => DisplayMessage(new Msg(msg, inst.GetType().FullName).SetLevel(LOG_INFO).Color(INFO_COLOUR).Italics().SetType("INFO"));

  public static void Debug<T>(string msg, T inst) => Debug(msg, inst.GetType().FullName);


  // Formatting Series
  public static void Error(string msg, string className = "") => DisplayMessage(new Msg(msg, className, false, true).SetLevel(LOG_ERROR).Color(ERR_COLOUR).Bold().SetType("ERROR"));

  public static void Warn(string msg, string className = "") => DisplayMessage(new Msg(msg, className, true, false).SetLevel(LOG_WARNING).Color(WARN_COLOUR).SetType("WARNING"));
  public static void Info(string msg, string className = "") => DisplayMessage(new Msg(msg, className).SetLevel(LOG_INFO).Color(INFO_COLOUR).Italics().SetType("INFO"));

  public static void Debug(string msg, string className = "") =>
#if DEBUG
    // Nice little QOL so I don't forget my random print statements
    DisplayMessage(new Msg(msg, className).SetLevel(LOG_DEBUG).Color(DEBUG_COLOUR).Italics().SetType("DEBUG"));
#endif


  /// <summary>
  /// A generic method for displaying a `Msg` which is a builder for formatted messages. All other print calls redirect here while applying custom formatting. This is exposed to allow custom messaging to be written as well. Additionally, all calls to this method will be appended to the squiggles log file.
  /// </summary>
  /// <param name="msg">an instance of the Msg message builder</param>
  public static void DisplayMessage(Msg msg) {
    AppendToLogFile(msg);

    if (!OS.IsStdOutVerbose() && msg.LogLevel < LogLevel) {
      // skip lower level messages for clarity. Defaults to WARNING in non-debug release
      // In a "verbose" context, no messages are skipped. Allowing for debugging a release version.
      return;
    }

    // TODO what formatting is desired for different contexts?
    var msgText = (OS.HasEnvironment("VS_DEBUG") || OS.HasFeature("editor")) ?
        $"{msg.MsgType}({msg.LogLevel}): {msg.WrapFormatting(msg.Text)}" // Debugging/Editor Context
      : $"{msg.MsgType}: {msg.Text}"; // Release Context, expect no formatting support

    // prepend date-time when run with "--verbose".
    msgText = OS.IsStdOutVerbose() ? $"({DateTime.Now.ToShortDateString()})({DateTime.Now.ToLongTimeString()})" + msgText : msgText;

    // append class name
    msgText = msg.ClassName.Length > 0 ? $"{msgText}\n\tClass={msg.ClassName}" : msgText;

    // append a stack trace with file and line numbers when printing an error or warning
    msgText = msg.IsError || msg.IsWarning ? msgText + "\n" + new StackTrace(true) : msgText;

    if (msg.IsError) {
      GD.PushError(msg.Text);
    }
    else if (msg.IsWarning) {
      GD.PushWarning(msg.Text);
    }

    GD.PrintRich(msgText);
  }

  private static void AppendToLogFile(Msg msg) {
    // TODO append to a log file
    using var logFile = FileAccess.Open(LOG_FILE, FileAccess.ModeFlags.ReadWrite);
    if (logFile is null) {
      using (var touch = FileAccess.Open(LOG_FILE, FileAccess.ModeFlags.Write)) {
        touch?.StoreLine("----------");
        touch?.StoreLine($"Logging Session | {DateTime.Now}");
        touch?.StoreLine("----------");
      }
      AppendToLogFile(msg);
      return;
    }
    logFile?.SeekEnd();
    logFile?.StoreLine($"({DateTime.Now.ToShortDateString()} - {DateTime.Now.ToLongTimeString()})[{msg.MsgType}({msg.LogLevel})][{msg.ClassName}] {msg.Text}");
    if (msg.IsError || msg.IsWarning) {
      logFile?.StoreLine($"--- Stack Trace ---\n" + new StackTrace(true) + "\n --- End Trace ---");
    }
  }

  public static void ClearLogFile() {
    using var dir = DirAccess.Open(LOG_FILE.GetBaseDir());
    if (dir is null) { return; }
    var root = "user://squiggles-logs/";
    if (!DirAccess.DirExistsAbsolute(root)) {
      DirAccess.MakeDirRecursiveAbsolute(ProjectSettings.GlobalizePath(root));
    }
    dir.Rename(LOG_FILE, $"{root}log-archived-{DateTime.Now.ToFileTimeUtc()}.txt");
  }

  public static void AddSystemRedirect() {
    // TODO Figure out how to read unhandled errors and print statements so they can be passed into
    // Technically godot creates its own logging files. However, due to the custom formatting the files look super gross to read through.
    // At least on linux (unix?) you can do `cat godot.log | less` to read the log with formatting fairly easily.
  }

}
