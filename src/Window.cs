
using System.ComponentModel;
using OpenSkies.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenSkies;

public class Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) 
    : GameWindow(gameWindowSettings, nativeWindowSettings) {

    protected override void OnLoad() {
        base.OnLoad();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f); 
        GL.Enable(EnableCap.DepthTest);

        m_vbo = new Buffer<float>(m_vertices, BufferTarget.ArrayBuffer);
        m_ebo = new Buffer<uint>(m_indices, BufferTarget.ElementArrayBuffer);
        m_vao = new VertexArray<float, uint>(m_vbo, m_ebo);

        m_shader = new Shader("shaders/main.vert", "shaders/main.frag");
        m_shader.Use();

        var vertexLocation = m_shader.GetAttribLocation("vertPosition");
        m_vao.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, 5, 0);

        var texCoordLocation = m_shader.GetAttribLocation("vertTexCoord");
        m_vao.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, 5, 3);

        m_texture = new Texture("resources/moss.png");
        m_texture.Bind(TextureUnit.Texture0);
        m_shader.SetUniform("tex0", 0);

        m_camera = new Camera(Vector3.UnitZ * 3, Size.X / (float) Size.Y);

        CursorState = CursorState.Grabbed;
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        m_time += 4.0 * args.Time;

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        m_vao.Bind();

        m_texture.Bind(TextureUnit.Texture0);
        m_shader.Use();

        var model = Matrix4.Identity; 
        
        m_shader.SetUniform("model", model);
        m_shader.SetUniform("view", m_camera.GetViewMatrix());
        m_shader.SetUniform("projection", m_camera.GetProjectionMatrix());

        GL.DrawElements(PrimitiveType.Triangles, m_indices.Length, DrawElementsType.UnsignedInt, 0);
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);

        if (!IsFocused) return;
        var input = KeyboardState;

        if (input.IsKeyPressed(Keys.Escape)) {
            m_cursorGrabbed = !m_cursorGrabbed;
        }
        if (input.IsKeyPressed(Keys.Q)) {
            Close();
        }

        CursorState = m_cursorGrabbed ? CursorState.Grabbed : CursorState.Normal;

        const float cameraSpeed = 3.0f;
        const float sensitivity = 0.1f;

        if (input.IsKeyDown(Keys.W)) {
            m_camera.Position += m_camera.Front * cameraSpeed * (float) args.Time; 
        }
        if (input.IsKeyDown(Keys.S)) {
            m_camera.Position -= m_camera.Front * cameraSpeed * (float) args.Time; 
        }
        if (input.IsKeyDown(Keys.A)) {
            m_camera.Position -= m_camera.Right * cameraSpeed * (float) args.Time; 
        }
        if (input.IsKeyDown(Keys.D)) {
            m_camera.Position += m_camera.Right * cameraSpeed * (float) args.Time; 
        }

        if (input.IsKeyDown(Keys.Space)) {
            m_camera.Position += m_camera.Up * cameraSpeed * (float) args.Time; 
        }
        if (input.IsKeyDown(Keys.LeftShift)) {
            m_camera.Position -= m_camera.Up * cameraSpeed * (float) args.Time; 
        }

        var mouse = MouseState;

        if (m_lastPos == default) {
            m_lastPos = new Vector2(mouse.X, mouse.Y);
        }
        else {
            var deltaX = mouse.X - m_lastPos.X;
            var deltaY = mouse.Y - m_lastPos.Y;

            m_lastPos = new Vector2(mouse.X, mouse.Y);

            m_camera.Yaw += deltaX * sensitivity;
            m_camera.Pitch -= deltaY * sensitivity; 
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e) {
        base.OnMouseWheel(e);  

        m_camera.Fov -= e.OffsetY;
        m_camera.Fov = Math.Clamp(m_camera.Fov, 1f, 90f);
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        
        GL.Viewport(0, 0, Size.X, Size.Y);
        m_camera.AspectRatio = Size.X / (float) Size.Y;
    }

    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
       
        m_vbo.Dispose();
        m_ebo.Dispose();
        m_vao.Dispose();
       
        m_shader.Dispose();
        m_texture.Dispose();
    }

    private Camera m_camera;

    private double m_time;

    private static readonly float[] m_vertices =
    [
        // Position         Texture coordinates
         0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
         0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
        -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left  
    ];

    private static readonly uint[] m_indices = 
    [
        0, 1, 3,
        1, 2, 3
    ];

    private Buffer<uint> m_ebo;
    private Buffer<float> m_vbo;
    private VertexArray<float, uint> m_vao;

    private Shader m_shader;
    private Texture m_texture;

    private Vector2 m_lastPos;
    private bool m_cursorGrabbed = true;
}