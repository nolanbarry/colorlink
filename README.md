# Colorlink
This is (or will be) a windows form clone of the classic Windows Mobile game Flow Free, which was a remix of the age-old concept of [numberlink](https://en.wikipedia.org/wiki/Numberlink).
It includes a level generator (does not guarentee the level is solvable) and a slow but effective level solver.

### Level Solver
The level solver is just a simple backtracking method. It will start by scanning through the given level and recording the start and points (or nodes) and colors of each node. They are all sorted correspondingly (i.e. the 0th start point will link with the 0th end point and they will be the 0th color). It will also generate a blank Path object, which stores directions (Up, Down, Right, Left), a start point, and an associated color. 

A level is stored when running as a rectangular grid where -1 represents an empty space and a non-negative number represents a color. It is stored much the same in a file except the blank cells are stored as any letter, symbol, or negative number. I use dashes.
i.e.:
```
- - 5 1 2
- 7 - - -
- 5 - - 2
7 - - 1 4
4 - - - -
```
will look like this:

<img src="http://u.cubeupload.com/nolanbarry/blank.png" width="350">

Once the important bits have been recorded, the recursive Solve method is called, which will go to the end of the last path (a list of paths is stored in the SolvingGrid object, which is the only parameter passed to Solve()), generate a list of all adjacent options to move to, and go to the list, moving in that direction and calling Solve() again, passing it a deep-cloned SolvingGrid object where the path has been extended in the chosen direction. This way, when a branch ends, the solver will just run back up the call stack until it finds a cell with another option to move to. When a branch reaches the endpoint for that color, it will move to the next start point and start a new path.
