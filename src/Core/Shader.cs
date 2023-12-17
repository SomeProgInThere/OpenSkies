
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenSkies.Core;

public class Shader : IDisposable {
    public Shader(string vertPath, string fragPath) {

        string aVertPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\net8.0\", @"\src\"), vertPath);
        int vertShader = LoadShader(aVertPath, ShaderType.VertexShader);
        
        string aFragPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\net8.0\", @"\src\"), fragPath);
        int fragShader = LoadShader(aFragPath, ShaderType.FragmentShader);

        m_handle = GL.CreateProgram();
        GL.AttachShader(m_handle, vertShader);
        GL.AttachShader(m_handle, fragShader);
        GL.LinkProgram(m_handle);    

        GL.GetProgram(m_handle, GetProgramParameterName.LinkStatus, out int status);
        if (status == 0) {
            throw new Exception($"Error linking to program: {GL.GetProgramInfoLog(m_handle)}");
        }

        GL.DetachShader(m_handle, vertShader);
        GL.DetachShader(m_handle, fragShader);
        GL.DeleteShader(vertShader);
        GL.DeleteShader(fragShader);
    }
    
    public void Use() => GL.UseProgram(m_handle);
    public int GetAttribLocation(string attribName) => GL.GetAttribLocation(m_handle, attribName);
        
    public void Dispose()
    {
        GL.DeleteProgram(m_handle);
        GC.SuppressFinalize(this);
    }

    public void SetUniform(string name, int value) {
        Use();
        
        int location = GL.GetUniformLocation(m_handle, name);
        if (location == -1) {
            throw new Exception($"Uniform {name} not found on shader!");
        }

        GL.Uniform1(location, value);
    }

    public void SetUniform(string name, float value) {
        Use();
        
        int location = GL.GetUniformLocation(m_handle, name);
        if (location == -1) {
            throw new Exception($"Uniform {name} not found on shader!");
        }

        GL.Uniform1(location, value);
    }

    public void SetUniform(string name, Matrix4 mat) {
        Use();

        int location = GL.GetUniformLocation(m_handle, name);
        if (location == -1) {
            throw new Exception($"Uniform {name} not found on shader!");
        }

        GL.UniformMatrix4(location, true, ref mat);
    }

    private static int LoadShader(string path, ShaderType type) {
        var shader = GL.CreateShader(type);
        var code = File.ReadAllText(path);

        GL.ShaderSource(shader, code);
        GL.CompileShader(shader);
        
        string infoLog = GL.GetShaderInfoLog(shader);
        if (!string.IsNullOrWhiteSpace(infoLog)) {
            Console.WriteLine($"Error compiling {type} shader: {infoLog}");
        }

        return shader;
    }

    private readonly int m_handle;
}
