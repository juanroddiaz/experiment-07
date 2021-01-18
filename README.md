# experiment-07
A test exercise as a variation of Doodle Jump: https://www.youtube.com/watch?v=w3POq5jNjT4&ab_channel=Uptodown


![Menu](/Images/menu_01.gif)
![Gameplay - iPad 4](/Images/game_01.gif)
![Gameplay - iPhone X](/Images/game_02.gif)
![Gameplay - Samsung Galaxy 10+](/Images/game_03.gif)

External Packages (unused assets were deleted):
* Free Platform Game Assets, Bayat Games: https://assetstore.unity.com/packages/2d/environments/free-platform-game-assets-85838
* A lot of Kenney's free resources: https://www.kenney.nl/. Some assets I got them from the itchio's Bundle for Racial Justice and Equality.
	* Explosion Pack
	* Fonts
	* isometric-blocks
	* Platform Assets Pixel
	* tooncharacters
	* uipack_fixed
	* sounds
* Sprite Trail Renderer, Little Pug Games: https://little-pug-games.itch.io/unity-sprite-trail-renderer

How to test:
* In standalone: start from Scenes/MenuScene or Scenes/GameScene. Starting from GameScene will initialize the last scene played.
* APK: just install "experiment-07.apk" in your Android device, download it from here -> 


Gameplay requirements:
	 * Character jumps automatically, you can control the robot with the left/right buttons or with a swipe. Swiping will move automatically towards a direction until landing in a platform.
	 * Game in infinite: you can check the level generation logic in ScenarioLogic and PlatformCataogue classes references. Generation uses rules to create procedural levels, three different options are created in Menu scene to show different difficulty configurations.
	 * Player dies if it touches the bottom of the level, which position is updated everytime to match the gameplay area. Also death triggers are located in the sides.
	 * Nice to have:
	 	* Game has different platforms: normal, trampoline, one-step destroyable, shallow and bomb types. Every type is related to a specific difficulty range.


Other requirements:
* Intro countdown
* Outro animation, displaying current height/platforms counters and best records.
* Game doesn't restart the scene, it just initializes all controllers again, ensuring a different level but using the same scenario configuration.
* UI Feedback for height and platforms
* Simulator is awesome! I could fix the overlaping UI and the game area dimensions thanks to it.
* Extras:
	* Adding carousel menu
	* Adding character full movement animation
	* Adding rainbow trail :)
	* Option to pause the game and go back to main menu: last selected level stored in device
	* Option to reset scores
	* Music! And of course "no music" button too

Considerations:
* I didn't implemented an object pool, but potentially I could use one for platforms instances.
* I feel Doodle Jump was a little bit difficult to control with swipe and in this test I have a similar impression: the jumping is very quick and it feels good but the space is not enough to retreat in case of mistake while swiping. I added a "landed action stop" in swipe based movement due to this.
* I used my very typical way to create logics between component: I didn't try to experiment with shaders or ECS/DOTS, as I wanted to deliver a more wholesome minigame as fast as I could. I'd like to go deep into ECS as next tech knowledge challenge.
* I think I'll keep adding more stuff to this demo! I had fun thinking about visuals and adding little details. Also, I'm not totally satisfied about the "juicy" factor: I don't like to put too many extra libraries in my test projects but Dotween would help me in this 
* I feel this game could be done faster, but I could not focus totally on the test as a lot of stuff happened last week, discounting around 3 workdays from my estimation: that's why I decided to ask for some extra time.
