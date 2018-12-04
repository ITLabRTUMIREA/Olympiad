﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using WebApp.Auth;
using WebApp.Helpers;
using WebApp.Models.Responses;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext context;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(UserManager<User> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions, IMapper mapper, ApplicationDbContext context)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            this.mapper = mapper;
            this.context = context;
            _jwtOptions = jwtOptions.Value;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody]CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(credentials.UserName) || string.IsNullOrEmpty(credentials.Password))
            {
                return BadRequest();
            }
            var user = await _userManager.FindByNameAsync(credentials.UserName);
            if (user == null) return BadRequest();
            if (!await _userManager.CheckPasswordAsync(user, credentials.Password)
                //|| await _userManager.IsEmailConfirmedAsync(userToVerify)
                )
            {
                return BadRequest();
            }

            var loginInfo = await GenerateResponse(user);
            return Json(loginInfo);
        }


        [HttpGet("getme")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Get()
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (user == null)
                return StatusCode(403);
            return Json(await GenerateResponse(user));
        }

        private async Task<LoginResponse> GenerateResponse(User user, string token = "")
        {
            var loginInfo = mapper.Map<LoginResponse>(user);
            loginInfo.Token = token;

            var sum = await context
                 .Exercises
                 .Where(e => e.Solution.Any(S => S.Status == SolutionStatus.Sucessful && S.UserId == user.Id))
                 .SumAsync(e => e.Score);
            loginInfo.TotalScore = sum;
            
            var identity = _jwtFactory.GenerateClaimsIdentity(user.UserName, user.Id.ToString(), _userManager.GetRolesAsync(user).Result.ToArray());

            loginInfo.Token = Tokens.GenerateJwt(identity, _jwtFactory, user.UserName).Result;
            return loginInfo;
        }
    }
}