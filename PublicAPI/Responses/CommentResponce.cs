﻿using System;

namespace PublicAPI.Responses
{
    public class CommentResponce
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Raw { get; set; }
        public DateTime Time { get; set; }
    }
}
