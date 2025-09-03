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

        return Ok(new ApiResponse<PagedResult<UserDto>>
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

        return Ok(new ApiResponse<UserDto>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Profile fetched successfully."
        });
    }

    [HttpPost("wallets")]
    public async Task<IActionResult> GetAllWalletsAsync([FromBody] WalletFilterDto filter)
    {
        

        var result = await adminService.GetAllWalletsAsync(filter);

        return Ok(new ApiResponse<PagedResult<WalletDto>>
        {
            IsSuccessful = true,
            Data = result,
            SuccessMessage = "Profile fetched successfully."
        });
    }
    [HttpGet("wallets/{userId}")]
    public async Task<IActionResult> GetUserWalletsAsync(Guid userId)
    {
        

        var result = await adminService.GetUserWalletsAsync(userId);

        return Ok(new ApiResponse<List<WalletDto>>
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

        return Ok(new ApiResponse<PagedResult<TransactionDto>>
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

        return Ok(new ApiResponse<PagedResult<TransactionDto>>
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

        return Ok(new ApiResponse<TransactionDto>
        {
            IsSuccessful = true,
            Data = result,SuccessMessage = "Profile fetched successfully."
        });
    }



}