using aurga.Common;
using aurga.Data;
using aurga.Model;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;

#region Web app setup
const string MIRROR_URL = "https://account.yourdomain.com";

const int VERSION = 1;
var builder = WebApplication.CreateBuilder(args);

var fixed_heartbeart_key = new byte[]{ 0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2, 0x03 };

// Get EmailServer from appsettings.json
MailSender.DefaultSender.EmailServer = builder.Configuration["EmailServer"].ToString();
MailSender.DefaultSender.EmailAccount = builder.Configuration["EmailAccount"].ToString();
MailSender.DefaultSender.EmailPassword = builder.Configuration["EmailPassword"].ToString();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#if !MIRROR
builder.Services.AddDbContext<DataContext>();
#endif

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyOrigin",
        builder => builder.WithOrigins("http://localhost:5173").
        AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var app = builder.Build();

#if !MIRROR
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        context.Database.EnsureCreated();
        // or use context.Database.Migrate(); if you're using migrations
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}
#endif

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowMyOrigin");
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

var cacheUsers = SharedStore.Users;
#endregion

#region Utils
static string Encode(string hexString, int bit, int flip)
{
    var binStr = "";
    var output = "";

    for (var i = 0; i < hexString.Length; i += 2)
    {
        var hex = hexString.Substring(i, 2);
        var bin = Convert.ToString(Convert.ToInt32(hex, 16), 2).PadLeft(8, '0');
        binStr += bin;
    }

    binStr = binStr.Substring(bit, binStr.Length - bit) + binStr.Substring(0, bit);

    for (var i = 0; i < binStr.Length; i += 8)
    {
        var byteStr = binStr.Substring(i, 8);
        var @byte = Convert.ToByte(byteStr, 2);

        var mask = 1 << flip; // Create a mask with the flip bit set
        @byte ^= (byte)mask; // Invert the bit

        mask = 1 << bit;
        @byte ^= (byte)mask; // Invert the bit

        output += @byte.ToString("x2");
    }

    return output;
}

static string Decode(string hexString, int bit, int flip)
{
    var binStr = "";
    var output = "";

    for (var i = 0; i < hexString.Length; i += 2)
    {
        var hex = hexString.Substring(i, 2);
        var bin = Convert.ToString(Convert.ToInt32(hex, 16), 2).PadLeft(8, '0');
        binStr += bin;
    }

    for (var i = 0; i < binStr.Length; i += 8)
    {
        var byteStr = binStr.Substring(i, 8);
        var @byte = Convert.ToByte(byteStr, 2);

        var mask = 1 << flip; // Create a mask with the flip bit set
        @byte ^= (byte)mask; // Invert the bit

        mask = 1 << bit;
        @byte ^= (byte)mask; // Invert the bit

        output += @byte.ToString("x2");
    }

    binStr = "";
    for (var i = 0; i < output.Length; i += 2)
    {
        var hex = output.Substring(i, 2);
        var bin = Convert.ToString(Convert.ToInt32(hex, 16), 2).PadLeft(8, '0');
        binStr += bin;
    }

    binStr = binStr.Substring(binStr.Length - bit, bit) + binStr.Substring(0, binStr.Length - bit);

    output = "";
    for (var i = 0; i < binStr.Length; i += 8)
    {
        var byteStr = binStr.Substring(i, 8);
        var hex = Convert.ToInt32(byteStr, 2).ToString("x2");
        output += hex;
    }

    return output;
}

Dictionary<string, DateTime> ipLimits = new Dictionary<string, DateTime>();

bool AllowAccess(string ipAddress, int ts = 2)
{
    //if (ipAddress == "mirror1.yourdomain.ip" ||
    //    ipAddress == "mirror2.yourdomain.ip" ||
    //    ipAddress == "mirror3.yourdomain.ip")
    //{
    //    Console.WriteLine($"Skip {ipAddress}");
    //    return true;
    //}
    // Define the time limit between registrations from the same IP
    TimeSpan limit = TimeSpan.FromSeconds(ts);
    lock (ipLimits)
    {
        if (ipLimits.ContainsKey(ipAddress))
        {
            var now = DateTime.Now;
            var internval = now - ipLimits[ipAddress];
            if ( internval.TotalSeconds < limit.TotalSeconds)
            {
                // If the last registration from this IP was less than the limit ago, deny registration
                return false;
            }
            else
            {
                // If the last registration from this IP was longer than the limit ago, update the last registration time and allow registration
                ipLimits[ipAddress] = DateTime.Now;
                return true;
            }
        }
        else
        {
            // If this IP has not registered before, add it to the dictionary and allow registration
            ipLimits.Add(ipAddress, DateTime.Now);
            return true;
        }
    }
}

void GenerateNewActivationCodeIfNecessary(UserInfo userInfo, string email)
{
    if (DateTime.Now.Subtract(userInfo.ActivationVerificationTime).TotalMinutes > 3 ||
        string.IsNullOrEmpty(userInfo.ActivationToken) ||
        string.IsNullOrEmpty(userInfo.ActivationVerificationCode))
    {
        userInfo.ActivationVerificationCode = (Random.Shared.Next() % 1000000).ToString("000000");
        userInfo.ActivationToken = Util.Bytes2Hex(Util.GenerateRandomBytes(8));
        userInfo.ActivationVerificationTime = DateTime.Now;

        EmailHelper.SendActivationEmail(email, userInfo.ActivationVerificationCode);
    }
}

void GenerateNewResetPasswordCodeIfNecessary(UserInfo userInfo, string email)
{
    if (DateTime.Now.Subtract(userInfo.ResetPasswordVerificationTime).TotalMinutes > 3 ||
        string.IsNullOrEmpty(userInfo.ResetPasswordToken) ||
        string.IsNullOrEmpty(userInfo.ResetPasswordVerificationCode))
    {
        userInfo.ResetPasswordVerificationCode = (Random.Shared.Next() % 1000000).ToString("000000");
        userInfo.ResetPasswordToken = Util.Bytes2Hex(Util.GenerateRandomBytes(8));
        userInfo.ResetPasswordVerificationTime = DateTime.Now;

        EmailHelper.SendResetPasswordEmail(email, userInfo.ResetPasswordVerificationCode);
    }
}

void GenerateDeactivationCodeIfNecessary(UserInfo userInfo, string email)
{
    if (DateTime.Now.Subtract(userInfo.DeactivationVerificationTime).TotalMinutes > 3 ||
        string.IsNullOrEmpty(userInfo.DeactivationToken) ||
        string.IsNullOrEmpty(userInfo.DeactivationVerificationCode))
    {
        userInfo.DeactivationVerificationCode = (Random.Shared.Next() % 1000000).ToString("000000");
        userInfo.DeactivationToken = Util.Bytes2Hex(Util.GenerateRandomBytes(8));
        userInfo.DeactivationVerificationTime = DateTime.Now;

        EmailHelper.SendDeactivationCodeEmail(email, userInfo.DeactivationVerificationCode);
    }
}

async Task<UserInfo> LoadUserInfoAsync(HttpContext http, User user)
{
    // check if user exists in cache
    UserInfo? userInfo;
    bool newUser = false;
    lock (cacheUsers)
    {
        userInfo = cacheUsers.FirstOrDefault(u => u.AID == user.UID);
        if (userInfo == null)
        {
            // User is not in cache, create new user info
            userInfo = new UserInfo
            {
                AID = user.UID,
                Activated = user.Activated,
                Token = Util.Bytes2Hex(Util.GenerateRandomBytes(16)),
            };

            // Add user to cache
            cacheUsers.Add(userInfo);

            newUser = true;
        }
    }

    if (newUser)
    {
        var devices = await http.RequestServices.GetRequiredService<DataContext>().Devices.Where(d => d.AUID == user.UID).ToListAsync();

        lock (cacheUsers)
        {
            // Add user's devices to cache
            foreach (var device in devices)
            {
                var deviceStatus = new DeviceStatus
                {
                    DUID = device.UID,
                    DeviceName = device.Name,
                    Model = (byte)device.Model,
                    LastActive = new DateTime(1970, 1, 1),
                };
                userInfo.Devices.Add(deviceStatus);

                deviceStatus.Update();
            }
        }
    }

    return userInfo;
}
#endregion

#region Hello
app.MapGet("/hello/{name}", (string name) =>
{
    var data = new { message = $"Hello, {name}" };
    return Results.Ok(data);
});
#endregion

#region User Sign Up
app.MapPost("/register/user", async (HttpContext http, RegisterUserReuqest user) =>
{
    try
    {
        // 
        var addr = http.Connection.RemoteIpAddress.ToString();
        var email = user.Email?.ToLower().Trim();
        if (!AllowAccess(addr))
        {
            return Results.Json(new { status = RC.IP_RESTRICT });
        }

        // Validate input
        if (string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(email))
        {
            return Results.Json(new { status = RC.INVALID_PARAMETERS });
        }
        if (user.Password.Length < 6 || user.Password.Length > 64)
        {
            return Results.Json(new { status = RC.INVALID_PARAMETERS });
        }

        // check if user.email is valid format

        if (!Regex.IsMatch(email, @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$"))
        {
            return Results.Json(new { status = RC.INVALID_EMAIL_FORMAT });
        }

#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/register/user", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif

        // Check if email already exists
        var existingUser = await http.RequestServices.GetRequiredService<DataContext>().Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            // Email already exists
            return Results.Json(new { status = RC.EMAIL_REGISTERED });
        }

        // Hash the email with MD5
        var emailHash = Util.Bytes2Hex(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(email)));
        var pwdHash = Util.Bytes2Hex(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(user.Password)));
        // Add new user
        var newUser = new User
        {
            UID = Util.Bytes2Hex(Util.GenerateRandomBytes(16)),
            Email = email,
            Activated = false,
            Password = user.Password,
            EmailHash = emailHash,
            PasswordHash = pwdHash,
            CreatedAt = DateTime.UtcNow,
            VisitedAt = DateTime.UtcNow,
            VisitedIP = addr,
        };
        http.RequestServices.GetRequiredService<DataContext>().Users.Add(newUser);
        await http.RequestServices.GetRequiredService<DataContext>().SaveChangesAsync();

        UserInfo userInfo;
        lock (cacheUsers)
        {
            userInfo = new UserInfo
            {
                AID = newUser.UID,
                Activated = newUser.Activated,
                Token = Util.Bytes2Hex(Util.GenerateRandomBytes(16)),
            };

            // Add user to cache
            cacheUsers.Add(userInfo);
        }

        GenerateNewActivationCodeIfNecessary(userInfo, email);

        return Results.Json(new { status = RC.SUCCESS, token=userInfo.ActivationToken });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
    
});
#endregion

#region User Sign In
app.MapPost("/login", async (HttpContext http, LoginRequest request) =>
{
    try
    {
        var email = request.Email.Trim().ToLower();
        var password = request.Password.Trim().ToLower();
        if (!AllowAccess(http.Connection.RemoteIpAddress.ToString()))
        {
            return Results.Json(new { status = RC.IP_RESTRICT });
        }
        // Validate input
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return Results.Json(new { status = RC.INVALID_PARAMETERS });
        }

#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/login", content);
        var responseString = await response.Content.ReadAsStringAsync();
        //var responseObject = JsonSerializer.Deserialize(responseString)

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif
        // Check if user exists
        var user = await http.RequestServices.GetRequiredService<DataContext>().Users.FirstOrDefaultAsync(u => u.EmailHash == email && u.PasswordHash == password);
        if (user == null)
        {
            // Invalid user
            return Results.Json(new { status = RC.ACCOUNT_NOT_EXISTS });
        }

        var userInfo = await LoadUserInfoAsync(http, user);

        if (!userInfo.Activated)
        {
            GenerateNewActivationCodeIfNecessary(userInfo, user.Email);

            return Results.Json(new { status = RC.ACCOUNT_NOT_ACTIVATED, token=userInfo.ActivationToken });
        }

        if (DateTime.UtcNow.Subtract(userInfo.LastAccess).TotalDays >= 3)
        {
            // Refresh token
            userInfo.LastToken = userInfo.Token;
            userInfo.Token = Util.Bytes2Hex(Util.GenerateRandomBytes(16));
        }

        userInfo.LastAccess = DateTime.UtcNow;
        var userToken = Util.GenerateUserToken(userInfo.Token);

        lock (cacheUsers)
        {
            int payloadLength = userInfo.Devices.Sum(d => d.Data.Length);
            int offset = 0;
            byte[] payload = null;

            if (payloadLength > 0)
            {
                payload = new byte[4 + payloadLength];
                offset = 4;
                Array.Copy(BitConverter.GetBytes(userInfo.Devices.Count), payload, 4);

                foreach (var item in userInfo.Devices)
                {
                    Array.Copy(item.Data, 0, payload, offset, item.Data.Length);
                    offset += item.Data.Length;
                }

                var aid = Util.Hex2Bytes(userInfo.AID);
                Util.GetDataByKey4(payload, aid);
            }

            var responseData = new { status = 0, uid = userInfo.AID, token = userToken, v = VERSION, payload = Util.Bytes2Hex(payload) };
            return Results.Json(responseData);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
   
});
#endregion

#region Login with token
app.MapPost("/loginWithToken", async (HttpContext http, LoginWithTokenRequest request) =>
{
    try
    {
#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/loginWithToken", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif

        var response_data = new { status = RC.SUCCESS };

        // Validate input
        if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Uid))
        {
            response_data = new { status = RC.INVALID_PARAMETERS };
            return Results.Json(response_data);
        }

        var token = Util.DecodeUserToken(request.Token);

        // check if user exists in cache
        UserInfo? userInfo;
        lock (cacheUsers)
        {
            userInfo = cacheUsers.FirstOrDefault(u => string.Equals(u.AID, request.Uid));
        }

        if (userInfo?.Token != token)
        {
            response_data = new { status = RC.TOKEN_MISMATCH };
            return Results.Json(response_data);
        }

        if (DateTime.UtcNow.Subtract(userInfo.LastAccess).TotalDays >= 7)
        {
            response_data = new { status = RC.TOKEN_EXPIRED };
            return Results.Json(response_data);
        }

        if (DateTime.UtcNow.Subtract(userInfo.LastAccess).TotalDays >= 3)
        {
            userInfo.Token = Util.Bytes2Hex(Util.GenerateRandomBytes(16));
        }

        userInfo.LastAccess = DateTime.UtcNow;
        var userToken = Util.GenerateUserToken(userInfo.Token);

        lock (cacheUsers)
        {
            int payloadLength = userInfo.Devices.Sum(d => d.Data.Length);
            int offset = 0;
            byte[] payload = null;

            if (payloadLength > 0)
            {
                payload = new byte[4 + payloadLength];
                offset = 4;
                Array.Copy(BitConverter.GetBytes(userInfo.Devices.Count), payload, 4);

                foreach (var item in userInfo.Devices)
                {
                    Array.Copy(item.Data, 0, payload, offset, item.Data.Length);
                    offset += item.Data.Length;
                }

                var aid = Util.Hex2Bytes(userInfo.AID);
                Util.GetDataByKey4(payload, aid);
            }

            var responseData = new { status = RC.SUCCESS, uid = userInfo.AID, token = userToken, v = VERSION, payload = Util.Bytes2Hex(payload) };
            return Results.Json(responseData);
        }
    }
    catch(Exception e)
    {
        Console.WriteLine(e);
        return Results.Json(new { status = RC.EXCEPTION });
    }
    
});
#endregion

#region Verify Activation
app.MapPost("/verify/activation", async (HttpContext http, ActivationVerificationReguest request) =>
{
    try
    {
        if (!AllowAccess(http.Connection.RemoteIpAddress.ToString(), 1))
        {
            return Results.Json(new { status = RC.IP_RESTRICT });
        }

        // Validate input
        if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.VerificationCode))
        {
            return Results.Json(new { status = RC.INVALID_PARAMETERS });
        }

#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/verify/activation", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif
        //bool need_fill_devices = false;
        // check if user exists in cache
        UserInfo? userInfo;
        lock (cacheUsers)
        {
            userInfo = cacheUsers.FirstOrDefault(u => u.ActivationVerificationCode == request.VerificationCode && u.ActivationToken == request.Token);
        }

        if (userInfo == null)
        {
            return Results.Json(new { status = RC.ACCOUNT_NOT_EXISTS });
        }

        if (userInfo.Activated)
        {
            return Results.Json(new { status = RC.ACCOUNT_IS_ACTIVATED });
        }

        var user = await http.RequestServices.GetRequiredService<DataContext>().Users.FirstOrDefaultAsync(u => u.UID == userInfo.AID);
        if (user == null)
        {
            // Invalid user
            return Results.Json(new { status = RC.ACCOUNT_NOT_EXISTS });
        }

        if (DateTime.Now.Subtract(userInfo.ActivationVerificationTime).TotalMinutes > 10)
        {
            GenerateNewActivationCodeIfNecessary(userInfo, user.Email);
            return Results.Json(new { status = RC.TOKEN_EXPIRED, token = userInfo.ActivationToken });
        }

        user.Activated = true;
        userInfo.Activated = true;

        EmailHelper.SendActivationSuccess(user.Email);
        http.RequestServices.GetRequiredService<DataContext>().Users.Update(user);
        await http.RequestServices.GetRequiredService<DataContext>().SaveChangesAsync();
        return Results.Json(new { status = RC.SUCCESS });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
});
#endregion

#region Reset Password
app.MapPost("/sendemail/resetpassword", async (HttpContext http, SendEmailRequest request) =>
{
    try
    {
        if (!AllowAccess(http.Connection.RemoteIpAddress.ToString(), 3))
        {
            return Results.Json(new { status = RC.IP_RESTRICT });
        }

#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/sendemail/resetpassword", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif

        // Validate input
        if (string.IsNullOrEmpty(request.Email))
        {
            return Results.Json(new { status = RC.INVALID_PARAMETERS });
        }

        // Check if user exists
        var user = await http.RequestServices.GetRequiredService<DataContext>().Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
        if (user == null)
        {
            // Invalid user
            return Results.Json(new { status = RC.ACCOUNT_NOT_EXISTS });
        }

        // check if user exists in cache
        UserInfo? userInfo = await LoadUserInfoAsync(http, user);
        GenerateNewResetPasswordCodeIfNecessary(userInfo, request.Email);
        return Results.Json(new { status = RC.SUCCESS, token = userInfo.ResetPasswordToken });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
});
#endregion

#region Verify Reset Password
app.MapPost("/verify/resetpassword", async (HttpContext http, ResetPasswordVerificationRequest request) =>
{
    try
    {
        if (!AllowAccess(http.Connection.RemoteIpAddress.ToString(), 1))
        {
            return Results.Json(new { status = RC.IP_RESTRICT });
        }

        // Validate input
        if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.VerificationCode) || string.IsNullOrEmpty(request.NewPassword))
        {
            return Results.Json(new { status = RC.INVALID_PARAMETERS });
        }

#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/verify/resetpassword", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif
        //bool need_fill_devices = false;
        // check if user exists in cache
        UserInfo? userInfo;
        lock (cacheUsers)
        {
            userInfo = cacheUsers.FirstOrDefault(u => u.ResetPasswordVerificationCode == request.VerificationCode && u.ResetPasswordToken == request.Token);
        }

        if (userInfo == null)
        {
            return Results.Json(new { status = RC.ACCOUNT_NOT_EXISTS });
        }

        var user = await http.RequestServices.GetRequiredService<DataContext>().Users.FirstOrDefaultAsync(u => u.UID == userInfo.AID);
        if (user == null)
        {
            // Invalid user
            return Results.Json(new { status = RC.ACCOUNT_NOT_EXISTS });
        }

        if (DateTime.Now.Subtract(userInfo.ResetPasswordVerificationTime).TotalMinutes > 10)
        {
            GenerateNewResetPasswordCodeIfNecessary(userInfo, "");
            return Results.Json(new { status = RC.TOKEN_EXPIRED, token = userInfo.ResetPasswordToken });
        }

        user.Password = request.NewPassword;
        user.PasswordHash = Util.Bytes2Hex(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(user.Password)));
        user.Activated = true;
        userInfo.Activated = true;
        userInfo.ResetPasswordToken = string.Empty;
        userInfo.ResetPasswordVerificationCode = string.Empty;
        userInfo.ResetPasswordVerificationTime = new DateTime(0);
        EmailHelper.SendResetPasswordSuccess(user.Email);
        http.RequestServices.GetRequiredService<DataContext>().Users.Update(user);
        await http.RequestServices.GetRequiredService<DataContext>().SaveChangesAsync();
        return Results.Json(new { status = RC.SUCCESS });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
});
#endregion

#region Device Heartbeat
app.MapPost("/device/heartbeat", async (HttpContext http, HeartbeatRequest request) =>
{
    var response_data = new { status = RC.SUCCESS };
    string natPayload = null;
    string wolPayload = null;


    // Validate input
    if ((request.v == 1 && request.Payload?.Length != 342) || (request.v == 2 && request.Payload?.Length != 48))
    {
        response_data = new { status = RC.INVALID_PARAMETERS };
        return Results.Json(response_data);
    }

    //if (hit) return Results.BadRequest();
    //hit = true;

    try
    {
#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/device/heartbeat", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif
        UserInfo? userInfo;
        DeviceStatus? deviceStatus;

        if (request.v == 1)
        {
            var payload = Util.Hex2Bytes(request.Payload);

            //var sb = new StringBuilder();
            var version = payload[0];
            var key = payload.Skip(1).Take(16).ToArray();

            //sb.AppendLine("************************");
            //sb.AppendLine($"Key: {Util.Bytes2Hex(key)}");
            var data = payload.Skip(17).Take(payload.Length - 17).ToArray();
            //sb.AppendLine($"Encrypted Data: {Util.Bytes2Hex(data)}");

            // do md5 hash to key
            MD5 md5Hasher = MD5.Create();
            var hash = md5Hasher.ComputeHash(key);

            //sb.AppendLine($"Hash: {Util.Bytes2Hex(hash)}");

            int[][] swaps = { new[] { 0, 0 }, new[] { 0, 0 } };

            swaps[0][0] = key[0] % 15;
            swaps[0][1] = key[2] % 15;
            swaps[1][0] = key[4] % 15;
            swaps[1][1] = key[6] % 15;


            if (swaps[0][0] == swaps[0][1])
            {
                if (swaps[0][0] > 8)
                {
                    swaps[0][1] = swaps[0][0] - 1;
                }
                else
                {
                    swaps[0][1] = swaps[0][0] + 1;
                }
            }

            if (swaps[1][0] == swaps[1][1])
            {
                if (swaps[1][0] > 8)
                {
                    swaps[1][1] = swaps[1][0] - 1;
                }
                else
                {
                    swaps[1][1] = swaps[1][0] + 1;
                }
            }

            int revert = key[9] % 15;
            int zero = key[13] % 15;
            if (revert == zero)
            {
                if (revert > 8)
                {
                    zero = revert - 1;
                }
                else
                {
                    zero = revert + 1;
                }
            }

            //sb.AppendLine($"Swaps: {swaps[0][0]} - {swaps[0][1]} - {swaps[1][0]} - {swaps[1][1]} - {revert} - {zero}");

            Util.PoisonSalt(hash, swaps, revert, zero);

            //sb.AppendLine($"Poison Hash: {Util.Bytes2Hex(hash)}");
            Util.Encrypt(data, hash);
            //sb.AppendLine($"Decrypted Data: {Util.Bytes2Hex(data)}");

            var aid = data.Take(16).ToArray();
            var did = data.Skip(16).Take(8).ToArray();
            var build = data.Skip(24).Take(8).ToArray();
            var fwVersion = data.Skip(32).Take(4).ToArray();
            var capabilities = data.Skip(36).Take(4).ToArray();
            var laddr = data.Skip(40).Take(28).ToArray();
            var laddr6 = data.Skip(68).Take(28).ToArray();
            var ipv4_nat_type = data[96];
            var ipv6_nat_type = data[97];
            var raddr = data.Skip(98).Take(28).ToArray();
            var raddr6 = data.Skip(126).Take(28).ToArray();

            var said = Util.Bytes2Hex(aid);
            var sdid = Util.Bytes2Hex(did);

            lock (cacheUsers)
            {
                userInfo = cacheUsers.FirstOrDefault(o => string.Equals(o.AID, said, StringComparison.InvariantCultureIgnoreCase));

                deviceStatus = userInfo?.Devices.FirstOrDefault(o => string.Equals(o.DUID, sdid, StringComparison.InvariantCultureIgnoreCase));
            }

            if (userInfo == null || deviceStatus == null)
            {
                //System.Diagnostics.Debug.WriteLine(sb.ToString());
                response_data = new { status = RC.DEVICE_NOT_EXISTS };
                return Results.Json(response_data);
            }

            //Console.WriteLine($"-------------------");
            //Console.WriteLine(Util.Bytes2Hex(data));

            Array.Copy(laddr, deviceStatus.LocalAddr, laddr.Length);
            Array.Copy(laddr6, deviceStatus.LocalAddr6, laddr6.Length);
            Array.Copy(raddr, deviceStatus.RemoteAddr, raddr.Length);
            Array.Copy(raddr6, deviceStatus.RemoteAddr6, raddr6.Length);

            deviceStatus.Version = BitConverter.ToUInt32(fwVersion);
            deviceStatus.Firmware = BitConverter.ToInt64(build);
            deviceStatus.Capability = BitConverter.ToUInt32(capabilities);
            deviceStatus.RemoteAddrNatType = ipv4_nat_type;
            deviceStatus.RemoteAddr6NatType = ipv6_nat_type;
            deviceStatus.LastActive = DateTime.Now;
            deviceStatus.Nonce = Util.Bytes2Hex(Util.GenerateRandomBytes(8));
            deviceStatus.Update();

            if (deviceStatus.RequestConnectionInfo ?.Length > 0)
            {
                var info = new byte[deviceStatus.RequestConnectionInfo.Length];
                Array.Copy(deviceStatus.RequestConnectionInfo, info, deviceStatus.RequestConnectionInfo.Length);
                Util.Encrypt(info, hash);
                natPayload = Util.Bytes2Hex(info);
                deviceStatus.RequestConnectionInfo = null;
                Console.WriteLine($"{DateTime.Now}: V:{request.v} NAT: Sent!");
            }

            if (deviceStatus.RequestWOLInfo?.Length > 0)
            {
                var info = new byte[deviceStatus.RequestWOLInfo.Length];
                Array.Copy(deviceStatus.RequestWOLInfo, info, deviceStatus.RequestWOLInfo.Length);
                Util.Encrypt(info, hash);
                wolPayload = Util.Bytes2Hex(info);
                deviceStatus.RequestWOLInfo = null;
                Console.WriteLine($"{DateTime.Now}: V:{request.v} WOL: Sent!");
                //return Results.Json(new { status = RC.SUCCESS, token = userInfo.Token, nonce = deviceStatus.Nonce, payload = resp_payload });
            }

            if (string.IsNullOrEmpty(natPayload) && string.IsNullOrEmpty(wolPayload))
            {
                return Results.Json(new { status = RC.SUCCESS, token = userInfo.Token, nonce = deviceStatus.Nonce });
            }
            else
            {
                if (!string.IsNullOrEmpty(natPayload) && !string.IsNullOrEmpty(wolPayload))
                {
                    return Results.Json(new { status = RC.SUCCESS, token = userInfo.Token, nonce = deviceStatus.Nonce , nat = natPayload, wol = wolPayload});
                }
                else if(!string.IsNullOrEmpty(natPayload))
                {
                    return Results.Json(new { status = RC.SUCCESS, token = userInfo.Token, nonce = deviceStatus.Nonce, nat = natPayload });
                }
                else
                {
                    return Results.Json(new { status = RC.SUCCESS, token = userInfo.Token, nonce = deviceStatus.Nonce, wol = wolPayload});
                }
            }
        }

        if (request.v == 2)
        {
            var payload = Util.Hex2Bytes(request.Payload);
            Util.Encrypt(payload, fixed_heartbeart_key);

            var token = Util.Bytes2Hex(payload.Take(16).ToArray());
            var nonce = Util.Bytes2Hex(payload.Skip(16).Take(8).ToArray());

            lock (cacheUsers)
            {
                userInfo = cacheUsers.FirstOrDefault(o => string.Equals(o.Token, token, StringComparison.InvariantCultureIgnoreCase));
                deviceStatus = userInfo?.Devices.FirstOrDefault(o => string.Equals(o.Nonce, nonce, StringComparison.InvariantCultureIgnoreCase));
            }

            if (userInfo == null || deviceStatus == null)
            {
                response_data = new { status = RC.DEVICE_NOT_EXISTS };
                return Results.Json(response_data);
            }

            deviceStatus.LastActive = DateTime.Now;

            if (deviceStatus.RequestConnectionInfo?.Length > 0)
            {
                var info = new byte[deviceStatus.RequestConnectionInfo.Length];
                Array.Copy(deviceStatus.RequestConnectionInfo, info, deviceStatus.RequestConnectionInfo.Length);
                var hash = payload.Skip(8).Take(16).ToArray();

                Util.Encrypt(info, hash);
                natPayload = Util.Bytes2Hex(info);
                deviceStatus.RequestConnectionInfo = null;
                Console.WriteLine($"{DateTime.Now}: V:{request.v} NAT: Sent!");
            }

            if (deviceStatus.RequestWOLInfo?.Length > 0)
            {
                var info = new byte[deviceStatus.RequestWOLInfo.Length];
                Array.Copy(deviceStatus.RequestWOLInfo, info, deviceStatus.RequestWOLInfo.Length);
                var hash = payload.Skip(8).Take(16).ToArray();

                Util.Encrypt(info, hash);
                wolPayload = Util.Bytes2Hex(info);
                deviceStatus.RequestWOLInfo = null;
                Console.WriteLine($"{DateTime.Now}: V:{request.v} WOL: Sent!");
            }

            if (string.IsNullOrEmpty(natPayload) && string.IsNullOrEmpty(wolPayload))
            {
                return Results.Json(new { status = RC.SUCCESS });
            }
            else
            {
                if (!string.IsNullOrEmpty(natPayload) && !string.IsNullOrEmpty(wolPayload))
                {
                    return Results.Json(new { status = RC.SUCCESS, nat = natPayload, wol = wolPayload });
                }
                else if (!string.IsNullOrEmpty(natPayload))
                {
                    return Results.Json(new { status = RC.SUCCESS, nat = natPayload });
                }
                else
                {
                    return Results.Json(new { status = RC.SUCCESS, wol = wolPayload });
                }
            }
        }

        return Results.Json(new { status = RC.INVALID_PARAMETERS });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
});
#endregion

#region Device Bind
app.MapPost("/device/register", async (HttpContext http, RegisterDeviceRequest request) =>
{
    try
    {
        var addr = http.Connection.RemoteIpAddress.ToString();
        var aid = request.Uid?.Trim().ToLower();

        if (!AllowAccess(addr))
        {
            return Results.Json(new { status = RC.IP_RESTRICT });
        }

#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/device/register", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif

        var response_data = new { status = RC.SUCCESS };

        // Validate input
        if (string.IsNullOrEmpty(request.Token)
            || string.IsNullOrEmpty(aid)
            || string.IsNullOrEmpty(request.Payload))
        {
            response_data = new { status = RC.INVALID_PARAMETERS };
            return Results.Json(response_data);
        }
        var token = Util.DecodeUserToken(request.Token);

        // check if user exists in cache
        UserInfo? userInfo;
        lock (cacheUsers)
        {
            userInfo = cacheUsers.FirstOrDefault(u => u.AID == aid);
        }

        if (userInfo?.Token != token)
        {
            response_data = new { status = RC.TOKEN_MISMATCH };
            return Results.Json(response_data);
        }

        var data = Util.Hex2Bytes(request.Payload);
        var key = Util.Hex2Bytes(request.Uid);
        Util.GetDataByKey3(data, key);

        var data1 = new byte[8];
        Array.Copy(data, 0, data1, 0, 8);
        int model = data[8];
        var name = System.Text.Encoding.UTF8.GetString(data, 40, data.Length - 40);
        var did = Util.Bytes2Hex(data1);

        var device = await http.RequestServices.GetRequiredService<DataContext>().Devices.FindAsync(did);

        var oaid = device?.AUID;

        if (device == null)
        {
            // Add new device
            device = new Device
            {
                AUID = aid,
                UID = did,
                Status = 1,
                Model = model,
                Name = name,
                RegisteredAt = DateTime.UtcNow
            };
            http.RequestServices.GetRequiredService<DataContext>().Devices.Add(device);
        }
        else
        {
            if (device.AUID == aid)
            {
                return Results.Json(new { status = RC.ACCOUNT_NOT_EXISTS });
            }

            device.Status = 1;
            device.AUID = aid;
            http.RequestServices.GetRequiredService<DataContext>().Devices.Update(device);
        }

        await http.RequestServices.GetRequiredService<DataContext>().SaveChangesAsync();

        lock (cacheUsers)
        {
            UserInfo? userInfo2 = null;
            if(oaid != null) userInfo2 = cacheUsers.FirstOrDefault(u => u.AID == oaid);
            var ds = userInfo2?.Devices.FirstOrDefault(d => d.DUID == did);
            userInfo2?.Devices.Remove(ds);
            if (ds == null)
            {
                ds = new DeviceStatus
                {
                    DUID = did,
                    DeviceName = name,
                    Model = (byte)model,
                    LastActive = DateTime.Now
                };

                ds.Update();
            }

            userInfo.Devices.Add(ds);
        }

        return Results.Json(new { status = RC.SUCCESS });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
});
#endregion

#region Device Unbind
app.MapPost("/device/unregister", async (HttpContext http, UnregisterDeviceRequest request) =>
{
    try
    {
        var addr = http.Connection.RemoteIpAddress.ToString();
        if (!AllowAccess(addr))
        {
            return Results.Json(new { status = RC.IP_RESTRICT });
        }
#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/device/unregister", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif

        // Validate input
        if (string.IsNullOrEmpty(request.Token)
            || string.IsNullOrEmpty(request.Uid)
            || string.IsNullOrEmpty(request.Did))
        {
            return Results.Json(new { status = RC.INVALID_PARAMETERS });
        }

        var token = Util.DecodeUserToken(request.Token);

        // check if user exists in cache
        UserInfo? userInfo;
        lock (cacheUsers)
        {
            userInfo = cacheUsers.FirstOrDefault(u => string.Equals(u.AID, request.Uid, StringComparison.InvariantCultureIgnoreCase));
        }

        if (userInfo?.Token != token)
        {
            return Results.Json(new { status = RC.TOKEN_EXPIRED });
        }

        var data = Util.Hex2Bytes(request.Did);
        var key = Util.Hex2Bytes(request.Uid);
        Util.GetDataByKey3(data, key);
        var did = Util.Bytes2Hex(data);

        // Add new device
        var device = await http.RequestServices.GetRequiredService<DataContext>().Devices.FindAsync(did);
        if (device == null)
        {
            return Results.Json(new { status = RC.DEVICE_NOT_EXISTS });
        }

        http.RequestServices.GetRequiredService<DataContext>().Devices.Remove(device);
        await http.RequestServices.GetRequiredService<DataContext>().SaveChangesAsync();

        lock (cacheUsers)
        {
            userInfo.Devices.RemoveAll(d => d.DUID == did);
        }

        return Results.Json(new { status = RC.SUCCESS });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
});
#endregion

#region Device Rename
app.MapPost("/device/rename", async (HttpContext http, RenameDeviceRequest request) =>
{
    try
    {
        var addr = http.Connection.RemoteIpAddress.ToString();
        if (!AllowAccess(addr))
        {
            return Results.Json(new { status = RC.IP_RESTRICT });
        }
#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/device/rename", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif

        // Validate input
        if (string.IsNullOrEmpty(request.Token)
            || string.IsNullOrEmpty(request.Uid)
            || string.IsNullOrEmpty(request.Did))
        {
            return Results.Json(new { status = RC.INVALID_PARAMETERS});
        }

        var token = Util.DecodeUserToken(request.Token);

        // check if user exists in cache
        UserInfo? userInfo;
        lock (cacheUsers)
        {
            userInfo = cacheUsers.FirstOrDefault(u => string.Equals(u.AID, request.Uid, StringComparison.InvariantCultureIgnoreCase));
        }

        if (userInfo?.Token != token)
        {
            return Results.Json(new { status = RC.TOKEN_EXPIRED });
        }

        var data = Util.Hex2Bytes(request.Did);
        var key = Util.Hex2Bytes(request.Uid);
        Util.GetDataByKey3(data, key);
        var did = Util.Bytes2Hex(data);

        // Add new device
        var device = await http.RequestServices.GetRequiredService<DataContext>().Devices.FindAsync(did);
        if (device == null)
        {
            return Results.Json(new { status = RC.DEVICE_NOT_EXISTS });
        }

        device.Name = request.Title;

        http.RequestServices.GetRequiredService<DataContext>().Devices.Update(device);
        await http.RequestServices.GetRequiredService<DataContext>().SaveChangesAsync();
        lock (cacheUsers)
        {
            var ds = userInfo.Devices.FirstOrDefault(o => o.DUID == did);
            if (ds != null)
            {
                ds.DeviceName = request.Title;
                ds.Update();
            }
        }

        return Results.Json(new { status = RC.SUCCESS });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
});
#endregion

#region Device Command
// The client sends connection request to the device.
app.MapPost("/device/command", async (HttpContext http, CommandDeviceRequest request) =>
{
    try
    {
        var addr = http.Connection.RemoteIpAddress.ToString();
        if (!AllowAccess(addr))
        {
            return Results.Json(new { status = RC.IP_RESTRICT });
        }
#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/device/command", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif

        // Validate input
        if (string.IsNullOrEmpty(request.Token)
            || string.IsNullOrEmpty(request.Uid)
            || string.IsNullOrEmpty(request.Did))
        {
            return Results.Json(new { status = RC.INVALID_PARAMETERS});
        }

        var token = Util.DecodeUserToken(request.Token);

        // check if user exists in cache
        UserInfo? userInfo;
        lock (cacheUsers)
        {
            userInfo = cacheUsers.FirstOrDefault(u => string.Equals(u.AID, request.Uid, StringComparison.InvariantCultureIgnoreCase));
        }

        if (userInfo?.Token != token)
        {
            Console.WriteLine($"{DateTime.Now}: NAT: mismatch token!");
            return Results.Json(new { status = RC.TOKEN_EXPIRED });
        }

        var data = Util.Hex2Bytes(request.Did);
        var key = Util.Hex2Bytes(request.Uid);
        Util.GetDataByKey3(data, key);
        var did = Util.Bytes2Hex(data);
        byte[] nat = null;
        byte[] wol = null;

        if (null != request.NAT)
        {
            nat = Util.Hex2Bytes(request.NAT); // requested addr
            Util.GetDataByKey3(nat, key);
        }

        if (null != request.WOL)
        {
            wol = Util.Hex2Bytes(request.WOL); // requested addr
            Util.GetDataByKey3(wol, key);
        }

        lock (cacheUsers)
        {
            var dev = userInfo.Devices.FirstOrDefault(d => d.DUID == did);
            if (dev != null)
            {
                if (nat != null)
                {
                    dev.RequestConnectionInfo = nat;
                    Console.WriteLine($"{DateTime.Now}: NAT: Requested!");
                }

                if (wol != null)
                {
                    dev.RequestWOLInfo = wol;
                    Console.WriteLine($"{DateTime.Now}: WOL: Requested!");
                }
            }
            else 
            {
                Console.WriteLine($"{DateTime.Now}: Command: {did} is not found!");
            }
        }

        return Results.Json(new { status = RC.SUCCESS });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
});
#endregion

#region Device Accept Connection
// The device acknowledge the connection request from the client.
app.MapPost("/device/accept_connection", async (HttpContext http, AcceptConnectionRequest request) =>
{
    try
    {
        //var addr = http.Connection.RemoteIpAddress.ToString();
        //if (!AllowAccess(addr))
        //{
        //    return Results.Json(new { status = RC.IP_RESTRICT });
        //}
#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/device/accept_connection", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif

        // Validate input
        if (string.IsNullOrEmpty(request.Token)
            || string.IsNullOrEmpty(request.Uid)
            || string.IsNullOrEmpty(request.Did))
        {
            return Results.Json(new { status = RC.INVALID_PARAMETERS });
        }

        var token = Util.DecodeUserToken(request.Token);

        // check if user exists in cache
        UserInfo? userInfo;
        lock (cacheUsers)
        {
            userInfo = cacheUsers.FirstOrDefault(u => string.Equals(u.AID, request.Uid, StringComparison.InvariantCultureIgnoreCase));
        }

        if (userInfo?.Token != token)
        {
            return Results.Json(new { status = RC.TOKEN_EXPIRED });
        }

        var data = Util.Hex2Bytes(request.Did);
        var key = Util.Hex2Bytes(request.Uid);
        Util.GetDataByKey3(data, key);
        var did = Util.Bytes2Hex(data);
        
        lock (cacheUsers)
        {
            var dev = userInfo.Devices.FirstOrDefault(d => d.DUID == did);
            if (dev != null) dev.RequestConnectionInfo = null;
        }

        return Results.Json(new { status = RC.SUCCESS });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
});
#endregion

#region Account Deactivate Request
app.MapPost("/account/deactivate_request", async (HttpContext http, AccountDeactivateRequest request) =>
{
    try
    {
        if (!AllowAccess(http.Connection.RemoteIpAddress.ToString(), 3))
        {
            return Results.Json(new { status = RC.IP_RESTRICT });
        }
#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/account/deactivate_request", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif

        var response_data = new { status = RC.SUCCESS };

        // Validate input
        if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Uid))
        {
            response_data = new { status = RC.INVALID_PARAMETERS };
            return Results.Json(response_data);
        }

        var token = Util.DecodeUserToken(request.Token);

        // check if user exists in cache
        UserInfo? userInfo;
        lock (cacheUsers)
        {
            userInfo = cacheUsers.FirstOrDefault(u => string.Equals(u.AID, request.Uid));
        }

        if (userInfo?.Token != token)
        {
            response_data = new { status = RC.TOKEN_MISMATCH };
            return Results.Json(response_data);
        }

        var user = await http.RequestServices.GetRequiredService<DataContext>().Users.FirstOrDefaultAsync(u => u.UID == userInfo.AID);

        if (user == null)
        {
            response_data = new { status = RC.ACCOUNT_NOT_EXISTS };
            return Results.Json(response_data);
        }

        GenerateDeactivationCodeIfNecessary(userInfo, user.Email);
        return Results.Json(new { status = RC.SUCCESS});
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
});
#endregion

#region Account Deactivate Confirm
app.MapPost("/account/deactivate_confirm", async (HttpContext http, AccountDeactivateConfirmRequest request) =>
{
    try
    {
        if (!AllowAccess(http.Connection.RemoteIpAddress.ToString(), 3))
        {
            return Results.Json(new { status = RC.IP_RESTRICT });
        }
#if MIRROR
        var httpClient = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{MIRROR_URL}/account/deactivate_confirm", content);
        var responseString = await response.Content.ReadAsStringAsync();

        return Results.Json(JsonSerializer.Deserialize<object>(responseString));
#endif

        var response_data = new { status = RC.SUCCESS };

        // Validate input
        if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Uid))
        {
            response_data = new { status = RC.INVALID_PARAMETERS };
            return Results.Json(response_data);
        }

        var token = Util.DecodeUserToken(request.Token);

        // check if user exists in cache
        UserInfo? userInfo;
        lock (cacheUsers)
        {
            userInfo = cacheUsers.FirstOrDefault(u => string.Equals(u.AID, request.Uid));
        }

        if (userInfo?.Token != token)
        {
            response_data = new { status = RC.TOKEN_MISMATCH };
            return Results.Json(response_data);
        }

        if (userInfo?.DeactivationVerificationCode != request.Code)
        {
            response_data = new { status = RC.VERIFICATION_CODE_MISMATCH };
            return Results.Json(response_data);
        }

        var user = await http.RequestServices.GetRequiredService<DataContext>().Users.FirstOrDefaultAsync(u => u.UID == userInfo.AID);

        if (user == null)
        {
            response_data = new { status = RC.ACCOUNT_NOT_EXISTS };
            return Results.Json(response_data);
        }

        if (DateTime.Now.Subtract(userInfo.DeactivationVerificationTime).TotalMinutes > 10)
        {
            return Results.Json(new { status = RC.VERIFICATION_CODE_EXPIRED });
        }

        var deletedUser = new DeletedUser();
        deletedUser.Id = user.Id;
        deletedUser.UID = user.UID;
        deletedUser.Email = user.Email;
        deletedUser.Name = user.Name;
        deletedUser.Password = user.Password;
        deletedUser.Created = user.CreatedAt.Ticks;
        deletedUser.DeletedAt = DateTime.Now.Ticks;

        await http.RequestServices.GetRequiredService<DataContext>().DeletedUsers.AddAsync(deletedUser);

        // delete user
        http.RequestServices.GetRequiredService<DataContext>().Users.Remove(user);

        EmailHelper.SendDeactivationConfirmationEmail(user.Email);
        await http.RequestServices.GetRequiredService<DataContext>().SaveChangesAsync();

        lock (cacheUsers)
        {
            cacheUsers.Remove(userInfo);
        }

        return Results.Json(new { status = RC.SUCCESS });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.Json(new { status = RC.EXCEPTION });
    }
});
#endregion

#region Web app start
app.Run();
MailSender.DefaultSender.Stop();
#endregion

#region Records
record RegisterUserReuqest(string Email, string Password);
record LoginWithTokenRequest(string Uid, string Token);
record HeartbeatRequest(int? v, string Payload);

record RegisterDeviceRequest(string Uid, string Token, string Payload);
record UnregisterDeviceRequest(string Uid, string Token, string Did);
record RenameDeviceRequest(string Uid, string Token, string Did, string Title);

record CommandDeviceRequest(string Uid, string Token, string Did, string? NAT, string? WOL);
record AcceptConnectionRequest (string Uid, string Token, string Did);

record SendEmailRequest(string Email);
record ActivationVerificationReguest(string Token, string VerificationCode);
record ResetPasswordVerificationRequest(string Token, string VerificationCode, string NewPassword);

record AccountDeactivateRequest(string Uid, string Token);
record AccountDeactivateConfirmRequest(string Uid, string Token, string Code);
#endregion
