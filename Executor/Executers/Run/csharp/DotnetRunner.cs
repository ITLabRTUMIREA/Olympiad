﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Models;
using Shared.Models;

namespace Executor.Executers.Run.dotnet
{
    [Language("csharp")]
    class JavaRunner : ProgramRunner
    {
        public JavaRunner(Func<Guid, SolutionStatus, Task> processSolution) : base(processSolution)
        {
        }
    }
}
