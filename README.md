# YOPO-Sim

A high-performance, multi-sensor supported off-road environment simulator based on Unity engine.

## Supported Unity Version
Unity 2022 or higher.

## Sensor Support
Provided by [UnitySensors](https://github.com/Field-Robotics-Japan/UnitySensors).
- RGB Camera
- Depth Camera
- Fisheye Camera with full support for:
  - Equidistant model 
  - EUCM model 
- Lidar
- IMU

<!-- Sensor video -->
https://github.com/user-attachments/assets/30a0dbd8-22fe-4592-9f34-5371e3ac14e9

## Randomized Environment
Provided by [Vista](https://assetstore.unity.com/packages/tools/terrain/procedural-terrain-hexmap-vista-personal-edition-297327).
- Terrain
- Trees

<!-- Randomized environment video -->
https://github.com/user-attachments/assets/b92b3cd3-88b8-4fed-8599-bd7ade1459f1

## Point Cloud Map Generation
It is separated into a different repository [VoxelGenerator](https://github.com/TJU-Aerial-Robotics/VoxelGenerator).

<!-- Point Cloud Map Generation -->
https://github.com/user-attachments/assets/25109004-25b0-4515-b65d-24ef552c27fd

## Data Acquisition
- Image
- Position
- Orientation
- Point Cloud Map

<!-- Data Acquisition -->
https://github.com/user-attachments/assets/418ebe5f-97aa-4e66-9375-4a68926ac15c

## Core Scripts
- **DataGenerationManager.cs**: Manages the data generation process.
- **FilePathManager.cs**: Manages file paths for data storage.
- **GameSettings.cs**: Handles game settings.
- **GoalPublisher.cs**: Publishes goal data for navigation.
- **ImageSampler.cs**: Samples images from various camera sensors.
- **ScenePointCloudSaver.cs**: Saves the point cloud of the entire scene.
- **TomlDataManager.cs**: Manages sampled data using TOML files.
- **DiabloControl**: Controls the Diablo vehicle in the simulation.

## How to Use
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/YOPO-Sim-WIP.git
   ```
2. Open the project in Unity Hub.
3. Acquire and import the third-party packages. All are free in the Unity Asset Store:
    - [Vista](https://assetstore.unity.com/packages/p/procedural-terrain-hexmap-vista-personal-edition-297327)
    - [Unity Terrain - URP Demo Scene](https://assetstore.unity.com/packages/p/unity-terrain-urp-demo-scene-213197)
3. Load the evaluation scene from `Assets/Scenes/EvaluationScene`, and the data generation scene from `Assets/Scenes/DataGenerationScene`.
4. Run the simulation.