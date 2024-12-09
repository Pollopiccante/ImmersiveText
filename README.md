## Architext

This prototypical package uses the VFX graph of Unity to display text along a specified three dimensional path. 
Multiple features of the letters appearance can be changed using textual notations, including:
- color + alpha
- scale
- back and forth movement
- metallic shine
- fade in / fade out timing

## How to install:

### 1. Install VFX-Graph package, Install URP package
Open the package manager (Window/Package Manager) and install the two packages:
"Universal RP 14.0.11" and "Visual Effects Graph 14.0.11"
### 2. Apply this Asset package
Download Artifacts/ArchitextPrototype.unitypackage. Unpack it into your Project by selecting Assets/Import Package
### 3. Select URP-Asset in Project Settings
Open the Project Settings, select the section "Graphics". 
Select the URP-Asset located under Assets/Architext/URP/.
### 4. 16->64 Workaround
Open the File Packages/VisualEffectsGraph/Editor/Data/VFXData.cs. 
Change Line 409 from:

```if (contextCount > 16)```

to

```if (contextCount > 64)```

and save.

### 5. Open VFX Effect
Navigate to Assets/Architext/Resources/VFXEffect open the file "StaticWorldMeshEffect". 
Save the file after opening, so it is recompiled. Now the installation is Done.

### 6. Check if everything works
Navigate to Assets/Architext/DataObjects/VFXObjects inspect one of the SciptableObjects and generated them into the scene.


## Demo Scene

Press escape in the demo scene to open the menu. You can move around with WASD, space to go up and shift to go down. Left click toggles the reading mode. The reading index is set to the index of the closest letter (within a certain range indicated by a sphere appearing) when toggeling reading mode.
