## Files for search games

This directory contains configuration files for search games. The game is played on a directed graph with loops. The attacker starts in a specially marked vertex (attackerStartingPosition) and has to reach a target vertex. There is only one attacker.  

 The defender can have multiple units and tries to catch the attacker. If at any time a defending unit is in the same vertex as the attacker or meets the attacker on an edge, the defender wins the game. Each defender can only move in a subset of vertices defined in defenderVertices. As their first move, defenders choose their starting position within the possible defenderVertices. This is modeled by adding an artificial starting vertex for each defender, which are only connected to the given possible defenderVertices.

 Both units can only wait in vertices which have loops (edges to itself).
 
  A move of the attacker leaves tracks in the vertex he is arriving if the vertex is in the trackVertices array. The defender discovers the tracks when entering the vertex. The attacker can also stay for one round in a vertex and cover up his tracks instead of moving on. This is only possible when the attacker can actually wait in this vertex (when it has a loop) and when coveringUpTracksPossible is true in the config file. 

 Defender payoffs are defined as follows:
  - 1 for winning the game (catching the attacker)
  - 0 when the game ends inconclusive (the time runs before before the attacker is caught or has reached a target)
  - -1 for losing the game (failing to prevent the attacker from reaching a target)

  Attacker payoffs are defined as follows:
  - payoff from targets array for reaching a target vertex
  - 0 when the game ends inconclusive (the time runs before before the attacker is caught or has reached a target)
  - -1 for losing the game (getting caught by the defender on a vertex or edge)

## File format

- graphConfig -- definition of the game graph
    - edges -- array of edges (defined as two numbers, from and to vertex)
    - vertexCount -- number of vertices in graph
- rounds -- number of time steps after game is over
- defenderUnitCount -- number of defender units
- attackerStartingPosition -- vertex in which the attacker (evader) can start
- defenderStartingPositions -- vertices in which the defenders start (one for each defender)
- targets -- array of target vertices with a given payoff which the attacker tries to reach
    - position -- the number of the vertex
    - payoff -- attacker payoff when reaching the vertex
- defenderVertices -- array of arrays of vertices. For each defender it defines the vertices he can enter.
- trackVertices -- vertices where the attacker leaves tracks
- coveringUpTracksPossible -- a flag which defines whether the attacker can cover up his tracks
- id -- UUID of the game