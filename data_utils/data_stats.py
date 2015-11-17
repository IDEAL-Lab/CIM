# reform the graph data 
# Node ID range: 0~(V-1))
# CSV: separated by space
# V E
# FromNodeID ToNodeID Indeg(ToNodeID)

# from __future__ import print_function

def graph_reform(infile, outfile, head_skip):
	rf = open(infile, 'rb')
	wf = open(outfile, 'wb')
	rlines = rf.readlines()

	print "skipped %d lines" % head_skip

	dict_nodes = {}
	cnt = 0
	nodeidx = 0
	for l in rlines:
		# skip heading comment lines
		cnt += 1
		if cnt <= head_skip:
			continue

		fromnode, tonode = l.strip().split()
		if fromnode not in dict_nodes:
			dict_nodes[fromnode] = [nodeidx, 0]
			nodeidx += 1

		if tonode not in dict_nodes:
			dict_nodes[tonode] = [nodeidx, 1]
			nodeidx += 1
		elif tonode in dict_nodes:
			dict_nodes[tonode][1] += 1

	print "V: %d, E: %d" % (nodeidx, cnt-head_skip)
	print >>wf, nodeidx, cnt-head_skip
	cnt = 0
	for l in rlines:
		# skip heading comment lines
		cnt += 1
		if cnt <= head_skip:
			continue
		
		fromnode, tonode = l.strip().split()
		print >>wf, dict_nodes[fromnode][0], dict_nodes[tonode][0], dict_nodes[tonode][1]

	rf.close()
	wf.close()


if __name__ == '__main__':
	import sys
	# sys.argv[1]: infile, sys.argv[2]: outfile, sys.argv[3]: head_skip
	graph_reform(sys.argv[1], sys.argv[2], int(sys.argv[3]))
