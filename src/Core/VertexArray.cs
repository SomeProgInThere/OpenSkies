
using OpenTK.Graphics.OpenGL4;

namespace OpenSkies.Core;

public class VertexArray<TVertexType, TIndexType> : IDisposable 
    where TVertexType : unmanaged where TIndexType : unmanaged {
    
    public VertexArray(Buffer<TVertexType> vertexBuffer, Buffer<TIndexType> elementBuffer) {
        m_handle = GL.GenVertexArray();

        Bind();
        vertexBuffer.Bind();
        elementBuffer.Bind();        
    }

    public unsafe void VertexAttribPointer(int index, int count, VertexAttribPointerType type, int size, int offset) {
        GL.VertexAttribPointer(index, count, type, false, size * sizeof(TVertexType), offset * sizeof(TVertexType));
        GL.EnableVertexAttribArray(index);
    }

    public void Bind() => GL.BindVertexArray(m_handle);

    public void Dispose() {
        GL.DeleteVertexArray(m_handle);
        GC.SuppressFinalize(this);
    }

    private readonly int m_handle;
}
