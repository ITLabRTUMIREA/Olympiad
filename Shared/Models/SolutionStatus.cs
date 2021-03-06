﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Olympiad.Shared.Models
{
    public enum SolutionStatus
    {
        ErrorWhileCompile = 0,
        CompileError = 1,
        RunTimeError = 2,
        WrongAnswer = 3,
        TooLongWork = 4,
        InQueue = 5,
        InProcessing = 6,
        Successful = 7,
    }
}
