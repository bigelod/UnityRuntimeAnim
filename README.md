# UnityRuntimeAnim
Basic runtime animations of transform values and variables in Unity that are loaded at runtime from a CSV format. No editor for animations other than manually editing them as text. Created as a proof-of-concept.

This is not for animating individual parts of a 3D model (eg: bones in a skeleton) but more for per-object animations, as it's being used as a reference point towards making a beginner-friendly animation feature for another project I am working on.

How it's meant to be used: Objects in-game assign themselves to "slots" on each animation, intended for use with another app that allows runtime configuration of 3D model-to-slot assignment

What is supported:
Hierarchy of objects
Local position, rotation, and scaling
Animating float values
Linear interpolation between values, or "instant" changes after keyframe reached
Looping animations (animation stops on last frame if not looping)
Changing to another animation after last keyframe (if looping is not enabled)

Unity version 2018.4.23f1
