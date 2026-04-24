# Synapse

### Project Overview:

Synapse is an adaptive boss trainer created in Unity. It uses Proximal Policy Optimisation (PPO) reinforcement learning to train a boss opponent with adaptive movesets and real-time performance calibration to adjust attack patterns and aggression. This project aimed to test whether machine learning systems are efficient in systematic, failure-driven game development.

This project was created as part of a research assignment during my BSc in Game Development.

### Features:

- Boss opponent trained with OpenAI's PPO algorithm.
- Comprehensive third-person camera system.
- Event-driven input system to ensure rapid iteration.
- Unity ML-Agents, an open source tool allowing training to be conducted in Unity.

### Quick Start: 

To run Synapse, either clone the repo or download the .zip file and import it to Unity; the game requires Unity version 2022.3.62f2.

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
