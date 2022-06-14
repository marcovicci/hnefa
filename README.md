# Hnefa

#### Unity version: 2021.3.2f1

I can't wait to date a chess piece. That's what the game is about, right?

## collaboration instructions

Clone this repo. Probably using
[Github Desktop](https://desktop.github.com/)
and the green button up top.\
You can also manually download the files as a .zip, but why?\
After you have a copy on your computer, open Unity Hub.\
Make sure the correct version of Unity is installed.\
Using "Open -> Add Project from Disk", navigate to your new Hnefa folder.\
Open it and click "Add Project".

## patch notes

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
