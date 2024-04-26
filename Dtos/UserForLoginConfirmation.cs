namespace DotnetAPI.Dtos;

partial class UserForLoginConfirmationDto
{
    byte[] PasswordHash { get; set; } = [];
    byte[] PasswordSalt { get; set; } = [];
}