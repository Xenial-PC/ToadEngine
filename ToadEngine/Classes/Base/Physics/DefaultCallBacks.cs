using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuPhysics;
using BepuUtilities;
using System.Numerics;
using System.Runtime.CompilerServices;
using ToadEngine.Classes.Base.Scripting.Base;

namespace ToadEngine.Classes.Base.Physics;

public class DefaultCallBacks
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

            pairMaterial.FrictionCoefficient = physics.FrictionCoefficient;
            pairMaterial.MaximumRecoveryVelocity = physics.MaximumRecoveryVelocity;
            pairMaterial.SpringSettings = physics.SpringSettings;

            if (IsTrigger(pair.A) || IsTrigger(pair.B))
            {
                TriggerManager.RegisterOverlap(pair.A.RawHandleValue, pair.B.RawHandleValue);
                return false;
            }

            return true;
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
        public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;
        public bool AllowSubstepsForUnconstrainedBodies => false;
        public bool IntegrateVelocityForKinematics => false;

        private Vector3Wide _gravityWideDt;

        public void Initialize(Simulation simulation)
        {
        }

        public void PrepareForIntegration(float dt)
        {
            _gravityWideDt = Vector3Wide.Broadcast(Service.Physics.Gravity * dt);
        }

        public void IntegrateVelocity(Vector<int> bodyIndices, Vector3Wide position, QuaternionWide orientation,
            BodyInertiaWide localInertia, Vector<int> integrationMask, int workerIndex, Vector<float> dt, ref BodyVelocityWide velocity)
        {
            velocity.Linear += _gravityWideDt;
        }
    }
}
