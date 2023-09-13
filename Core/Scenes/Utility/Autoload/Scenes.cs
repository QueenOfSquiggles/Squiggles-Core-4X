using System.Threading.Tasks;
using Godot;
using queen.error;
using queen.extension;

public partial class Scenes : Node
{

    public static Scenes Instance => _Instance;
    private static Scenes _Instance;

    [Export] private bool _Testing = false;
    [Export] private AnimationPlayer _Anim;
    public override void _Ready()
    {
        if (!this.EnsureSingleton(ref _Instance)) { return; }
    }

    public static void LoadSceneAsync(string file_path, bool use_sub_threads = false) => Instance?._LoadSceneAsync(file_path, use_sub_threads);
    private async void _LoadSceneAsync(string file_path, bool use_sub_threads = false)
    {
        var err = ResourceLoader.LoadThreadedRequest(file_path, "PackedScene", use_sub_threads);
        if (err != Error.Ok)
        {
            Print.Error($"Failed to load the scene file at {file_path} with the threaded load");
            return;
        }

        if (_Testing) Print.Debug($"Starting load scene: {file_path}");

        await FadeOut();


        if (ResourceLoader.LoadThreadedGet(file_path) is not PackedScene scene)
        {
            Print.Error($"Failed to get scene resource. Path='{file_path}'");
            return;
        }

        _LoadSceneImmediate(scene);
    }

    public static void LoadSceneImmediate(PackedScene scene) => Instance?._LoadSceneImmediate(scene);
    private async void _LoadSceneImmediate(PackedScene scene)
    {
        if (scene is null) return;
        GetTree().ChangeSceneToPacked(scene);
        await FadeIn();
        if (_Testing) Print.Debug($"Finished loading scene: {scene.ResourcePath}");
    }

    private SignalAwaiter FadeOut()
    {
        _Anim.Play("FadeOut");
        return ToSignal(_Anim, "animation_finished");
    }
    private SignalAwaiter FadeIn()
    {
        _Anim.Play("FadeIn");
        return ToSignal(_Anim, "animation_finished");
    }

}
