# TOADEngine
TOAD – Transform, Optimize, Animate, Deploy

Toad Engine is built to mimic the layout and structure of Unity as thats what I know best.

Added:
GameObject: Holds the model view, and handles drawing,
Behaviour: Very similar to unities MonoBehaviour these are components that are added directly to the GameObjects,
Physics: Using BepuPhysics Engine -> Colliders and Tick based Physics have been added, along with Raycasting.
UI: Using Guinevere for Immediate mode UI (In Game) and ImGUI for Editor UI.

TODO:
Write Game Engine Docs

Improve graphics: 
Point Shadows
Normal Mapping,
HDR Pipeline,
Bloom,
Deffered Shading,
SSAO

Work further on Sample Game -> Simple Platformer

Warning: This project isnt perfect nor, this is a learning process for me to get better at game developement and learning how game engines work in the background.
I have taken some shortcuts when it comes to Physics, and UI. I felt like it wasnt worth re-inventing the wheel after I learned the basics behind it.

Hopefully anyone who stumbles upon this project will find it as a learning experience as much as I have so far. If you have any comments on what I can improve feel free to leave a PR.
Thanks! - 
Xenial

Special Thanks to the open source libraries:

UI / GUI:
https://github.com/aybe/DearImGui
https://github.com/MASS4ORG/Guinevere

Physcis:
https://github.com/bepu/bepuphysics2
