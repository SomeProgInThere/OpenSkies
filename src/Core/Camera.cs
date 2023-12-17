
using OpenTK.Mathematics;

namespace OpenSkies.Core;

public class Camera(Vector3 position, float aspectRatio)
{
    public Vector3 Position { get; set; } = position;
    public float AspectRatio { private get; set; } = aspectRatio;
    public Vector3 Front => m_front;
    public Vector3 Up => m_up;
    public Vector3 Right => m_right;

    public float Pitch {
        get => MathHelper.RadiansToDegrees(m_pitch);
        set {
            var angle = MathHelper.Clamp(value, -89f, 89f);
            m_pitch = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
        }
    }

    public float Yaw {
        get => MathHelper.RadiansToDegrees(m_yaw);
        set {
            m_yaw = MathHelper.DegreesToRadians(value);
            UpdateVectors();
        }
    }

    public float Fov {
        get => MathHelper.RadiansToDegrees(m_fov);
        set{
            var angle = MathHelper.Clamp(value, 1f, 90f);
            m_fov = MathHelper.DegreesToRadians(angle);
        }
    }

    public Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, Position + m_front, m_up);
    public Matrix4 GetProjectionMatrix() => Matrix4.CreatePerspectiveFieldOfView(m_fov, AspectRatio, 0.01f, 100f);
    
    private void UpdateVectors() {
        m_front.X = MathF.Cos(m_pitch) * MathF.Cos(m_yaw);
        m_front.Y = MathF.Sin(m_pitch);
        m_front.Z = MathF.Cos(m_pitch) * MathF.Sin(m_yaw);

        m_front = Vector3.Normalize(m_front);

        m_right = Vector3.Normalize(Vector3.Cross(m_front, Vector3.UnitY));
        m_up = Vector3.Normalize(Vector3.Cross(m_right, m_front));
    }

    private Vector3 m_front = -Vector3.UnitZ;
    private Vector3 m_up = Vector3.UnitY;
    private Vector3 m_right = Vector3.UnitX;
    private float m_pitch;
    private float m_yaw = -MathHelper.PiOver2; 
    private float m_fov = MathHelper.PiOver2;
} 
