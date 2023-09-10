using System;
using Godot;
using queen.data;
using queen.extension;

public partial class ScreenShakeDriver : Node
{
    [Export] private FastNoiseLite noise;
    [Export] private NodePath path_target_camera = "..";

    private float noise_index = 0.0f;
    private float shake_strength = 0.0f;
    private float speed = 0.0f;
    private float decay = 0.0f;
    private Camera3D camera;

    public override void _Ready()
    {
        this.GetNode(path_target_camera, out camera);
        noise.Seed = new Random().Next();
    }

    public void ApplyShake(float speed, float strength, float decay_rate)
    {
        shake_strength = strength;
        this.speed = speed;
        decay = decay_rate;
    }

    public override void _Process(double delta)
    {
        shake_strength = Mathf.Lerp(shake_strength, 0, decay * (float)delta);
        var off = GetCurrentNoise((float)delta);
 
        if (camera is CameraBrain brain) brain.Offset = new Vector2(off.X, off.Y);
        else camera.Position = off;
    }

    private Vector3 GetCurrentNoise(float delta)
    {
        noise_index += delta * speed;
        return new Vector3()
        {
            X = noise.GetNoise2D(1, noise_index) * shake_strength,
            Y = noise.GetNoise2D(100, noise_index) * shake_strength,
            // TODO is Z shake helpful? I feel like it might not be
            // Z = noise.GetNoise2D(1000, noise_index) * shake_strength, 
        } * Effects.Instance.ScreenShakeStrength;
    }


}
