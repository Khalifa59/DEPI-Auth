using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UserAuthenticationAPI.Models.AuthModels;
using UserAuthenticationAPI.Services.Interfaces;

namespace UserAuthenticationAPI.Controllers
{
    [Route("api/members")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.Register(request);
                return CreatedAtAction(nameof(Register), new { username = result.User.Username },
                    new { message = "Registration successful", user = result.User });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.Login(request);

                return Ok(new
                {
                    message = "Login successful",
                    membershipTier = result.User.MembershipTier
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = "Invalid username/email or password" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("upgrade/{userId}")]
        public async Task<IActionResult> UpgradeMembership(Guid userId, [FromBody] UpgradeTierRequest request)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Check if user already has the requested membership tier
                if (user.MembershipTier == request.MembershipTier)
                {
                    return BadRequest(new { message = $"User already has {request.MembershipTier} membership" });
                }

                // Store the old tier for the response message
                string oldTier = user.MembershipTier;

                // Update the membership tier
                user.MembershipTier = request.MembershipTier;
                await _userService.UpdateUserAsync(user);

                // Determine if this is an upgrade or downgrade
                string action = IsUpgrade(oldTier, request.MembershipTier) ? "upgraded" : "downgraded";

                return Ok(new
                {
                    message = $"Membership {action} successfully",
                    userId = user.UserId,
                    username = user.Username,
                    previousTier = oldTier,
                    currentTier = user.MembershipTier
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Helper method to determine if changing from oldTier to newTier is an upgrade
        private bool IsUpgrade(string oldTier, string newTier)
        {
            // Simple tier logic - you can expand this for more complex tier hierarchies
            var tierValues = new Dictionary<string, int>
            {
                { "Basic", 1 },
                { "Premium", 2 },
                { "Enterprise", 3 }
            };

            // Default to lowest tier if not found
            int oldValue = tierValues.ContainsKey(oldTier) ? tierValues[oldTier] : 0;
            int newValue = tierValues.ContainsKey(newTier) ? tierValues[newTier] : 0;

            return newValue > oldValue;
        }
    }
}