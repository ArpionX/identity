﻿namespace Turnos.data.DTOs.Authentication
{
    public class LoginDto
    {
        public string EmailOrUserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
