using Prowl.Echo;
using ToadEditor.Classes.Base;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;
using Window = ToadEngine.Classes.Window.Window;

namespace ToadEditor.Classes.EditorCore.Modules;

public class EditorStateManager
{
    public static EchoObject BackedUpScene;
    public static Window Window;
    public static NativeWindow WHandler;

    public static void InvokePlayState()
    {
        EditorRuntimeSettings.IsPlaying = !EditorRuntimeSettings.IsPlaying;
        if (EditorRuntimeSettings.IsPlaying)
        {
            BackedUpScene = Service.Scene.Serialized;
            File.WriteAllText($"{Directory.GetCurrentDirectory()}/Scene.txt", BackedUpScene.WriteToString());
            Service.Scene.Settings.IsRunning = EditorRuntimeSettings.IsPlaying;
            return;
        }

        var scene = Serializer.Deserialize<Scene>(BackedUpScene);
        
        Service.Window.LoadScene(scene);
        HookManager.SetupHooks();
    }
}
