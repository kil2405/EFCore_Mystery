using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MysteryBox.Api.Data;
using MysteryBox.Api.Services;
using MysteryBox.Api.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = null;
});

string? conn =
    builder.Configuration["EFCORE_CONNECTION"] // ① dotnet-ef에서 넘길 용도 (key 단독)
    ?? builder.Configuration.GetConnectionString("DefaultConnection") // ② 보편 키
    ?? builder.Configuration.GetConnectionString("Default")           // ③ 백업 키
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"]   // ④ 평문 키
    ?? builder.Configuration["ConnectionStrings:Default"];            // ⑤ 백업 키

if (string.IsNullOrWhiteSpace(conn))
{
    throw new InvalidOperationException(
        "No connection string found. Set EFCORE_CONNECTION or ConnectionStrings:DefaultConnection.");
}

// 2) DbContext 등록 (Pomelo MySQL)
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseMySql(conn, ServerVersion.AutoDetect(conn),
        o => o.EnableRetryOnFailure());
});

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MysteryBox API (EF Core)", Version = "v1" });

    // Bearer (표준)
    var bearer = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Use: Bearer {token}"
    };
    c.AddSecurityDefinition("Bearer", bearer);

    // X-Auth-Token (Bearer 없이 토큰만 전달하고 싶을 때)
    //var xauth = new OpenApiSecurityScheme
    //{
    //  Name = "X-Auth-Token",
    //  Type = SecuritySchemeType.ApiKey,
    //  In = ParameterLocation.Header,
    //  Description = "Raw JWT without 'Bearer'"
    //};
    //c.AddSecurityDefinition("XAuthToken", xauth);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { bearer, Array.Empty<string>() }
    });
});

// App services
builder.Services.AddSingleton<RedisService>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<ResourceLoader>();
builder.Services.AddSingleton<GeoIpService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// JWT settings
var jwtKey = builder.Configuration["Jwt:Key"] ?? "devonly-change-me-super-secret";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MysteryBox";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "MysteryBoxAudience";
var enforceSingleSession =
    builder.Configuration.GetValue<bool>("Auth:EnforceSingleSession", true);

// 권장: 인바운드 클레임 매핑 해제(원본 'sub' 등 보존)
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.IncludeErrorDetails = true; // 개발 중 원인 파악 도움

        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = JwtKeyProvider.GetKey(jwtKey),
            ClockSkew = TimeSpan.Zero
        };

        o.Events = new JwtBearerEvents
        {
            // Bearer 없이도 동작하도록 토큰 추출 커스터마이즈
            OnMessageReceived = ctx =>
            {
                // 1) Authorization: Bearer <token>
                var auth = ctx.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrWhiteSpace(auth))
                {
                    const string bearerPrefix = "Bearer ";
                    if (auth.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Token = auth.Substring(bearerPrefix.Length).Trim();
                        return Task.CompletedTask;
                    }
                    // 2) Authorization: <token> (공백/스킴 없이)
                    if (auth.IndexOf(' ') == -1)
                    {
                        ctx.Token = auth.Trim();
                        return Task.CompletedTask;
                    }
                }

                // 3) 커스텀 헤더
                //var raw = ctx.Request.Headers["X-Auth-Token"].ToString();
                //if (!string.IsNullOrWhiteSpace(raw))
                //{
                //    ctx.Token = raw.Trim();
                //    return Task.CompletedTask;
                //}

                // 4) (선택) 쿼리스트링: ?access_token=...  — 필요할 때만 사용하세요.
                //var qs = ctx.Request.Query["access_token"].ToString();
                //if (!string.IsNullOrWhiteSpace(qs))
                //{
                //    ctx.Token = qs.Trim();
                //    return Task.CompletedTask;
                //}

                return Task.CompletedTask;
            },

            // 가장 최근 로그인 토큰(JTI)만 허용
            OnTokenValidated = async ctx =>
            {
                var sub = ctx.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                        ?? ctx.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var jti = ctx.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(jti))
                {
                    ctx.Fail("invalid_token");
                    return;
                }

                if (!enforceSingleSession) return;

                try
                {
                    var redis = ctx.HttpContext.RequestServices.GetRequiredService<RedisService>();
                    var current = await redis.GetStringAsync($"auth:current:{sub}");
                    if (!string.Equals(current, jti, StringComparison.Ordinal))
                    {
                      ctx.Fail("revoked_or_outdated_token");
                    }
                }
                catch (Exception ex)
                {
                    ctx.Fail("token_state_check_failed: " + ex.Message);
                }
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
