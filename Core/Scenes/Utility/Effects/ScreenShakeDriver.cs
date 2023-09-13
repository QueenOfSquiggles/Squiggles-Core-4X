using System;
using Godot;
using queen.data;
using queen.extension;

public partial class ScreenShakeDriver : Node
{
    [Export] private FastNoiseLite _Noise;

    private float _NoiseIndex = 0.0f;
    private float _ShakeStrength = 0.0f;
    private float _Speed = 0.0f;
    private float _Decay = 0.0f;
    [Export] private Camera3D camera;

    public override void _Ready()
    {
        if (_Noise is null) return;
        _Noise.Seed = new Random().Next();
    }

    public void ApplyShake(float speed, float strength, float decay_rate)
    {
        _ShakeStrength = strength;
        _Speed = speed;
        _Decay = decay_rate;
    }

    public override void _Process(double delta)
    {
        _ShakeStrength = Mathf.Lerp(_ShakeStrength, 0, _Decay * (float)delta);
        var off = GetCurrentNoise((float)delta);

        if (camera is null) return;
        if (camera is CameraBrain brain) brain.Offset = new Vector2(off.X, off.Y);
        else camera.Position = off;
    }

    private Vector3 GetCurrentNoise(float delta)
    {
        if (_Noise is null) return Vector3.Zero;
        _NoiseIndex += delta * _Speed;
        return new Vector3()
        {
            X = _Noise.GetNoise2D(1, _NoiseIndex) * _ShakeStrength,
            Y = _Noise.GetNoise2D(100, _NoiseIndex) * _ShakeStrength,
            // TODO is Z shake helpful? I feel like it might not be
            // Z = noise.GetNoise2D(1000, noise_index) * shake_strength,
        } * Effects.Instance.ScreenShakeStrength;
    }


}
