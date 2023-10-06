namespace Squiggles.ThirdParty.DiscordRichPresence;

using System;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using Godot;
using Squiggles.Core.API;
using Squiggles.Core.Error;

/// <summary>
/// A node that can be added to your project's autoload nodes to handle a discord rich presence session
/// <para/>
/// Make sure you have "discord_rich_presence_app_id" in your "appconfig.json" that is the app id registed on <see href="https://discord.com/developers/applications">Discord's app management site</see>
/// </summary>
/// <remarks>
/// Would third party integrations be better implemented as a secondary addition? Like as a separate module that can be installed independantly of SC4X???
/// <para/>
/// Additionally, this is a non-exhaustive implementation of Discord Rich Presence's functions. There do exist more functions available that are not implemented here such as multiplayer party session joining invites and such. The `DiscordRpcClient` instance is exposed for the purposes of implementing those features yourself. Else improvements can be contributed to this as well.
/// <para/>
/// There are specifically no null checks for <see cref="Client"/> because it will be null if it is not initialized. Which means there's something wrong with the configuration.
/// </remarks>
public partial class DiscordSession : Node {

  #region Godot Node Connection
  /// <summary>
  /// Automatically initializes the session with the default app id key name. Add to the project's autoload nodes to ensure the presence is initialized on startup of the application.
  /// </summary>
  public override void _Ready() => InitSession("discord_rich_presence_app_id");
  public override void _PhysicsProcess(double delta) => Client?.Invoke();
  public override void _ExitTree() => Client?.Dispose();
  #endregion


  #region Discord static functionality
  public static event Action<object, ReadyMessage> DiscordOnReady;
  public static event Action<object, PresenceMessage> DiscordOnPresenceUpdate;
  public static DiscordRpcClient Client { get; private set; }

  /// <summary>
  /// Initializes the session with the given key name. Useful
  /// </summary>
  /// <param name="keyName"></param>
  public static void InitSession(string keyName) {
    var key = Auth.GetKey(keyName);
    if (key is null) {
      // if you're stubborn enough to use your own key name then just delete this print warning
      Print.Warn($"Failed to initialized discord rich presence with app id key name '{keyName}'. Make sure that key name is present in your appconfig.json file!");
      return;
    }
    Client = new(key.GetSecureKey()) {
      Logger = new ConsoleLogger(LogLevel.Warning)
    };
    Client.OnReady += (sender, e) => DiscordOnReady?.Invoke(sender, e);
    Client.OnPresenceUpdate += (sender, e) => DiscordOnPresenceUpdate?.Invoke(sender, e);
    if (!Client.Initialize()) {
      Print.Error("Failed to initialize DiscordRPC client connection");
      Client = null;
      return;
    }

    SetPresence(details: "Launching...");
    Print.Debug("Initialized discord rich presence");
  }

  /// <summary>
  /// Alters the existing presence text for any filled out values
  /// </summary>
  /// <param name="details">The 'main' details for the presence. Recommended to be a short sentence fragment</param>
  /// <param name="state">The 'state' of the game. In multiplayer competitive games this would be the game-mode the player is currently playing, and possibly the current score if space allows.</param>
  public static void UpdatePresenceText(string details = "", string state = "") {
    var presence = Client.CurrentPresence ?? new RichPresence();
    presence.Details = details != "" ? details : presence.Details;
    presence.State = state != "" ? state : presence.State;
    Client.SetPresence(presence);
  }

  /// <summary>
  /// Alters the existing presence assets for any filled out values
  /// </summary>
  /// <param name="largeIconID">the key name for the registered icon from the discord application manager web app. Used as the main icon of the rich presence</param>
  /// <param name="largeIconText">The text to show when the large icon is hovered</param>
  /// <param name="smallIconID">the key name for the registered icon from the discord application manager web app. Shown as a small icon in the bottom-right corner of the large icon.</param>
  /// <param name="smallIconText">The text to show when the small icon is hovered</param>
  public static void UpdatePresenceAssets(string largeIconID = "", string largeIconText = "", string smallIconID = "", string smallIconText = "") {
    var presence = Client.CurrentPresence;
    var assets = Client.CurrentPresence.Assets ?? new Assets();
    Client.SetPresence(presence.WithAssets(new Assets() {
      LargeImageKey = largeIconID != "" ? largeIconID : assets.LargeImageKey,
      LargeImageText = largeIconText != "" ? largeIconText : assets.LargeImageText,
      SmallImageKey = smallIconID != "" ? smallIconID : assets.SmallImageKey,
      SmallImageText = smallIconText != "" ? smallIconText : assets.SmallImageText,
    }));
  }

  /// <summary>
  /// Clears the existing presence in favour of the new set values
  /// </summary>
  /// <param name="details">The 'main' details for the presence. Recommended to be a short sentence fragment</param>
  /// <param name="state">The 'state' of the game. In multiplayer competitive games this would be the game-mode the player is currently playing, and possibly the current score if space allows.</param>
  /// <param name="largeIconID">the key name for the registered icon from the discord application manager web app. Used as the main icon of the rich presence</param>
  /// <param name="largeIconText">The text to show when the large icon is hovered</param>
  /// <param name="smallIconID">the key name for the registered icon from the discord application manager web app. Shown as a small icon in the bottom-right corner of the large icon.</param>
  /// <param name="smallIconText">The text to show when the small icon is hovered</param>
  public static void SetPresence(string details = "", string state = "", string largeIconID = "", string largeIconText = "", string smallIconID = "", string smallIconText = "") {
    var presence = new RichPresence {
      Details = details,
      State = state,
      Assets = new() {
        LargeImageKey = largeIconID,
        LargeImageText = largeIconText,
        SmallImageKey = smallIconID,
        SmallImageText = smallIconText
      }
    };
    Client.SetPresence(presence);
  }

  /// <summary>
  /// Sets the start time of the presence without altering anything else about the presence.
  /// </summary>
  /// <param name="dateTime">The exact time that the contextual event started. Shown as an upwards counting timer</param>
  public static void SetStartTime(DateTime dateTime) {
    var presence = Client.CurrentPresence?.WithTimestamps(new Timestamps() {
      Start = dateTime
    });
    if (presence is not null) {
      Client.SetPresence(presence);
    }
  }
  /// <summary>
  /// Sets the end time of the presence without altering anything else about the presence.
  /// </summary>
  /// <param name="dateTime">The exact time that the contextual event will end. Shown as a downwards counting timer</param>
  public static void SetEndTime(DateTime dateTime) {
    var presence = Client.CurrentPresence?.WithTimestamps(new Timestamps() {
      End = dateTime
    });
    if (presence is not null) {
      Client.SetPresence(presence);
    }
  }

  /// <summary>
  /// Removes all timestamps (start and end events) from the presence.
  /// </summary>
  public static void ClearTimestamps() => Client.SetPresence(Client.CurrentPresence.WithTimestamps(new Timestamps()));

  #endregion
}
