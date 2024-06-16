# 3D Physics Engine

This project is a 3D physics engine developed using C# and OpenTK. It features Phong lighting for realistic illumination, collision detection using the Separating Axis Theorem (SAT), and optimizations using Axis-Aligned Bounding Boxes (AABB) and Octrees.

## Features

### Phong Lighting
The Phong lighting model is implemented to simulate realistic lighting effects on 3D objects. It consists of three main components:
- **Ambient Lighting:** Provides a constant illumination to simulate indirect light scattered around the scene.
- **Diffuse Lighting:** Simulates the light scattered in all directions from a rough surface, providing the perception of depth.
- **Specular Lighting:** Simulates the bright spots of light that appear on shiny objects, enhancing the realism of reflective surfaces.

### Collision Detection
Collision detection is a critical feature of any physics engine. Our engine uses the Separating Axis Theorem (SAT) to detect collisions between convex shapes. The SAT works by projecting the shapes onto various axes and checking for overlaps. If no overlaps are found on any axis, the shapes are not colliding. This method is both efficient and reliable for real-time applications.

### Optimization Techniques
To ensure the engine runs efficiently, especially in complex scenes, we employ several optimization techniques:

#### Axis-Aligned Bounding Box (AABB)
AABB is a simple yet effective method for bounding objects with boxes aligned to the coordinate axes. These boxes simplify the collision detection process by allowing quick overlap tests. If the bounding boxes do not overlap, the objects inside cannot collide, thus saving computational resources.

#### Octree
An Octree is a hierarchical spatial partitioning structure that divides the 3D space into smaller regions (nodes). Each node can contain multiple objects, and nodes are recursively subdivided as needed. This structure significantly improves the performance of collision detection and other spatial queries by reducing the number of comparisons needed.

## Technical Overview

### Phong Lighting Implementation
The Phong lighting model enhances the visual realism of the 3D scene. Our implementation calculates lighting per vertex, interpolating across the surfaces of the objects for smooth shading. The following equations are used for the lighting components:

- **Ambient Component:**
  - `I_ambient = k_ambient * I_light`

- **Diffuse Component:**
  - `I_diffuse = k_diffuse * I_light * (L . N)`

- **Specular Component:**
  - `I_specular = k_specular * I_light * (R . V)^n`

  Where:
  - `I_light` is the intensity of the light source.
  - `L` is the light direction.
  - `N` is the surface normal.
  - `R` is the reflection direction.
  - `V` is the view direction.
  - `n` is the shininess coefficient.

### SAT Collision Detection
The SAT collision detection method checks for potential separation axes between two convex shapes. If a separating axis is found where the projections of the shapes do not overlap, it is concluded that the shapes do not collide. This method involves:

1. **Generating Axes:** Axes are derived from the edge normals of the shapes.
2. **Projecting Shapes:** Each shape is projected onto each axis.
3. **Checking Overlaps:** Overlaps between projections on each axis are checked. If all projections overlap, a collision is detected.

### AABB and Octree for Optimization
To optimize collision detection, AABBs and an Octree structure are used:

- **AABB:** Each object is encapsulated in an AABB, simplifying the initial collision checks. These checks quickly eliminate non-colliding pairs without detailed shape comparisons.
- **Octree:** The 3D space is divided into nodes, with each node potentially containing multiple objects. The Octree helps in narrowing down the number of objects to be checked for collisions by focusing only on those within the same or neighboring nodes.

## Conclusion
This 3D physics engine demonstrates the integration of advanced lighting, robust collision detection, and effective optimization techniques. It provides a solid foundation for developing more complex simulations and interactive 3D applications.
