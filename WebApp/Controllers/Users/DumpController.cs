﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using PublicAPI.Responses.Dump;
using WebApp.Models;

namespace WebApp.Controllers.Users
{
    [Produces("application/json")]
    [Route("api/dump")]
    [Authorize(Roles = "Admin")]
    public class DumpController : AuthorizeController
    {
        private readonly ApplicationDbContext dbContext;

        public DumpController(
            UserManager<User> userManager,
            ApplicationDbContext dbContext) : base(userManager)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("{challengeId:guid}")]
        public async Task<Dictionary<string, Dictionary<string, SolutionDumpView>>> ChallengeResults(Guid challengeId)
        {
            if (!await dbContext.Challenges.AnyAsync(ch => ch.Id == challengeId))
                throw StatusCodeException.BadRequest();
            var allSolutions = await dbContext
                .Solutions
                .Where(s => s.Exercise.ChallengeId == challengeId)
                .ProjectTo<SolutionDumpView>()
                .GroupBy(s => s.UserId)
                .ToDictionaryAsync(us => us.Key, us => 
                    us.GroupBy(a => a.ExerciseName)
                        .ToDictionary(g => g.Key, g => g.Aggregate((a, b) => a.Status > b.Status ? a : b)));

            return allSolutions;
        }
    }
}