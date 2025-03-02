using BlogIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BlogIT.Controllers
{
    /* Hàm built in c#
    // ActionResult: Lớp cơ sở cho tất cả các kết quả action trong MVC
    // Controller: Lớp cơ sở cho tất cả controllers
    // HttpNotFound(): Phương thức trả về lỗi 404
    // RedirectToAction(): Chuyển hướng đến action khác
    // View(): Trả về view
    // ModelState.IsValid: Kiểm tra tính hợp lệ của model
    // TempData[]: Lưu trữ dữ liệu tạm thời giữa các requests
    // ViewBag: Dynamic property để truyền dữ liệu từ controller đến view
    // Session: Quản lý phiên làm việc
    */



    /*
     Index()
     Hàm có sẵn: Select(), Count(), OrderByDescending(), Take(), ToList()
     Chú thích: Các hàm này thuộc LINQ và EF, giúp truy vấn và xử lý dữ liệu hiệu quả.

    Hàm có sẵn: Count(), HasValue, Value
    Chú thích: Hàm Count() với điều kiện để đếm số lượng bài viết được tạo trong tháng hiện tại.
     
    /* 
    ManageTag()
    Hàm có sẵn: Count(), OrderBy(), Skip(), Take(), ToList()
    Chú thích: Dùng các hàm LINQ để hỗ trợ phân trang và sắp xếp tag.

    /* 
    LockPosts()
   Hàm có sẵn: Find(), Remove(), SaveChanges()
   Chú thích: Find() tìm kiếm đối tượng theo khóa chính; Remove() để xóa và SaveChanges() để cập nhật cơ sở dữ liệu.

     /* 
    CreateTag(Tag tag)
   Hàm có sẵn: Where(), Add(), SaveChanges()
   Chú thích: Sử dụng LINQ và EF để kiểm tra, thêm tag mới và lưu dữ liệu.

     /* 
    ManageUsers(Tag tag)
    Hàm có sẵn: OrderBy(), Skip(), Take(), ToList()
    Chú thích: Hàm LINQ hỗ trợ truy vấn và phân trang dữ liệu.

    /* 
     Logout()
    Hàm có sẵn: Remove(), RedirectToAction()
    Chú thích: Remove() loại bỏ thông tin trong phiên; RedirectToAction() điều hướng người dùng.

     */


    public class AdminController : Controller
    {   /* Đây là một controller MVC kế thừa từ lớp Controller cơ bản
        Tạo một đối tượng context database BlogITEntities để thực hiện các thao tác với CSDL */
                BlogITEntities db = new BlogITEntities();

        // GET: Trang Admin
        public ActionResult Index()
        {
            // Lấy danh sách bài viết với số lượt thích và bình luận nhiều nhất
            var topPosts = db.BlogPost
                .Select(post => new TopPostViewModel
                {
                    Post = post,
                    LikeCount = post.PostLike.Count(),
                    CommentCount = post.Comment.Count()
                })
                .OrderByDescending(x => x.LikeCount + x.CommentCount)
                .Take(10) // Giới hạn số bài viết hiển thị
                .ToList();

            // Thống kê số lượng bài viết được tạo ra trong tháng
            var postCountThisMonth = db.BlogPost
                .Count(post => post.CreatedAt.HasValue && post.CreatedAt.Value.Month == DateTime.Now.Month && post.CreatedAt.Value.Year == DateTime.Now.Year);

            ViewBag.TopPosts = topPosts;
            ViewBag.PostCountThisMonth = postCountThisMonth;

            return View();
        }

        public ActionResult BlogPosts()
        {
            var blogPosts = db.BlogPost.ToList();
            return View(blogPosts);
        }
        public ActionResult LockPosts(int id)
        {
            var blogPost = db.BlogPost.Find(id);
            if (blogPost != null)
            {
                db.BlogPost.Remove(blogPost);
                db.SaveChanges();
            }
            return RedirectToAction("BlogPosts");
        }
        public ActionResult Users()
        {
            var users = db.User.ToList();
            return View(users);
        }
        // GET: Admin/ManageTag
        public ActionResult ManageTag(string searchString, int page = 1, int pageSize = 6)
        {
            var Tag = db.Tag.AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                Tag = Tag.Where(t => t.Name.Contains(searchString));
            }

            // Phân trang
            var totalTag = Tag.Count();
            var TagToShow = Tag.OrderBy(t => t.TagID).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalTag / pageSize);

            return View(TagToShow);
        }

        // GET: Admin/ManageTag/Create
        public ActionResult CreateTag()
        {
            return View();
        }

        // POST: Admin/ManageTag/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTag(Tag tag)
        {
            if (ModelState.IsValid)
            {
                var kt = db.Tag.Any(u => u.Name == tag.Name); // Sửa lại điều kiện kiểm tra
                if (kt)
                {
                    TempData["mess"] = "Đã có tag này rồi";
                    return View(tag);
                }
                db.Tag.Add(tag);
                db.SaveChanges();
                TempData["mess"] = "Thêm thẻ mới thành công";
                return RedirectToAction("ManageTag");
            }
            return View(tag);
        }


        // GET: Admin/ManageTag/Edit/5
        public ActionResult EditTag(int id)
        {
            Tag tag = db.Tag.Find(id);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View(tag);
        }

        // POST: Admin/ManageTag/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTag(Tag tag)
        {
            if (ModelState.IsValid)
            {
                var edittag = db.Tag.Find(tag.TagID);
                edittag.Name = tag.Name;
                edittag.Status = tag.Status;
                db.SaveChanges();
                TempData["mess"] = "Sửa thẻ thành công";
                return RedirectToAction("ManageTag");
            }
            return View(tag);
        }

        // GET: Admin/ManageTag/Delete/5
        public ActionResult DeleteTag(int id)
        {
            Tag tag = db.Tag.Find(id);
            if (tag == null)
            {
                return HttpNotFound();
            }
            db.Tag.Remove(tag);
            db.SaveChanges();
            TempData["mess"] = "Xóa thẻ thành công";
            return RedirectToAction("ManageTag");
        }
        // GET: Admin/ManageComments
        public ActionResult ManageComments(string searchString, int page = 1, int pageSize = 6)
        {
            var comments = db.Comment.Include("BlogPost").Include("User").AsQueryable();

            // Tìm kiếm theo ten
            if (!string.IsNullOrEmpty(searchString))
            {
                if (int.TryParse(searchString, out int id))
                {
                    comments = comments.Where(c => c.CommentID == id);
                }
            }

            // Phân trang
            var totalComments = comments.Count();
            var commentsToShow = comments.OrderBy(c => c.CommentID).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalComments / pageSize);

            return View(commentsToShow);
        }
        public ActionResult ViewCommentReplies(int commentId)
        {
            var replies = db.CommentReply.Where(r => r.CommentID == commentId).ToList();
            ViewBag.CommentID = commentId; // Truyền ID bình luận gốc để hiển thị
            return View(replies);
        }
        public ActionResult LockCommentReply(int id)
        {
            var reply = db.CommentReply.Find(id);
            var commentId = reply.CommentID;
            if (reply != null)
            {
                db.CommentReply.Remove(reply);
                db.SaveChanges();
                TempData["mess"] = "Xóa bình luận phản hồi thành công!";
            }
            return RedirectToAction("ViewCommentReplies", new { commentId = commentId });
        }

        //POST: Comment/LockComment
        public ActionResult LockComment(int id)
        {
            var comment = db.Comment.Find(id);
            if (comment != null)
            {
                db.Comment.Remove(comment);
                db.SaveChanges();
                TempData["mess"] = "Bình luận đã được xóa thành công.";
            }
            return RedirectToAction("ManageComments");
        }
        // Quản lý người dùng với tìm kiếm và phân trang
        public ActionResult ManageUsers(string searchString, int page = 1, int pageSize = 6)
        {
            var users = db.User.Where(u=>u.UserID != 1 ).AsQueryable();

            // Tìm kiếm người dùng theo tên hoặc email
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.FullName.Contains(searchString) || u.Email.Contains(searchString));
            }

            // Phân trang
            var totalUsers = users.Count();
            var usersToShow = users.OrderBy(u => u.UserID).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            return View(usersToShow);
        }

        public ActionResult ChangeUserRole(int id, string role)
        {
            var user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (role == "admin" && user.Role == "customer")
            {
                user.Role = "admin";
                TempData["mess"] = "Đã phân quyền Admin cho người dùng thành công!";
            }
            else if (role == "customer" && user.Role == "admin")
            {
                user.Role = "customer";
                TempData["mess"] = "Đã hạ quyền xuống Customer thành công!";
            }
            else
            {
                TempData["mess"] = "Không thể thay đổi vai trò.";
            }

            db.SaveChanges();
            return RedirectToAction("ManageUsers");
        }

        // Khóa tài khoản người dùng
        public ActionResult LockUser(int id)
        {
            var user = db.User.Find(id);
            if (user != null)
            {
                user.Status = !user.Status;
                db.SaveChanges();
                TempData["mess"] = "Trạng thái tài khoản đã được cập nhật";
            }
            return RedirectToAction("ManageUsers");
        }
        public ActionResult ManagePosts(string searchString, int page = 1, int pageSize = 6)
        {
            var posts = db.BlogPost.AsQueryable();

            // Tìm kiếm bài viết
            if (!string.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(p => p.Title.Contains(searchString));
            }

            // Lọc bài viết công khai
            posts = posts.Where(p => p.IsPublic);

            ViewBag.CurrentSearch = searchString;

            // Phân trang
            ViewBag.TotalPages = Math.Ceiling((double)posts.Count() / pageSize);
            var paginatedPosts = posts.OrderBy(p => p.CreatedAt)
                                       .Skip((page - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToList();

            return View(paginatedPosts);
        }

        public ActionResult DeletePost(int id)
        {
            var post = db.BlogPost.Find(id);
            if (post != null)
            {
                db.BlogPost.Remove(post);
                db.SaveChanges();
            }
            return RedirectToAction("ManagePosts");
        }
        public ActionResult ManageReports(int? blogPage, int? commentPage, int? replyPage, int? userPage)
        {
            int pageSize = 5;

            // Các trang hiện tại cho từng loại báo cáo
            int currentBlogPage = blogPage ?? 1;
            int currentCommentPage = commentPage ?? 1;
            int currentReplyPage = replyPage ?? 1;
            int currentUserPage = userPage ?? 1;

            // Dữ liệu từ cơ sở dữ liệu
            var allBlogReports = db.ReportBlog.ToList();
            var allCommentReports = db.ReportComment.ToList();
            var allCommentReplyReports = db.ReportCommentReply.ToList();
            var allUserReports = db.ReportUser.ToList();

            // Phân trang cho từng loại báo cáo
            ViewBag.BlogReports = allBlogReports.Skip((currentBlogPage - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.CommentReports = allCommentReports.Skip((currentCommentPage - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.CommentReplyReports = allCommentReplyReports.Skip((currentReplyPage - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.UserReports = allUserReports.Skip((currentUserPage - 1) * pageSize).Take(pageSize).ToList();

            // Tổng số trang
            ViewBag.TotalBlogPages = (int)Math.Ceiling((double)allBlogReports.Count() / pageSize);
            ViewBag.TotalCommentPages = (int)Math.Ceiling((double)allCommentReports.Count() / pageSize);
            ViewBag.TotalCommentReplyPages = (int)Math.Ceiling((double)allCommentReplyReports.Count() / pageSize);
            ViewBag.TotalUserPages = (int)Math.Ceiling((double)allUserReports.Count() / pageSize);

            // Trang hiện tại
            ViewBag.CurrentBlogPage = currentBlogPage;
            ViewBag.CurrentCommentPage = currentCommentPage;
            ViewBag.CurrentReplyPage = currentReplyPage;
            ViewBag.CurrentUserPage = currentUserPage;

            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("user");
            return RedirectToAction("Login","Home");
        }
    }
}
