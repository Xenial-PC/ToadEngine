using OpenTK.Audio.OpenAL;
using System.Reflection.PortableExecutable;
using ToadEngine.Classes.Base.Audio;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Physics;
using ToadEngine.Classes.Shaders;
using static ToadEngine.Classes.Base.Rendering.RenderObject;

namespace ToadEngine.Classes.Base.Rendering;

public class Scene
{
    public Dictionary<string, GameObject> RenderGameObjects { get; set; } = new();
    public Dictionary<string, GameObject> RenderGameObjectsLast { get; set; } = new();

    public NativeWindow WHandler;
    public Window.Window Window;
    public PhysicsManager PhysicsManager = new();

    private float _accumulator;
    private const float FixedDelta = 1f / 60f;

    public GameObject Scripts = new("Scripts");

    public AudioManager AudioManager = null!;

    public static Shader ShadowMapShader = null!;
    
    public enum InstantiateType
    {
        Early,
        Late
    }

    public virtual void Setup() { }
    public virtual void OnStart() { }
    public virtual void OnDraw(float deltaTime) { }
    public virtual void OnUpdate(FrameEventArgs e) { }
    public virtual void OnLateUpdate(FrameEventArgs e) { }
    public virtual void Dispose() { }

    public Scene(string sceneName)
    {
        if (RenderObject.Scenes.ContainsValue(this)) return;
        RenderObject.Scenes.Add(sceneName, this);

        AudioManager = new AudioManager();
        AudioManager.SetDistanceModel(ALDistanceModel.InverseDistanceClamped);

        AudioManager.Init();
    }

    public void Instantiate(GameObject gameObject, InstantiateType type = InstantiateType.Early)
    {
        gameObject.OnSetup();

        if (type == InstantiateType.Early)
        {
            RenderGameObjects.TryAdd(gameObject.Name, gameObject);
            return;
        }

        RenderGameObjectsLast.TryAdd(gameObject.Name, gameObject);
    }

    public void Instantiate(List<GameObject> gameObjects, InstantiateType type = InstantiateType.Early)
    {
        foreach (var gameObject in gameObjects)
        {
            gameObject.OnSetup();

            if (type == InstantiateType.Early)
            {
                RenderGameObjects.TryAdd(gameObject.Name, gameObject);
                continue;
            }

            RenderGameObjectsLast.TryAdd(gameObject.Name, gameObject);
        }
    }

    public void DestroyObject(GameObject? gameObject, InstantiateType type = InstantiateType.Early)
    {
        if (gameObject == null) return;

        if (type == InstantiateType.Early)
        {
            RenderGameObjects.Remove(gameObject.Name);
            gameObject.Dispose();
            return;
        }

        RenderGameObjectsLast.Remove(gameObject.Name);
        gameObject.Dispose();
    }

    public void DestroyObject(List<GameObject> gameObjects, InstantiateType type = InstantiateType.Early)
    {
        foreach (var gameObject in gameObjects)
        {
            if (type == InstantiateType.Early)
            {
                RenderGameObjects.Remove(gameObject.Name);
                gameObject.Dispose();
                continue;
            }

            RenderGameObjectsLast.Remove(gameObject.Name);
            gameObject.Dispose();
        }
    }

    public GameObject? FindGameObject(string name)
    {
        return RenderGameObjects.GetValueOrDefault(name);
    }

    public void Start()
    {
        foreach (var render in RenderGameObjects)
        {
            render.Value.OnSetup();
            render.Value.UpdateWorldTransform();
        }

        foreach (var render in RenderGameObjectsLast)
        {
            render.Value.OnSetup();
            render.Value.UpdateWorldTransform();
        }
        OnStart();
    }

    public void Draw(float deltaTime)
    {
        SetCoreShader(ShadowMapShader);
        foreach (var light in RenderGameObjects.Values.Where(l => l is DirectionLight))
        {
            var caster = light.GetComponent<ShadowCaster>();
            if (caster == null || !caster.IsCastingShadows) continue;

            caster.ConfigureShaderAndMatrices();
            GetCoreShader().SetMatrix4("lightSpaceMatrix", caster.LightSpaceMatrix);

            GL.Viewport(0, 0, caster.ShadowWidth, caster.ShadowHeight);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, caster.CasterFBO);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            DrawScene(deltaTime);
        }

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        SetCoreShader(Window.CoreShader);

        var textureIndex = 10;
        foreach (var light in RenderGameObjects.Values.Where(l => l is DirectionLight))
        {
            GL.Viewport(0, 0, Window.Width, Window.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var caster = light.GetComponent<ShadowCaster>();
            if (caster == null || !caster.IsCastingShadows) continue;

            if (textureIndex >= 31)
            {
                Console.WriteLine("Error ran out of space to store shadow maps");
                return;
            }

            switch (light)
            {
                case DirectionLight dirLight:
                    GetCoreShader().SetMatrix4($"dirLight.fragPosLightSpace", caster.LightSpaceMatrix);
                    GetCoreShader().SetInt1($"dirLight.shadowMap", ++textureIndex);
                    
                    GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
                    GL.BindTexture(TextureTarget.Texture2D, caster.ShadowMap);
                    continue;
                case PointLight pointLight:
                    GetCoreShader().SetMatrix4($"pointLights[{pointLight.CurrentIndex}].fragPosLightSpace", caster.LightSpaceMatrix);
                    GetCoreShader().SetInt1($"pointLights[{pointLight.CurrentIndex}].shadowMap", ++textureIndex);

                    GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
                    GL.BindTexture(TextureTarget.Texture2D, caster.ShadowMap);
                    continue;
                case SpotLight spotLight:
                    GetCoreShader().SetMatrix4($"spotLights[{spotLight.CurrentIndex}].fragPosLightSpace", caster.LightSpaceMatrix);
                    GetCoreShader().SetInt1($"spotLights[{spotLight.CurrentIndex}].shadowMap", ++textureIndex);

                    GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
                    GL.BindTexture(TextureTarget.Texture2D, caster.ShadowMap);
                    continue;
            }
        }
        DrawScene(deltaTime);
    }

    private void SetCoreShader(Shader shader)
    {
        RemoveService(shader!);
        AddService(shader);
        shader.Use();
    }

    private void DrawScene(float deltaTime)
    {
        foreach (var render in RenderGameObjects)
            render.Value.OnDraw(deltaTime);

        foreach (var render in RenderGameObjectsLast)
            render.Value.OnDraw(deltaTime);

        OnDraw(deltaTime);
    }

    public void Update(FrameEventArgs e)
    {
        _accumulator += (float)e.Time;
        while (_accumulator >= FixedDelta)
        {
            if (!GetCurrentScene().PhysicsManager.IsPhysicsPaused)
                GetCurrentScene().PhysicsManager.Step(FixedDelta);
            _accumulator -= FixedDelta;
        }

        OnUpdate(e);
        foreach (var render in RenderGameObjects)
        {
            render.Value.OnUpdate((float)e.Time);
            render.Value.UpdateWorldTransform();
            render.Value.UpdateBehaviours((float)e.Time);
        }

        foreach (var render in RenderGameObjectsLast)
        {
            render.Value.OnUpdate((float)e.Time);
            render.Value.UpdateWorldTransform();
            render.Value.UpdateBehaviours((float)e.Time);
        }
        OnLateUpdate(e);
    }

    public virtual void OnResize(FramebufferResizeEventArgs e)
    {
        foreach (var render in RenderGameObjects)
            render.Value.OnResize(e);

        foreach (var render in RenderGameObjectsLast)
            render.Value.OnResize(e);
    }

    public void Load(NativeWindow state, Window.Window window)
    {
        WHandler = state;
        Window = window;
        
        AddService(WHandler);
        AddService(Window);

        ShadowMapShader = new Shader("shadowmap.vert", "shadowmap.frag");

        Setup();
        Start();

        Instantiate(Scripts);
        SetupBehaviours();
    }

    private void SetupBehaviours()
    {
        foreach (var render in RenderGameObjects)
            render.Value.SetupBehaviours();

        foreach (var render in RenderGameObjectsLast)
            render.Value.SetupBehaviours();
    }

    public void Destroy()
    {
        GUI.GuiCallBack = null!;

        foreach (var renderObject in RenderGameObjects)
            renderObject.Value.Dispose();

        foreach (var renderObject in RenderGameObjectsLast)
            renderObject.Value.Dispose();

        RenderGameObjects?.Clear();
        RenderGameObjectsLast?.Clear();
        AudioManager?.Dispose();
        ShadowMapShader?.Dispose();
        Dispose();
    }
}
