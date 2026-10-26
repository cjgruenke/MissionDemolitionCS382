# Mission Demolition (CS382 Project 2)

**Author:** Cole Gruenke
**Unity version:** 2021.3 LTS (Built-In Render Pipeline)
**Play it in your browser:** https://cjgruenke.github.io/MissionDemolitionCS382/

Mission Demolition is a slingshot physics game based on the tutorial in Chapter 30 of Jeremy Gibson Bond's *Introduction to Game Design, Prototyping, and Development*. Fling projectiles at a castle and knock one into the green goal to move on to the next level.

## How to play

- **Aim:** hover over the slingshot, then click and drag backward.
- **Shoot:** release the mouse button.
- **Detonate:** while your projectile is in the air (or still rolling), press **Right-click** or **Space** to blow it up.
- **Change view:** the button in the top-right corner cycles the camera between the slingshot, the castle, and both.
- Hit the green goal with a projectile to clear the level. Clear all four castles in as few shots as you can.

## Enhancements

### a. Correct Unity version and .gitignore
The project was made in Unity 2021.3 LTS (see `ProjectSettings/ProjectVersion.txt`). The repository uses GitHub's standard Unity `.gitignore`, so generated folders such as `Library`, `Temp`, `Logs`, and `UserSettings` are not uploaded.

### b. Game Over screen with Play Again
After the fourth castle is cleared, a Game Over screen appears showing the total number of shots taken and the best (lowest) score, which is saved between sessions with `PlayerPrefs`. The **Play Again** button reloads the scene and starts over at Level 1. (`MissionDemolition.cs`)

### c. Four new castles, ordered easy to hard
None of these are the castles from the book.

| Level | Castle | Description |
|---|---|---|
| 1 | Hut | Two walls and a roof, with a large goal sitting on top. Any decent shot clears it. |
| 2 | Watchtower | A two-story tower with the goal hidden inside the ground floor. The front wall has to be knocked out. |
| 3 | Twin Towers | Two two-story towers joined by a bridge, with the goal tucked in the narrow gap between them. |
| 4 | Fortress | A three-story, three-bay fortress with a stone shield wall in front and the goal hidden in the back bay. |

### d. Line Renderer rubber band
A `LineRenderer` draws the rubber band from the left arm of the slingshot, through the projectile while you aim, to the right arm. When nothing is loaded, the band rests straight between the arms. (`Slingshot.cs`)

### e. Sound
A rubber band snap plays when the projectile is released (`Slingshot.cs`), and an explosion sound plays when a projectile detonates (`Projectile.cs`). Both sound effects were generated for this project, so there are no licensing restrictions.

### f. Making the game cooler: explosive projectiles
Each projectile can be detonated once while it is moving by pressing Right-click or Space. The explosion uses `Physics.OverlapSphere` and `Rigidbody.AddExplosionForce` to blast nearby castle pieces outward and upward, spawns a particle burst, and plays the explosion sound. The camera holds on the blast for a few seconds so you can watch the castle come apart, then returns to the slingshot.

This adds a timing skill to the game. Instead of only choosing an angle and power, the player also decides *when* to detonate: early to blow open a front wall, or late after the projectile lands inside the castle. It is what makes the harder castles, like the Fortress, beatable. Only the most recently fired projectile responds, so older projectiles lying on the ground can't be set off by accident.

## Web build

The playable WebGL build is in the `docs` folder and is hosted with GitHub Pages at https://cjgruenke.github.io/MissionDemolition/.

## Scripts

| Script | Purpose |
|---|---|
| `Slingshot.cs` | Aiming, launching, rubber band, snap sound |
| `Projectile.cs` | Tracks projectiles, detonation, explosion force, sound, and effect |
| `ProjectileLine.cs` | Draws the trail behind each shot and fades older trails |
| `FollowCam.cs` | Camera that follows the projectile and switches between views |
| `MissionDemolition.cs` | Levels, shot count, UI, Game Over, and Play Again |
| `Goal.cs` | Detects when a projectile reaches the goal |
| `RigidBodySleep.cs` | Keeps castle pieces still until something hits them |
| `Cloud.cs`, `CloudCrafter.cs` | Procedurally generated drifting clouds |
