# Road-Crossing-Simulator-1st-Perspective

## Overview
This project is a Unity-based pedestrian road-crossing simulator (1st Perspective) integrated with the CAREN system.
The simulator receives real-time biomechanical and motion data via TCP, such as treadmill speed and motion capture marker positions, and uses this information to control the movement of a virtual pedestrian in the simulation environment.

The system aims to replicate realistic walking and road-crossing behaviour by synchronising the player's movement with treadmill and motion capture data.

## Features
* Real-time TCP data streaming from CAREN system
* Integration of treadmill speed for forward movement control
* Motion capture marker tracking for player orientation and position
* Unity-based interactive simulation environment
* Real-time UI display of simulation information
* Data logging for experiment analysis

## Technologies Used
* Unity (C#)
* TCP Networking
* Motion Capture Integration
* Real-time Data Processing
* CSV Data Logging

## Requirements
* Unity Editor Version (Unity 6.3 LTS (6000.3.6f1))
* Windows
* CAREN system with motion capture and treadmill
* TCP data streaming enabled

## How It Works
1. The CAREN system streams biomechanical data via TCP.
2. Unity receives the incoming data in real time.
3. Treadmill speed controls the player's forward movement.
4. Motion capture marker positions are used to estimate head orientation and body direction.
5. The virtual pedestrian moves within the simulated road-crossing environment accordingly.

## How to Run
1. Clone the repository
2. Open with Unity Hub
3. Load the main scene
4. Press Play

## Author
Tianbao Deng  
Master of Biomedical Engineering  
University of Melbourne
