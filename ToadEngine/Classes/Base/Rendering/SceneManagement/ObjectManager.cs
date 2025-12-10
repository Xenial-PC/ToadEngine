using System.Reflection.PortableExecutable;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;
namespace ToadEngine.Classes.Base.Rendering.SceneManagement;

public enum InstantiateType
{
    Early,
    Late
}

public class ObjectManager
{
    public Dictionary<string, GameObject> GameObjects { get; set; } = new();
    public Dictionary<string, GameObject> GameObjectsLast { get; set; } = new();

    private int _goIndex;

    public GameObject? FindGameObject(string name) =>
        GameObjects.GetValueOrDefault(name) ?? GameObjectsLast.GetValueOrDefault(name);

    public void Instantiate(GameObject gameObject, InstantiateType type = InstantiateType.Early)
    {
        gameObject.Setup();
        gameObject.Name ??= $"go_{_goIndex++}";

        FinalizeBehaviors(gameObject);
        if (type == InstantiateType.Early)
        {
            GameObjects.TryAdd(gameObject.Name, gameObject);
            return;
        }

        GameObjectsLast.TryAdd(gameObject.Name, gameObject);
    }

    public void Instantiate(List<GameObject> gameObjects, InstantiateType type = InstantiateType.Early)
    {
        foreach (var gameObject in gameObjects)
        {
            gameObject.Setup();
            gameObject.Name ??= $"go_{_goIndex++}";

            FinalizeBehaviors(gameObject);
            if (type == InstantiateType.Early)
            {
                GameObjects.TryAdd(gameObject.Name, gameObject);
                continue;
            }

            GameObjectsLast.TryAdd(gameObject.Name, gameObject);
        }
    }

    public void DestroyObject(GameObject? gameObject, InstantiateType type = InstantiateType.Early)
    {
        if (gameObject == null) return;

        if (type == InstantiateType.Early)
        {
            GameObjects.Remove(gameObject.Name!);
            return;
        }

        GameObjectsLast.Remove(gameObject.Name!);
    }

    public void DestroyObject(List<GameObject> gameObjects, InstantiateType type = InstantiateType.Early)
    {
        foreach (var gameObject in gameObjects)
        {
            if (type == InstantiateType.Early)
            {
                GameObjects.Remove(gameObject.Name!);
                continue;
            }

            GameObjectsLast.Remove(gameObject.Name!);
        }
    }

    public void SetupGameObjects()
    {
        foreach (var render in GameObjects)
        {
            render.Value.UpdateWorldTransform();
            render.Value.Renderers.ForEach(r => r.Setup());
        }

        foreach (var render in GameObjectsLast)
        {
            render.Value.UpdateWorldTransform();
            render.Value.Renderers.ForEach(r => r.Setup());
        }
    }

    public void DrawGameObjects()
    {
        var coreShader = Service.CoreShader;
        var camera = Service.MainCamera;

        coreShader.SetMatrix4("view", camera.GetViewMatrix());
        coreShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        coreShader.SetVector3("viewPos", camera.Transform.LocalPosition);

        foreach (var render in GameObjects)
            render.Value.Renderers.ForEach(r => r.Draw());

        foreach (var render in GameObjectsLast)
            render.Value.Renderers.ForEach(r => r.Draw());
    }

    public void UpdateGameObjects()
    {
        foreach (var render in GameObjects)
        {
            render.Value.UpdateWorldTransform();
            render.Value.UpdateBehaviors();
        }

        foreach (var render in GameObjectsLast)
        {
            render.Value.UpdateWorldTransform();
            render.Value.UpdateBehaviors();
        }
    }

    public void UpdateBehaviorsFixedTime()
    {
        foreach (var render in GameObjects)
        {
            render.Value.UpdateWorldTransform();
            render.Value.UpdateBehaviorsFixedTime();
        }
        
        foreach (var render in GameObjectsLast)
        {
            render.Value.UpdateWorldTransform();
            render.Value.UpdateBehaviorsFixedTime();
        }
    }

    public void ResizeGameObjects(FramebufferResizeEventArgs e)
    {
        foreach (var render in GameObjects)
        {
            render.Value.Renderers.ForEach(r => r.Resize(e));
            render.Value.ResizeBehaviors(e);
        }

        foreach (var render in GameObjectsLast)
        {
            render.Value.Renderers.ForEach(r => r.Resize(e));
            render.Value.ResizeBehaviors(e);
        }
    }

    public void FinalizeBehaviors(GameObject go) => go.Component.FinalizeComponents();
   
    public void Dispose()
    {
        foreach (var renderObject in GameObjects)
        {
            renderObject.Value.CleanupBehaviors();
            DestroyObject(renderObject.Value);
        }

        foreach (var renderObject in GameObjectsLast)
        {
            renderObject.Value.CleanupBehaviors();
            DestroyObject(renderObject.Value);
        }

        Behavior.BodyToGameObject.Clear();
    }
}