# Hnefa

#### Unity version: 2021.3.2f1 - Copy and paste this into your browser to install in Unity Hub:
#### unityhub://2021.3.2f1/d6360bedb9a0

I can't wait to date a chess piece. That's what the game is about, right?

## collaboration instructions

Install Unity Hub, and add the correct version of Unity using the link above.\
Clone this repo. Probably using
[Github Desktop](https://desktop.github.com/)
and the green button up top.\
You can also manually download the files as a .zip, but why?\
After you have a copy on your computer, open Unity Hub.\
Make sure the correct version of Unity is installed.\
Using "Open -> Add Project from Disk", navigate to your new Hnefa folder.\
Open it and click "Add Project".

## patch notes

#### 05/11/2022

Pre-exposure bugfix!\
The skip/next button now toggles off after use, until the next turn when you can use it again.\
This prevents breaking the game by mashing it too fast.

#### 20/10/2022

DAWN OF THE FINAL DAY\
24 HOURS REMAIN\
I added fonts and linked up, like, the whole game. Don't worry about it. The fonts are nice.\
Gameplay loop COMPLETE! Sort of.\
Feel free to watch [the technical demonstration video on Youtube](https://www.youtube.com/watch?v=1UPW7-cijK0) if you want a squiz.

#### 19/10/2022

Added sprites, death icons and regular icons for all board pieces.\
Added faces for various emotional states.\
Bird piece is in. Clicking on it will allow you to move it to various allied pieces until its energy runs out.\
Continuing work on the dialogue system.

#### 20/09/2022
Long time no see!

Added to BasePiece class:\
mCurrentEASA variable, which is four integers (of course) in an array. mTotalVariance integer and CalculateVariance() function to return the total emotional variance of this piece.\
SelectNewSpot() function with a default "random mode" that just randomly picks a cell to move to.

Added to PieceManager class:\
PickAlliedPiece() and FindMaxVariance() functions that can go through all living allied pieces, call CalculateVariance() on each, determine the highest value and select that piece.\
PickEnemyPiece() that just rolls the dice on each piece to pick somebody who should move.\

Added dummy scenes (with a canvas and event system but currently nothing else):\
DialogueWindow\
OptionsMenu\
EndingScreen\
Tutorial

#### 26/07/2022

Removed FMOD. I was silly for including it and it'll make things more complicated for no reason, like whenever I become obsessed with reinstalling Linux. I apologize for my silliness.\
Added a direct link to get the correct version of Unity to these instructions. 

#### 23/07/2022

Branches are a thing now. Each coder will have their own branch, *development-XX* (where XX is their initials.)

#### 12/07/2022

Added a new class - Simulator - for making and evaluating theoretical moves on a virtual fake game board. A lot of the code is reused from elsewhere and it's a little silly. But trust me, it will work!\
Added a Board History variable... list... tuple? Whatever it is, it can store current positions of pieces for each team and states of all the cells on the board. In theory, this means the AI will be able to roll back from potential moves and build the board at any state of play using that data. This is used in PieceManager and our new Simulator class.\
Pieces can now be "virtual" so that the movement of simulated pieces won't trigger real game board changes.\
The Jarl will send some information about his states to the Simulator, since this is pretty important to scoring. (Other pieces will also do this, once the emotional system is more present.)\
In general, I'm laying the groundwork for the minimax system we'll be using for AI decisionmaking. Stay tuned for more...

#### 14/06/2022

You can now take pieces by sandwiching them between your pieces (and you can also use a corner space or empty throne for this)! In order to take the jarl you have to surround him on all four sides.\
You can win by getting the jarl to a corner, or lose by having him die. Right now this just restarts the game.\
New function in BasePiece to check cell states against a target, to assist with our functions for taking pieces.\
New function in BasePiece to check if an enemy piece has been sandwiched and initiate its Kill() function.\
The jarl has his own function to check if he is actually surrounded before running Kill(). He can be surrounded by 4 enemy pieces or by 3 enemy pieces and the throne.\
Kill() also tells the cell in which it occurred that somebody died there.

#### 13/06/2022

Added some basic movement mechanics - pieces can now be dragged and dropped around and snap to our grid.\
BasePiece class has some new functions to evaluate movement paths and that kind of thing. It also hooks into a bunch of drag events. Of course, these will later be triggered by internal AI instead of by player input directly. After a piece is moved successfully, control is automatically switched to the opposing side and those pieces become draggable.\
Basic movement is implemented aside from taking pieces, but including mechanics like "only Jarls can land on the throne, but all pieces can pass over it."\
SingleCell class can now handle emptying itself of pieces and has bools for different cell types (throne, corner pieces, and cells where a death has occurred).\
GameBoard is where the really exciting stuff is happening - just kidding, it's also pretty normal looking in there. But it does have a new enum to determine types of game cells that pieces can actually potentially move to.\
Added a Resources folder and basic dummy code that will allow each piece to load its unique icon when that is placed in the Resources folder. Yay!\
Next up: taking pieces, winning and losing.

#### 04/06/2022

Removed external ChessGame package; turns out home-made code tastes better.\
Added a Documentation folder for behind-the-scenes nonsense that still needs to be here.\
Made the Github repo PUBLIC - hello world!

New scene added: **Hnefa Board!**\
Added prefabs for cells in the board, pieces, all sorts of wacky stuff.\
Created a class for game pieces, and added Types for each character + one for all enemies.\
![Piece manager code](/Documentation/piecemanager.png)\
After placing them in an array in the Piece Manager code, Artist, Trickster, Mother, Seeker, Child, Coward, Sage, Hero, Outlaw, Orphan, Witch, Lover, Jarl and Enemy pieces will populate the board. (Due to coding black magic they are being placed reversed on the Y axis... but the jarl is in the right place, so don't worry about it.)\
![Populated hnefa board](/Documentation/hnefaboard.png)

#### 26/05/2022

Imported a basic FMOD setup. Added relevant stuff to the .gitignore.\
Added a single dummy sound to the FMOD project to escape error messages.\
Added basic instructions to access a copy of the project + required Unity version.

#### 25/05/2022
Had the idea to make the readme very slightly better.\
Hooked up very basic menu system, mostly with the Unity event system.\
Grid layout components are kinda fun.
