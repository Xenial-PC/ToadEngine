namespace ToadEngine.Classes.Extensions;

public static class MathExtensions
{
    public static System.Numerics.Quaternion ToEuler(this Vector3 eulerDeg)
    {
        var rad = new Vector3(
            MathHelper.DegreesToRadians(eulerDeg.X),
            MathHelper.DegreesToRadians(eulerDeg.Y),
            MathHelper.DegreesToRadians(eulerDeg.Z)
        );

        var qx = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitX, rad.X);
        var qy = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, rad.Y);
        var qz = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitZ, rad.Z);

        return System.Numerics.Quaternion.Normalize(qy * qx * qz);
    }

    public static System.Numerics.Quaternion ToEuler(this System.Numerics.Vector3 eulerDeg)
    {
        var rad = new Vector3(
            MathHelper.DegreesToRadians(eulerDeg.X),
            MathHelper.DegreesToRadians(eulerDeg.Y),
            MathHelper.DegreesToRadians(eulerDeg.Z)
        );

        var qx = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitX, rad.X);
        var qy = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, rad.Y);
        var qz = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitZ, rad.Z);

        return System.Numerics.Quaternion.Normalize(qy * qx * qz);
    }

    public static Quaternion ToEulerOpenTK(this Vector3 eulerDeg)
    {
        var rad = new Vector3(
            MathHelper.DegreesToRadians(eulerDeg.X),
            MathHelper.DegreesToRadians(eulerDeg.Y),
            MathHelper.DegreesToRadians(eulerDeg.Z)
        );

        var qx = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitX, rad.X);
        var qy = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, rad.Y);
        var qz = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitZ, rad.Z);

        return Quaternion.Normalize((Quaternion)qy * (Quaternion)qx * (Quaternion)qz);
    }

    public static Quaternion ToEulerOpenTK(this System.Numerics.Vector3 eulerDeg)
    {
        var rad = new Vector3(
            MathHelper.DegreesToRadians(eulerDeg.X),
            MathHelper.DegreesToRadians(eulerDeg.Y),
            MathHelper.DegreesToRadians(eulerDeg.Z)
        );

        var qx = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitX, rad.X);
        var qy = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, rad.Y);
        var qz = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitZ, rad.Z);

        return Quaternion.Normalize((Quaternion)qy * (Quaternion)qx * (Quaternion)qz);
    }

    public static void DecomposeMatrix(
        in Matrix4 matrix,
        out Vector3 translation,
        out Quaternion rotationQuat,
        out Vector3 scale)
    {
        translation = matrix.ExtractTranslation();
        scale = matrix.ExtractScale();
        rotationQuat = matrix.ExtractRotation();
    }

    public static void DecomposeMatrixToEuler(
        in Matrix4 matrix,
        out Vector3 translation,
        out Vector3 rotationEulerDegrees,
        out Vector3 scale)
    {
        DecomposeMatrix(matrix, out translation, out var rotQuat, out scale);

        var eulerRad = QuaternionToEulerRadians(rotQuat);
        rotationEulerDegrees = new Vector3(
            MathHelper.RadiansToDegrees(eulerRad.X),
            MathHelper.RadiansToDegrees(eulerRad.Y),
            MathHelper.RadiansToDegrees(eulerRad.Z)
        );
    }

    public static Vector3 QuaternionToEulerRadians(Quaternion q)
    {
        q = Quaternion.Normalize(q);

        var sinr_cosp = 2f * (q.W * q.X + q.Y * q.Z);
        var cosr_cosp = 1f - 2f * (q.X * q.X + q.Y * q.Y);
        var roll = MathF.Atan2(sinr_cosp, cosr_cosp);

        var sinp = 2f * (q.W * q.Y - q.Z * q.X);
        float pitch;
        pitch = MathF.Abs(sinp) >= 1f ? MathF.CopySign(MathF.PI / 2f, sinp) :
            MathF.Asin(sinp);

        var siny_cosp = 2f * (q.W * q.Z + q.X * q.Y);
        var cosy_cosp = 1f - 2f * (q.Y * q.Y + q.Z * q.Z);
        var yaw = MathF.Atan2(siny_cosp, cosy_cosp);

        return new Vector3(roll, pitch, yaw);
    }
}
