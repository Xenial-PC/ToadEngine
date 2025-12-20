using ToadEngine.Classes.Base.Objects.BuiltIn;
using ToadEngine.Classes.Base.Rendering.SceneManagement;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEditor.Classes.EditorCore.Scenes
{
    public class TestScene : Scene
    {
        public TestScene() { }

        public override void Setup()
        {
            var cube = BuiltIn.Primitives.Cube;
            cube.Name = "Parent Cube";
            cube.Transform.Position = new Vector3(0f, 0f, -1f);
            cube.AddComponent<FallScript>();

            var childCube = BuiltIn.Primitives.Cube;
            childCube.Name = "Child Cube";
            childCube.Transform.LocalPosition.Y += 2f;

            var childCube2 = BuiltIn.Primitives.Cube;
            childCube2.Name = "Child Cube 2";
            childCube2.Transform.LocalPosition.Y += 2f;

            var childCube3 = BuiltIn.Primitives.Cube;
            childCube3.Name = "Child Cube 3";
            childCube3.Transform.LocalPosition.Y += 2f;

            cube.AddChild(childCube);
            cube.AddChild(childCube3);

            childCube.AddChild(childCube2);

            Instantiate(cube);
        }

        public override void OnStart()
        {
            
        }

        public override void OnUpdate(FrameEventArgs e)
        {
        }

        public override void OnResize(FramebufferResizeEventArgs e)
        {
        }

        public override void Dispose()
        {
        }
    }
}

public class FallScript : MonoBehavior
{
    public void Update()
    {
        GameObject.Transform.Position.Y -= 12.05f * Time.DeltaTime;
    }
}
