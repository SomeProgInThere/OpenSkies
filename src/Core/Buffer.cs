
using OpenTK.Graphics.OpenGL4;

namespace OpenSkies.Core;

public class Buffer<TDataType> : IDisposable where TDataType : unmanaged {
    public unsafe Buffer(TDataType[] data, BufferTarget bufferTarget) {
        m_bufferTarget = bufferTarget;
        m_handle = GL.GenBuffer();

        Bind();
        GL.BufferData(m_bufferTarget, data.Length * sizeof(TDataType), data, BufferUsageHint.StaticDraw);
    }

    public void Bind() => GL.BindBuffer(m_bufferTarget, m_handle);

    public void Dispose() {
        GL.DeleteBuffer(m_handle);
        GC.SuppressFinalize(this);
    }

    private readonly int m_handle;
    private readonly BufferTarget m_bufferTarget;
}