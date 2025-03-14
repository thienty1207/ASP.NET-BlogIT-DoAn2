USE [master]
GO
/****** Object:  Database [BlogIT]    Script Date: 25/10/2024 9:32:59 CH ******/
CREATE DATABASE [BlogIT]

USE [BlogIT]
GO
/****** Object:  Table [dbo].[BlogPost]    Script Date: 25/10/2024 9:32:59 CH ******/

SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlogPost](
	[PostID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[IsPublic] [bit] NOT NULL,
	[CreatedAt] [datetime] NULL,
	[Image] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[PostID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bookmark]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookmark](
	[BookmarkID] [int] IDENTITY(1,1) NOT NULL,
	[PostID] [int] NULL,
	[UserID] [int] NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[BookmarkID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Comment]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comment](
	[CommentID] [int] IDENTITY(1,1) NOT NULL,
	[PostID] [int] NULL,
	[UserID] [int] NULL,
	[Content] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CommentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CommentReply]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommentReply](
	[ReplyID] [int] IDENTITY(1,1) NOT NULL,
	[CommentID] [int] NULL,
	[UserID] [int] NULL,
	[Content] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReplyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Follow]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Follow](
	[FollowerID] [int] NOT NULL,
	[FollowingID] [int] NOT NULL,
	[FollowedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[FollowerID] ASC,
	[FollowingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[NotificationID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[Message] [nvarchar](max) NULL,
	[IsRead] [bit] NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[NotificationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PostLike]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PostLike](
	[LikeID] [int] IDENTITY(1,1) NOT NULL,
	[PostID] [int] NULL,
	[UserID] [int] NULL,
	[LikedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[LikeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PostTag]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PostTag](
	[PostID] [int] NOT NULL,
	[TagID] [int] NOT NULL,
	[PostTagID] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_posttag] PRIMARY KEY CLUSTERED 
(
	[PostTagID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportBlog]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportBlog](
	[ReportID] [int] IDENTITY(1,1) NOT NULL,
	[PostID] [int] NULL,
	[ReporterID] [int] NULL,
	[Reason] [nvarchar](max) NULL,
	[ReportedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportComment]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportComment](
	[ReportID] [int] IDENTITY(1,1) NOT NULL,
	[CommentID] [int] NULL,
	[ReporterID] [int] NULL,
	[Reason] [nvarchar](max) NULL,
	[ReportedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportUser]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportUser](
	[ReportID] [int] IDENTITY(1,1) NOT NULL,
	[ReportedUserID] [int] NULL,
	[ReporterID] [int] NULL,
	[Reason] [nvarchar](max) NULL,
	[ReportedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tag]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tag](
	[TagID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Status] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TagID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 25/10/2024 9:32:59 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[FullName] [nvarchar](100) NULL,
	[Bio] [nvarchar](max) NULL,
	[ProfileImage] [nvarchar](255) NULL,
	[Role] [nvarchar](20) NULL,
	[CreatedAt] [datetime] NULL,
	[Status] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[BlogPost] ON 

INSERT [dbo].[BlogPost] ([PostID], [UserID], [Title], [Content], [IsPublic], [CreatedAt], [Image]) VALUES (1, 3, N'Khám Phá Công Nghệ Mới Nhất Năm 2024', N'<h2>Công Nghệ Mới Nhất</h2><p>Năm 2024 đã đến và công nghệ tiếp tục phát triển với tốc độ chóng mặt. Trong thời đại số hiện nay, chúng ta chứng kiến sự ra đời của nhiều công nghệ tiên tiến, từ trí tuệ nhân tạo đến thực tế ảo. Các công ty lớn đang đầu tư mạnh vào nghiên cứu và phát triển, nhằm mang đến những sản phẩm và dịch vụ tốt nhất cho người tiêu dùng. Trí tuệ nhân tạo đang dần trở thành một phần không thể thiếu trong cuộc sống hàng ngày của chúng ta, từ việc hỗ trợ trong công việc đến giải trí. Chúng ta hãy cùng khám phá những công nghệ mới này và những ứng dụng của chúng trong cuộc sống hàng ngày.</p><p>Công nghệ 5G đang dần trở thành hiện thực, mang lại tốc độ internet nhanh hơn bao giờ hết. Điều này không chỉ giúp người dùng trải nghiệm tốt hơn mà còn mở ra nhiều cơ hội cho các doanh nghiệp trong việc phát triển các ứng dụng và dịch vụ mới. Hãy cùng theo dõi những xu hướng công nghệ mới và tìm hiểu cách chúng sẽ thay đổi cuộc sống của chúng ta trong những năm tới.</p>', 1, CAST(N'2024-10-23T22:06:33.307' AS DateTime), N'blog_07.jpg')
INSERT [dbo].[BlogPost] ([PostID], [UserID], [Title], [Content], [IsPublic], [CreatedAt], [Image]) VALUES (2, 3, N'Top 5 Smartphone Đáng Mua Nhất Năm 2024', N'<h2>Điện Thoại Thông Minh</h2><p>Năm 2024, thị trường điện thoại thông minh đang sôi động với sự ra mắt của nhiều sản phẩm mới. Dưới đây là danh sách 5 mẫu smartphone đáng mua nhất trong năm nay. Đầu tiên, chúng ta không thể không nhắc đến mẫu điện thoại flagship của một thương hiệu nổi tiếng với thiết kế sang trọng và hiệu năng vượt trội. Với camera chất lượng cao, bạn có thể chụp những bức ảnh đẹp như ý. Thứ hai, một mẫu điện thoại tầm trung với giá cả phải chăng nhưng vẫn đảm bảo hiệu năng mượt mà. Đối với những ai yêu thích chụp ảnh, mẫu điện thoại này cũng không làm bạn thất vọng với hệ thống camera đa dạng và tính năng chụp ảnh ấn tượng.</p><p>Bên cạnh đó, một chiếc điện thoại với viên pin lớn giúp bạn có thể sử dụng suốt cả ngày mà không lo hết pin. Một lựa chọn khác dành cho những ai yêu thích sự nhỏ gọn, chiếc điện thoại này mang đến trải nghiệm tuyệt vời mà không tốn quá nhiều không gian. Cuối cùng, mẫu điện thoại này nổi bật với khả năng chống nước và bụi, rất phù hợp cho những người thường xuyên hoạt động ngoài trời. Hãy cùng khám phá và chọn cho mình một chiếc smartphone ưng ý trong năm 2024 này.</p>', 1, CAST(N'2024-10-23T22:06:33.307' AS DateTime), N'blog_08.jpg')
INSERT [dbo].[BlogPost] ([PostID], [UserID], [Title], [Content], [IsPublic], [CreatedAt], [Image]) VALUES (3, 2, N'Xu Hướng Thiết Kế Web Năm 2024', N'<h2>Thiết Kế Web Hiện Đại</h2><p>Thiết kế web không chỉ là việc tạo ra một trang web hấp dẫn mà còn là việc tối ưu hóa trải nghiệm người dùng. Năm 2024, có một số xu hướng thiết kế web nổi bật mà các nhà thiết kế cần chú ý. Đầu tiên, việc sử dụng hình ảnh lớn và video sẽ ngày càng trở nên phổ biến, giúp thu hút sự chú ý của người dùng ngay từ cái nhìn đầu tiên. Thứ hai, thiết kế tối giản vẫn tiếp tục được ưa chuộng với việc sử dụng màu sắc nhạt và không gian trắng để tạo cảm giác thoáng đãng.</p><p>Các yếu tố tương tác, như nút cuộn và hiệu ứng hover, cũng là những điểm nhấn quan trọng giúp nâng cao trải nghiệm người dùng. Thêm vào đó, thiết kế responsive là điều không thể thiếu, khi mà người dùng ngày càng sử dụng nhiều thiết bị khác nhau để truy cập internet. Cuối cùng, tối ưu hóa SEO trong thiết kế web cũng trở thành một phần quan trọng, giúp trang web của bạn dễ dàng tiếp cận hơn với người dùng. Hãy cùng theo dõi những xu hướng thiết kế này để tạo ra những trang web đẹp và hiệu quả trong năm 2024.</p>', 1, CAST(N'2024-10-23T22:06:33.307' AS DateTime), N'blog_09.jpg')
INSERT [dbo].[BlogPost] ([PostID], [UserID], [Title], [Content], [IsPublic], [CreatedAt], [Image]) VALUES (4, 2, N'Làm Thế Nào Để Tăng Tương Tác Trên Mạng Xã Hội', N'<h2>Tăng Tương Tác</h2><p>Mạng xã hội đã trở thành một phần không thể thiếu trong cuộc sống hàng ngày của chúng ta. Để tạo ra những nội dung hấp dẫn và thu hút sự chú ý, bạn cần phải có những chiến lược phù hợp. Đầu tiên, việc hiểu rõ đối tượng mục tiêu là rất quan trọng. Hãy nghiên cứu những gì họ thích, những gì họ quan tâm và thời gian họ hoạt động trên mạng xã hội. Một khi bạn đã nắm bắt được thông tin này, bạn có thể tạo ra nội dung phù hợp hơn với nhu cầu của họ.</p><p>Bên cạnh đó, việc sử dụng hình ảnh, video hoặc infographics có thể giúp nội dung của bạn trở nên sinh động hơn. Những bài viết có sự kết hợp giữa văn bản và hình ảnh thường nhận được nhiều sự tương tác hơn. Thêm vào đó, hãy nhớ tạo ra các câu hỏi mở để khuyến khích người dùng tham gia thảo luận. Cuối cùng, bạn cũng nên theo dõi và phân tích các chỉ số tương tác để điều chỉnh chiến lược của mình cho phù hợp. Hãy thử nghiệm với các loại nội dung khác nhau để xem điều gì hoạt động tốt nhất với cộng đồng của bạn.</p>', 1, CAST(N'2024-10-23T22:06:33.307' AS DateTime), N'blog_10.jpg')
INSERT [dbo].[BlogPost] ([PostID], [UserID], [Title], [Content], [IsPublic], [CreatedAt], [Image]) VALUES (5, 2, N'Sự Phát Triển Của AI Trong Doanh Nghiệp', N'<h2>AI Trong Kinh Doanh</h2><p>Trí tuệ nhân tạo (AI) đang trở thành một yếu tố quan trọng trong việc cải thiện quy trình và hiệu quả làm việc của các doanh nghiệp. Năm 2024, AI đã có những bước tiến lớn trong việc tự động hóa các tác vụ lặp đi lặp lại, giúp giảm thiểu khối lượng công việc cho nhân viên. Hơn nữa, AI còn được sử dụng để phân tích dữ liệu lớn, cung cấp những thông tin hữu ích giúp các nhà quản lý đưa ra quyết định tốt hơn.</p><p>Ngoài ra, việc tích hợp AI vào các sản phẩm và dịch vụ cũng ngày càng trở nên phổ biến, từ chatbot hỗ trợ khách hàng đến các hệ thống gợi ý sản phẩm. Điều này không chỉ giúp nâng cao trải nghiệm khách hàng mà còn tăng cường khả năng cạnh tranh cho doanh nghiệp. Hãy cùng khám phá những tiềm năng mà AI mang lại cho doanh nghiệp của bạn trong tương lai và cách bạn có thể áp dụng chúng để phát triển bền vững.</p>', 1, CAST(N'2024-10-23T22:06:33.307' AS DateTime), N'blog_11.jpg')
INSERT [dbo].[BlogPost] ([PostID], [UserID], [Title], [Content], [IsPublic], [CreatedAt], [Image]) VALUES (6, 3, N'Cách Chọn Laptop Phù Hợp Với Nhu Cầu', N'<h2>Chọn Laptop</h2><p>Chọn lựa laptop phù hợp với nhu cầu sử dụng không phải là điều dễ dàng, đặc biệt là trong một thị trường có quá nhiều lựa chọn. Đầu tiên, bạn cần xác định mục đích sử dụng laptop của mình: làm việc, học tập hay chơi game. Mỗi loại nhu cầu sẽ có những yêu cầu khác nhau về cấu hình. Ví dụ, nếu bạn cần một chiếc laptop để chơi game, bạn sẽ cần một máy với card đồ họa mạnh và CPU nhanh. Ngược lại, nếu chỉ cần một chiếc laptop để lướt web và làm việc văn phòng, bạn có thể chọn một chiếc có cấu hình thấp hơn.</p><p>Thứ hai, kích thước và trọng lượng của laptop cũng là yếu tố quan trọng cần xem xét. Nếu bạn thường xuyên di chuyển, một chiếc laptop mỏng nhẹ sẽ là lựa chọn tối ưu. Cuối cùng, hãy xem xét đến giá cả và thương hiệu. Đôi khi, một chiếc laptop giá rẻ từ một thương hiệu không nổi tiếng có thể không mang lại chất lượng tốt như một sản phẩm từ thương hiệu lớn. Hãy cân nhắc kỹ lưỡng trước khi đưa ra quyết định cuối cùng để chọn được chiếc laptop ưng ý nhất cho mình.</p>', 1, CAST(N'2024-10-23T22:06:33.307' AS DateTime), N'blog_12.jpg')
INSERT [dbo].[BlogPost] ([PostID], [UserID], [Title], [Content], [IsPublic], [CreatedAt], [Image]) VALUES (7, 2, N'Bí Quyết Thành Công Trong Kinh Doanh Online', N'<h2>Kinh Doanh Online</h2><p>Kinh doanh online đã trở thành xu hướng không thể thiếu trong thời đại số hiện nay. Để thành công trong lĩnh vực này, bạn cần có những bí quyết riêng. Đầu tiên, việc xây dựng thương hiệu cá nhân là rất quan trọng. Hãy tạo ra một hình ảnh chuyên nghiệp và nhất quán trên tất cả các nền tảng mạng xã hội. Điều này sẽ giúp khách hàng nhận diện thương hiệu của bạn dễ dàng hơn.</p><p>Thứ hai, bạn cần nắm rõ thị trường và đối thủ cạnh tranh. Nghiên cứu đối thủ sẽ giúp bạn tìm ra những điểm mạnh và điểm yếu của họ, từ đó có chiến lược kinh doanh hợp lý hơn. Cuối cùng, hãy đảm bảo rằng bạn cung cấp dịch vụ khách hàng tốt nhất. Đừng quên theo dõi phản hồi từ khách hàng để cải thiện dịch vụ và sản phẩm của mình. Hãy luôn sẵn sàng lắng nghe và học hỏi từ khách hàng để phát triển bền vững trong lĩnh vực kinh doanh online.</p>', 1, CAST(N'2024-10-23T22:06:33.307' AS DateTime), N'blog_13.jpg')
INSERT [dbo].[BlogPost] ([PostID], [UserID], [Title], [Content], [IsPublic], [CreatedAt], [Image]) VALUES (8, 3, N'Khám Phá Vẻ Đẹp Thiên Nhiên Việt Nam', N'<h2>Vẻ Đẹp Thiên Nhiên</h2><p>Việt Nam là một trong những quốc gia có vẻ đẹp thiên nhiên hùng vĩ và đa dạng. Từ những bãi biển xanh mướt đến những dãy núi trùng điệp, cảnh quan thiên nhiên ở đây thật sự mê hoặc lòng người. Đặc biệt, vùng núi phía Bắc với những cánh đồng xanh trải dài và những thửa ruộng bậc thang là điểm đến lý tưởng cho những ai yêu thích khám phá. Hơn nữa, các khu vực ven biển như Đà Nẵng, Nha Trang hay Phú Quốc nổi tiếng với bãi biển trắng mịn, nước biển trong xanh và hệ sinh thái phong phú.</p><p>Việt Nam còn có nhiều khu bảo tồn thiên nhiên với động vật hoang dã và thực vật quý hiếm, là nơi tuyệt vời để bạn có thể tìm hiểu và khám phá. Những hoạt động như trekking, leo núi hay tham gia tour sinh thái cũng rất phổ biến và hấp dẫn du khách. Hãy cùng nhau khám phá vẻ đẹp của thiên nhiên Việt Nam và cảm nhận những điều kỳ diệu mà thiên nhiên ban tặng cho đất nước chúng ta.</p>', 1, CAST(N'2024-10-23T22:06:33.307' AS DateTime), N'blog_14.jpg')
SET IDENTITY_INSERT [dbo].[BlogPost] OFF
GO
SET IDENTITY_INSERT [dbo].[Bookmark] ON 

INSERT [dbo].[Bookmark] ([BookmarkID], [PostID], [UserID], [CreatedAt]) VALUES (3, 1, 4, NULL)
INSERT [dbo].[Bookmark] ([BookmarkID], [PostID], [UserID], [CreatedAt]) VALUES (4, 2, 4, NULL)
SET IDENTITY_INSERT [dbo].[Bookmark] OFF
GO
SET IDENTITY_INSERT [dbo].[Comment] ON 

INSERT [dbo].[Comment] ([CommentID], [PostID], [UserID], [Content], [CreatedAt]) VALUES (1, 1, 4, N'test', CAST(N'2024-10-25T09:29:44.450' AS DateTime))
INSERT [dbo].[Comment] ([CommentID], [PostID], [UserID], [Content], [CreatedAt]) VALUES (2, 1, 4, N'được', CAST(N'2024-10-25T10:54:44.880' AS DateTime))
SET IDENTITY_INSERT [dbo].[Comment] OFF
GO
SET IDENTITY_INSERT [dbo].[CommentReply] ON 

INSERT [dbo].[CommentReply] ([ReplyID], [CommentID], [UserID], [Content], [CreatedAt]) VALUES (1, 1, 4, N'oke', CAST(N'2024-10-25T10:00:41.127' AS DateTime))
INSERT [dbo].[CommentReply] ([ReplyID], [CommentID], [UserID], [Content], [CreatedAt]) VALUES (2, 1, 4, N'hay', CAST(N'2024-10-25T10:54:38.283' AS DateTime))
SET IDENTITY_INSERT [dbo].[CommentReply] OFF
GO
INSERT [dbo].[Follow] ([FollowerID], [FollowingID], [FollowedAt]) VALUES (4, 3, CAST(N'2024-10-25T21:15:26.913' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[PostLike] ON 

INSERT [dbo].[PostLike] ([LikeID], [PostID], [UserID], [LikedAt]) VALUES (4, 2, 4, CAST(N'2024-10-25T10:46:53.180' AS DateTime))
SET IDENTITY_INSERT [dbo].[PostLike] OFF
GO
SET IDENTITY_INSERT [dbo].[PostTag] ON 

INSERT [dbo].[PostTag] ([PostID], [TagID], [PostTagID]) VALUES (1, 5, 1)
INSERT [dbo].[PostTag] ([PostID], [TagID], [PostTagID]) VALUES (2, 3, 2)
INSERT [dbo].[PostTag] ([PostID], [TagID], [PostTagID]) VALUES (2, 10, 3)
INSERT [dbo].[PostTag] ([PostID], [TagID], [PostTagID]) VALUES (3, 2, 4)
SET IDENTITY_INSERT [dbo].[PostTag] OFF
GO
SET IDENTITY_INSERT [dbo].[Tag] ON 

INSERT [dbo].[Tag] ([TagID], [Name], [Status]) VALUES (1, N'Công nghệ', 1)
INSERT [dbo].[Tag] ([TagID], [Name], [Status]) VALUES (2, N'Smartphone', 1)
INSERT [dbo].[Tag] ([TagID], [Name], [Status]) VALUES (3, N'Thiết kế web', 1)
INSERT [dbo].[Tag] ([TagID], [Name], [Status]) VALUES (4, N'Mạng xã hội', 1)
INSERT [dbo].[Tag] ([TagID], [Name], [Status]) VALUES (5, N'Trí tuệ nhân tạo', 1)
INSERT [dbo].[Tag] ([TagID], [Name], [Status]) VALUES (6, N'Laptop', 1)
INSERT [dbo].[Tag] ([TagID], [Name], [Status]) VALUES (7, N'Kinh doanh online', 1)
INSERT [dbo].[Tag] ([TagID], [Name], [Status]) VALUES (8, N'Bảo mật', 1)
INSERT [dbo].[Tag] ([TagID], [Name], [Status]) VALUES (9, N'Blockchain', 1)
INSERT [dbo].[Tag] ([TagID], [Name], [Status]) VALUES (10, N'Blog', 1)
SET IDENTITY_INSERT [dbo].[Tag] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([UserID], [PasswordHash], [Email], [FullName], [Bio], [ProfileImage], [Role], [CreatedAt], [Status]) VALUES (1, N'123456', N'admin@gmail.com', N'Admin', N'Admin User Bio', N'default_profile_image.jpg', N'admin', CAST(N'2024-10-23T22:06:33.303' AS DateTime), 1)
INSERT [dbo].[User] ([UserID], [PasswordHash], [Email], [FullName], [Bio], [ProfileImage], [Role], [CreatedAt], [Status]) VALUES (2, N'123456', N'khach1@gmail.com', N'Khách Hàng 1', N'Bio for Khách Hàng 1', N'cmnt-1.jpg', N'customer', CAST(N'2024-10-23T22:06:33.303' AS DateTime), 1)
INSERT [dbo].[User] ([UserID], [PasswordHash], [Email], [FullName], [Bio], [ProfileImage], [Role], [CreatedAt], [Status]) VALUES (3, N'123456', N'khach2@gmail.com', N'Khách Hàng 2', N'Bio for Khách Hàng 2', N'cmnt-3.jpg', N'customer', CAST(N'2024-10-23T22:06:33.303' AS DateTime), 1)
INSERT [dbo].[User] ([UserID], [PasswordHash], [Email], [FullName], [Bio], [ProfileImage], [Role], [CreatedAt], [Status]) VALUES (4, N'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', N'duc43697@gmail.com', N'Trần Đăng Đức', N'<p>oke test</p>
', N'cmnt-1.jpg', N'customer', CAST(N'2024-10-24T14:09:05.203' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[User] OFF
GO
/****** Object:  Index [UQ__Bookmark__7B6AECF3DAEE0479]    Script Date: 25/10/2024 9:32:59 CH ******/
ALTER TABLE [dbo].[Bookmark] ADD UNIQUE NONCLUSTERED 
(
	[PostID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__PostLike__7B6AECF36C9443F6]    Script Date: 25/10/2024 9:32:59 CH ******/
ALTER TABLE [dbo].[PostLike] ADD UNIQUE NONCLUSTERED 
(
	[PostID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BlogPost] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Bookmark] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Comment] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CommentReply] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Follow] ADD  DEFAULT (getdate()) FOR [FollowedAt]
GO
ALTER TABLE [dbo].[Notification] ADD  DEFAULT ((0)) FOR [IsRead]
GO
ALTER TABLE [dbo].[Notification] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[PostLike] ADD  DEFAULT (getdate()) FOR [LikedAt]
GO
ALTER TABLE [dbo].[ReportBlog] ADD  DEFAULT (getdate()) FOR [ReportedAt]
GO
ALTER TABLE [dbo].[ReportComment] ADD  DEFAULT (getdate()) FOR [ReportedAt]
GO
ALTER TABLE [dbo].[ReportUser] ADD  DEFAULT (getdate()) FOR [ReportedAt]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT ('user') FOR [Role]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[BlogPost]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Bookmark]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Bookmark]  WITH CHECK ADD  CONSTRAINT [FK_Bookmark_BlogPost] FOREIGN KEY([PostID])
REFERENCES [dbo].[BlogPost] ([PostID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Bookmark] CHECK CONSTRAINT [FK_Bookmark_BlogPost]
GO
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD  CONSTRAINT [FK_Comment_BlogPost] FOREIGN KEY([PostID])
REFERENCES [dbo].[BlogPost] ([PostID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Comment] CHECK CONSTRAINT [FK_Comment_BlogPost]
GO
ALTER TABLE [dbo].[CommentReply]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[CommentReply]  WITH CHECK ADD  CONSTRAINT [FK_CommentReply_Comment] FOREIGN KEY([CommentID])
REFERENCES [dbo].[Comment] ([CommentID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CommentReply] CHECK CONSTRAINT [FK_CommentReply_Comment]
GO
ALTER TABLE [dbo].[Follow]  WITH CHECK ADD FOREIGN KEY([FollowerID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Follow]  WITH CHECK ADD FOREIGN KEY([FollowingID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[PostLike]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[PostLike]  WITH CHECK ADD  CONSTRAINT [FK_PostLike_BlogPost] FOREIGN KEY([PostID])
REFERENCES [dbo].[BlogPost] ([PostID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PostLike] CHECK CONSTRAINT [FK_PostLike_BlogPost]
GO
ALTER TABLE [dbo].[PostTag]  WITH CHECK ADD  CONSTRAINT [FK_PostTag_BlogPost1] FOREIGN KEY([PostID])
REFERENCES [dbo].[BlogPost] ([PostID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PostTag] CHECK CONSTRAINT [FK_PostTag_BlogPost1]
GO
ALTER TABLE [dbo].[PostTag]  WITH CHECK ADD  CONSTRAINT [FK_PostTag_Tag] FOREIGN KEY([TagID])
REFERENCES [dbo].[Tag] ([TagID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PostTag] CHECK CONSTRAINT [FK_PostTag_Tag]
GO
ALTER TABLE [dbo].[ReportBlog]  WITH CHECK ADD FOREIGN KEY([ReporterID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportBlog]  WITH CHECK ADD  CONSTRAINT [FK_ReportBlog_BlogPost] FOREIGN KEY([PostID])
REFERENCES [dbo].[BlogPost] ([PostID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReportBlog] CHECK CONSTRAINT [FK_ReportBlog_BlogPost]
GO
ALTER TABLE [dbo].[ReportComment]  WITH CHECK ADD FOREIGN KEY([ReporterID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportComment]  WITH CHECK ADD  CONSTRAINT [FK_ReportComment_Comment] FOREIGN KEY([CommentID])
REFERENCES [dbo].[Comment] ([CommentID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReportComment] CHECK CONSTRAINT [FK_ReportComment_Comment]
GO
ALTER TABLE [dbo].[ReportUser]  WITH CHECK ADD FOREIGN KEY([ReportedUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportUser]  WITH CHECK ADD FOREIGN KEY([ReporterID])
REFERENCES [dbo].[User] ([UserID])
GO
USE [master]
GO
ALTER DATABASE [BlogIT] SET  READ_WRITE 
GO
