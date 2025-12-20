using OpenTK.Audio.OpenAL;
using Prowl.Echo;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Audio;
using ToadEngine.Classes.Base.Objects.BuiltIn;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Objects.View;
using ToadEngine.Classes.Base.Objects.World;
using ToadEngine.Classes.Base.Physics.Managers;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;
using DirectionLight = ToadEngine.Classes.Base.Objects.Lights.DirectionLight;
using PointLight = ToadEngine.Classes.Base.Objects.Lights.PointLight;
using SpotLight = ToadEngine.Classes.Base.Objects.Lights.SpotLight;

namespace ToadEngine.Classes.Base.Rendering.SceneManagement;

public class SceneSettings
{
    public bool IsRunning;
}

public class Scene
{
    public Scene() { Init(); }

    [SerializeIgnore] public NativeWindow WHandler = null!;
    [SerializeIgnore] public Window.Window Window = null!;

    public AudioManager AudioManager = null!;
    public ObjectManager ObjectManager = new();

    public SceneSettings Settings = new();

    [SerializeIgnore] public IRenderTarget RenderTarget = null!;

    [SerializeIgnore] public Shader CoreShader;
    [SerializeIgnore] public Shader ShadowMapShader = null!;

    public AssetManager AssetManager = new();
    public ShaderManager ShaderManager = new();

    public GameObject Scripts = new() { Name = "Scripts" };

    public Skybox? Skybox;
    public DirectionLight? DirectionLight;

    [SerializeIgnore] public Action? PreUpdate, PostUpdate, PreRender, PostRender;

    public virtual void Setup() { }
    public virtual void OnStart() { }
    public virtual void OnDraw(float deltaTime) { }
    public virtual void OnUpdate(FrameEventArgs e) { }
    public virtual void OnLateUpdate(FrameEventArgs e) { }
    public virtual void Dispose() { }

    public void Instantiate(GameObject go, InstantiateType type = InstantiateType.Early) => ObjectManager.Instantiate(go, type);
    public void Instantiate(List<GameObject> gameObjects, InstantiateType type = InstantiateType.Early) => ObjectManager.Instantiate(gameObjects, type);

    public void DestroyObject(GameObject go, InstantiateType type = InstantiateType.Early) => ObjectManager.DestroyObject(go, type);
    public void DestroyObject(List<GameObject> gameObjects, InstantiateType type = InstantiateType.Early) => ObjectManager.DestroyObject(gameObjects, type);

    public void Start()
    {
        ObjectManager.SetupGameObjects();
        AddDefaults();
        OnStart();
    }

    private void AddDefaults()
    {
        BuiltIn.World.CreateMainCamera();

        var baseDirectory = $"{Directory.GetCurrentDirectory()}/Resources/";

        if (Skybox == null)
        {
            Skybox = BuiltIn.World.Skybox;
            Skybox.Material = new SkyboxMaterial()
            {
                Right = $"{baseDirectory}Textures/level_one_skybox/right.png",
                Left = $"{baseDirectory}Textures/level_one_skybox/left.png",
                Top = $"{baseDirectory}Textures/level_one_skybox/top.png",
                Bottom = $"{baseDirectory}Textures/level_one_skybox/bottom.png",
                Front = $"{baseDirectory}Textures/level_one_skybox/front.png",
                Back = $"{baseDirectory}Textures/level_one_skybox/back.png",
            };
        }

        if (DirectionLight == null)
        {
            DirectionLight = BuiltIn.Lights.DirectionLight;
            DirectionLight.Settings.Direction = new Vector3(0f, -1f, 0);
            DirectionLight.Transform.Rotation = new Vector3(-1f, -1.5f, -1f);

            DirectionLight.Settings.Specular = new Vector3(0.3f);
            DirectionLight.Settings.Ambient = new Vector3(0.5f);
            DirectionLight.Settings.Diffuse = new Vector3(0.3f);
        }

        Instantiate(Skybox.GameObject, InstantiateType.Late);
        Instantiate(DirectionLight.GameObject);
    }

    public void Draw(float deltaTime)
    {
        PreRender?.Invoke();
        DrawFirstPass(deltaTime);
        DrawSecondPass(deltaTime);
        PostRender?.Invoke();
    }

    private void DrawSecondPass(float deltaTime)
    {
        Service.CoreShader = CoreShader;
       
        RenderTarget.Bind();
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var textureIndex = 10;
        foreach (var light in ObjectManager.GameObjects.Values.Where(l => l.GetComponent<Light>() != null))
        {
            var caster = light.GetComponent<ShadowCaster>();
            if (caster == null || !caster.IsCastingShadows) continue;

            if (textureIndex >= 31)
            {
                Console.WriteLine("Error ran out of space to store shadow maps");
                return;
            }

            var baseLight = light.GetComponent<Light>();
            switch (baseLight)
            {
                case DirectionLight dirLight:
                    CoreShader.SetMatrix4($"dirLight.fragPosLightSpace", caster.LightSpaceMatrix);
                    CoreShader.SetInt1($"dirLight.shadowMap", ++textureIndex);

                    GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
                    GL.BindTexture(TextureTarget.Texture2D, caster.ShadowMap);
                    break;
                case PointLight pointLight:
                    CoreShader.SetMatrix4($"pointLights[{pointLight.CurrentIndex}].fragPosLightSpace", caster.LightSpaceMatrix);
                    CoreShader.SetInt1($"pointLights[{pointLight.CurrentIndex}].shadowMap", ++textureIndex);

                    GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
                    GL.BindTexture(TextureTarget.Texture2D, caster.ShadowMap);
                    break;
                case SpotLight spotLight:
                    CoreShader.SetMatrix4($"spotLights[{spotLight.CurrentIndex}].fragPosLightSpace", caster.LightSpaceMatrix);
                    CoreShader.SetInt1($"spotLights[{spotLight.CurrentIndex}].shadowMap", ++textureIndex);

                    GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
                    GL.BindTexture(TextureTarget.Texture2D, caster.ShadowMap);
                    break;
            }
        }
        DrawScene(deltaTime);
        RenderTarget.Unbind();
    }

    private void DrawFirstPass(float deltaTime)
    {
        Service.CoreShader = ShadowMapShader;
        foreach (var light in ObjectManager.GameObjects.Values.Where(l => l.GetComponent<Light>() != null))
        {
            var caster = light.GetComponent<ShadowCaster>();
            if (caster == null || !caster.IsCastingShadows) continue;

            caster.ConfigureShaderAndMatrices();
            CoreShader.SetMatrix4("lightSpaceMatrix", caster.LightSpaceMatrix);

            GL.Viewport(0, 0, caster.ShadowWidth, caster.ShadowHeight);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, caster.CasterFBO);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            DrawScene(deltaTime);
        }
    }

    private void DrawScene(float deltaTime)
    {
        ObjectManager.DrawGameObjects();
        OnDraw(deltaTime);
    }

    public void Update(FrameEventArgs e)
    {
        Time.DeltaTime = (float)e.Time;

        PreUpdate?.Invoke();
        if (!Settings.IsRunning) return;

        Time.AccumulatedTime += (float)e.Time;
        while (Time.AccumulatedTime >= Time.FixedDeltaTime)
        {
            if (!Service.Physics.IsPhysicsPaused) 
                Service.Physics.Step(Time.FixedDeltaTime);
            
            ObjectManager.UpdateBehaviorsFixedTime();
            Time.AccumulatedTime -= Time.FixedDeltaTime;
        }

        OnUpdate(e);
        ObjectManager.UpdateGameObjects();
        ObjectManager.UpdateBehaviors();
        OnLateUpdate(e);
        PostUpdate?.Invoke();
    }

    public virtual void OnResize(FramebufferResizeEventArgs e)
    {
        ObjectManager.ResizeGameObjects(e);
        Camera.MainCamera.AspectRatio = (RenderTarget.Width / (float)RenderTarget.Height);
    }

    public void Load(NativeWindow state, Window.Window window, IRenderTarget? target)
    {
        CreateCoreShaders();

        Init();

        WHandler = state;
        Window = window;

        Service.Add(WHandler);
        Service.Add(Window);

        RenderTarget = target ?? new WindowRenderTarget();

        Setup();
        Start();

        ObjectManager.Instantiate(Scripts);
        GameObject.SetupTriggers();
    }

    private void Init()
    {
        Service.Add(AssetManager);
        Service.Add(ShaderManager);

        if (AudioManager == null)
            AudioManager = new AudioManager();

        AudioManager.SetDistanceModel(ALDistanceModel.InverseDistanceClamped);
        AudioManager.Init();
    }

    private void CreateCoreShaders()
    {
        CoreShader = ShaderManager.Add("CoreShader", $"core.vert", $"lighting.frag");
        CoreShader.Use();

        ShadowMapShader = ShaderManager.Add("ShadowMap", "shadowmap.vert", "shadowmap.frag");
        
        Service.Add(CoreShader);
    }

    public void Destroy()
    {
        GUI.GuiCallBack = null!;

        ObjectManager.Dispose();
        AudioManager?.Dispose();

        CoreShader?.Dispose();
        ShadowMapShader?.Dispose();

        ShaderManager.Reset();
        AssetManager.Reset();
        PhysicsManager.Reset();
        Texture.ClearTextures();

        Service.Clear();
        Dispose();
    }

    public EchoObject Serialized => Serializer.Serialize(this);
}
