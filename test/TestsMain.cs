namespace Squiggles.Test;

using Chickensoft.GoDotTest;
using Godot;
using Squiggles.Core.Error;

public class TestsMain : TestClass {
  public TestsMain(Node testScene) : base(testScene) { }

  [SetupAll] public void SetupAll() => Print.Debug("Testing suite setting up");
  [Setup] public void Setup() => Print.Debug("Setting up main testing class");
  [Test] public void Test() => Print.Debug("Test");
  [Cleanup] public void Cleanup() => Print.Debug("Cleaning up main testing class");
  [CleanupAll] public void CleanupAll() => Print.Debug("Testing suite cleaning up");
  [Failure] public void Failure() => Print.Error("Current Testing suite has failed!");

}
