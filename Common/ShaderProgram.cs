using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PhysicsEngine
{
    public readonly struct ShaderUniform
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveUniformType Type;

        public ShaderUniform(string name, int location, ActiveUniformType type)
        {
            Name = name;
            Location = location;
            Type = type;
        }
    }

    public readonly struct ShaderAttribute
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveAttribType Type;

        public ShaderAttribute(string name, int location, ActiveAttribType type)
        {
            Name = name;
            Location = location;
            Type = type;
        }
    }
    public sealed class ShaderProgram : IDisposable
    {
        private bool disposed;

        public readonly int ShaderProgramObject;
        public readonly int VertexShaderObject;
        public readonly int PixelShaderObject;

        private readonly ShaderUniform[] uniforms;
        private readonly ShaderAttribute[] attributes;
        public ShaderProgram(string vertexShaderCode, string pixelShaderCode)
        {
            disposed = false;

            if (!CompileVertexShader(vertexShaderCode, out VertexShaderObject, out string vertexShaderCompileError))
            {
                throw new ArgumentException(vertexShaderCompileError);
            }
            if (!CompilePixelShader(pixelShaderCode, out PixelShaderObject, out string pixelShaderCompileError))
            {
                throw new ArgumentException(pixelShaderCompileError);
            }


            ShaderProgramObject = CreateLinkProgram(VertexShaderObject, PixelShaderObject);

            uniforms = CreateUniformList(ShaderProgramObject);
            attributes = CreateAttributeList(ShaderProgramObject);
        }


        ~ShaderProgram() { Dispose(); }

        public void Dispose()
        {
            if (disposed) return;

            GL.DeleteShader(VertexShaderObject);
            GL.DeleteShader(PixelShaderObject);

            GL.UseProgram(0);
            GL.DeleteProgram(ShaderProgramObject);


            disposed = true;
            GC.SuppressFinalize(this);
        }

        public ShaderUniform[] GetUniformList()
        {
            ShaderUniform[] result = new ShaderUniform[uniforms.Length];
            Array.Copy(uniforms, result, uniforms.Length);
            return result;
        }

        public ShaderAttribute[] GetAttributeList()
        {
            ShaderAttribute[] result = new ShaderAttribute[attributes.Length];
            Array.Copy(attributes, result, attributes.Length);
            return result;
        }

        public void SetUniform(string name, float v1)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException($"{name} was not found.");
            }

            if (uniform.Type != ActiveUniformType.Float)
            {
                throw new ArgumentException("Uniform type is not float.");
            }

            GL.UseProgram(ShaderProgramObject);
            GL.Uniform1(uniform.Location, v1);
            GL.UseProgram(0);
        }
        public void SetUniform(string name, float v1, float v2)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException($"{name} was not found.");
            }

            if (uniform.Type != ActiveUniformType.FloatVec2)
            {
                throw new ArgumentException("Uniform type is not FloatVec2.");
            }

            GL.UseProgram(ShaderProgramObject);
            GL.Uniform2(uniform.Location, v1, v2);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, Matrix4 matrix4)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException($"{name} was not found.");
            }

            if (uniform.Type != ActiveUniformType.FloatMat4)
            {
                throw new ArgumentException("Uniform type is not FloatMat4.");
            }

            GL.UseProgram(ShaderProgramObject);
            GL.UniformMatrix4(uniform.Location, true, ref matrix4);
            GL.UseProgram(0);
        }

        private bool GetShaderUniform(string name, out ShaderUniform uniform)
        {
            uniform = new ShaderUniform();

            for (int i = 0; i < uniforms.Length; i++)
            {
                uniform = uniforms[i];

                if (name == uniform.Name)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool CompileVertexShader(string vertexShaderCode, out int vertexShaderObject, out string errorMessage)
        {
            errorMessage = string.Empty;

            vertexShaderObject = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderObject, vertexShaderCode);
            GL.CompileShader(vertexShaderObject);

            string vertexShaderInfo = GL.GetShaderInfoLog(vertexShaderObject);
            if (vertexShaderInfo != string.Empty)
            {
                errorMessage = vertexShaderInfo;
                return false;
            }
            return true;
        }
        public static bool CompilePixelShader(string pixelShaderCode, out int pixelShaderObject, out string errorMessage)
        {
            errorMessage = string.Empty;
            pixelShaderObject = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderObject, pixelShaderCode);
            GL.CompileShader(pixelShaderObject);

            string pixelShaderInfo = GL.GetShaderInfoLog(pixelShaderObject);
            if (pixelShaderInfo != string.Empty)
            {
                errorMessage = pixelShaderInfo;
                return false;

            }
            return true;
        }

        public static int CreateLinkProgram(int vertexShaderObject, int pixelShaderObject)
        {
            int shaderProgramObject = GL.CreateProgram();

            GL.AttachShader(shaderProgramObject, vertexShaderObject);
            GL.AttachShader(shaderProgramObject, pixelShaderObject);

            GL.LinkProgram(shaderProgramObject);

            GL.DetachShader(shaderProgramObject, vertexShaderObject);
            GL.DetachShader(shaderProgramObject, pixelShaderObject);

            return shaderProgramObject;
        }

        public static ShaderUniform[] CreateUniformList(int shaderProgramObject)
        {
            GL.GetProgram(shaderProgramObject, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            ShaderUniform[] uniforms = new ShaderUniform[uniformCount];

            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(shaderProgramObject, i, 256, out _, out _, out ActiveUniformType type, out string name);
                int location = GL.GetUniformLocation(shaderProgramObject, name);
                uniforms[i] = new ShaderUniform(name, location, type);
            }

            return uniforms;
        }

        public static ShaderAttribute[] CreateAttributeList(int shaderProgramObject)
        {
            GL.GetProgram(shaderProgramObject, GetProgramParameterName.ActiveAttributes, out int attributeCount);

            ShaderAttribute[] attributes = new ShaderAttribute[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                GL.GetActiveAttrib(shaderProgramObject, i, 256, out _, out _, out ActiveAttribType type, out string name);
                int location = GL.GetAttribLocation(shaderProgramObject, name);
                attributes[i] = new ShaderAttribute(name, location, type);
            }

            return attributes;
        }
    }
}
