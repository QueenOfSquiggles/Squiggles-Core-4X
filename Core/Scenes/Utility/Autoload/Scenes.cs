using Godot;
using queen.error;
using queen.extension;

public partial class Scenes : Node
{

    public static Scenes Instance => _instance;
    private static Scenes _instance;

    [Export] private NodePath path_anim;
    [Export] private bool testing = false;
    private AnimationPlayer anim;
    public override void _Ready()
    {
        if (!this.EnsureSingleton(ref _instance)) return;
        this.GetNode(path_anim, out anim);
    }

    public static void LoadSceneAsync(string file_path, bool use_sub_threads = false) => Instance._LoadSceneAsync(file_path, use_sub_threads);
    private async void _LoadSceneAsync(string file_path, bool use_sub_threads = false)
    {
        var err = ResourceLoader.LoadThreadedRequest(file_path, "PackedScene", use_sub_threads);
        if (err != Error.Ok)
        {
            Print.Error($"Failed to load the scene file at {file_path} with the threaded load");
            return;
        }

        if (testing) Print.Debug($"Starting load scene: {file_path}");
        await FadeOut();

        if (ResourceLoader.LoadThreadedGet(file_path) is not PackedScene scene)
        {
            Print.Error($"Failed to get scene resource. Path='{file_path}'");
            return;
        }
        _LoadSceneImmediate(scene);
    }

    public static void LoadSceneImmediate(PackedScene scene) => Instance._LoadSceneImmediate(scene);
    private async void _LoadSceneImmediate(PackedScene scene)
    {
        if (scene == null) return;
        GetTree().ChangeSceneToPacked(scene);
        await FadeIn();
        if (testing) Print.Debug($"Finished loading scene: {scene.ResourcePath}");
    }

    private SignalAwaiter FadeOut()
    {
        anim.Play("FadeOut");
        return anim.WaitForCurrentAnimEnd();
    }
    private SignalAwaiter FadeIn()
    {
        anim.Play("FadeIn");
        return anim.WaitForCurrentAnimEnd();
    }

}
