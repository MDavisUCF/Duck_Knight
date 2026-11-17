# 🦆 Duck Knight
### A Tiny Knight. A Mighty Sword. A Kingdom to Defend.

**Duck Knight** is a 3D arcade survival game developed in Unity, featuring a fearless duck armed with an oversized sword and shield.

Players must survive increasingly chaotic waves of goblins and incoming fireballs while protecting their precious eggs. With both traditional keyboard and mouse controls and experimental physical motion capture support, Duck Knight combines accessible arcade gameplay with unconventional interaction design.

## ⚔️ The Game

**Survive 60 seconds. Protect your eggs. Defend the kingdom.**

- **Sword Combat:** Swing an oversized sword to defeat approaching goblins.
- **Shield Defense:** Activate your shield to intercept incoming attacks.
- **Progressive Difficulty:** Goblins spawn faster as the countdown approaches zero.
- **Egg Health System:** Four precious eggs stand between victory and defeat.
- **Multiple Enemies:** Battle several goblin variants and dodge incoming fireballs.
- **Dual Control Modes:** Play traditionally or wield physical tracked props.

## 🎮 Controls

### Keyboard & Mouse

| Input | Action |
|---|---|
| WASD | Move Duck Knight |
| Mouse | Aim and rotate |
| Left Click | Swing sword |
| Space | Activate shield (5 seconds) |
| R | Restart |
| M | Toggle control mode (debug) |

### Motion Capture

Duck Knight supports physical sword and shield interaction through the **OptiTrack Motive** motion capture system.

- **Tracked Sword:** Controls movement, orientation, and attacks.
- **Tracked Shield:** Provides defensive positioning and activation.
- **Independent Rigid Bodies:** Sword and shield use separate tracking IDs.
- **Tracking Awareness:** Lost tracking automatically disables the corresponding weapon's active state.

Before entering battle, players must ensure both props are available in Motive.

And, naturally, announce:

**"BEGIN, MY LIEGE!"**

## 🛠️ Built With

- **Unity 6** | Game engine
- **Universal Render Pipeline (URP)** | Rendering
- **C#** | Gameplay programming
- **OptiTrack Motive** | Motion capture
- **OptiTrack Unity Plugin** | Rigid-body streaming
- **TextMeshPro** | User interface

The game incorporates both custom functionality and third-party assets.

## 🚀 Getting Started

### Requirements

- Unity 6 with Universal Render Pipeline support
- Unity Hub
- OptiTrack Motive and compatible tracking hardware (Motion Capture Mode only)

### Setup

1. Clone or download this repository.
2. Open Unity Hub.
3. Select **Add → Add project from disk**.
4. Select the cloned project directory.
5. Open the project using its configured Unity Editor version.
6. Open `DuckKnight_Menu`.
7. Enter Play Mode and select your preferred control method.

### Motion Capture Configuration

Motion Capture Mode requires a properly configured OptiTrack environment.

1. Launch Motive and enable rigid-body streaming.
2. Ensure the physical sword and shield are configured as separate rigid bodies.
3. Connect Unity's OptiTrack Streaming Client to Motive.
4. Verify that the rigid-body IDs match their corresponding Unity components.
5. Calibrate tracked positions and rotations for the physical play area.
6. Select Motion Capture Mode from the main menu.

The game includes a pre-round setup screen to allow players to prepare their equipment before starting the countdown.

## 🧪 Development

Duck Knight is an experimental project exploring the intersection of traditional arcade game mechanics and physical interaction.

Its gameplay systems support both standard desktop input and real-time motion capture, allowing the same game to be experienced through substantially different control methods.

**Current Status:** Playable Prototype | Active Development

### Development Focus

- Motion capture calibration and physical prop integration
- Gameplay balancing and difficulty progression
- Combat responsiveness and player feedback
- Environmental and visual polish
- User experience across control modes

## 📜 Credits & Assets

Developed by **Madeline Davis**.

Created using Unity, OptiTrack technology, and a combination of custom implementations and appropriately licensed third-party assets.

Third-party assets remain the property of their respective creators.

---