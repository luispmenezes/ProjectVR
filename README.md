![Alt text](/logo.png?raw=true)

# Project VR #

Project VR is a modern project that approaches virtual reality in a different manner. Our project consists on incorporating various sensors in a way they can interact between them. They can either be working together or separately.
The first big iteration on the project will be to develop a Multiplayer world that allows different players to use various types of sensors like Oculus Rift, Leap motion, kinect etc and interact between them giving great immersiveness virtual reality experience. There will also be another iteration on the project regarding more specifically the Phantom Omni, his iteration will consist on trying to manipulate 3D skull models for medical purposes.
The project is developed using Unity 5 game engine, which is a great tool giving its users handful ways to create cool projects.

### Technical Documentation ###

**The unity engine**
Unity is a development environment that supports 2 major development assets, the unity editor and scripting. The editor is more focused on the design view of the project, while as the scripting is focused on the behavior of  the objects.

**The unity editor**
	Regarding the Remote_VR-Config project the editor has been mainly used to create gameobject prefabs, and insertion of static game objects, such as the NetworkManager. The editor also allows to associate components (such as scripts) to the gameobject or prefab. If a script is associated as a component to a gameobject, it is possible to link other component, gameobject or prefab references to a script’s input argument.

**Player prefab structure**
	Each type of player (defined by sensors) is required to have its own prefab in order to be instantiated correctly by the NetManager. The player prefab is required to have a script component associated to it (ObserverSensor Script) in order to manage input movement from the keyboard. This script will also be associated to a NetworkView component, meaning it is responsible for transmitting the transform data to the network.
	The player prefab also must have the RiftController prefab associated as his first children. This is due to the user’s choice to use the occulus rift as a visualization method. The remaining children are the prefabs necessary for the sensor to function with unity.
