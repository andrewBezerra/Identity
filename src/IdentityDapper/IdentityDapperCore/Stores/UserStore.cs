using Dapper;
using IdentityDapperCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityDapperCore.Stores
{
    public class UserStore :
        IUserStore<ApplicationUser>,
        IUserEmailStore<ApplicationUser>,
        IUserPhoneNumberStore<ApplicationUser>,
        IUserTwoFactorStore<ApplicationUser>,
        IUserPasswordStore<ApplicationUser>,
        IQueryableUserStore<ApplicationUser>,
        IUserRoleStore<ApplicationUser>
    {
        private readonly string _connectionString;

        public IQueryable<ApplicationUser> Users
        {
            get
            {
                var sql = @"
                            SELECT 
                                *
                            FROM ApplicationUser";

                using (var connection = new SqlConnection(_connectionString))
                {
                    var result = connection.Query<ApplicationUser>(sql);

                    return result.AsQueryable();
                }
            }
        }

        public UserStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                user.Id = await connection.QuerySingleAsync<int>($@"INSERT INTO [ApplicationUser] 
                ([UserName], 
                [NormalizedUserName], 
                [Email],
                [NormalizedEmail], 
                [EmailConfirmed],
                [PasswordHash], 
                [PhoneNumber], 
                [PhoneNumberConfirmed], 
                [TwoFactorEnabled])
                VALUES (
                @{nameof(ApplicationUser.UserName)}, 
                @{nameof(ApplicationUser.NormalizedUserName)}, 
                @{nameof(ApplicationUser.Email)},
                @{nameof(ApplicationUser.NormalizedEmail)}, 
                @{nameof(ApplicationUser.EmailConfirmed)}, 
                @{nameof(ApplicationUser.PasswordHash)},
                @{nameof(ApplicationUser.PhoneNumber)}, 
                @{nameof(ApplicationUser.PhoneNumberConfirmed)}, 
                @{nameof(ApplicationUser.TwoFactorEnabled)});
                SELECT CAST(SCOPE_IDENTITY() as int)", user);
            }

            return IdentityResult.Success;

        }
    

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($"DELETE FROM [ApplicationUser] WHERE [Id] = @{nameof(ApplicationUser.Id)}", user);
            }

            return IdentityResult.Success;
        }

        public void Dispose()
        {
           
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM [ApplicationUser]
                WHERE [NormalizedEmail] = @{nameof(normalizedEmail)}", new { normalizedEmail });
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM [ApplicationUser]
                WHERE [Id] = @{nameof(userId)}", new { userId });
            }
        }
        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM [ApplicationUser]
                WHERE [NormalizedUserName] = @{nameof(normalizedUserName)}", new { normalizedUserName });
            }
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"UPDATE [ApplicationUser] SET
                [UserName] = @{nameof(ApplicationUser.UserName)},
                [NormalizedUserName] = @{nameof(ApplicationUser.NormalizedUserName)},
                [Email] = @{nameof(ApplicationUser.Email)},
                [NormalizedEmail] = @{nameof(ApplicationUser.NormalizedEmail)},
                [EmailConfirmed] = @{nameof(ApplicationUser.EmailConfirmed)},
                [PasswordHash] = @{nameof(ApplicationUser.PasswordHash)},
                [PhoneNumber] = @{nameof(ApplicationUser.PhoneNumber)},
                [PhoneNumberConfirmed] = @{nameof(ApplicationUser.PhoneNumberConfirmed)},
                [TwoFactorEnabled] = @{nameof(ApplicationUser.TwoFactorEnabled)}
                WHERE [Id] = @{nameof(ApplicationUser.Id)}", user);
            }

            return IdentityResult.Success;
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }
        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                var sqlRole = "SELECT ID FROM ApplicationRole where Name = @Name";

                var returnedRoleID = await connection.QueryFirstOrDefaultAsync<int>(sqlRole, new { Name = roleName });

                var sql = " INSERT INTO [ApplicationUserRole] " +
                          "            ([UserID]				" +
                          "            ,[RoleID])				" +
                          "      VALUES						" +
                          "            (@UserID				" +
                          "            ,@RoleID)				";
                await connection.ExecuteAsync(sql, new { UserID = user.Id, RoleID = returnedRoleID });

            }
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                var sqlRoles = " select Name from ApplicationRole R                    " +
                               " inner join ApplicationUserRole UR on UR.ROLEID = R.ID " +
                               " WHERE UR.USERID = @USERID							   ";

                var roles = await connection.QueryAsync<string>(sqlRoles, new { USERID = user.Id });
                return await Task.FromResult(roles.ToList());

            }
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                var sqlRole = "SELECT ID FROM ApplicationRole where Name = @Name";

                var returnedRoleID = await connection.QueryFirstOrDefaultAsync<int>(sqlRole, new { Name = roleName });

                var sql = " Select UserID from [ApplicationUserRole] " +
                          " WHERE RoleID =@RoleID and UserID=@UserID " ;

                var returnedUserID= await connection.QueryFirstOrDefaultAsync<int>(sql, new { UserID = user.Id, RoleID = returnedRoleID });

                return await Task.FromResult(returnedUserID == user.Id);
            }
        }

        public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
