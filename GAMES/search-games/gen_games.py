#!/usr/bin/env python3

import argparse
import random
import uuid
import json

def g1Config():
    return {
        "edges": [
            {
                "from": 2,
                "to": 2
            },
            {
                "from": 3,
                "to": 3
            },
            {
                "from": 4,
                "to": 4
            },
            {
                "from": 5,
                "to": 5
            },
            {
                "from": 6,
                "to": 6
            },
            {
                "from": 7,
                "to": 7
            },
            {
                "from": 8,
                "to": 8
            },
            {
                "from": 9,
                "to": 9
            },
            {
                "from": 10,
                "to": 10
            },
            {
                "from": 0,
                "to": 1
            },
            {
                "from": 0,
                "to": 2
            },
            {
                "from": 0,
                "to": 3
            },
            {
                "from": 0,
                "to": 4
            },
            {
                "from": 1,
                "to": 2
            },
            {
                "from": 1,
                "to": 3
            },
            {
                "from": 1,
                "to": 4
            },
            {
                "from": 2,
                "to": 3
            },
            {
                "from": 3,
                "to": 2
            },
            {
                "from": 3,
                "to": 4
            },
            {
                "from": 4,
                "to": 3
            },
            {
                "from": 14,
                "to": 2
            },
            {
                "from": 14,
                "to": 3
            },
            {
                "from": 14,
                "to": 4
            },
            {
                "from": 2,
                "to": 5
            },
            {
                "from": 3,
                "to": 6
            },
            {
                "from": 4,
                "to": 7
            },
            {
                "from": 5,
                "to": 6
            },
            {
                "from": 7,
                "to": 6
            },
            {
                "from": 5,
                "to": 8
            },
            {
                "from": 6,
                "to": 9
            },
            {
                "from": 7,
                "to": 10
            },
            {
                "from": 8,
                "to": 9
            },
            {
                "from": 9,
                "to": 8
            },
            {
                "from": 9,
                "to": 10
            },
            {
                "from": 10,
                "to": 9
            },
            {
                "from": 15,
                "to": 8
            },
            {
                "from": 15,
                "to": 9
            },
            {
                "from": 15,
                "to": 10
            },
            {
                "from": 8,
                "to": 11
            },
            {
                "from": 9,
                "to": 12
            },
            {
                "from": 10,
                "to": 13
            }
        ],
        "vertexCount": 16
    }

def g2config():
    return {
        "edges": [
            {
                "from": 2,
                "to": 2
            },
            {
                "from": 3,
                "to": 3
            },
            {
                "from": 4,
                "to": 4
            },
            {
                "from": 5,
                "to": 5
            },
            {
                "from": 6,
                "to": 6
            },
            {
                "from": 7,
                "to": 7
            },
            {
                "from": 8,
                "to": 8
            },
            {
                "from": 9,
                "to": 9
            },
            {
                "from": 10,
                "to": 10
            },
            {
                "from": 0,
                "to": 1
            },
            {
                "from": 0,
                "to": 2
            },
            {
                "from": 0,
                "to": 3
            },
            {
                "from": 0,
                "to": 4
            },
            {
                "from": 1,
                "to": 2
            },
            {
                "from": 1,
                "to": 3
            },
            {
                "from": 1,
                "to": 4
            },
            {
                "from": 2,
                "to": 3
            },
            {
                "from": 3,
                "to": 2
            },
            {
                "from": 3,
                "to": 4
            },
            {
                "from": 4,
                "to": 3
            },
            {
                "from": 14,
                "to": 2
            },
            {
                "from": 14,
                "to": 3
            },
            {
                "from": 14,
                "to": 4
            },
            {
                "from": 2,
                "to": 5
            },
            {
                "from": 3,
                "to": 6
            },
            {
                "from": 4,
                "to": 7
            },
            {
                "from": 5,
                "to": 6
            },
            {
                "from": 6,
                "to": 5
            },
            {
                "from": 7,
                "to": 6
            },
            {
                "from": 6,
                "to": 7
            },        
            {
                "from": 5,
                "to": 8
            },
            {
                "from": 6,
                "to": 9
            },
            {
                "from": 7,
                "to": 10
            },
            {
                "from": 8,
                "to": 9
            },
            {
                "from": 9,
                "to": 8
            },
            {
                "from": 9,
                "to": 10
            },
            {
                "from": 10,
                "to": 9
            },
            {
                "from": 15,
                "to": 8
            },
            {
                "from": 15,
                "to": 9
            },
            {
                "from": 15,
                "to": 10
            },
            {
                "from": 8,
                "to": 11
            },
            {
                "from": 9,
                "to": 12
            },
            {
                "from": 10,
                "to": 13
            }
        ],
        "vertexCount": 16
    }

def g3config():
    return {
        "edges": [
            {
                "from": 2,
                "to": 2
            },
            {
                "from": 3,
                "to": 3
            },
            {
                "from": 4,
                "to": 4
            },
            {
                "from": 5,
                "to": 5
            },
            {
                "from": 6,
                "to": 6
            },
            {
                "from": 7,
                "to": 7
            },
            {
                "from": 8,
                "to": 8
            },
            {
                "from": 9,
                "to": 9
            },
            {
                "from": 10,
                "to": 10
            },
            {
                "from": 0,
                "to": 1
            },
            {
                "from": 0,
                "to": 2
            },
            {
                "from": 0,
                "to": 3
            },
            {
                "from": 0,
                "to": 4
            },
            {
                "from": 1,
                "to": 2
            },
            {
                "from": 1,
                "to": 3
            },
            {
                "from": 1,
                "to": 4
            },
            {
                "from": 2,
                "to": 3
            },
            {
                "from": 3,
                "to": 2
            },
            {
                "from": 3,
                "to": 4
            },
            {
                "from": 4,
                "to": 3
            },
            {
                "from": 14,
                "to": 2
            },
            {
                "from": 14,
                "to": 3
            },
            {
                "from": 14,
                "to": 4
            },
            {
                "from": 2,
                "to": 5
            },
            {
                "from": 3,
                "to": 6
            },
            {
                "from": 4,
                "to": 7
            },
            {
                "from": 5,
                "to": 8
            },
            {
                "from": 6,
                "to": 9
            },
            {
                "from": 7,
                "to": 10
            },
            {
                "from": 8,
                "to": 9
            },
            {
                "from": 9,
                "to": 8
            },
            {
                "from": 9,
                "to": 10
            },
            {
                "from": 10,
                "to": 9
            },
            {
                "from": 15,
                "to": 8
            },
            {
                "from": 15,
                "to": 9
            },
            {
                "from": 15,
                "to": 10
            },
            {
                "from": 8,
                "to": 11
            },
            {
                "from": 9,
                "to": 12
            },
            {
                "from": 10,
                "to": 13
            }
        ],
        "vertexCount": 16
    }  

def create_game(graphConfig, steps, defenderUnitCount, attackerStartingPosition, defenderStartingPositions, targets, defenderVertices, trackVertices, coveringUpTracksPossible, id):
    return {
        "graphConfig": graphConfig,
        "rounds": steps,
        "defenderUnitCount": defenderUnitCount,
        "attackerStartingPosition": attackerStartingPosition,
        "defenderStartingPositions": defenderStartingPositions,
        "targets": targets,
        "defenderVertices": defenderVertices,
        "trackVertices": trackVertices,
        "coveringUpTracksPossible": coveringUpTracksPossible,
        "id": id
    }

def target(position, payoff):
    return {"position": position, "payoff": payoff}

def args():
    parser = argparse.ArgumentParser()

    parser.add_argument('-ls', type=int, default=4, help='Step count from which the program will start to generate games')
    parser.add_argument('-us', type=int, default=6, help='Step count of the longest generated games')
    parser.add_argument("-lp", type=float, default=1.0, help='Lower bound of random attacker payoffs')
    parser.add_argument("-up", type=float, default=2.0, help='Upper bound of random attacker payoffs')
    parser.add_argument('-n', type=int, default=5, help='Number of games to generate for every parameter set')
    parser.add_argument('-s', type=int, default=None, help='RNG seed (only for payoffs, not for id generation)')
    
    args = parser.parse_args()
    return args.ls, args.us, args.lp, args.up, args.n, args.s

def main():
    minsteps, maxsteps, minPayoff, maxPayoff, n, seed = args()
    random.seed(seed)
    graphs = [g1Config(), g2config(), g3config()]
    for steps in range(minsteps, maxsteps + 1):
        for (i, g) in enumerate(graphs):
            for coveringUpTracksPossible in [True, False]:
                for j in range(n):
                    game = create_game(graphConfig=g,
                        steps=steps, 
                        defenderUnitCount=2,
                        attackerStartingPosition=0,
                        defenderStartingPositions=[14,15],
                        targets=[target(p, random.uniform(minPayoff, maxPayoff)) for p in [11, 12, 13]],
                        defenderVertices=[[2, 3, 4, 14],[8, 9, 10, 15]],
                        trackVertices=[2, 3, 4],
                        coveringUpTracksPossible=coveringUpTracksPossible,
                        id=str(uuid.uuid4())
                    )
                    game_suffix = "SA" if coveringUpTracksPossible else "SD"
                    game_name = "G{ii}-steps{steps}-{game_suffix}-{j:04}.sgame".format(steps=steps, game_suffix=game_suffix, j=j, ii=i+1)
                    with open(game_name, 'w') as f:
                        json.dump(game, f)


if __name__ == '__main__':
    main()
