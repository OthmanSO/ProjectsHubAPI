﻿namespace ProjectsHub.Model
{
    public class ShortPost
    {
        public string _id { set; get; }
        public string Title { set; get; }
        public DateTime CreatedDate { set; get; }
        public string CoverPicture { set; get; }
        public int UsersWhoLiked { get; set; }
        public int Comments { get; set; }
        public bool IsLiked { get; set; }
        public UserShortProfileDto Author { get; set; }
        public bool IsAuthorFollowed { get; set; }
    }
}
