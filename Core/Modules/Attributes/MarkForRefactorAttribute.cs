namespace Squiggles.Core.Attributes;

using System;

/// <summary>
/// An alternative to <see cref="ObsoleteAttribute"/> that doesn't produce errors. Used to mark things as planned for a refactor. Communicates through code how and why things are intended to be refactored.
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = true)]
public class MarkForRefactorAttribute : Attribute {

  public string PlannedRefactorName;
  public string ReasoningMessage;

  public MarkForRefactorAttribute(string refactorName, string reasoning = "") {
    PlannedRefactorName = refactorName;
    ReasoningMessage = reasoning;
  }

}
