using BepuPhysics.Collidables;
using BepuPhysics;
using BepuUtilities.Memory;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Physics.Managers;

public class ColliderManager(Simulation sim, BufferPool bPool)
{
    public Dynamic CreateDynamic = new(sim, bPool);
    public Static CreateStatic = new(sim, bPool);
    public Kinematic CreateKinematic = new(sim, bPool);
    public Trigger CreateTrigger = new(sim, bPool);

    public class Dynamic(Simulation sim, BufferPool bPool)
    {
        public Simulation Simulation => sim;
        public BufferPool BufferPool => bPool;

        public BodyHandle Box(Vector3 pos, Vector3 size, float mass = 1f)
        {
            var shape = new Box(size.X, size.Y, size.Z);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateDynamic(
                new RigidPose(pos, Quaternion.Identity),
                shape.ComputeInertia(mass),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f)
            );
            return Simulation.Bodies.Add(bodyDesc);
        }

        public BodyHandle Sphere(Vector3 pos, float radius, float mass = 1f)
        {
            var shape = new Sphere(radius);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateDynamic(
                new RigidPose(pos, Quaternion.Identity),
                shape.ComputeInertia(mass),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f)
            );
            return Simulation.Bodies.Add(bodyDesc);
        }

        public BodyHandle Capsule(Vector3 pos, Vector2 radLength, float mass = 1f)
        {
            var shape = new Capsule(radLength.X, radLength.Y);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateDynamic(
                new RigidPose(pos, Quaternion.Identity),
                shape.ComputeInertia(mass),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f)
            );
            return Simulation.Bodies.Add(bodyDesc);
        }

        public BodyHandle Cylinder(Vector3 pos, Vector2 radLength, float mass = 1f)
        {
            var shape = new Cylinder(radLength.X, radLength.Y);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateDynamic(
                new RigidPose(pos, Quaternion.Identity),
                shape.ComputeInertia(mass),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f)
            );
            return Simulation.Bodies.Add(bodyDesc);
        }

        public BodyHandle Mesh(Vector3 pos, Buffer<Triangle> triangles, Vector3 scale, float mass = 1f)
        {
            var shape = new Mesh(triangles, scale, BufferPool);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateDynamic(
                new RigidPose(pos, Quaternion.Identity),
                shape.ComputeClosedInertia(mass),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f)
            );
            return Simulation.Bodies.Add(bodyDesc);
        }
    }

    public class Static(Simulation sim, BufferPool bPool)
    {
        public Simulation Simulation => sim;
        public BufferPool BufferPool => bPool;

        public StaticHandle Box(Vector3 pos, Vector3 size)
        {
            var shape = new Box(size.X, size.Y, size.Z);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = new StaticDescription(
                new RigidPose(pos, Quaternion.Identity),
                shapeIndex
            );
            return Simulation.Statics.Add(bodyDesc);
        }

        public StaticHandle Sphere(Vector3 pos, float radius)
        {
            var shape = new Sphere(radius);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = new StaticDescription(
                new RigidPose(pos, Quaternion.Identity),
                shapeIndex
            );
            return Simulation.Statics.Add(bodyDesc);
        }

        public StaticHandle Capsule(Vector3 pos, Vector2 radLength)
        {
            var shape = new Capsule(radLength.X, radLength.Y);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = new StaticDescription(
                new RigidPose(pos, Quaternion.Identity),
                shapeIndex
            );
            return Simulation.Statics.Add(bodyDesc);
        }

        public StaticHandle Cylinder(Vector3 pos, Vector2 radLength)
        {
            var shape = new Cylinder(radLength.X, radLength.Y);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = new StaticDescription(
                new RigidPose(pos, Quaternion.Identity),
                shapeIndex
            );
            return Simulation.Statics.Add(bodyDesc);
        }

        public StaticHandle Mesh(Vector3 pos, Buffer<Triangle> triangles, Vector3 scale)
        {
            var shape = new Mesh(triangles, scale, BufferPool);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = new StaticDescription(
                new RigidPose(pos, Quaternion.Identity),
                shapeIndex
            );
            return Simulation.Statics.Add(bodyDesc);
        }
    }

    public class Kinematic(Simulation sim, BufferPool bPool)
    {
        public Simulation Simulation => sim;
        public BufferPool BufferPool => bPool;

        public BodyHandle Mesh(Vector3 pos, Buffer<Triangle> triangles, Vector3 scale)
        {
            var shape = new Mesh(triangles, scale, BufferPool);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateKinematic(
                new RigidPose(pos),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f));

            return Simulation.Bodies.Add(bodyDesc);
        }

        public BodyHandle Box(Vector3 pos, Vector3 size)
        {
            var shape = new Box(size.X, size.Y, size.Z);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateKinematic(
                new RigidPose(pos),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f));

            return Simulation.Bodies.Add(bodyDesc);
        }

        public BodyHandle Sphere(Vector3 pos, float radius)
        {
            var shape = new Sphere(radius);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateKinematic(
                new RigidPose(pos),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f));

            return Simulation.Bodies.Add(bodyDesc);
        }

        public BodyHandle Capsule(Vector3 pos, Vector2 radLength)
        {
            var shape = new Capsule(radLength.X, radLength.Y);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateKinematic(
                new RigidPose(pos),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f));

            return Simulation.Bodies.Add(bodyDesc);
        }

        public BodyHandle Cylinder(Vector3 pos, Vector2 radLength)
        {
            var shape = new Cylinder(radLength.X, radLength.Y);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateKinematic(
                new RigidPose(pos),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f));

            return Simulation.Bodies.Add(bodyDesc);
        }
    }

    public class Trigger(Simulation sim, BufferPool bPool)
    {
        public Simulation Simulation => sim;
        public BufferPool BufferPool => bPool;

        public BodyHandle Box(Vector3 pos, Vector3 size)
        {
            var shape = new Box(size.X, size.Y, size.Z);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateKinematic(
                new RigidPose(pos),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f));

            var handle = Simulation.Bodies.Add(bodyDesc);
            TriggerRegistry.Triggers.Add(handle.Value);
            return handle;
        }

        public BodyHandle Cylinder(Vector3 pos, Vector2 radLength)
        {
            var shape = new Cylinder(radLength.X, radLength.Y);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateKinematic(
                new RigidPose(pos),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f));

            var handle = Simulation.Bodies.Add(bodyDesc);
            TriggerRegistry.Triggers.Add(handle.Value);

            return handle;
        }

        public BodyHandle Sphere(Vector3 pos, float radius)
        {
            var shape = new Sphere(radius);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateKinematic(
                new RigidPose(pos),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f));

            var handle = Simulation.Bodies.Add(bodyDesc);
            TriggerRegistry.Triggers.Add(handle.Value);

            return handle;
        }

        public BodyHandle Mesh(Vector3 pos, Buffer<Triangle> triangles, Vector3 scale)
        {
            var shape = new Mesh(triangles, scale, BufferPool);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateKinematic(
                new RigidPose(pos),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f));

            var handle = Simulation.Bodies.Add(bodyDesc);
            TriggerRegistry.Triggers.Add(handle.Value);

            return handle;
        }

        public BodyHandle Capsule(Vector3 pos, Vector2 radLength)
        {
            var shape = new Capsule(radLength.X, radLength.Y);
            var shapeIndex = Simulation.Shapes.Add(shape);
            var bodyDesc = BodyDescription.CreateKinematic(
                new RigidPose(pos),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f));

            var handle = Simulation.Bodies.Add(bodyDesc);
            TriggerRegistry.Triggers.Add(handle.Value);

            return handle;
        }
    }
}