﻿using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PhysicsEngine;

namespace PhysicsEngine
{

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

        public bool isRigidBody=false;
        public RigidBody body;

        public VerletParticle(Vector3 initialPosition, float mass = 1.0f, bool isFixed = false, bool isControlled = false)
        {
            Position = initialPosition;
            PreviousPosition = initialPosition;
            Mass = mass;
            IsFixed = isFixed;
            IsControlled = isControlled;
            Shape = new Cube(initialPosition, ShapeShaderType.ColorLight, Color4.Chocolate, new Vector3(.5f));
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
            Vector3 velocity = Position - PreviousPosition;// bu önemli

            Vector3 gravity = new Vector3(0, -9.81f, 0); // Yerçekimi kuvveti

            velocity *= 0.99f;
            Acceleration *= 0.99f;

            velocity += gravity * deltaTime * 0.02f; // Yer çekimi kuvvetini ekle (hayvani bir sonuç verdiği için *0.001f)

            Position = Position + velocity + Acceleration * (deltaTime * deltaTime);

            //Console.WriteLine(velocity);


            Shape.RenderBasic();
            Shape.Teleport(Position);
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
            Vector3 correction = delta * .7f * difference;

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

        float time;

        public StringCubes(Vector3 firstPosition)
        {
            Particles = new List<VerletParticle>();
            Constraints = new List<Constraint>();
            CreateStringCubes(firstPosition);
            AttachRigidBody();
        }

        public void AttachRigidBody()
        {         
            var previousParticle = Particles.Last();
            RigidBody.CreateSphereBody(
                0.5f,
                previousParticle.Position+new Vector3(0,-1,0),
                0.7f,
                false,
                0.7f,
                Color4.AliceBlue,
                out RigidBody body
                );

            VerletParticle particle = new VerletParticle(body.position);
            particle.Shape = body.shape;
            particle.isRigidBody = true;
            particle.body = body;
            particle.IsFixed = false;


            float restLength = (particle.Position - previousParticle.Position).Length;
            Particles.Add(particle);
            Constraints.Add(new Constraint(previousParticle, particle, restLength));


        }

        public void CreateStringCubes(Vector3 firstPosition)
        {
            VerletParticle previousParticle = null;

            for (int i = 0; i < 10; i++)
            {
                bool isFixed = (i == 0);
                bool isControlled = (i == 9);// son küp için

                VerletParticle particle = new VerletParticle(firstPosition, 1.0f, isFixed, isControlled);
                Particles.Add(particle);

                if (previousParticle != null)
                {
                    float restLength = (particle.Position - previousParticle.Position).Length;
                    Constraints.Add(new Constraint(previousParticle, particle, restLength));
                }

                previousParticle = particle;
                firstPosition += new Vector3(0, -0.8f, 0);
            }
        }

        public void Update(float deltaTime)
        {
            this.time = deltaTime;

          
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
                endParticle.Position += newPosition * time;


            }
        }
        private void SetEndParticlePosition(Vector3 newPosition)
        {
            VerletParticle endParticle = Particles.Last();
            if (endParticle.IsControlled)
            {
                endParticle.Position = newPosition;

            }
        }

        public void UpdateVelocity(KeyboardState state, FrameEventArgs e)
        {


            Vector3 velocity = Vector3.Zero;
            if (state.IsKeyDown(Keys.Up))
            {
                velocity += new Vector3(0, 03f, 0); // Yukarı yönde hız ekle
            }
            if (state.IsKeyDown(Keys.Down))
            {
                velocity += new Vector3(0, -03f, 0); // Aşağı yönde hız ekle
            }
            if (state.IsKeyDown(Keys.Left))
            {
                velocity += new Vector3(-03f, 0, 0); // Sola doğru hız ekle
            }
            if (state.IsKeyDown(Keys.Right))
            {
                velocity += new Vector3(03f, 0, 0); // Sağa doğru hız ekle
            }

            if (state.IsKeyDown(Keys.Q))
            {
                this.Particles.Last().Position = new Vector3(-5, 15, 0);
            }

            this.Particles.Last().Position += velocity;


        }

        public void StopIfEnergyLow(float threshold)
        {
            foreach (var particle in Particles)
            {
                particle.StopIfEnergyLow(threshold);
            }
        }
    }






    //public class Constraint
    //{
    //    public RigidBody BodyA;
    //    public RigidBody BodyB;
    //    public float RestLength;

    //    public Constraint(RigidBody bodyA, RigidBody bodyB, float restLength)
    //    {
    //        BodyA = bodyA;
    //        BodyB = bodyB;
    //        RestLength = restLength;
    //    }

    //    public void SatisfyConstraint()
    //    {
    //        Vector3 delta = BodyB.position - BodyA.position;
    //        float currentLength = delta.Length;
    //        float difference = (currentLength - RestLength) / currentLength;
    //        Vector3 correction = delta * 0.5f * difference;

    //        if (!BodyA.isStatic)
    //        {
    //            BodyA.position += correction * 0.5f;
    //            BodyA.MoveTo(BodyA.position);

    //        }
    //        if (!BodyB.isStatic)
    //        {
    //            BodyB.position -= correction * 0.5f;
    //            BodyB.MoveTo(BodyB.position);
    //        }
    //    }
    //}

    //public class StringCubes
    //{
    //    public const int MIN_ITERATIONS = 1;
    //    public const int MAX_ITERATIONS = 128;

    //    PhysicsWorld World;

    //    public List<RigidBody> Bodies;
    //    public List<Constraint> Constraints;

    //    public StringCubes(Vector3 firstPosition, PhysicsWorld world)
    //    {
    //        World = world;
    //        Bodies = new List<RigidBody>();
    //        Constraints = new List<Constraint>();
    //        CreateStringCubes(firstPosition);
          
    //    }

    //    public void CreateStringCubes(Vector3 firstPosition)
    //    {
    //        RigidBody previousBody = null;

    //        for (int i = 0; i < 10; i++)
    //        {
    //            bool isStatic = (i == 0);
    //            bool isControlled = (i == 9); // son küp için



    //            RigidBody.CreateCubeBody(new Cube(firstPosition,ShapeShaderType.ColorLight, new Vector3(0.5f)),isStatic,0.7f,out RigidBody body);

    //            Bodies.Add(body);
    //            World.AddBody(body);

    //            if (previousBody != null)
    //            {
    //                float restLength = (body.position - previousBody.position).Length;
    //                Constraints.Add(new Constraint(previousBody, body, restLength));
    //            }

    //            previousBody = body;
    //            firstPosition += new Vector3(0, -1.5f, 0);
    //        }


    //    }

    //    public void Update(float deltaTime)
    //    {
    //        Vector3 gravity = new Vector3(0, -9.81f, 0);

    //        //// Update velocities and positions
    //        //foreach (var body in Bodies)
    //        //{
    //        //    if (!body.isStatic)
    //        //    {
    //        //        body.Update(deltaTime, gravity*deltaTime*deltaTime, 1);
    //        //    }
    //        //}

    //        // Satisfy constraints multiple times
    //        for (int j = 0; j < 10; j++)
    //        {
    //            foreach (var constraint in Constraints)
    //            {
    //                constraint.SatisfyConstraint();
    //            }
    //        }

    //        // Apply damping to velocities to simulate friction
    //        float damping = 0.99f;
    //        foreach (var body in Bodies)
    //        {
    //            if (!body.isStatic)
    //            {
    //                body.linearVelocity *= damping;
    //            }
    //        }
    //    }

    //    private void SetEndBodyVelocity(Vector3 newVelocity)
    //    {
    //        RigidBody endBody = Bodies[Bodies.Count - 1];
    //        if (!endBody.isStatic)
    //        {
    //            endBody.linearVelocity += newVelocity;
    //            endBody.Move(newVelocity);

    //        }
    //    }

    //    private void SetEndBodyPosition(Vector3 newPosition)
    //    {
    //        RigidBody endBody = Bodies[Bodies.Count - 1];
    //        if (!endBody.isStatic)
    //        {
    //            endBody.position = newPosition;
    //        }
    //    }

    //    public void UpdateVelocity(KeyboardState state, FrameEventArgs e)
    //    {
    //        Vector3 velocity = Vector3.Zero;
    //        if (state.IsKeyDown(Keys.Up))
    //        {
    //            velocity += new Vector3(0, 50f, 0); // Yukarı yönde hız ekle
    //        }
    //        if (state.IsKeyDown(Keys.Down))
    //        {
    //            velocity += new Vector3(0, -50f, 0); // Aşağı yönde hız ekle
    //        }
    //        if (state.IsKeyDown(Keys.Left))
    //        {
    //            velocity += new Vector3(-50f, 0, 0); // Sola doğru hız ekle
    //        }
    //        if (state.IsKeyDown(Keys.Right))
    //        {
    //            velocity += new Vector3(50f, 0, 0); // Sağa doğru hız ekle
    //        }

    //        if (state.IsKeyDown(Keys.Q))
    //        {
    //            SetEndBodyPosition(new Vector3(4, 10, 0));
    //        }

    //        SetEndBodyVelocity(velocity);
    //    }

    //    public void StopIfEnergyLow(float threshold)
    //    {
    //        foreach (var body in Bodies)
    //        {

    //        }
    //    }
    //}
}
