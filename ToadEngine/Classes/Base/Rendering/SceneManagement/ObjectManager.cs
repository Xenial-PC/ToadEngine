using ToadEngine.Classes.Base.Rendering.Object;
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

    public GameObject? FindGameObject(string name) => GameObjects.GetValueOrDefault(name);

    public void Instantiate(GameObject gameObject, InstantiateType type = InstantiateType.Early)
    {
        gameObject.OnSetup();
        gameObject.Name ??= $"go_{_goIndex++}";

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
            gameObject.OnSetup();
            gameObject.Name ??= $"go_{_goIndex++}";

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
            gameObject.Dispose();
            return;
        }

        GameObjectsLast.Remove(gameObject.Name!);
        gameObject.Dispose();
    }

    public void DestroyObject(List<GameObject> gameObjects, InstantiateType type = InstantiateType.Early)
    {
        foreach (var gameObject in gameObjects)
        {
            if (type == InstantiateType.Early)
            {
                GameObjects.Remove(gameObject.Name!);
                gameObject.Dispose();
                continue;
            }

            GameObjectsLast.Remove(gameObject.Name!);
            gameObject.Dispose();
        }
    }

    public void SetupGameObjects()
    {
        foreach (var render in GameObjects)
        {
            render.Value.OnSetup();
            render.Value.UpdateWorldTransform();
        }

        foreach (var render in GameObjectsLast)
        {
            render.Value.OnSetup();
            render.Value.UpdateWorldTransform();
        }
    }

    public void DrawGameObjects(float deltaTime)
    {
        foreach (var render in GameObjects)
            render.Value.OnDraw(deltaTime);

        foreach (var render in GameObjectsLast)
            render.Value.OnDraw(deltaTime);
    }

    public void UpdateGameObjects(float deltaTime)
    {
        foreach (var render in GameObjects)
        {
            render.Value.OnUpdate(deltaTime);
            render.Value.UpdateWorldTransform();
            render.Value.UpdateBehaviours(deltaTime);
        }

        foreach (var render in GameObjectsLast)
        {
            render.Value.OnUpdate(deltaTime);
            render.Value.UpdateWorldTransform();
            render.Value.UpdateBehaviours(deltaTime);
        }
    }

    public void ResizeGameObjects(FramebufferResizeEventArgs e)
    {
        foreach (var render in GameObjects)
        {
            render.Value.OnResize(e);
            render.Value.ResizeBehaviours(e);
        }

        foreach (var render in GameObjectsLast)
        {
            render.Value.OnResize(e);
            render.Value.ResizeBehaviours(e);
        }
    }

    public void SetupBehaviors()
    {
        foreach (var render in GameObjects)
            render.Value.SetupBehaviours();

        foreach (var render in GameObjectsLast)
            render.Value.SetupBehaviours();
    }

    public void Dispose()
    {
        foreach (var renderObject in GameObjects)
        {
            renderObject.Value.CleanupBehaviours();
            renderObject.Value.Dispose();
            DestroyObject(renderObject.Value);
        }

        foreach (var renderObject in GameObjectsLast)
        {
            renderObject.Value.CleanupBehaviours();
            renderObject.Value.Dispose();
            DestroyObject(renderObject.Value);
        }

        Behavior.BodyToGameObject.Clear();
    }
}