﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Web.Server.Utilities.DiscordMessager;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Web.Server.Utilities.Database;
using System.Collections.Generic;
using AspNet.Security.ApiKey.Providers;
using System;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly DatabaseManager _database;
        private readonly DiscordMessager _messager;

        public PlayersController(DatabaseManager database, DiscordMessager messager)
        {
            this._database = database;
            this._messager = messager;
        }

        //[HttpPost]
        //public async Task<Player> CreatePlayer([FromBody] Player player)
        //{
        //    player = _database.CreatePlayer(player);

        //    if (player != null)
        //    {
                
        //    }
        //    return player;
        //}

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<Player> GetMyPlayer()
        {
            Player player;
            if (User.Identity.IsAuthenticated)
            {
                player = _database.GetPlayer(User.FindFirst(ClaimTypes.NameIdentifier).Value.Substring(37));
            } else
                player = null;

            return player;
        }

        [HttpGet("avatar/{playerId}")]
        [AllowAnonymous]
        public ActionResult GetPlayerAvatar(string playerId)
        {
            return File(_database.GetPlayer(playerId).PlayerAvatar, "image/jpg");
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public Dictionary<string, string> GetPlayersSearch()
        {
            return _database.GetPlayersSearch();
        }


        [HttpGet("{playerId}")]
        [Authorize(AuthenticationSchemes = ApiKeyDefaults.AuthenticationScheme)]
        public async Task<Player> GetPlayer(string playerId, [FromQuery] string ip)
        {
            Player player = await _database.GetInitializedPlayerAsync(playerId, ip);
            return player;
        }
    }
}