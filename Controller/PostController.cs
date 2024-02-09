using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string postsSql = @"Select * from TutorialAppSchema.Posts";
            return  _dapper.LoadData<Post>(postsSql);
        }

        [HttpGet("GetSinglePost/{postId}")]
        public IActionResult GetSinglePost(int postId)
        {
             string postsSql = @"Select * from TutorialAppSchema.Posts Where PostId = "+
             postId.ToString();
            Post post =_dapper.LoadDataSingle<Post>(postsSql);
            if(post != null)
            {
                throw new Exception("Post is not fount.");
            }
            return Ok(post);
            
        }

        [HttpGet("GetMyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string postsSql = @"Select * from TutorialAppSchema.Posts Where UserId = " +
                User.FindFirst("userId")?.Value;
            return  _dapper.LoadData<Post>(postsSql);
        }

        [HttpGet("GetPostsByUser/{userId}")]
        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            string postsSql = @"Select * from TutorialAppSchema.Posts Where UserId = " +
                userId.ToString();
            return  _dapper.LoadData<Post>(postsSql);
        }

        [HttpPost("Create")]
        public IActionResult CreatePost(PostToAddDto postToAdd)
        {
            string sqlUser  = @"Insert TutorialAppSchema.Posts 
                            ([UserId], [PostTitle], [PostContent], [PostCreated], [PostUpdated])
                            VALUES ('"+ User.FindFirst("userId")?.Value +
                            "', '" + postToAdd.PostTitle +
                            "','" + postToAdd.PostContent +
                            "', GETDATE(), GETDATE())";
       
            if(_dapper.ExecuteSql(sqlUser))
            {
                return Ok();
            }
            throw new Exception("Failed to create new post");
        }

        [HttpPut("Edit")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            string sql = @"Update TutorialAppSchema.Posts
                SET [PostTitle] = '" + postToEdit.PostTitle +
                "',[PostContent] = '" + postToEdit.PostContent +
                "',[PostUpdated] = GETDATE()" + 
                " Where PostId = "+postToEdit.PostId;
            Console.WriteLine(sql);
            if(_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed updating post");
        }

        [HttpDelete("Delete/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"DELETE FROM TutorialAppSchema.Posts
                Where PostId = "+postId.ToString();
            Console.WriteLine(sql);
            if(_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete post");
        }

        [HttpGet("Search/{searchStr}")]
        public IEnumerable<Post> GetPostsBySearch(string searchStr)
        {
            string postsSql = @"Select * from TutorialAppSchema.Posts Where PostTitle like '%" +
                searchStr + "%' OR PostContent Like '%"+ searchStr+"%'";
            return  _dapper.LoadData<Post>(postsSql);
        }

    }
}