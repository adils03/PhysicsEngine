using OpenTK.Mathematics;
using PhysicsEngine.Common.BufferObjects;
using PhysicsEngine.Common;
using OpenTK.Graphics.OpenGL4;

namespace PhysicsEngine.Shapes
{    
    public struct PointLight
    {
        public Vector3 position;
        public float constant;
        public float linear;
        public float quadratic;
        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;
    }
    public class Transform
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public Transform()
        {
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scale = Vector3.One;
        }
    }

    public class Shape
    {
        protected VertexArray vertexArray;
        protected VertexBuffer vertexBuffer;
        protected IndexBuffer indexBuffer;

        protected VertexPositionNormalTexture[] vertices;
        protected int[] indices;

        protected Texture diffuseMap;
        protected Texture specularMap;

        protected Transform Transform = new Transform();

        public Shape()
        {
        }

        ~Shape()
        {
            Dispose();
        }

        public void Dispose()
        {
            vertexArray?.Dispose();
            indexBuffer?.Dispose();
            vertexBuffer?.Dispose();
        }

        protected void CreateBuffers()
        {
            vertexBuffer = new VertexBuffer(VertexPositionNormalTexture.VertexInfo, vertices.Length, true);
            vertexBuffer.SetData(vertices, vertices.Length);

            indexBuffer = new IndexBuffer(indices.Length, true);
            indexBuffer.SetData(indices, indices.Length);

            vertexArray = new VertexArray(vertexBuffer);
        }

        protected virtual void AssignVerticesIndices()
        {
        }

        protected virtual void LoadTexture(string diffuseMapPath = "Resources/container2.png", string specularMapPath = "Resources/container2_specular.png")
        {
        }

        protected void Render(PointLight[] pointLights, Camera cam, ShaderProgram shader, bool lighting = false)
        {
            shader.Use();

            if (lighting)
                diffuseMap.Use(TextureUnit.Texture0);
            else
                specularMap.Use(TextureUnit.Texture1);

            vertexBuffer.SetData(vertices, vertices.Length);//translate için
            GL.BindVertexArray(vertexArray.VertexArrayHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer.IndexBufferHandle);

            shader.SetUniform("model", Matrix4.Identity);
            shader.SetUniform("view", cam.GetViewMatrix());
            shader.SetUniform("projection", cam.GetProjectionMatrix());


            if (lighting)
            {

                //gün ışığı
                shader.SetUniform("dirLight.direction", new Vector3(1.0f));
                shader.SetUniform("dirLight.ambient", new Vector3(0.3f));
                shader.SetUniform("dirLight.diffuse", new Vector3(0.3f));
                shader.SetUniform("dirLight.specular", new Vector3(0.0f));

                shader.SetUniform("material.diffuse", 0);
                shader.SetUniform("material.specular", 1);
                shader.SetUniform("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
                shader.SetUniform("material.shininess", 32.0f);

                shader.SetUniform("viewPos", cam.Position);


                // pointLights
                for (int i = 0; i < pointLights.Length; i++)
                {
                    shader.SetUniform($"pointLights[{i}].constant", 1.0f);
                    shader.SetUniform($"pointLights[{i}].linear", 0.09f);
                    shader.SetUniform($"pointLights[{i}].quadratic", 0.032f);


                    shader.SetUniform($"pointLights[{i}].position", pointLights[i].position);
                    shader.SetUniform($"pointLights[{i}].ambient", new Vector3(0.5f));
                    shader.SetUniform($"pointLights[{i}].diffuse", new Vector3(2.5f));
                    shader.SetUniform($"pointLights[{i}].specular", new Vector3(5.0f));
                }
            }
            else
            {
                shader.SetUniform("Color", new Vector3(1f, 1f, 1f));
            }

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void RenderLighting(PointLight[] pointLights, Camera cam, ShaderProgram lightingShader)
        {
            Render(pointLights, cam, lightingShader, true);
        }
        public void RenderObject(Camera cam, ShaderProgram shader)
        {
            Render(new PointLight[4], cam, shader, false);
        }
        public void Translate(Vector3 translateVector)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position += translateVector;
            }
            Transform.Position += translateVector;
        }
        public void Rotate(Vector3 rotateVector)
        {
            // Derece cinsinden açıları radyan cinsine dönüştürme
            float radiansX = MathHelper.DegreesToRadians(rotateVector.X);
            float radiansY = MathHelper.DegreesToRadians(rotateVector.Y);
            float radiansZ = MathHelper.DegreesToRadians(rotateVector.Z);

            // Radyan cinsinden açıları kullanarak Quaternion oluşturma
            Quaternion quaternion = Quaternion.FromEulerAngles(radiansX, radiansY, radiansZ);

            Transform.Rotation = quaternion;

            for (int i = 0; i < vertices.Length; i++)
            {
                // Vertex pozisyonunu radyan cinsinden döndürme
                Vector3 newPosition = Vector3.Transform(vertices[i].Position - Transform.Position, quaternion) + Transform.Position;
                vertices[i].Position = newPosition;

                Vector3 newNormal = Vector3.Transform(vertices[i].Normal, quaternion);
                vertices[i].Normal = newNormal;
            }

        }

    }
}
