# The Siege Development Documentation

---
## About
The Siege is a 3D tower-defense game. The player must protect their Bastion from the endless waves of enemies;
players may place and upgrade buildings in order to protect their land.

This project was made in 72 hours, where the goal was to create an easily-scalable tower defense game.

### Downloads
[Download for Windows (ZIP)](https://github.com/Rographa/the-siege/releases/download/prototype/The.Siege.v0.1.zip)

---

## Gameplay
Player starts the game with the Bastion positioned on the battlefield. As soon as the game starts,
enemies will start to spawn in waves and aim to destroy the Bastion.

### Buildings 
Buildings may be placed on the battlefield if the player can afford it. The Building cost scales up for each of the
same building already placed in the battlefield.

The Bastion is also a Building; it will also attack enemies in range and try to protect itself. When the Bastion gets
destroyed, it's game over.

Buildings, except the Bastion, will deteriorate for every passed wave in a random damage; once their health drops to 0,
the building is destroyed. When that happens, the cost for that building will also decrease, as it doesn't count as one 
of the buildings in the battlefield anymore.

| Name            | Cost | Health | Damage | Attack Speed | Range | Deterioration*    |
|-----------------|------|--------|--------|--------------|-------|-------------------|
| Bastion         | 80   | 200    | 15     | 0.7          | 50    | 0                 |
| Gatling Turret  | 20   | 100    | 1.5    | 6            | 15    | Min: 4.25 Max: 10 |
| Elven Hightower | 30   | 100    | 12     | 1            | 50    | Min: 3.5  Max: 14 |
| Wizard Tower    | 25   | 100    | 4      | 1.5          | 25    | Min: 5  Max: 12   |

_*Multiplied by Difficulty Multiplier_

Upgrades may be bought to enhance the Building stats and recover its health to maximum.

| Name            | Health | Damage | Attack Speed | Range | Cost                 |
|-----------------|--------|--------|--------------|-------|----------------------|
| Bastion         | 10%    | 30%    | 20%          | 10%   | 80 * 1.5 ^ (Level-1) |
| Gatling Turret  | 30%    | 20%    | 10%          | 30%   | 20 * 1.5 ^ (Level-1) |
| Elven Hightower | 15%    | 30%    | 10%          | 10%   | 30 * 1.5 ^ (Level-1) |
| Wizard Tower    | 20%    | 20%    | 30%          | 10%   | 25 * 1.5 ^ (Level-1) |


### Enemies
Enemies will constantly spawn and try to reach the Bastion; when they succeed, they will start damaging it.

| Name     | Health* | Damage* | Attack Speed* | Range | Size | Move Speed* | Reward*         |
|----------|---------|---------|---------------|-------|------|-------------|-----------------|
| Assassin | 10      | 7       | 0.8           | 1     | 0.9  | 5           | Min: 3  Max: 5  |
| Goblin   | 40      | 2       | 1             | 2     | 0.8  | 2           | Min: 2  Max: 6  |
| Golem    | 200     | 20      | 0.1           | 1     | 1.8  | 1           | Min: 6  Max: 15 |
| Imp      | 20      | 4       | 0.5           | 1     | 0.6  | 4           | Min: 1  Max: 4  |
| Orc      | 120     | 10      | 0.2           | 1     | 1.25 | 1.5         | Min: 4  Max: 12 |

_*Multiplied by Difficulty Multiplier_

These will grow stronger in stats and quantity while the game continues, with the following multipliers:
- **Difficulty Multiplier**: +30% to all stats for every 10 Waves.
- **Quantity Multiplier**: +50% to number of enemies spawned for every 5 Waves.

---
## Development

Made in Unity 2022.3.62f3 (LTS)
The Siege was made in less than 72 hours, using only Unity built-in components and my personal code stash.

I started the project aiming for the essentials: create a scalable tower-defense game. I tried to keep it simple and
flexible. 

We have majorly two different types of entities: Units and Buildings. For each of these, there's a single prefab that
will receive a Data ScriptableObject containing all info it requires to setup and to take shape of it. When initialized,
the prefab is ready to start its behavior with the correct data.

All data scriptable objects can be found at `Assets/Data`. 

Most of the game configuration data is held by GameConfig.asset, allowing easy customization for balancing values.
This asset can be found in `Assets/Data/Config/GameConfig.asset`.

Unit-specific and building-specific data can also be easily edited through tweaking the values in the ScriptableObjects.
You can find them at `Assets/Data/Entities/Units` and `Assets/Data/Entities/Buildings` respectively.

Waves can also be customized and are pretty simple, consisting on a List of a selected enemy and the quantity to spawn.
These can be found at `Assets/Data/Waves`

The **Game Manager** is the major controller for the game, with the following responsibilities:
- Maintain Core-game loop
- Spawn enemy waves
- Manage most player inputs
- Operate player currency
- Update game speed
- Update simple HUD

Meanwhile, the Build Manager is the core controller for the Build System. Its responsibilities relies on:
- Update and manage building catalog UI
- Keep track of current placed buildings
- Listen to player input to show building tooltips and place new buildings
- Show preview for the selected building to be placed

Units and Buildings inherit from Entity its main methods. Each of these override some specific methods in order
to behave as expected. Their major difference is that Buildings have the Shooter component attached, allowing them
to perform projectile or ray attacks against the enemy, while Units have NavMeshAgent component, which allows them to 
move to the target in order to attack it.

---
## Conclusion

I really loved to develop this game! Although it's pretty simple at the moment, I'd love to put some extra time on it
and see what it could be. I'm a fan of tower-defense games and it felt great designing my own. 

I'm really proud and happy by accomplishing this project by the scope of 72 hours. 

Feel free to reach me out:

Email: gobiraphael@gmail.com

LinkedIn: https://www.linkedin.com/in/raphaelgobi

Thank you for reading and I hope you enjoy **The Siege**!

Raphael Gobi
