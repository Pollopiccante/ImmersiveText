# Progression Report: Immersive Text

## Task in General
The general task, as I understood it, was to prototype creative, generalizable, immersive and efficient ways of displaying text in VR.

## Brainstormed Ideas
Multible Ideas of what exactly to implement were explored in the beginning:

* Path related ideas
    * Text along a path to let the user explore an area while reading
    * Diverging storylines that happen simultaniously (in the story, and in space)
    * Turbulence to create a storm of words flowing along a Path 
* Fractal related ideas
    * Letters built of smalller Letters
    * Buildings, Sculptures, Labyrinths of Letters
    * 3d Path Fractals of Letters
* LLM related ideas
    * diving into made up substories on demand, based on a selected part of the original story
    * automatic styling of generated text (let LLM assess meaning, style accordingly)
    * LLM-Agents that live invisibly in the scene, only their thoughts / actions are visible through floating letters
* Inserting spoken words of the user into the scene
* Replacing scene objects with clusters of words that make them up (Door being replaced by "Door")
* A whole book in one scene: roughly 5 letters per word, 200.000 words (Moby Dick): 1 million meshes

## Direction
While exploring some of these Ideas in practice I decided to aim for the ideas not related to LLMs first, as the method for rendering text would lay the basis for most of the more complex ideas. But I keept some of them in mind and tried to include methods to enable them later. I decided to go into a "basics" direction with this project, trying create an intuitive tool to enable the creation of text mesh renderings. 

## Failed Explorations
Things I tested that did not make it into the final Project include:

### Text Mesh Pro
Contrary to what the name would make you belive, the package Text Mesh Pro is not able to display 3d Meshes of text, only 2d billboards located at a 3d position.  

### Turbulence
I tested Signed Distance Fields in Combination with Turbulence Noise in the VFX Graph.
I was not able to chain together letters and their successors to make the "letter storm" readable in any way.
For that reason I did not explore this idea further.

### Random Path
I created a Random Path Generator that bounces off of a specified Signed Distance Field. It is included in the project but is not used at runtime, only to create path-SOs.

### Static Meshes For Letter Display
Static meshes were employed as a first test of how to align letters in a shaped way. A conversion script was created, that could be attached to a mesh (a convex one) to convert it into text. This conversion was done by shooting a ray from the central axis of the mesh to the outside while incrementing the angle of the ray. This would basically create a spiral ontop of the original faces, which would server for a baseline to put text on.
This method yielded plesant visual results, but was not flexible (relied on convex meshes with big faces).
Also it was quickly visible that its performance was not good enough to entertain one million meshes.

### VFX Graph
The most substancial problem that arouse using the VFX Graph was the limited number of meshes allowed per particle (4), and the limited number of output contexts (16). As these properties cap the number of possible meshes to (64: 4*16) a workaround had to be found to use a full set of characters.
#### VFX Graph - 4 Mesh per Particle Rewrite
I tried to edit the local VFX-Graph packages code to allow for more than 4 meshes per output particle. And while I succeded to do so in the UI of the VFX-Graph, I was not able to edit the code in a way to successfully compile a VFX-Graph. Right now I am not entirely sure, but suspect that this rewrite is not possible in a simple way, but requires additional multi mesh support to be implemented deeper in the package.
#### VFX Graph - Shader Graph Submesh Filtering
One method of evading the 4 mesh per output context cap, is to use a mesh with submeshes and collapsing the vertices of all but one submesh in the shader graph. While this method souded promising, as it would reduce the number of output contexts to one (32 submeshes per mesh: 4*32 submeshes per output context), its performance made it impossible to justify.

## Static Mesh, VFX Graph, Manual Draw Calls
Out of the three mentioned methods I only tested the first two, and only researched manual draw calls online.
After failing to use static meshes with decent performance, the VFX Graph seemed like the best solution. Mainly because it automatically runs on the GPU and is parallelized on a per particle basis. Having little hands on experience with programming for GPUs I went with the VFX-Graph for the rest of the project.

## Datastructures and Tools
These are the Datastructures and Tools used to implement the Text VFX-Effect. 

### Font to Mesh Converter
To use multible character sets, a blender script was created to read tff files and output extruded meshes.
(in the repo, under /BlenderFontTo3d)
### 3d Path System
To create a flexible way of displacing text in space while keeping its natural one-dimensional flow, I aimed to create a "3d path" abstraction. This would represent a line floating in space on which to put text. In practice a LineRenderer component was used to display the 3dPath in the scene, and additional information about the rotation of the line sections was included. This way text could be for example spiraled around the line, or be written upside down. 

With this system in place any desirable static shape could be modeled by wrapping a line around it. One limit being that 3dPaths do not allow for gaps at this moment. With gaps enabled this model could even handle ideas like the diverging storylines. Without it can already handle most of the path related and fractal related ideas.

#### Scriptable Object: Path
To make 3dPaths easy to work with I represented them as Scriptable objects, and added a custom editor window.
This editor Window allows for the creation of a simple version of a VFX-Effect to test a path. It also shows the rotational data of the path using custom gizmos, and has an interface to step through that path and apply subpath insertions. 

#### Subpath insertion
This feature was created to further enable the idea of exploring a text along a path through for example a real building.
It allows for the creation of a "main path" that traverses some area, but is replaced in certain parts by more complex subpath shapes.

#### Path to Effect Data
The main feature of the 3dPath object is of course to calculate the positions of a text snippit. This is done by calculating the individual width of every letter, treating it as a subpath of this length, and inserting it. After every letter has been inserted as a linear sub-path, the resulting main-path contains the positional and rotational values to render the Effect.

#### Gcode Path
A rudementary parser was created to convert GCode data be treated as a 3dPath. This allows for the easy creation of complex arrangements like Buildings / Sculptures. As the 3dPath object does not support holes, this can be more messy than expected for some GCode files.

### Annotation System

#### Annotation Data
With the LLM related ideas in mind I wanted to create a text based method of interfacing the artistic design process for a text arranement. I decided to use an annotation system. Special symbols: "$(", "|" and ")$" delimit special sections of the used text that have an annotation value. Values include basic types like floats, strings, bools. A copy of the original file must be created for every new annotation type. The annotation data is collected into a dictionary per letter and can then be processed further. For example, a letters data could look like this: {"sadness": 0.78, "spokenByMainCharacter": false}.

#### Mapping
This data is then mapped onto the display dimensions by a user specified mapping. In this mapping the user can freely translate annotated data to display properties. It is a single letter translation at the moment, so data of successor or predecessor letters can not be taken into account. This system allows for the decopling of the texts meaning and its displayed styling in the scene. A texts annotation can be "interpretet" with different mappings. Furthermore it might be accessable by LLMs in the future. Annotations and therefore styling could be created automatically without concerning the LLM with the detailed effect-features, ultimately simplifying the prompts used.

#### Effect Features
Effect features currently include: 
* Scale
* Fly in / out timing
* Wave movement effect parameters (amblitude, frequence)
* Submesh-Path
* Color, alpha
* Shader parameters like metallic shine

### VFX Graph Effect
#### VFX Graph Effect / SO
The Scriptable objects of an VFX graph effect encapsulates an effect that can be inserted into a scene. It contains point cache texture data that represents that various effect-feature parameters (scale, position etc.), a reference to a mesh alphabet and the used vfx effect.

The VFX Graph itself solves the mentioned 4 mesh limitation, by rewriting the the local VFX-package to allow for up to 64 output contexts. Multible mesh output particles are then used to represent every character. The character texture toggles off all but one of these output particles to display only the needed character.

Two subgraph programms allow for simple access of the input textures. A shader graph node is used to implement the wave motion, metalic shine, color and alpha properties. Position and rotation are simply set in the particle output nodes.

#### VFX Graph Reader, Index Control
The VFX-Reader script and Index Control script are additional scripts used to controll the reader and time index parameters of the effect. They increments the reading and time index. The reader script also checks the players proximity to an effect and highlights parts of the path that are close enough to start a reading action by creating a sphere. A highlighted text part can then be forced onto a line in front of the player to allow easier reading.

#### Performance Tests VFX Graph
I tested the VFX-Graph Effects using the unity profiler on my laptop that only has an integrated GPU. For large effects the framerate got pretty low (around 10 and lower). Profiling showed that the application was GPU bounded. 

## Conclusions / Futher Research
The VFX-Graph should be tested in a VR enviroment to see if the mentioned output context workaround is compatible with the hardware. If so, the performance should be tested to see if it is worthwhile to employ the VFX-Graph for a huge amount of text meshes. If these tests are reasonably successful, the VFX-Graph can be considered at least a good middle ground solution for immersive text scenes in my opinion. It offers a lot of builtin functionality that was not used to its fullest in this project, but lacks some flexibility in other areas (mesh count etc.). 

The implemented 3d-Path abstaction proved useful throughout the project. It is independent of the used display method of the VFX-graph, and allows for the programmatic and editor based creation of arrangements. Arguably, the 3dPath relies on the CPU to preprocess the letter positions and rotations. Also the concept should be advanced further by including gaps.

The Blender script for font conversion is a small piece of this project but it could be reused.

A demo scene containing two complex effects was created (3d-printed ship, dragon curve fractal), that shows the capabilities of the prototype. Furthermore an asset package of the project was created, after I failed to create an embeded package.

This project is ultimately unfinished and only a first idea of how to implement large text displays in VR with some useful concepts that could be included in further projects. 
