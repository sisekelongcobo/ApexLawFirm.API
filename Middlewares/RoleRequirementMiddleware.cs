using System.Security.Claims;

public class RoleRequirementMiddleware {
  private readonly RequestDelegate _next;
  private readonly string _requiredRole;

  public RoleRequirementMiddleware(RequestDelegate next, string requiredRole) {
    _next = next;
    _requiredRole = requiredRole;
  }

  public async Task InvokeAsync(HttpContext context){
    var user = context.User;

    if(!user.Identity?.IsAuthenticated ?? true || !user.IsInRole(_requiredRole)){
      context.Response.StatusCode = StatusCodes.Status403Forbidden;
      await context.Response.WriteAsync("Access denied. Admins only.");
      return;
    }

    await _next(context);
  }
}
