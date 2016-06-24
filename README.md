# PlayClubVR

This is a mod for Play Club that introduces VR capabilities for both the Vive and the Oculus Rift using OpenVR. It provides both a seated and a standing mode to be usable in any environment.


## Installation

1. Extract files into your *PlayClub* directory.
2. Run *PlayClubVR.exe* or *PlayClubStudioVR.exe*
3. Enjoy!

**Caution:** SteamVR needs to be installed, set up, and running! Rift users might otherwise experience a weird "monitor" effect.

## Modes & Controls

PlayClub comes in two *modes*:

| Mode        | Description         |
| ----------- | ------------------- |
| Seated      | *Default mode.* This mode lets you play the game with a mouse, keyboard, or gamepad.<br />The controls are essentially the same as in the main game. The screen is presented on a big monitor in front of you. |
| Standing    | As soon as *tracked controllers* are registered by the game, it switches into *standing mode*, also called *room scale mode*. In this mode, you can freely move around and use your Vive or Touch controllers to do cool stuff. |


### Seated Mode

As stated earlier, the controls are basically the same as in the main game with the exception of a few VR-related shortcuts. You are presented with a screen in front of your that replaces your monitor and can be configured via the settings via shortcus (see below).

#### Keyboard Shortcuts

Keys      | Effect
----      | ------
<kbd>Ctrl</kbd>+<kbd>C</kbd>, <kbd>Ctrl</kbd>+<kbd>C</kbd> | Change to *standing mode*.
<kbd>Ctrl</kbd>+<kbd>C</kbd>, <kbd>Ctrl</kbd>+<kbd>V</kbd> | Enable (very experimental) third person camera. [Was used for this video](https://www.youtube.com/watch?v=0klN6gd1ybM).
<kbd>Alt</kbd>+<kbd>S</kbd> | Save settings (IPD, screen position, etc.).
<kbd>Alt</kbd>+<kbd>L</kbd> | Load settings (last saved state).
<kbd>Ctrl</kbd>+<kbd>Alt</kbd>+<kbd>L</kbd> | Reset settings to the initial state.
<kbd>F4</kbd> | Switch GUI projection mode (flat, curved, spherical).
<kbd>F5</kbd> | Toggle camera lock (enabled by default). This prevents the camera to *tilt* because such movements are known to cause cyber sickness.
<kbd>Ctrl</kbd>+<kbd>F5</kbd> | Apply shaders (only for the brave)
<kbd>F12</kbd> | Recenter
<kbd>Ctrl</kbd>+<kbd>X</kbd> | Impersonate protagonist. Places the camera at the head position of the protagonist and moves along a little.
<kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>X</kbd> | Impersonate protagonist. Places the camera at the head position of the protagonist and imitates *all* head movements.
<kbd>NumPad +</kbd> <br /> <kbd>NumPad –</kbd> | Move GUI up / down.
<kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>NumPad +</kbd> <br /> <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>NumPad –</kbd> | Move GUI left / right
<kbd>Ctrl</kbd>+<kbd>NumPad +</kbd> <br /> <kbd>Ctrl</kbd>+<kbd>NumPad –</kbd> | Increase / decrease GUI size.
<kbd>Alt</kbd>+<kbd>NumPad +</kbd> <br /> <kbd>Alt</kbd>+<kbd>NumPad –</kbd> | Increase / decrease player scale
<kbd>Shift</kbd>+<kbd>NumPad +</kbd> <br /> <kbd>Shift</kbd>+<kbd>NumPad –</kbd> | Increase / decrease GUI distance

#### Gamepad Bindings

Keys      | Effect
----      | ------
<kbd>Move LS</kbd> | Control cursor
<kbd>Move RS</kbd> | Rotate & zoom
<kbd>LT</kbd>+<kbd>Move LS</kbd> | Pan left/right, up/down
<kbd>LT</kbd>+<kbd>Move RS</kbd> | Rotate camera
<kbd>RT</kbd>+<kbd>Move LS</kbd> | Pan left/right, foward/backward
<kbd>RT</kbd>+<kbd>Move RS</kbd> | Rotate camera
<kbd>A</kbd> | Left click
<kbd>B</kbd> | Right click
<kbd>X</kbd> | Toggle "grind" animation
<kbd>Y</kbd> | Toggle "piston" animation
<kbd>Back</kbd> | Toggle GUI
<kbd>DPad Up</kbd> / <kbd>DPad Down</kbd> | Change animation speed
<kbd>DPad Left</kbd> / <kbd>DPad Right</kbd> | Change position (prev / next)
<kbd>Press RS</kbd> | Come inside
<kbd>Press LS</kbd> | Come outside
<kbd>LB</kbd>+<kbd>LS Horizontally</kbd> | Move GUI left / right
<kbd>LB</kbd>+<kbd>LS Vertically</kbd> | Move GUI up / down
<kbd>LB</kbd>+<kbd>RS Horizontally</kbd> | Change GUI distance
<kbd>LB</kbd>+<kbd>RS Vertically</kbd> | Change GUI width
<kbd>LB</kbd>+<kbd>DPad Up</kbd> / <kbd>LB</kbd>+<kbd>DPad Down</kbd> | Change player scale
<kbd>LB</kbd>+<kbd>Y</kbd> | Impersonate protatonist

### Standing Mode

The *standing mode* is where things start to get interesting. This mode is pretty much disconnected from the usual game in that it comes with its very own controls -- although you can still use your mouse and your keyboard.

#### Keyboard Shortcuts

Keys      | Effect
----      | ------
<kbd>Ctrl</kbd>+<kbd>C</kbd>, <kbd>Ctrl</kbd>+<kbd>C</kbd> | Change to *standing mode*.
<kbd>Ctrl</kbd>+<kbd>C</kbd>, <kbd>Ctrl</kbd>+<kbd>V</kbd> | Enable (very experimental) third person camera. [Was used for this video](https://www.youtube.com/watch?v=0klN6gd1ybM).
<kbd>Alt</kbd>+<kbd>S</kbd> | Save settings (IPD, screen position, etc.).
<kbd>Alt</kbd>+<kbd>L</kbd> | Load settings (last saved state).
<kbd>Ctrl</kbd>+<kbd>Alt</kbd>+<kbd>L</kbd> | Reset settings to the initial state.
<kbd>Ctrl</kbd>+<kbd>F5</kbd> | Apply shaders (only for the brave)
<kbd>Alt</kbd>+<kbd>NumPad +</kbd> <br /> <kbd>Alt</kbd>+<kbd>NumPad –</kbd> | Increase / decrease player scale

#### Impersonation

*Impersonating* someone is as easy as moving at the position of that character's head. The head will disappear and you can pretend to be him / her. By setting `<FullImpersonation>` you can even control their hands!

#### Interaction

Interacting is currently only possible either with the conductor tool (which lets you move around the limbs of characters), or by touching the breasts, hair or skirts of girls. They will then start to wiggle more or less realistically, but it can be fun.

## Tools

These tools are mainly meant to be used in *standing mode* but some of them are also available in *seated mode*.

### Menu Tool (seated / standing)

STUB

### Warp Tool (standing)

STUB

### Play Tool (standing)

STUB

### Conductor Tool (seated / standing)

**Caution:** Requires [Maestro Mode](hongfire.com/forum/showthread.php/440160-%28Illusion%29-Play-Club?p=3667135#post3667135) to work in the main game!

STUB

## Settings & Tweaks

Settings can be changed in the file *vr_settings.xml*, which is generated the first time you start the game. Use `RenderScale` to tweak the resolution, **not** the internal resolution dialog, as that one will currently only change the resolution of the GUI.

Tag      | Default | Effect | Mode
----     | ------  | ------ | ----
`<Distance>` | 0.3 | Sets the distance between the camera and the GUI at `[0,0,0]`. | Seated
`<Angle>` | 170 | Sets the width of the arc the GUI takes up.  | Seated
`<IPDScale>` | 1 | Sets the scale of the camera. The higher, the more gigantic the player is. | Seated / Standing
`<OffsetY>` | 0 | Sets the vertical offset of the GUI in meters. | Seated
`<Rotation>` | 0 | Sets by how many degrees the GUI is rotated (around the y / up axis) | Seated
`<Rumble>` | True | Sets whether or not rumble is activated. | Seated / Standing
`<RenderScale>` | 1 | Sets the render scale of the renderer. Increase for better quality but less performance, decrease for more performance but poor quality. | Seated / Standing
`<MirrorScreen>` | False | Sets whether or not the view should be mirrored in the game window. | Seated / Standing
`<FullImpersonation>` | False | Sets whether or not you should take control over the character's hands whenever you impersonate someone. | Standing

## Building PlayClubVR

PlayClubVR depends on the [VRGIN.Core](https://github.com/Eusth/VRGIN.Core) library which is included as a submodule. It is therefore important that when you clone the project, you clone it recursively.

```
git clone --recursive https://github.com/Eusth/PlayClubVR.git
cd PlayClubVR
```

After cloning the repo and setting up the submodule, you should be able to compile the project by simply opening the *.sln file and building.

Note that there is a build configuration called "Install" that will extract your Play Club install directory from the registry and copy the files where they belong. 
