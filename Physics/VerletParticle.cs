using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PhysicsEngine;

public class VerletParticle
{
    public Vector3 Position;
    public Vector3 PreviousPosition;
    public Vector3 Acceleration;
    public float Mass;
    public Shape Shape;
    public bool IsFixed;
    public bool IsControlled;
    public float deltaTime;

    public VerletParticle(Vector3 initialPosition, float mass = 1.0f, bool isFixed = false, bool isControlled = false)
    {
        Position = initialPosition;
        PreviousPosition = initialPosition;
        Mass = mass;
        IsFixed = isFixed;
        IsControlled = isControlled;
        Shape = new Cube(initialPosition, ShapeShaderType.ColorLight, Color4.Chocolate, new Vector3(0.1f));
    }

    public float KineticEnergy
    {
        get
        {
            float speedSquared = (Position - PreviousPosition).LengthSquared / deltaTime;
            return 0.5f * Mass * speedSquared;
        }
    }

    public void StopIfEnergyLow(float threshold)
    {
        if (KineticEnergy < threshold)
        {
            Acceleration = Vector3.Zero; // Hareketi durdurmak için ivmeyi sıfırla
        }
    }

    public void Update(float deltaTime)
    {
        this.deltaTime = deltaTime;
        if (IsFixed)
        {
            Shape.RenderBasic();
            return;
        }

        Vector3 temp = Position;
        Vector3 velocity = Position - PreviousPosition;

        Vector3 gravity = new Vector3(0, -9.81f, 0); // Yerçekimi kuvveti

        velocity *= 0.999f;
        Acceleration *= 0.999f;
        velocity += gravity * deltaTime*0.001f; // Yer çekimi kuvvetini ekle (hayvani bir sonuç verdiği için *0.001f)
        Position = Position + velocity + Acceleration * (deltaTime * deltaTime);

        Shape.Teleport(Position);
        Shape.RenderBasic();
        PreviousPosition = temp;
        Acceleration = Vector3.Zero; // İvme her adımda yeniden hesaplanacak
    }


    public void ApplyForce(Vector3 force)
    {
        if (!IsFixed)
        {
            Acceleration += force; // Kütlenin 1 olduğunu varsayıyoruz
        }
    }
}

public class Constraint
{
    public VerletParticle ParticleA;
    public VerletParticle ParticleB;
    public float RestLength;

    public Constraint(VerletParticle particleA, VerletParticle particleB, float restLength)
    {
        ParticleA = particleA;
        ParticleB = particleB;
        RestLength = restLength;
    }

    public void SatisfyConstraint()
    {
        Vector3 delta = ParticleB.Position - ParticleA.Position;
        float currentLength = delta.Length;
        float difference = (currentLength - RestLength) / currentLength;
        Vector3 correction = delta * 0.5f * difference;

        if (!ParticleA.IsFixed)
        {
            ParticleA.Position += correction;
        }
        if (!ParticleB.IsFixed)
        {
            ParticleB.Position -= correction;
        }
    }
}

public class StringCubes
{
    public List<VerletParticle> Particles;
    public List<Constraint> Constraints;

    public StringCubes(Vector3 firstPosition)
    {
        Particles = new List<VerletParticle>();
        Constraints = new List<Constraint>();
        CreateStringCubes(firstPosition);
    }

    public void CreateStringCubes(Vector3 firstPosition)
    {
        VerletParticle previousParticle = null;

        for (int i = 0; i < 50; i++)
        {
            bool isFixed = (i == 0);
            bool isControlled = (i == 49);// son küp için

            VerletParticle particle = new VerletParticle(firstPosition, 1.0f, isFixed, isControlled);
            Particles.Add(particle);

            if (previousParticle != null)
            {
                float restLength = (particle.Position - previousParticle.Position).Length-0.05f;
                Constraints.Add(new Constraint(previousParticle, particle, restLength));
            }

            previousParticle = particle;
            firstPosition += new Vector3(0, -0.15f, 0);
        }
    }

    public void Update(float deltaTime)
    {
        foreach (var particle in Particles)
        {
            particle.Update(deltaTime);
        }

        for (int i = 0; i < 5; i++)
        {
            foreach (var constraint in Constraints)
            {
                constraint.SatisfyConstraint();
            }
        }
    }

    private void SetEndParticleVelocity(Vector3 newPosition)
    {
        VerletParticle endParticle = Particles.Last();
        if (endParticle.IsControlled)
        {
            endParticle.Position += newPosition;
            //endParticle.PreviousPosition += newPosition;
        }
    }
    private void SetEndParticlePosition(Vector3 newPosition)
    {
        VerletParticle endParticle = Particles.Last();
        if (endParticle.IsControlled)
        {
            endParticle.Position = newPosition;
            //endParticle.PreviousPosition = newPosition;
        }
    }

    public void UpdateVelocity(KeyboardState state, FrameEventArgs e)
    {
       

        Vector3 velocity = Vector3.Zero;
        if (state.IsKeyDown(Keys.Up))
        {
            velocity += new Vector3(0, 0.3f, 0); // Yukarı yönde hız ekle
        }
        if (state.IsKeyDown(Keys.Down))
        {
            velocity += new Vector3(0, -0.3f, 0); // Aşağı yönde hız ekle
        }
        if (state.IsKeyDown(Keys.Left))
        {
            velocity += new Vector3(-0.3f, 0, 0); // Sola doğru hız ekle
        }
        if (state.IsKeyDown(Keys.Right))
        {
            velocity += new Vector3(0.3f, 0, 0); // Sağa doğru hız ekle
        }

        if (state.IsKeyDown(Keys.Q))
        {
            SetEndParticlePosition(new Vector3(4,10,0));
        }


        SetEndParticleVelocity(velocity * (float)e.Time);
    }


    public void StopIfEnergyLow(float threshold)
    {
        foreach (var particle in Particles)
        {
            particle.StopIfEnergyLow(threshold);
        }
    }
}
