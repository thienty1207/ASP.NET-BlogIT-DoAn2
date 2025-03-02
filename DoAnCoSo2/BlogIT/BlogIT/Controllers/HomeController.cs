using BlogIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using System.IO;

namespace BlogIT.Controllers
{
    public class HomeController : Controller
    {
        BlogITEntities db = new BlogITEntities();
        public ActionResult Index(string searchTerm = null, int? tagId = null, int page = 1, int pageSize = 5)
        {
            // Truy vấn lấy danh sách blog có trạng thái là công khai và sắp xếp theo thời gian đăng
            var blogPostsQuery = db.BlogPost.Where(p => p.IsPublic);

            // Nếu có từ khóa tìm kiếm, thêm điều kiện vào truy vấn
            if (!string.IsNullOrEmpty(searchTerm))
            {
                blogPostsQuery = blogPostsQuery.Where(p => p.Title.Contains(searchTerm));
                ViewBag.search = searchTerm;
            }
            // Nếu có tagId, thêm điều kiện vào truy vấn để lọc theo tag
            if (tagId.HasValue)
            {
                blogPostsQuery = blogPostsQuery.Where(p => p.PostTag.Any(t => t.TagID == tagId));
                ViewBag.tag = tagId;
            }
            var blogPosts = blogPostsQuery
                               .OrderByDescending(p => p.CreatedAt)
                               .Skip((page - 1) * pageSize) // Bỏ qua các bài viết ở các trang trước
                               .Take(pageSize) // Lấy số bài viết cho trang hiện tại
                               .ToList();

            // Truy vấn lấy bài viết phổ biến (dựa trên số lượt like và comment)
            var popularPosts = db.BlogPost
                .OrderByDescending(p => p.PostLike.Count() + p.Comment.Count()) // Sắp xếp theo tổng số like và comment
                .Take(3) // Lấy 5 bài viết phổ biến nhất
                .ToList();

            // Truy vấn lấy các tag nổi bật (dựa trên số lần sử dụng)
            var popularTags = db.Tag
                .OrderByDescending(t => t.PostTag.Count()) // Sắp xếp theo số lần tag được sử dụng
                .Take(5) // Lấy 5 tag nổi bật nhất
                .ToList();

            // Lưu các bài viết phổ biến và tag nổi bật vào ViewBag
            ViewBag.PopularPosts = popularPosts;
            ViewBag.PopularTags = popularTags;
            // Tính toán tổng số trang
            int totalPosts = blogPostsQuery.Count(p => p.IsPublic);
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalPosts / pageSize);
            ViewBag.CurrentPage = page;
          
            // Lưu danh sách tags đầy đủ vào ViewBag
            ViewBag.Tags = db.Tag.ToList();

            return View(blogPosts);
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // bảo vệ khỏi tấn công CSRF: Cross-Site Request Forgery
        public ActionResult Login(string email, string password)
        {
            // Kiểm tra thông tin đăng nhập
            var hashpasss = HashPassword(password);
            var user = db.User.SingleOrDefault(u=> u.Email == email && u.PasswordHash == hashpasss);

            if (user != null)
            {
                // Đăng nhập thành công, có thể lưu thông tin người dùng vào session hoặc cookie
                Session["user"] = user; // Ví dụ lưu email vào session
                if (user.Status == false )
                {
                    ViewBag.ErrorMessage = "Tài khoản của bạn đã bị khóa vui lòng liên hệ admin";
                    return View();
                }
                if (user.Role == "customer")
                {
                    return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chính
                }

                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            else
            {
                // Đăng nhập thất bại
                ViewBag.ErrorMessage = "Địa chỉ email hoặc mật khẩu không đúng.";
            }

            return View();
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string FullName, string Email, string Password, string ConfirmPassword)
        {
            // Kiểm tra thông tin hợp lệ
            if (Password != ConfirmPassword)
            {
                // Thêm thông báo vào ViewBag
                ViewBag.ErrorMessage = "Mật khẩu không khớp.";
                return View();
            }

            // Kiểm tra xem email đã tồn tại chưa
            var existingUser = db.User.FirstOrDefault(u => u.Email == Email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Email đã được sử dụng.";
                return View();
            }

            // Tạo người dùng mới
            var user = new User
            {
                FullName = FullName,
                Email = Email,
                Role = "customer",
                PasswordHash = HashPassword(Password), // Hàm HashPassword sẽ mã hóa mật khẩu
                CreatedAt = DateTime.Now,
                Status = true // Hoặc tùy chỉnh theo yêu cầu của bạn
            };

            db.User.Add(user);
            db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            ViewBag.ErrorMessage = "Tạo tài khoản thành công";
            // Chuyển hướng đến trang chủ hoặc trang khác
            return View();
        }


        private string HashPassword(string password)
        {
            // Thực hiện mã hóa mật khẩu, ví dụ: sử dụng SHA256
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        public ActionResult Logout()
        {
            Session.Remove("user");
            return RedirectToAction("Index","Home");
        }
        public ActionResult Forget()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Forget(string email)
        {
            try
            {
                // Kiểm tra xem email có tồn tại trong cơ sở dữ liệu hay không
                var emailExists = db.User.SingleOrDefault(u=>u.Email == email);

                if (emailExists == null)
                {
                    ViewBag.Message = "Email không tồn tại.";
                    return View();
                }

                // Tạo mật khẩu mới
                string newPassword = GenerateRandomPassword(6); // Giả sử bạn muốn mật khẩu có 6 chữ số

                // Cập nhật mật khẩu mới vào cơ sở dữ liệu
                emailExists.PasswordHash = HashPassword(newPassword);
                db.SaveChanges();
                // Tạo email message
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("tytybill123@gmail.com"), // Địa chỉ email của bạn
                    Subject = "Khôi phục mật khẩu",
                    IsBodyHtml = true // Nếu bạn muốn gửi email dưới dạng HTML
                };

                // Thiết lập nội dung email với mật khẩu mới
                mail.Body = $"<p>Xin chào,</p>" +
                             $"<p>Mật khẩu mới của bạn là: <strong>{newPassword}</strong></p>" +
                             $"<p>Vui lòng đăng nhập và thay đổi mật khẩu ngay khi bạn có thể.</p>";

                mail.To.Add(email); // Gửi email tới địa chỉ của người dùng

                // Cấu hình SMTP
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("tytybill123@gmail.com", "eqir hqsv bnxv lrkd"); // Đăng nhập email
                    smtpClient.EnableSsl = true; // Sử dụng SSL

                    // Gửi email
                    smtpClient.Send(mail);
                }

                // Thông báo gửi thành công
                ViewBag.Message = "Email khôi phục mật khẩu đã được gửi đến bạn!";
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                ViewBag.Message = "Gửi email thất bại: " + ex.Message;
            }

            return View();
        }

        public ActionResult ChangePassword()
        {
            return View(); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var user = Session["user"] as User; // Lấy người dùng hiện tại

            if (user == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra mật khẩu cũ
            if (user.PasswordHash != HashPassword(oldPassword))
            {
                ViewBag.Message = "Mật khẩu cũ không chính xác.";
                return View(user); // Trả về view với thông báo lỗi
            }

            // Kiểm tra mật khẩu mới và nhập lại mật khẩu mới
            if (newPassword != confirmPassword)
            {
                ViewBag.Message = "Mật khẩu mới và mật khẩu xác nhận không khớp.";
                return View(user); // Trả về view với thông báo lỗi
            }

            // Cập nhật mật khẩu mới
            var usertt = db.User.Find(user.UserID);
            usertt.PasswordHash = HashPassword(newPassword);
            db.SaveChanges(); // Lưu thay đổi vào DB

            ViewBag.Message = "Mật khẩu đã được thay đổi thành công!";
            return View();  // Chuyển hướng trở lại trang đổi mật khẩu
        }

        private string GenerateRandomPassword(int length)
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // Tạo mật khẩu 6 chữ số
        }


        public ActionResult Bookmarks(int page = 1, int pageSize = 5) // pageSize là số lượng bookmark hiển thị mỗi trang
        {
            var userId = (Session["user"] as User)?.UserID; // Lấy ID người dùng từ session

            if (userId == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách bookmark của người dùng từ database
            var bookmarks = db.Bookmark.Where(b => b.UserID == userId).ToList();

            // Tính toán số lượng trang
            int totalItems = bookmarks.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Lấy bookmark cho trang hiện tại
            var pagedBookmarks = bookmarks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Lưu thông tin vào ViewBag
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(pagedBookmarks);
        }


        public ActionResult Detail(int id)
        {
            var blog = db.BlogPost.SingleOrDefault(u=> u.PostID == id);
            // Lấy danh sách bình luận của bài viết
            var comments = db.Comment.Where(c => c.PostID == id).ToList();

            // Truyền danh sách bình luận qua ViewBag
            ViewBag.Comments = comments;
            var latestBlogs = db.BlogPost
                       .Where(b => b.UserID == blog.UserID)  // Lọc theo tác giả
                       .OrderByDescending(b => b.CreatedAt)  // Sắp xếp theo ngày đăng
                       .Take(3)  // Lấy 3 blog mới nhất
                       .ToList();

            ViewBag.LatestBlogs = latestBlogs;
            return View(blog);
        }
        [HttpPost]
        public ActionResult PostComment(int postId, string content)
        {
            var userId = (Session["user"] as User)?.UserID; // Lấy ID người dùng từ session
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var user = db.BlogPost.SingleOrDefault(u => u.PostID == postId);
            var usercommet = db.User.SingleOrDefault(u=>u.UserID == userId);
            // Tạo bình luận mới
            Comment newComment = new Comment
            {
                PostID = postId,
                UserID = userId,
                Content = content,
                CreatedAt = DateTime.Now
            };
            var newNoti = new Notification
            {
                UserID = user.UserID,
                Message = usercommet.FullName + " vừa bình luận bài viết của bạn",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            db.Notification.Add(newNoti);
            db.SaveChanges();
            // Thêm vào cơ sở dữ liệu
            db.Comment.Add(newComment);
            db.SaveChanges();

            // Chuyển hướng về lại trang chi tiết của bài viết
            return RedirectToAction("Detail", new { id = postId });
        }


        [HttpPost]
        public ActionResult PostReply(int commentId, string content)
        {
            var userId = (Session["user"] as User)?.UserID; // Lấy ID người dùng từ session
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var user = db.User.SingleOrDefault(u => u.UserID == userId);
            // Tạo trả lời bình luận mới
            CommentReply newReply = new CommentReply
            {
                CommentID = commentId, // ID của bình luận gốc
                UserID = userId,
                Content = content,
                CreatedAt = DateTime.Now
            };

            // Thêm vào cơ sở dữ liệu
            db.CommentReply.Add(newReply);
            db.SaveChanges();
            var comment = db.Comment.SingleOrDefault(u => u.CommentID == commentId);
            var newNoti = new Notification
            {
                UserID = comment.UserID,
                Message = user.FullName + " vừa trả lời bình luận của bạn",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            db.Notification.Add(newNoti);
            db.SaveChanges();
            // Chuyển hướng về lại trang chi tiết của bài viết
            var postId = db.Comment.Where(c => c.CommentID == commentId).Select(c => c.PostID).FirstOrDefault();
            return RedirectToAction("Detail", new { id = postId });
        }

        [HttpPost]
        public ActionResult DeleteComment(int commentId)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["user"] == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện thao tác này." });
            }

            // Lấy thông tin người dùng hiện tại
            var currentUser = (BlogIT.Models.User)Session["user"];

            // Tìm bình luận theo commentId
            var comment = db.Comment.FirstOrDefault(c => c.CommentID == commentId);

            if (comment == null)
            {
                return Json(new { success = false, message = "Bình luận không tồn tại." });
            }

            // Kiểm tra quyền của người dùng (chỉ cho phép xóa bình luận của chính mình)
            if (comment.UserID != currentUser.UserID)
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa bình luận này." });
            }

            try
            {
                // Xóa bình luận
                db.Comment.Remove(comment);
                db.SaveChanges();

                return Json(new { success = true, message = "Đã xóa bình luận thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra. Vui lòng thử lại sau." });
            }
        }
        [HttpPost]
        public ActionResult DeleteReply(int replyId)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["user"] == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện thao tác này." });
            }

            // Lấy thông tin người dùng hiện tại
            var currentUser = (BlogIT.Models.User)Session["user"];

            // Tìm phản hồi theo replyId
            var reply = db.CommentReply.FirstOrDefault(r => r.ReplyID == replyId);

            if (reply == null)
            {
                return Json(new { success = false, message = "Phản hồi không tồn tại." });
            }

            // Kiểm tra quyền của người dùng (chỉ cho phép xóa phản hồi của chính mình)
            if (reply.UserID != currentUser.UserID)
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa phản hồi này." });
            }

            try
            {
                // Xóa phản hồi
                db.CommentReply.Remove(reply);
                db.SaveChanges();

                return Json(new { success = true, message = "Đã xóa phản hồi thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra. Vui lòng thử lại sau." });
            }
        }
        // GET: Hiển thị form báo cáo bình luận
        public ActionResult ReportComment(int commentId)
        {
            var currentUser = Session["user"] as User;

            // Kiểm tra nếu người dùng chưa đăng nhập
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.CommentID = commentId; // Truyền ID bình luận bị tố cáo sang View
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportComment(int commentID, string reason)
        {
            var currentUser = Session["user"] as User;

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Kiểm tra xem đã tồn tại bản ghi tố cáo chưa
            var existingReport = db.ReportComment.SingleOrDefault(r => r.ReporterID == currentUser.UserID && r.CommentID == commentID);

            if (existingReport != null)
            {
                TempData["Message"] = "Bạn đã tố cáo bình luận này trước đó.";
            }
            else
            {
                // Thêm bản ghi tố cáo mới
                var newReport = new ReportComment
                {
                    ReporterID = currentUser.UserID,
                    CommentID = commentID,
                    Reason = reason,
                    ReportedAt = DateTime.Now
                };

                db.ReportComment.Add(newReport);
                db.SaveChanges();

                TempData["Message"] = "Tố cáo bình luận thành công.";
            }

            return RedirectToAction("Index", "Home"); // Hoặc redirect đến view thích hợp
        }
        // GET: Hiển thị form báo cáo bình luận
        public ActionResult ReportCommentReply(int commentId)
        {
            var currentUser = Session["user"] as User;

            // Kiểm tra nếu người dùng chưa đăng nhập
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.CommentReplyID = commentId; // Truyền ID bình luận bị tố cáo sang View
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportCommentReply(int commentID, string reason)
        {
            var currentUser = Session["user"] as User;

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Kiểm tra xem đã tồn tại bản ghi tố cáo chưa
            var existingReport = db.ReportCommentReply.SingleOrDefault(r => r.ReporterID == currentUser.UserID && r.CommentReplyID == commentID);

            if (existingReport != null)
            {
                TempData["Message"] = "Bạn đã tố cáo bình luận này trước đó.";
            }
            else
            {
                // Thêm bản ghi tố cáo mới
                var newReport = new ReportCommentReply
                {
                    ReporterID = currentUser.UserID,
                    CommentReplyID = commentID,
                    Reason = reason,
                    ReportedAt = DateTime.Now
                };

                db.ReportCommentReply.Add(newReport);
                db.SaveChanges();

                TempData["Message"] = "Tố cáo bình luận thành công.";
            }

            return RedirectToAction("Index", "Home"); // Hoặc redirect đến view thích hợp
        }
        public ActionResult ReportBlog(int blogId)
        {
            var currentUser = Session["user"] as User;

            // Kiểm tra nếu người dùng chưa đăng nhập
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.ReportedBlogID = blogId; // Truyền ID blog bị tố cáo sang View
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportBlog(int reportedBlogID, string reason)
        {
            var currentUser = Session["user"] as User;

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Kiểm tra xem đã tồn tại bản ghi tố cáo chưa
            var existingReport = db.ReportBlog.SingleOrDefault(r => r.ReporterID == currentUser.UserID && r.PostID == reportedBlogID);

            if (existingReport != null)
            {
                TempData["Message"] = "Bạn đã tố cáo blog này trước đó.";
            }
            else
            {
                // Thêm bản ghi tố cáo mới
                var newReport = new ReportBlog
                {
                    ReporterID = currentUser.UserID,
                    PostID = reportedBlogID,
                    Reason = reason,
                    ReportedAt = DateTime.Now
                };

                db.ReportBlog.Add(newReport);
                db.SaveChanges();

                TempData["Message"] = "Tố cáo thành công.";
            }

            return RedirectToAction("Detail", "Home", new { id = reportedBlogID });
        }


        [HttpPost]
        public JsonResult ToggleLike(int postId)
        {
            var userId = (Session["user"] as User)?.UserID; // Lấy ID người dùng từ session
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện thao tác này." });
            }

            var existingLike = db.PostLike.FirstOrDefault(l => l.PostID == postId && l.UserID == userId);

            if (existingLike != null)
            {
                // Nếu đã like, thì bỏ like
                db.PostLike.Remove(existingLike);
                db.SaveChanges();
                return Json(new { success = true, liked = false });
            }
            else
            {
                // Nếu chưa like, thì thêm like
                var newLike = new PostLike
                {
                    PostID = postId,
                    UserID = userId,
                    LikedAt = DateTime.Now
                };
                db.PostLike.Add(newLike);
                //tìm kiếm bài post
                var post = db.BlogPost.SingleOrDefault(u=>u.PostID == postId);
                var user = db.User.SingleOrDefault(u=>u.UserID == userId);
                //tạo thông báo và lưu vào db để thông báo cho người dùng
                var newNoti = new Notification
                {
                    UserID = post.UserID,
                    Message = user.FullName + " vừa thích bài viết của bạn",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                db.Notification.Add(newNoti);
                db.SaveChanges();
                return Json(new { success = true, liked = true });
            }
        }
        public JsonResult MarkNotificationsAsRead()
        {
            var user = Session["user"] as User; // Lấy thông tin người dùng từ session
            if (user != null)
            {
                // Lấy tất cả thông báo của người dùng
                var notifications = db.Notification
                                      .Where(n => n.UserID == user.UserID && n.IsRead == false)
                                      .ToList();

                // Đánh dấu tất cả thông báo là đã đọc
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }

                db.SaveChanges(); // Lưu thay đổi vào database

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "User not found." });
        }
        [HttpPost]
        public JsonResult ToggleBookmark(int postId)
        {
            var userId = (Session["user"] as User)?.UserID; // Lấy ID người dùng từ session
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện thao tác này." });
            }

            var bookmark = db.Bookmark.FirstOrDefault(b => b.PostID == postId && b.UserID == userId);
            if (bookmark == null)
            {
                // Nếu chưa đánh dấu, thêm mới
                bookmark = new Bookmark { PostID = postId, UserID = userId };
                db.Bookmark.Add(bookmark);
                db.SaveChanges();
                return Json(new { success = true, bookmarked = true });
            }
            else
            {
                // Nếu đã đánh dấu, xóa
                db.Bookmark.Remove(bookmark);
                db.SaveChanges();
                return Json(new { success = true, bookmarked = false });
            }
        }
        public ActionResult Profile(int id, int? page)
        {
            var userId = (Session["user"] as User)?.UserID; // Lấy ID người dùng từ session
         

            // Lấy thông tin người dùng từ cơ sở dữ liệu
            var user = db.User.FirstOrDefault(u => u.UserID == id );
            if (user == null)
            {
                return HttpNotFound();
            }

            // Thiết lập số lượng bài đăng trên mỗi trang
            int pageSize = 4;
            int currentPage = page ?? 1;
            var blogPosts = user.BlogPost;
            if (userId == id) {
                // Lấy danh sách bài đăng của người dùng và phân trang
                blogPosts = user.BlogPost
                                .OrderByDescending(b => b.CreatedAt)
                                .Skip((currentPage - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            }
            else
            {
                // Lấy danh sách bài đăng của người dùng và phân trang
                blogPosts = user.BlogPost
                                .Where(b => b.IsPublic)
                                .OrderByDescending(b => b.CreatedAt)
                                .Skip((currentPage - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            }
           

            // Tổng số trang
            int totalPosts = user.BlogPost.Count(b => b.IsPublic);
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            // Truyền dữ liệu qua ViewBag
            ViewBag.PagedPosts = blogPosts;
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;

            return View(user);
        }
        public ActionResult FollowUser(int userId)
        {
            var currentUser = Session["user"] as User;

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Home"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            var followRecord = db.Follow.SingleOrDefault(f => f.FollowerID == currentUser.UserID && f.FollowingID == userId);

            if (followRecord != null)
            {
                // Nếu đã theo dõi, thực hiện hủy theo dõi
                db.Follow.Remove(followRecord);
                db.SaveChanges();
            }
            else
            {
                // Nếu chưa theo dõi, thực hiện theo dõi
                var newFollow = new Follow
                {
                    FollowerID = currentUser.UserID,
                    FollowingID = userId,
                    FollowedAt = DateTime.Now
                };
                db.Follow.Add(newFollow);
                var newNoti = new Notification
                {
                    UserID = userId,
                    Message = currentUser.FullName + " vừa theo dõi bạn",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                db.Notification.Add(newNoti);
                db.SaveChanges();
            }

            return RedirectToAction("Profile", new { id = userId });
        }
        public ActionResult ReportUser(int userId)
        {
            var currentUser = Session["user"] as User;

            // Kiểm tra nếu người dùng chưa đăng nhập
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.ReportedUserID = userId; // Truyền ID người dùng bị tố cáo sang View
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportUser(int reportedUserID, string reason)
        {
            var currentUser = Session["user"] as User;

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Kiểm tra xem đã tồn tại bản ghi tố cáo chưa
            var existingReport = db.ReportUser.SingleOrDefault(r => r.ReporterID == currentUser.UserID && r.ReportedUserID == reportedUserID);

            if (existingReport != null)
            {
                TempData["Message"] = "Bạn đã tố cáo người dùng này trước đó.";
            }
            else
            {
                // Thêm bản ghi tố cáo mới
                var newReport = new ReportUser
                {
                    ReporterID = currentUser.UserID,
                    ReportedUserID = reportedUserID,
                    Reason = reason,
                    ReportedAt = DateTime.Now
                };

                db.ReportUser.Add(newReport);
                db.SaveChanges();

                TempData["Message"] = "Tố cáo thành công.";
            }

            return RedirectToAction("Profile", new { id = reportedUserID });
        }

        public ActionResult CreatePost() {
            ViewBag.Tags = db.Tag.Where(u=> u.Status == true).ToList();
            return View();
        
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(BlogPost post, HttpPostedFileBase Image, bool IsPublic, int[] Tags)
        {
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/images/blog/"), fileName);
                    Image.SaveAs(path);
                    post.Image = fileName; // Lưu tên hình ảnh
                }

                post.UserID = ((User)Session["user"]).UserID; // Gán UserID cho bài viết
                post.CreatedAt = DateTime.Now;
                post.IsPublic = IsPublic; // Gán giá trị IsPublic từ form
                db.BlogPost.Add(post);
                db.SaveChanges();
                // Lưu tag cho bài viết
                if (Tags != null)
                {
                    foreach (var tagId in Tags)
                    {
                        var postTag = new PostTag
                        {
                            PostID = post.PostID, // Gán PostID đã được tạo
                            TagID = tagId
                        };
                        db.PostTag.Add(postTag);
                    }
                    db.SaveChanges();
                }
                // Thêm thông báo cho tất cả người theo dõi
                var followers = db.Follow
                                  .Where(f => f.FollowingID == post.UserID)
                                  .Select(f => f.FollowerID)
                                  .ToList();

                foreach (var followerId in followers)
                {
                    var notification = new Notification
                    {
                        UserID = followerId,
                        Message = $"{((User)Session["user"]).FullName} đã tạo một bài viết mới: {post.Title}",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    db.Notification.Add(notification);
                }
                db.SaveChanges(); // Lưu tất cả thông báo vào cơ sở dữ liệu

                return RedirectToAction("Profile", new { id = post.UserID });
            }

            return View(post); // Trả về view nếu có lỗi
        }
        public ActionResult EditPost(int id)
        {
            var post = db.BlogPost.Find(id); // Tìm bài viết theo ID
            if (post == null)
            {
                return HttpNotFound(); // Trả về 404 nếu không tìm thấy
            }
            ViewBag.Tags = db.Tag.ToList();
            return View(post); // Trả về view với thông tin bài viết
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(BlogPost post, HttpPostedFileBase Image, bool IsPublic, int[] Tags)
        {
            if (ModelState.IsValid)
            {
                var existingPost = db.BlogPost.Find(post.PostID); // Tìm bài viết hiện tại
                if (existingPost != null)
                {
                    // Cập nhật thông tin bài viết
                    existingPost.Title = post.Title;
                    existingPost.Content = post.Content;
                    existingPost.IsPublic = IsPublic; // Gán giá trị IsPublic từ form

                    if (Image != null)
                    {
                        // Xóa hình ảnh cũ nếu cần thiết
                        if (!string.IsNullOrEmpty(existingPost.Image))
                        {
                            var oldImagePath = Path.Combine(Server.MapPath("~/images/blog/"), existingPost.Image);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath); // Xóa hình ảnh cũ
                            }
                        }
                     

                        var fileName = Path.GetFileName(Image.FileName);
                        var path = Path.Combine(Server.MapPath("~/images/blog/"), fileName);
                        Image.SaveAs(path);
                        existingPost.Image = fileName; // Lưu tên hình ảnh mới
                    }
                    if (Tags != null)
                    {
                        var ta = db.PostTag.Where(u => u.PostID == post.PostID);
                        if (ta != null) { 
                            db.PostTag.RemoveRange(ta);
                            
                        }
                        foreach (var tagId in Tags)
                        {
                            var postTag = new PostTag
                            {
                                PostID = existingPost.PostID, // Gán PostID đã được tạo
                                TagID = tagId
                            };
                            db.PostTag.Add(postTag);
                        }
                        db.SaveChanges();
                    }
                    db.SaveChanges(); // Lưu thay đổi

                    return RedirectToAction("Profile", new { id = existingPost.UserID });
                }
            }

            return View(post); // Trả về view nếu có lỗi
        }
        //xoá bài viết
        public ActionResult DeletePost(int id)
        {
            var post = db.BlogPost.Find(id);
            if (post != null)
            {
                db.BlogPost.Remove(post); // Xóa bài viết
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }
            var userId = (Session["user"] as User)?.UserID; // Lấy ID người dùng từ session
            return RedirectToAction("Profile", new { id = userId }); // Chuyển hướng về trang Profile sau khi xóa
        }
        public ActionResult ChangeProfile()
        {
            var userId = (Session["user"] as User)?.UserID; // Lấy ID người dùng từ session
            var user = db.User.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        // POST: User/ChangeProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeProfile(User user, HttpPostedFileBase ProfileImage)
        {
            if (ModelState.IsValid)
            {
                var userId = (Session["user"] as User).UserID; // Lấy ID người dùng từ session
                var existingUser = db.User.Find(userId);
                if (existingUser != null)
                {
                    existingUser.FullName = user.FullName;
                    existingUser.Bio = user.Bio;

                    // Xử lý lưu hình ảnh
                    if (ProfileImage != null && ProfileImage.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(ProfileImage.FileName);
                        var path = Path.Combine(Server.MapPath("~/images/avt"), fileName);
                        ProfileImage.SaveAs(path);
                        existingUser.ProfileImage = fileName; // Lưu tên hình ảnh vào cơ sở dữ liệu
                    }

                    db.SaveChanges();
                    Session["user"] = existingUser;
                    return RedirectToAction("Profile", new {id= userId }); // Redirect về trang profile sau khi cập nhật
                }
            }
            return View(user);
        }
    }
}