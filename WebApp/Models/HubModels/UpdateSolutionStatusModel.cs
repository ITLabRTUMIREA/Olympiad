﻿using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.HubModels
{
    public class UpdateSolutionStatusModel
    {
        public Guid SolutionId { get; set; }
        public SolutionStatus SolutionStatus { get; set; }
    }
}