namespace OpsFlow.Domain.Authorization;

public static class SystemRoles
{
    public static readonly Guid OwnerId =
        new("11111111-1111-1111-1111-111111111111");

    public static readonly Guid AdminId =
        new("22222222-2222-2222-2222-222222222222");

    public static readonly Guid ProjectManagerId =
        new("33333333-3333-3333-3333-333333333333");

    public static readonly Guid TeamLeadId =
        new("44444444-4444-4444-4444-444444444444");

    public static readonly Guid MemberId =
        new("55555555-5555-5555-5555-555555555555");

    public static readonly Guid ViewerId =
        new("66666666-6666-6666-6666-666666666666");
}
