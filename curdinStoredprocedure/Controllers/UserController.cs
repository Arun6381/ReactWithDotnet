using Microsoft.AspNetCore.Mvc;
using curdinStoredprocedure.DataAccessLayer;
using curdinStoredprocedure.Models;

namespace curdinStoredprocedure.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase

    {
        private readonly UserDataAccess _userDataAccess;

        public UserController(UserDataAccess userDataAccess)
        {
            _userDataAccess = userDataAccess;
        }

        // Check if an email exists
        [HttpGet("check-email")]
        public IActionResult CheckEmailExists([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { message = "Email is required." });

            bool exists = _userDataAccess.CheckEmailExists(email);
            return Ok(new { exists });
        }

        // Insert a new user
        [HttpPost]
        public IActionResult InsertUser([FromBody] UserModel user)
        {
          
            if (ModelState.IsValid)
            {
                try
                {
                    _userDataAccess.InsertUser(user);
                    return Ok(new { message = "User created successfully." });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            return BadRequest(ModelState);
        }

        [HttpGet("get-by-email")]
        public IActionResult GetUserByEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { message = "Email is required." });

            var user = _userDataAccess.GetUserByEmail(email);
            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }
    }
}
