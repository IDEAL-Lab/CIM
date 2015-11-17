import numpy as np

def func_rand_assign(outfile, num_nodes):
	# {1: x^2, 2: x, 3: 2x-x^2}

    # assignment 0 -> 0: 5%, 1: 15%, 2: 50%, 3: 30%
    # prob = [0.05, 0.10, 0.85]
    
    # assignment 1 -> 0: 10%, 1: 15%, 2: 50%, 3: 25%
    # prob = [0.10, 0.15, 0.75]

    # assignment 2 -> 0: 15%, 1: 20%, 2: 40%, 3: 25%
    prob = [0.15, 0.20, 0.65]
    rand_assign = np.random.choice(3, num_nodes, p=prob)
    with open(outfile, 'wb') as f:
        for nodeID, assign in zip(range(num_nodes), rand_assign):
            print >>f, nodeID, assign+1

if __name__ == '__main__':
    import sys
    # output file name: sys.argv[1], number of nodes: sys.argv[2]
    func_rand_assign(sys.argv[1], int(sys.argv[2]))
