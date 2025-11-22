using Electro.Core.Dtos;
using Electro.Core.Dtos.Account;
using Electro.Core.Errors;
using Electro.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Interface
{
    public interface IAccountService
    {
        Task<ApiResponse> RegisterAsync(Register dto);
        Task<ApiResponse> LoginAsync(Login dto);
        Task<ApiResponse> ForgotPasswordAsync(string email);
        Task<ApiResponse> VerifyOtpAsync(VerifyOtp dto);
        Task<ApiResponse> VerifyEmailOtpAsync(VerifyEmailDto dto);
        Task<ApiResponse> ResetPasswordAsync(ResetPassword dto);
        Task<ApiResponse> UpdateUserAsync(UpdateUserDto dto);
        Task<ApiResponse> DeleteUserImageAsync();
        Task<ApiResponse> ChangePasswordAsync(ChangePasswordDto dto);
        Task<ApiResponse> GetUserInfoAsync();
        Task<List<UserDto>> GetAllUsersAsync();
        Task<ApiResponse> ResendEmailOtpAsync(string email);
        Task<ApiResponse> GetUsersByStatusAsync(UserStatus? status = null);
        Task<ApiResponse> UpdateUserStatusAsync(string userId, UserStatus status);
        Task<ApiResponse> SoftDeleteUserAsync(string userId);
        Task<ApiResponse> SoftDeleteProfileAsync(string userId);
        Task<PaginatedResult<UserDto>> GetCustomerAsync(FilterDto filter);

    }
}
