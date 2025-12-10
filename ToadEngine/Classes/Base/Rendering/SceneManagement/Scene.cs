using OpenTK.Audio.OpenAL;
using ToadEngine.Classes.Base.Assets;
using ToadEngine.Classes.Base.Audio;
using ToadEngine.Classes.Base.Objects.Lights;
using ToadEngine.Classes.Base.Physics.Managers;
using ToadEngine.Classes.Base.Rendering.Object;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Shaders;
using ToadEngine.Classes.Textures;

namespace ToadEngine.Classes.Base.Rendering.SceneManagement;

public class Scene
{
    public NativeWindow WHandler = null!;
    public Window.Window Window = null!;

    public AudioManager AudioManager = null!;
    public ObjectManager ObjectManager = new();

    public Shader CoreShader => Service.CoreShader;
    public static Shader ShadowMapShader = null!;

    public GameObject Scripts = new() { Name = "Scripts" };

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
        OnStart();
    }

    public void Draw(float deltaTime)
    {
        Service.CoreShader = ShadowMapShader;
        foreach (var light in ObjectManager.GameObjects.Values.Where(l => l is DirectionLight))
        {
            var caster = light.Component.Get<ShadowCaster>();
            if (caster == null || !caster.IsCastingShadows) continue;

            caster.ConfigureShaderAndMatrices();
            CoreShader.SetMatrix4("lightSpaceMatrix", caster.LightSpaceMatrix);

            GL.Viewport(0, 0, caster.ShadowWidth, caster.ShadowHeight);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, caster.CasterFBO);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            DrawScene(deltaTime);
        }

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        Service.CoreShader = Window.CoreShader;

        var textureIndex = 10;
        foreach (var light in ObjectManager.GameObjects.Values.Where(l => l is DirectionLight))
        {
            GL.Viewport(0, 0, Window.Width, Window.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var caster = light.Component.Get<ShadowCaster>();
            if (caster == null || !caster.IsCastingShadows) continue;

            if (textureIndex >= 31)
            {
                Console.WriteLine("Error ran out of space to store shadow maps");
                return;
            }

            switch (light)
            {
                case DirectionLight dirLight:
                    CoreShader.SetMatrix4($"dirLight.fragPosLightSpace", caster.LightSpaceMatrix);
                    CoreShader.SetInt1($"dirLight.shadowMap", ++textureIndex);

                    GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
                    GL.BindTexture(TextureTarget.Texture2D, caster.ShadowMap);
                    continue;
                case PointLight pointLight:
                    CoreShader.SetMatrix4($"pointLights[{pointLight.CurrentIndex}].fragPosLightSpace", caster.LightSpaceMatrix);
                    CoreShader.SetInt1($"pointLights[{pointLight.CurrentIndex}].shadowMap", ++textureIndex);

                    GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
                    GL.BindTexture(TextureTarget.Texture2D, caster.ShadowMap);
                    continue;
                case SpotLight spotLight:
                    CoreShader.SetMatrix4($"spotLights[{spotLight.CurrentIndex}].fragPosLightSpace", caster.LightSpaceMatrix);
                    CoreShader.SetInt1($"spotLights[{spotLight.CurrentIndex}].shadowMap", ++textureIndex);

                    GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
                    GL.BindTexture(TextureTarget.Texture2D, caster.ShadowMap);
                    continue;
            }
        }
        DrawScene(deltaTime);
    }

    private void DrawScene(float deltaTime)
    {
        ObjectManager.DrawGameObjects();
        OnDraw(deltaTime);
    }

    public void Update(FrameEventArgs e)
    {
        Time.DeltaTime = (float)e.Time;
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
        OnLateUpdate(e);
    }

    public virtual void OnResize(FramebufferResizeEventArgs e)
    {
        ObjectManager.ResizeGameObjects(e);
    }

    public void Load(NativeWindow state, Window.Window window)
    {
        WHandler = state;
        Window = window;

        Service.Add(WHandler);
        Service.Add(Window);

        AudioManager = new AudioManager();
        AudioManager.SetDistanceModel(ALDistanceModel.InverseDistanceClamped);

        AudioManager.Init();
        ShadowMapShader = new Shader("shadowmap.vert", "shadowmap.frag");

        Setup();
        Start();

        ObjectManager.Instantiate(Scripts);
        GameObject.SetupTriggers();
    }

    public void Destroy()
    {
        GUI.GuiCallBack = null!;

        ObjectManager.Dispose();

        AudioManager?.Dispose();
        ShadowMapShader?.Dispose();

        Window?.CoreShader?.Dispose();

        Service.Clear();
        Texture.ClearTextures();

        AssetManager.Reset();
        PhysicsManager.Reset();

        Dispose();
    }
}
