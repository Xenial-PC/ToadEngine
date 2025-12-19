using System.Reflection;
using ToadEngine.Classes.Base.Rendering.Object;

namespace ToadEngine.Classes.Base.Scripting.Base;

public class MethodRegistry
{
    public static void RegisterMethods(MonoBehavior monoBehavior)
    {
        monoBehavior.AwakeMethod = RegisterMethod<Action>(paramLength: 0, "Awake", monoBehavior);
        monoBehavior.StartMethod = RegisterMethod<Action>(paramLength: 0, "Start", monoBehavior);

        monoBehavior.UpdateMethod = RegisterMethod<Action>(paramLength: 0, "Update", monoBehavior);
        monoBehavior.FixedUpdateMethod = RegisterMethod<Action>(paramLength: 0, "FixedUpdate", monoBehavior);
        monoBehavior.OnGuiMethod = RegisterMethod<Action>(paramLength: 0, "OnGUI", monoBehavior);

        monoBehavior.OnTriggerEnterMethod = RegisterMethod<Action<GameObject>>(paramLength: 1, "OnTriggerEnter", monoBehavior);
        
        monoBehavior.OnTriggerExitMethod = RegisterMethod<Action<GameObject>>(paramLength: 1, "OnTriggerExit", monoBehavior);
        monoBehavior.OnResizeMethod = RegisterMethod<Action<FramebufferResizeEventArgs>>(paramLength: 1, "OnResize", monoBehavior);

        monoBehavior.DisposeMethod = RegisterMethod<Action>(paramLength: 0, "Dispose", monoBehavior);

        var actions = Service.Physics.Actions;
        actions.OnCollision += RegisterMethod<Action<GameObject>>(paramLength: 1, "OnCollision", monoBehavior);

        actions.OnPreStep += RegisterMethod<Action<float>>(paramLength: 1, "OnPhysicsPreStep", monoBehavior);
        actions.OnPostStep += RegisterMethod<Action<float>>(paramLength: 1, "OnPhysicsPostStep", monoBehavior);
    }

    private static T? RegisterMethod<T>(int paramLength, string method, MonoBehavior monoBehavior) where T : class
    {
        var type = monoBehavior.GetType();
        var paramTypes = typeof(T).GetMethod("Invoke")!.GetParameters().Select(p => p.ParameterType).ToArray();
        var action = type.GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, paramTypes, null);
        if (action == null || action.GetParameters().Length != paramLength) return null;
        return (Delegate.CreateDelegate(typeof(T), monoBehavior, action) as T);
    }
}
