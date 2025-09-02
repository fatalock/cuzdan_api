using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.DTOs;
using Cuzdan.Domain.Enums;


namespace Cuzdan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminController(IAdminService adminService) : ControllerBase
{
    [HttpPost("users")]
    public async Task<IActionResult> GetAllUsersProfileAsync([FromBody] UserFilterDto filter)
    {
        var result = await adminService.GetAllUsersProfileAsync(filter);

        return Ok(new ApiResponse<PagedResult<AdminUserDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Profiles fetched successfully."
        });
    }
    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserProfileAsync(Guid userId)
    {
        var result = await adminService.GetUserProfileAsync(userId);

        return Ok(new ApiResponse<AdminUserDto>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Profile fetched successfully."
        });
    }

    [HttpPost("wallets")]
    public async Task<IActionResult> GetAllWalletsAsyc([FromBody] WalletFilterDto filter)
    {
        

        var result = await adminService.GetAllWalletsAsyc(filter);

        return Ok(new ApiResponse<PagedResult<AdminWalletDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Profile fetched successfully."
        });
    }
    [HttpGet("wallets/{userId}")]
    public async Task<IActionResult> GetUserWalletsAsyc(Guid userId)
    {
        

        var result = await adminService.GetUserWalletsAsyc(userId);

        return Ok(new ApiResponse<List<AdminWalletDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Profile fetched successfully."
        });
    }
    [HttpPost("transactions")]
    public async Task<IActionResult> GetAllTransactionsAsync(
        [FromBody] TransactionFilterDto filter)
    {
        

        var result = await adminService.GetAllTransactionsAsync(filter);

        return Ok(new ApiResponse<PagedResult<AdminTransactionDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Profile fetched successfully."
        });
    }
    [HttpPost("transactions/wallet/{walletId}")]
    public async Task<IActionResult> GetTransactionsByWalletAsync(
        Guid walletId,
        [FromBody] TransactionFilterDto filter)
    {
        

        var result = await adminService.GetTransactionsByWalletAsync(walletId, filter);

        return Ok(new ApiResponse<PagedResult<AdminTransactionDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Profile fetched successfully."
        });
    }
    [HttpGet("transactions/{transactionId}")]
    public async Task<IActionResult> GetTransactionAsync(Guid transactionId)
    {
        

        var result = await adminService.GetTransactionAsync(transactionId);

        return Ok(new ApiResponse<AdminTransactionDto>
        {
            IsSuccessful = true,
            Data = result,SuccessMessage = "Profile fetched successfully."
        });
    }



}