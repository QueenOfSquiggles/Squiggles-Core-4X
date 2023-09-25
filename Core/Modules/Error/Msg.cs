namespace Squiggles.Core.Error;

/// <summary>
/// A builder style class for organizing meaningful message data including formatting.
/// Builder style means that most/all methods will return the calling instance, allowing for chaining calls to concisely construct a meaningful chunk of data. In this case the data is a message.
/// </summary>
public class Msg {
  /// <summary>
  /// The text of this message
  /// </summary>
  public string Text { get; private set; } = "";

  /// <summary>
  /// The calling class which pushed this message out. Useful for debugging and finding problem children
  /// </summary>
  public string ClassName { get; private set; } = "";
  /// <summary>
  /// A constructed formatting of BBCode used with Godot's <c>GD.PrintRich</c>.
  /// </summary>
  public string ColourModifiers { get; private set; } = " __MSG__ ";
  /// <summary>
  /// A generic string value for the category of the message. Usually populated by <see cref="Print"/> but when constructing a custom message, you may choose anything you like.
  /// </summary>
  public string MsgType { get; private set; } = "MISC";
  /// <summary>
  /// An integer log level. Determines whether or not the message will be printed to the console. (Does not affect log files)
  /// </summary>
  public int LogLevel { get; private set; }
  /// <summary>
  /// Whether or not this message is a warning. Meaning something's bad but still functional
  /// </summary>
  public bool IsWarning { get; private set; }
  /// <summary>
  /// Whether or not this message is an error. Meaning something is not completely functional. When things are seriously broken, throw an error instead because taht will cause a crash.
  /// </summary>
  public bool IsError { get; private set; }


  /// <summary>
  /// Constructs a new Msg (Message) with the given data
  /// </summary>
  /// <param name="text">the text of the message</param>
  /// <param name="className">the source class</param>
  /// <param name="isWarn">whether or not this is a warning</param>
  /// <param name="isErr">whether or not this is an error</param>
  public Msg(string text, string className, bool isWarn = false, bool isErr = false) {
    Text = text;
    IsWarning = isWarn;
    IsError = isErr;
    ClassName = className;
  }

  /// <summary>
  /// Change the log level of this message
  /// </summary>
  /// <param name="level"></param>
  /// <returns></returns>
  public Msg SetLevel(int level) {
    LogLevel = level;
    return this;
  }

  /// <summary>
  /// Change the type of this message
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public Msg SetType(string type) {
    MsgType = type;
    return this;
  }

  /// <summary>
  /// Formats the message with particular colour. It allows built-in colour names as well as 6 digit hexadecimal codes ("ffffff"="white")
  /// See Godot.Colors for names (There's a ton of them)
  /// </summary>
  /// <param name="col"></param>
  /// <returns></returns>
  public Msg Color(string col) {
    ColourModifiers = $"[color={col}]{ColourModifiers}[/color]";
    return this;
  }

  /// <summary>
  /// Marks the message as bold.
  /// </summary>
  /// <returns></returns>
  public Msg Bold() {
    ColourModifiers = $"[b]{ColourModifiers}[/b]";
    return this;
  }
  /// <summary>
  /// Marks this message to use italics.
  /// </summary>
  /// <returns></returns>
  public Msg Italics() {
    ColourModifiers = $"[i]{ColourModifiers}[/i]";
    return this;
  }

  /// <summary>
  /// Marks this message to be underlined
  /// </summary>
  /// <returns></returns>
  public Msg Underlined() {
    ColourModifiers = $"[u]{ColourModifiers}[/u]";
    return this;
  }

  /// <summary>
  /// Markes this message to be entirely monospaced (forces a "code" font)
  /// </summary>
  /// <returns></returns>
  public Msg Monospaced() {
    ColourModifiers = $"[code]{ColourModifiers}[/code]";
    return this;
  }

  /// <summary>
  /// Applied a background colour to the message. Helps with contrast and style.
  /// </summary>
  /// <param name="colour"></param>
  /// <returns></returns>
  public Msg BGColour(string colour) {
    ColourModifiers = $"[bgcolor={colour}]{ColourModifiers}[/bgcolor]";
    return this;
  }
  /// <summary>
  ///  Applied a foreground colour to the message.
  /// </summary>
  /// <param name="colour"></param>
  /// <returns></returns>
  public Msg FGColour(string colour) {
    ColourModifiers = $"[fgcolor={colour}]{ColourModifiers}[/fgcolor]";
    return this;
  }
  /// <summary>
  /// Sets the message to align center
  /// </summary>
  /// <returns></returns>
  public Msg AlignCenter() {
    ColourModifiers = $"[center]{ColourModifiers}[/center]";
    return this;
  }
  /// <summary>
  /// Sets the message to alighn right
  /// </summary>
  /// <returns></returns>
  public Msg AlignRight() {
    ColourModifiers = $"[right]{ColourModifiers}[/right]";
    return this;
  }
  /// <summary>
  /// Sets the message to alighn left
  /// </summary>
  /// <returns></returns>
  public Msg AlignLeft() {
    ColourModifiers = $"[left]{ColourModifiers}[/left]";
    return this;
  }

  /// <summary>
  /// Applies one level of indent to this message (allows repeat)
  /// </summary>
  /// <returns></returns>
  public Msg Indent() {
    ColourModifiers = $"[indent]{ColourModifiers}[/indent]";
    return this;
  }

  /// <summary>
  /// Wraps the stored formatting around the provided string. Generally used to format the text of this message.
  /// </summary>
  /// <param name="input"></param>
  /// <returns></returns>
  public string WrapFormatting(string input) => ColourModifiers.Replace("__MSG__", input);


}
