using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "Permission and RolePermission are intentional domain concepts in the RBAC model.",
    Scope = "type",
    Target = "~T:OpsFlow.Domain.Authorization.Permission")]

[assembly: SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "RolePermission is an intentional domain concept representing the Role-Permission relationship.",
    Scope = "type",
    Target = "~T:OpsFlow.Domain.Authorization.RolePermission")]
