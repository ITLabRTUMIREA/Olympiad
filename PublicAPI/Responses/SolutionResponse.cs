﻿using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAPI.Responses
{
    public class SolutionResponse
    {
        public Guid Id { get; set; }
        public string Language { get; set; }
        public Guid UserId { get; set; }
        public Guid ExerciseId { get; set; }
        public SolutionStatus Status { get; set; }
        public DateTime SendingTime { get; set; }
        public DateTime? StartCheckingTime { get; set; }
        public DateTime? CheckedTime { get; set; }
    }
}