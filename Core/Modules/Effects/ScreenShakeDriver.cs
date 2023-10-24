namespace Squiggles.Core.Scenes.Utility;

using System;
using Godot;
using Squiggles.Core.Data;
using Squiggles.Core.Scenes.Utility.Camera;

/// <summary>
/// a generic driver for applying screen shake to a camera. Works best with <see cref="CameraBrain"/> but some basic support for any Camera3D is implemented.
/// </summary>
[GlobalClass]
public partial class ScreenShakeDriver : Node {
  /// <summary>
  /// The noise function to use for calculating screen shake. Probably will require some tweaking if you want something non-default
  /// </summary>
  [Export] private FastNoiseLite _noise = new();

  /// <summary>
  /// A reference to the target camera
  /// </summary>
  [Export] private Camera3D _camera;

  private float _noiseIndex;
  private float _shakeStrength;
  private float _speed;
  private float _decay;

  public override void _Ready() {
    if (_noise is null) {
      return;
    }
    _noise.Seed = new Random().Next();
    Effects.RequestScreenShake += ApplyShake;
  }
  public override void _ExitTree() => Effects.RequestScreenShake -= ApplyShake;

  /// <summary>
  /// Applies the shake desired
  /// </summary>
  /// <param name="speed">how fast to move side to side</param>
  /// <param name="strength">how far to move from side to side</param>
  /// <param name="decay_rate">how quickly the effect should "decay" (decrease in intensity)</param>
  public void ApplyShake(float speed, float strength, float decay_rate) {
    _shakeStrength = strength;
    _speed = speed;
    _decay = decay_rate;
  }

  public override void _Process(double delta) {
    _shakeStrength = Mathf.Lerp(_shakeStrength, 0, _decay * (float)delta);
    var off = GetCurrentNoise((float)delta);

    switch (_camera) {
      case null:
        return;
      case CameraBrain brain:
        brain.Offset = new Vector2(off.X, off.Y);
        break;
      default:
        _camera.Position = off;
        break;
    }
  }

  private Vector3 GetCurrentNoise(float delta) {
    if (_noise is null) {
      return Vector3.Zero;
    }

    _noiseIndex += delta * _speed;
    return new Vector3() {
      X = _noise.GetNoise2D(1, _noiseIndex) * _shakeStrength,
      Y = _noise.GetNoise2D(100, _noiseIndex) * _shakeStrength,
      // TODO: is Z shake helpful? I feel like it might not be
      // Z = noise.GetNoise2D(1000, noise_index) * shake_strength,
    } * Effects.ScreenShakeStrength;
  }


}
