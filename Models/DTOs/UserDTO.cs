﻿namespace SnackShackAPI.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string DiscordUserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
