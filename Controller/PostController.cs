using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DotnetAPI.Controller
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{search}")]
        public IEnumerable<Post> GetPosts(int postId, int userId, string search = "none")
        {
            string postsSql = @"EXEC TutorialAppSchema.spPosts_Get ";
            string parameters = "";
            DynamicParameters dinParams = new DynamicParameters();

            if (postId != 0)
            {
                parameters += ", @PostId=@PostIdParam";
                dinParams.Add("@PostIdParam", postId, DbType.Int32);
            }
            if (userId != 0)
            {
                parameters += ", @UserId=@UserIdParam";
                dinParams.Add("@UserIdParam", postId, DbType.Int32);
            }
            if (search != "none")
            {
                parameters += ", @SearchValue=@SearchValueParam";
                dinParams.Add("@SearchValueParam", search, DbType.String);
            }

            if (parameters.Length > 0)
            {
                postsSql += parameters[1..];
            }

            return _dapper.LoadDataWithParams<Post>(postsSql, dinParams);
        }



        [HttpGet("GetMyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string postsSql = @"EXEC TutorialAppSchema.spPosts_Get  @UserId = @UserIdParam";
            DynamicParameters dinParams = new DynamicParameters();
            dinParams.Add("@UserIdParam", User.FindFirst("userId")?.Value, DbType.Int32);

            return _dapper.LoadDataWithParams<Post>(postsSql, dinParams);
        }


        [HttpPut("Upsert")]
        public IActionResult Upsert(Post postToAdd)
        {
            string sqlUser = @"EXEC TutorialAppSchema.spPosts_Upster 
                    @UserId=@UserIdParam,
                    @PostTitle=@PostTitleParam,
                    @PostContent=@PostContentParam";

            DynamicParameters dinParams = new DynamicParameters();

            dinParams.Add("@UserIdParam", User.FindFirst("userId")?.Value, DbType.Int32);
            dinParams.Add("@PostTitleParam", postToAdd.PostTitle, DbType.String);
            dinParams.Add("@PostContentParam", postToAdd.PostContent, DbType.String);

            if (postToAdd.PostId > 0)
            {
                sqlUser += ", @PostId=@PostIdParam";
                dinParams.Add("@PostIdParam", postToAdd.PostId, DbType.Int32);
            }


            if (_dapper.ExecuteSqlWithParams(sqlUser, dinParams))
            {
                return Ok();
            }
            throw new Exception("Failed to create new post");
        }



        [HttpDelete("Delete/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Delete @PostId=@PostIdParam";

            DynamicParameters dinParams = new DynamicParameters();

            dinParams.Add("@PostIdParam", postId, DbType.Int32);

            if (_dapper.ExecuteSqlWithParams(sql, dinParams))
            {
                return Ok();
            }
            throw new Exception("Failed to delete post");
        }

    }
}