using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics;
using BepuUtilities;
using System.Numerics;
using System.Runtime.CompilerServices;
using ToadEngine.Classes.Base.Scripting.Base;
using ToadEngine.Classes.Base.Physics.Managers;
using ToadEngine.Classes.Base.Rendering.Object;
using Vector3 = System.Numerics.Vector3;

namespace ToadEngine.Classes.Base.Physics;

public class PhysicsActions
{
    public Action<float>? OnPreStep;
    public Action<float>? OnPostStep;
    public Action<GameObject>? OnCollision;
}

public class PhysicsCallBacks
{
    public struct NarrowPhaseCallbacks() : INarrowPhaseCallbacks
    {
        public void Initialize(Simulation simulation)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b,
            ref float speculativeMargin)
        {
            return (a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold,
            out PairMaterialProperties pairMaterial) where TManifold : unmanaged, IContactManifold<TManifold>
        {
            var physics = Service.Physics;

            var matA = PhysicsMaterialRegistry.Get(pair.A.RawHandleValue);
            var matB = PhysicsMaterialRegistry.Get(pair.B.RawHandleValue);
            var isMaterialsFound = matA != null && matB != null;

            pairMaterial.FrictionCoefficient = isMaterialsFound ? MathF.Sqrt(matA!.Friction * matB!.Friction)
                : physics.Settings.Friction;

            pairMaterial.MaximumRecoveryVelocity = isMaterialsFound ? MathF.Max(matA!.Restitution, matB!.Restitution)
                : physics.Settings.Restitution;

            pairMaterial.SpringSettings = isMaterialsFound ? matA!.SpringSettings : physics.Settings.SpringSettings;

            var gameObjectA = Behavior.BodyToGameObject[pair.A.RawHandleValue];
            var gameObjectB = Behavior.BodyToGameObject[pair.B.RawHandleValue];

            var collisionChecks =
                isMaterialsFound &&
                (PhysicsLayer.ShouldCollideLayer(matA!.PhysicsLayer.Layer, matB!.PhysicsLayer.Layer) ||
                 PhysicsLayer.ShouldCollideObject(gameObjectA, gameObjectB));

            physics.Actions.OnCollision?.Invoke(gameObjectB);

            if (IsTrigger(pair.A) || IsTrigger(pair.B))
            {
                TriggerManager.RegisterOverlap(pair.A.RawHandleValue, pair.B.RawHandleValue);
                return false;
            }

            return collisionChecks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB,
            ref ConvexContactManifold manifold)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsTrigger(CollidableReference collidable)
        {
            return collidable.Mobility != CollidableMobility.Static &&
                   TriggerRegistry.IsTrigger(collidable.RawHandleValue);
        }

        public void Dispose()
        {

        }
    }

    public struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks
    {
        public AngularIntegrationMode AngularIntegrationMode => Service.Physics.Settings.AngularIntegrationMode;
        public bool AllowSubstepsForUnconstrainedBodies => Service.Physics.Settings.AllowSubstepsForUnconstrainedBodies;
        public bool IntegrateVelocityForKinematics => Service.Physics.Settings.IntegrateVelocityForKinematics;

        public void Initialize(Simulation simulation)
        {
        }

        public void PrepareForIntegration(float dt)
        {
        }

        public void IntegrateVelocity(Vector<int> bodyIndices, Vector3Wide position, QuaternionWide orientation,
            BodyInertiaWide localInertia, Vector<int> integrationMask, int workerIndex, Vector<float> dt, ref BodyVelocityWide velocity)
        {
            ApplyForce(bodyIndices, integrationMask, dt, ref velocity);
        }

        private void ApplyForce(Vector<int> bodyIndices, Vector<int> integrationMask, Vector<float> dt, ref BodyVelocityWide velocity)
        {
            var physics = Service.Physics;
            Vector3Wide appliedForcesLinear = default;
            for (var laneIndex = 0; laneIndex < Vector<int>.Count; laneIndex++)
            {
                if (integrationMask[laneIndex] == 0) continue;

                var bodyId = bodyIndices[laneIndex];
                var physicsMaterial = physics.GetBoundMaterial(bodyId);

                var gravity = Gravity(physicsMaterial);
                Vector3Wide.WriteSlot(gravity, laneIndex, ref appliedForcesLinear);
            }

            velocity.Linear += appliedForcesLinear * dt;
        }

        private static Vector3 Gravity(PhysicsMaterial? physMaterial)
        {
            var gravity = physMaterial != null ?
                new Vector3(0, physMaterial.Gravity, 0)
                : Service.Physics.Settings.Gravity;

            return gravity;
        }
    }
}
