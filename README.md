# Glenn Korver - Code Samples - Portfolio

## About

This is a repository with simple & small scale code samples to reflect coding style and general project structure, to be viewed by anyone interested, but mostly intended for job applications.

Each sample was created with Unity Version 2021 or higher and includes a minimal project folder to be dragged and dropped into unity.

To get a full overview of my portfolio visit https://glennkorver.weebly.com/ 


## Unity C#

### [Cards Creation - Data driven](https://github.com/GlennPr/Portfolio/tree/master/Unity-C%23/Cards-DataDriven)
A simple setup of how to use Scriptable objects as data containers to drive the creation of playcards.

Additionally an interface is implemented to easliy layout the drawn cards.

![CardsCreation](https://user-images.githubusercontent.com/15729395/164309788-99b80bda-7fc1-4de6-8465-75de5c65f5a8.PNG)


### [Scriptable References](https://github.com/GlennPr/Portfolio/tree/master/Unity-C%23/Scriptable-References)
Use Scriptable objects as Reference and Event holders to be used across multiple objects.

Some of the benefits include:
- Avoid the need for scripts to reference eachother just to catch simple events
- Allow for easily dynamically filled-in references

Aditionally there is a MaterialManager to keep material instancing to a minimum and only create new materials when needed.


### [MaterialRenderOrder](https://github.com/GlennPr/Portfolio/tree/master/Unity-C%23/EditorWindow-MaterialRenderOrder)
An EditorWindow to display and organize shaders & materials within the project. **Must** use unity 2021 or higher due to changes by Unity regarding the EditorWindow class.

This provides a developer a clearer overview of the order in which materials are rendered and allows for direct editing of the renderQueue.

![MaterialRenderOrder1](https://user-images.githubusercontent.com/15729395/164308213-978997f8-1045-4de5-92bc-c7453987eccf.PNG)
![MaterialRenderOrder3](https://user-images.githubusercontent.com/15729395/164308257-b8d0b169-0a26-4d74-98a0-8fc0e5bb882f.PNG)
![MaterialRenderOrder2](https://user-images.githubusercontent.com/15729395/164308266-b12b7832-9a61-4ad8-80a7-1f280a7e2542.PNG)


### [Tower Defence OOP](https://github.com/GlennPr/Portfolio/tree/master/Unity-C%23/TowerDefence-OOP/MinimalProject/Assets)
*An endless heard of space bunnies will attack your base, your defence will try to stop them, but it is inevitable.* 

A simple tower Defence game showcasing general project structure and script interactions in an OOP fashion.

Mobs are spawned by independent prefab spawners, which will then hop towards a target from their self contained singleton manager.
Turrets derived from a common base class will detect these mobs and intercept them.
All damage interactions between objects are implemented through the IDamagable interface.


![TowerDefence](https://user-images.githubusercontent.com/15729395/164556726-80fdd160-a8f4-41a3-8d14-69a4f379c246.PNG)



## Unity Shaders

### [IntersectionVisualizer](https://github.com/GlennPr/Portfolio/tree/master/Unity-Shader/IntersectionVisualizer)
Shader using the stencil buffer and depth information to figure out if a mesh is within the volume of another mesh, the mesh entered will be highlighted with a configurable pixel sharp pattern.

![IntersectionVisualizer](https://user-images.githubusercontent.com/15729395/164309442-ca2bb610-3143-4c34-846b-c88886450882.PNG)


### [ChevronArrowSDF](https://github.com/GlennPr/Portfolio/tree/master/Unity-Shader/ChevronArrowSDF)
Shader using simple SDF box logic to create a moving / bending Chevron (vertically & horizontally), as commonly seen in UI.

![ChevronArrowSDF](https://user-images.githubusercontent.com/15729395/164309055-d57ef00f-7688-46ad-aa0e-1f542dbb5eb2.gif)


### [Motion Vector Particles](https://github.com/GlennPr/Portfolio/tree/master/Unity-Shader/ParticlesMotionVectors)
2D particles perfectly following animated 3D geometry by their Motion Vectors, with a bunch of settings to control their look and behaviour.

![ParticleMotionVectors](https://user-images.githubusercontent.com/15729395/164328223-9915cb1a-e445-4261-85b5-7942f790a015.PNG)
