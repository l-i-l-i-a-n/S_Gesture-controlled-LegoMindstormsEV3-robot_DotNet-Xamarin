# Lego Mindstorms EV3 robot, controlled by a MYO gesture recognition armband, and streaming onboard camera feed to phone

# Project presentation

The objective of this project is to design and program a Lego EV3 robot with Xamarin. 

The robot has a slot to hold a smartphone, that will be used as a camera filming the environment around the robot.
Our goal is that the user can see what the robot sees, through a "cardboard"-like VR headset (holding a second phone), so that the user put themselves in the robot's place.
When the user moves turns his head, the camera on the robot should rotate at the same time.

To control the robot, we plan to use a Myo gesture control armband. The Myo armband should allow the user to make the robot move: forwards, backwards, rotate right and left.

# Peripherals

* **The appropriate custom Lego EV3 robot**
* **A MYO gesture control armband:** to control the robot's movements, wear the Myo armband on the **right arm**
* **A cardboard VR headset:** to hold the phone display the robot's point of view
* **2 smartphones:** one to be used as a camera on the robot, and the other in the VR headset displaying the robot's images.

# How to run project

You should load this project with Visual Studio. Load it with RobotLegoXamarin

After loading you should run the FirstSampleApp.Android. For now only Android work.

You can run a simluator or Connect a phone to debug it at phone

## Known Issues

Don't put the project in a deep folder. Keep the path simple (Ex" C:/legoev3mindstorms). Otherwise you get compiling errors.

## Progression on the project

We made the robot with Lego and we can use the app to control the robot: Both the movement and the rotate of the phone holder on the robot.

### To-Do

* Still need to stream the camera feed from the phone on the robot to the phone in the headset

# Features

### Control the robot with Myo armband gestures
We use 4 native poses of the Myo armband to control the robot:
* **Fingers spread:** the robot moves forward
* **Fist:** the robot moves backward
* **Wave in:** the robot rotates left
* **Wave out:** the robot rotates left
Notice that each action will be performed continuously until the Myo armband detects a **Rest** event. 
For example, the robot will keep moving forward until you change your pose or rest.

### Control the robot's camera by moving your head
Simply turn your head and the camera held by the robot will rotate at the same time.
