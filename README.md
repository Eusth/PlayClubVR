# PlayClubVR

This is a mod for Play Club that introduces VR capabilities for both the Vive and the Oculus Rift using OpenVR. It provides both a seated and a standing mode to be usable in any environment.

## Building PlayClubVR

PlayClubVR depends on the [VRGIN.Core](https://github.com/Eusth/VRGIN.Core) library which is included as a submodule. It is therefore important that when you clone the project, you clone it recursively.

```
git clone --recursive https://github.com/Eusth/PlayClubVR.git
cd PlayClubVR
```

After cloning the repo and setting up the submodule, you should be able to compile the project by simply opening the *.sln file and building.

Note that there is a build configuration called "Install" that will extract your Play Club install directory from the registry and copy the files where they belong. 