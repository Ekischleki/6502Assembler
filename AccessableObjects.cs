﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASI
{
    public class AccessableObjects
    {
        public Hashtable accessableVars = new();   //List<Var> accessableVars = new();
        public bool inLoop = false;
        public CancellationTokenSource? cancellationTokenSource;
        public Global global;
        public AccessableObjects(Hashtable accessableVars, Global global)
        {
            this.accessableVars = accessableVars;
            this.global = global;
        }
    }
}
