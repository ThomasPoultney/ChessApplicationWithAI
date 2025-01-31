﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessB
{
    public class MoveTree
    {
        private Node rootNode;
        private int numNodes;

        public MoveTree(Node rootNode)
        {
            this.rootNode = rootNode;
            this.numNodes = 1;
        }

        public int getNumNodes()
        {
            return this.numNodes;
        }

        public void incrementNumNodes()
        {
            this.numNodes++;
        }
    }
}
