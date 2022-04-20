# Glenn Korver - Code Samples - Portfolio

## About

This is a code sample repository, to be viewed by anyone interested, mostly intended for job applications.

Each sample was created with Unity Version 2021 or higher and includes a minimal project folder to be dragged and dropped into unity 


## Unity C#

### [Cards Creation - Data driven](https://github.com/GlennPr/Portfolio/tree/master/Unity-C%23/Cards-DataDriven)
A simple setup of how to use Scriptable objects as data containers to drive the creation of playcards.

Additionally an interface is implemented to easliy layout the drawn cards.

![CardsCreation](https://user-images.githubusercontent.com/15729395/164309788-99b80bda-7fc1-4de6-8465-75de5c65f5a8.PNG)

### Scriptable References
Use Scriptable objects as Reference and Event holders to be used across multiple objects.

Some of the benefits include:
- Avoid the need for scripts to reference eachother just to catch simple events
- Allow for easily dynamically filled-in references

Aditionally there is a MaterialManager to keep material instancing to a minimum and only create new materials when needed.

### [MaterialRenderOrder](https://github.com/GlennPr/Portfolio/tree/master/Unity-C%23/EditorWindow-MaterialRenderOrder)
An EditorWindow to display and organize shaders & materials within the project.

This provides a developer a clearer overview of the order in which materials are rendered and allows for direct editing of the renderQueue.

![MaterialRenderOrder1](https://user-images.githubusercontent.com/15729395/164308213-978997f8-1045-4de5-92bc-c7453987eccf.PNG)
![MaterialRenderOrder3](https://user-images.githubusercontent.com/15729395/164308257-b8d0b169-0a26-4d74-98a0-8fc0e5bb882f.PNG)
![MaterialRenderOrder2](https://user-images.githubusercontent.com/15729395/164308266-b12b7832-9a61-4ad8-80a7-1f280a7e2542.PNG)


### Tower Defence
A simple tower Defence game showcasing general project structure and script interactions in an OOP fashion.


## Unity Shaders

### [IntersectionVisualizer](https://github.com/GlennPr/Portfolio/tree/master/Unity-Shader/IntersectionVisualizer)
Shader using the stencil buffer and depth information to figure out if a mesh is within the volume of another mesh, the mesh entered will be highlighted with a configurable pixel sharp pattern.

![IntersectionVisualizer](https://user-images.githubusercontent.com/15729395/164309442-ca2bb610-3143-4c34-846b-c88886450882.PNG)


### [ChevronArrowSDF](https://github.com/GlennPr/Portfolio/tree/master/Unity-Shader/ChevronArrowSDF)
Shader using simple SDF box logic to create a moving / bending Chevron (vertically & horizontally), as commonly seen in UI.

![ChevronArrowSDF](https://user-images.githubusercontent.com/15729395/164309055-d57ef00f-7688-46ad-aa0e-1f542dbb5eb2.gif)


### Motion Vector Particles
2D particles perfectly following animated 3D geometry by their Motion Vectors, with a bunch of settings to control their look and behaviour.
