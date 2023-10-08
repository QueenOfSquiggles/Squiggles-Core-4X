# Installation Guide

This is a guide on how to install Squiggles Core 4X into your project. 

## Prerequisites

- Godot 4.1-Mono installed
- Your preferred dotnet version (6 or 7). Core is built for dotnet6 so this is recommended.
- A programming environment that is currently working with Godot 4.1 and C# scripting
- Fundamental understanding of computer systems

## Instructional Steps

You have options for your installation method. Installing from the respository guarantees that your installation will include the latest features and bug fixes, but requires extra effort to install. INstalling through the Godot Asset Library is significantly simpler, but can lag behind as updates must be made manually by the developer.

### Godot Asset Library Method

[Asset Library Page](https://godotengine.org/asset-library/asset/2142)

1. From inside of the Godot editor, click on the "AssetLib" tab
2. In the search bar enter "Squiggles Core 4X"
3. Find the "Squiggles Core 4X" entry by "QueenOfSquiggles" and click on it
4. Click on the download button and wait a few seconds
5. Click on the install button.
6. ***Disable*** all checkboxes except for:

- addons/ *(optional)*
- Core/

  > This means that those two folders are the only ones being installed. All else is extra tooling that is used for the development of this core system and is not necessary for you to create your game with.

7. Click install
8. Follow the common "Final Setup" steps to finish installation

### Repository Method

[Repository](https://github.com/QueenOfSquiggles/Squiggles-Core-4X)

> You may want to `git clone` the respository and symlink the directories. If you understand why that would be helpful, I wager you have enough experience to understand how to do that.

1. Click on the "<> Code" button
2. Click on the "Download ZIP" button and wait for your download to finish.
3. Open the zip archive file as well as your project folder
4. Copy over the folders into the root of your projects folder.
  - addons/ *(optional)*
  - Core/
5. Follow the common "Final Setup" steps to finish installation

### Final Setup
1. Open "Project"->"Project Settings" in the top left corner
2. Go to the Plugins tab and enable:
  - squiggles_core_autoloads
  - squigglesbt
3. A dialog window should appear with options to create relevant items for SC4X. Leave as default for the suggested installation.
  - If not, click on "Project"->"Tools"->"SC4X Setup Dialog" to open it any time
4. Click on the "Ok" button to apply configuration
5. Wait a few seconds (dotnet dependencies really slow things down)
6. (Optional) Click on "Project"->"Reload Current Project" to reload the project and ensure all files are properly imported.
7. Open "res://squiggles_config.tres" and fill out your desired properties (Create it if it was not created earlier). Ensure you create the resources for resource parameters (embedded is totally OK and even recommended)

### Translation/Internationalization

To ensure translation files are set up, go to "Project"->"Project Settings"->"Localization"->"Translations" and add "res://Core/Assets/Translation/translations_sheet.en.translation" (english translations)

To add translations for different languages, modify "res://Core/Assets/Translation/translations_sheet.csv" with new columns for the language of your choice. Refer to the [offical documentation](https://docs.godotengine.org/en/stable/tutorials/assets_pipeline/importing_translations.html#doc-importing-translations) for instructions. Refer to [this page](https://docs.godotengine.org/en/stable/tutorials/i18n/internationalizing_games.html) for further help providing translation for your game/project.


## Closing

As always, if you have issues or concerns don't hesitate to let me know!
