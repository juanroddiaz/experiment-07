# experiments-03
A test exercise as a variation of the King of Thieves game (a physics-based platformer)


![Menu](/Images/exp1_01.gif)
![Gameplay](/Images/exp1_02.gif)

External Packages (unused assets were deleted):
* Free Platform Game Assets, Bayat Games: https://assetstore.unity.com/packages/2d/environments/free-platform-game-assets-85838
* A lot of Kenney's free resources: https://www.kenney.nl/
	* Fonts
	* isometric-blocks
	* tooncharacters
	* uipack_fixed
	* sounds
* Sprite Trail Renderer, Little Pug Games: https://little-pug-games.itch.io/unity-sprite-trail-renderer

"You could improve any aspect of the game to increase the user experience of players":
* Adding carousel menu
* Adding character animation
* Adding rainbow trail :)
* Adding visual countdown to grab a chest
* Creates an unique time bonus per level, to add 10 seconds to current gameplay
* Option to pause the game and go back to main menu: last selected level stored in device
* Option to reset scores
* Music! And of course "no music" button too
* Dangerous Cave level: spikes instantly defeat you, finishing the level.

Considerations:
* I didn't implemented an object pool, but potentially I could use one for particles and feedback instances
* I didn't use Scriptable Objects for data, but in case of, the Level Data in GameController prefab would be a candidate: this way some level design aspect would be easier to configure.
* I used my very typical way to create logics between component: I didn't try to experiment with shaders or ECS/DOTS, as I wanted to deliver a more wholesome minigame as fast as I could. I'd like to go deep into ECS as next tech knowledge challenge.
* I'd like to check more about collisions in edges: it was super easy to break the wall vs ground states for movement and animations, surely there are better ways to avoid this kind of problem.
* I think I'll keep adding more stuff to this demo! I had fun thinking about visuals and adding little details.
