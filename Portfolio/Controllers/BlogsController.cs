﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Models;
using Portfolio.Services;
using Portfolio.ViewModels;

namespace Portfolio.Controllers
{
    public class BlogsController : Controller
    {
        // GET: Blogs
        public ActionResult Index()
        {
            // Reverse to get in order of latest
            var blogs = new BlogService().RetrieveBlogs();
            blogs.Reverse();
            // Take first three
            var viewModel = blogs.Take(3);

            return View(viewModel);
        }

        // GET: Blogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var viewModel = new BlogService().RetrieveBlogVmById(id);

            if (viewModel == null) return NotFound();

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult LoadMoreBlog(int count)
        {
            try
            {
                //  Get blogs & reverse to get in order of latest
                var blogs = new BlogService().RetrieveBlogs();
                blogs.Reverse();

                // Skip the blogs already loaded, select next three
                var model = blogs.Skip(count).Take(3);

                // Set ViewBag for loops in View
                ViewBag.Count = count; // Blogs skipped
                ViewBag.BlogCount = model.Count(); // Amount of blogs to display

                return View("_LoadBlog", model);
            }
            catch (Exception e)
            {
                Console.WriteLine("fail :(" + e);
                return View("Index");
            }
        }

        // GET: Blogs/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BlogViewModel blogViewModel)
        {
            if (!ModelState.IsValid) return View(blogViewModel);

            new BlogService().CreateBlog(blogViewModel);

            return RedirectToAction(nameof(Index));
        }

        // GET: Blogs/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var viewModel = new BlogService().RetrieveBlogVmById(id);

            if (viewModel == null) return NotFound();

            return View(viewModel);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, BlogViewModel blog)
        {
            if (!ModelState.IsValid) return View(blog);

            if (id != blog.BlogId) return NotFound();

            new BlogService().EditBlog(id, blog);

            return RedirectToAction(nameof(Index));
        }

        // GET: Blogs/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var viewModel = new BlogService().RetrieveBlogVmById(id);

            if (viewModel == null) return NotFound();

            return View(viewModel);
        }

        // POST: Blogs/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            new BlogService().DeleteBlogById(id);

            return RedirectToAction(nameof(Index));
        }

        #region Comment

        // POST: New Comment
        [HttpPost]
        public ActionResult CreateComment(CommentViewModel comment)
        {
            if (!ModelState.IsValid) return View(nameof(Details), new BlogService().RetrieveBlogVmById(comment.BlogId));

            new BlogService().CreateComment(comment);

            return RedirectToAction(nameof(Details),  new  {id = comment.BlogId });
        }

        // POST: Delete Comment
        [HttpPost]
        public ActionResult DeleteComment(CommentViewModel comment)
        {
            new BlogService().DeleteCommentById(comment.CommentId);

            return RedirectToAction(nameof(Details), new { id = comment.BlogId });
        }

        #endregion

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
                { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
