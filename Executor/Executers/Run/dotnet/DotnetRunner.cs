﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Models;

namespace Executor.Executers.Run.dotnet
{
    [Language("csharp")]
    class JavaRunner : ProgramRunner
    {
        public JavaRunner(Action proccessSolution) : base(proccessSolution)
        {
        }

        public override string DockerImageName => "runner:dotnet";
    }
}
