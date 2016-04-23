﻿using nzy3d_wpfDemo.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nzy3d_wpfDemo
{
    class MyMapper : nzy3D.Plot3D.Builder.Mapper
    {
        private double[][] values2D;

        private bool inited = false;

        public override double f(double x, double y)
        {
            if (!inited)
            {
                double[][] experiment = Solver.buildExperiment(10, 3000);
                Matrix.set(experiment, 4000, 3, 3, 6, 6);
                Solver.experiment = Matrix.matrixToRow(experiment);
                values2D = Solver.solve(10, 3, 3);
                inited = true;
            }
            int i = (int) x / 50;
            int j = (int) y / 50;
            return values2D[i][j];
        }

    }
}
