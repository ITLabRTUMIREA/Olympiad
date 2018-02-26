﻿using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Executor.Executers.Build.dotnet
{
    [Language("csharp")]
    class JavaBuilder : ProgramBuilder
    {
        public JavaBuilder(Action<Guid, SolutionStatus> proccessSolution, Action<DirectoryInfo, Solution> finishBuildSolution)
            : base(proccessSolution, finishBuildSolution)
        {
        }

        protected override string ProgramFileName => "Program.cs";


        protected override string BuildFailedCondition => "Build FAILED";

        protected override string GetBinariesDirectory(DirectoryInfo startDir)
            => Path.Combine(startDir.FullName, "pub");
    }

}

