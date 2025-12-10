using System.Reflection;
using ToadEngine.Classes.Base.Rendering.Object;

namespace ToadEngine.Classes.Base.Scripting.Base;

public class MethodRegistry
{
    public static void RegisterMethods(Behavior behavior)
    {
        
        behavior.AwakeMethod = RegisterMethod<Action>(paramLength: 0, "Awake", behavior);
        behavior.StartMethod = RegisterMethod<Action>(paramLength: 0, "Start", behavior);

        behavior.UpdateMethod = RegisterMethod<Action>(paramLength: 0, "Update", behavior);
        behavior.FixedUpdateMethod = RegisterMethod<Action>(paramLength: 0, "FixedUpdate", behavior);
        behavior.OnGuiMethod = RegisterMethod<Action>(paramLength: 0, "OnGUI", behavior);

        behavior.OnTriggerEnterMethod = RegisterMethod<Action<GameObject>>(paramLength: 1, "OnTriggerEnter", behavior);
        
        behavior.OnTriggerExitMethod = RegisterMethod<Action<GameObject>>(paramLength: 1, "OnTriggerExit", behavior);
        behavior.OnResizeMethod = RegisterMethod<Action<FramebufferResizeEventArgs>>(paramLength: 1, "OnResize", behavior);

        behavior.DisposeMethod = RegisterMethod<Action>(paramLength: 0, "Dispose", behavior);

        var actions = Service.Physics.Actions;
        actions.OnCollision += RegisterMethod<Action<GameObject>>(paramLength: 1, "OnCollision", behavior);

        actions.OnPreStep += RegisterMethod<Action<float>>(paramLength: 1, "OnPhysicsPreStep", behavior);
        actions.OnPostStep += RegisterMethod<Action<float>>(paramLength: 1, "OnPhysicsPostStep", behavior);
    }

    /*private static T? RegisterMethod<T>(int paramLength, string method, Behavior behavior) where T : class
    {
        var type = behavior.GetType();
        var action = type.GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (action == null || action.GetParameters().Length != paramLength) return null;
        return (Delegate.CreateDelegate(typeof(T), behavior, action) as T);
    }*/

    private static T? RegisterMethod<T>(int paramLength, string method, Behavior behavior) where T : class
    {
        var type = behavior.GetType();
        var paramTypes = typeof(T).GetMethod("Invoke")!.GetParameters().Select(p => p.ParameterType).ToArray();
        var action = type.GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, paramTypes, null);
        if (action == null || action.GetParameters().Length != paramLength) return null;
        return (Delegate.CreateDelegate(typeof(T), behavior, action) as T);
    }
}
