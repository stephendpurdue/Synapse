# Synapse

### Project Overview:

Synapse is an adaptive boss trainer created in Unity. It uses Proximal Policy Optimisation (PPO) reinforcement learning to train a boss opponent with adaptive movesets and real-time performance calibration to adjust attack patterns and aggression. This project aimed to test whether machine learning systems are efficient in systematic, failure-driven game development.

This project was deployed as part of a research assignment during my BSc in Game Development.

### Features:

- Boss opponent trained with OpenAI's PPO algorithm.
- Comprehensive third-person camera system.
- Event-driven input system to ensure rapid iteration.
- Unity ML-Agents, an open source tool allowing training to be conducted in Unity.

### Quick Start: 

To run Synapse, either clone the repo or download the .zip file and import it to Unity; the game requires Unity version 2022.3.62f2.

If you wish to train your own boss opponent, simply import a new prefab, swap out the preset Zombie boss with your imported one, attach the relevant scripts, and begin training. You can then export the opponent and the YAML file once training concludes, it is advised to train for a minimum of 500,000 steps to ensure proper training. 

### Project Structure:


```text
Assets/
├─ Scripts/
│  ├─ AttackTracker.cs
│  ├─ GameScripts.asmdef
│  ├─ HealthSystem.cs
│  ├─ HealthUI.cs
│  ├─ PlayerAttackTracker.cs
│  ├─ PlayerController.cs
│  ├─ PlayerControls.cs
│  ├─ PlayerHealth.cs
│  ├─ ThirdPersonCameraController.cs
│  ├─ ZombieAgent.cs
│  └─ ZombieController.cs
├─ PlayerControls.cs
├─ Tests/
│  ├─ AttackTrackerTests.cs
│  ├─ HealthSystemTests.cs
│  ├─ MLAgentsInstallationTest.cs
│  └─ Tests.asmdef
└─ TextMesh Pro/
```
