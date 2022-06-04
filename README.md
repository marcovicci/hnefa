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

#### 04/06/2022

New scene added: Hnefa Board!\
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
