// Mirrors Security.Domain's entities/DTOs. ASP.NET Core serializes to camelCase by default.

export interface PermissionType {
  id: number;
  description: string | null;
}

// Returned by GET /api/Permissions/GetPermissions (List<Permissions>)
export interface Permission {
  id: number;
  employeeForename: string | null;
  employeeSurname: string | null;
  permissionType: number;
  permissionDate: string; // ISO date string
  permissionTypeRef: PermissionType | null;
}

// Returned by POST RequestPermission / ModifyPermission (PermissionResponse)
export interface PermissionResponse {
  id: number;
  employeeForename: string | null;
  employeeSurname: string | null;
  permissionType: number;
  permissionDate: string;
  permissionsType: PermissionType | null;
}

// Body for POST /api/Permissions/RequestPermission (RequestPermissionCommand)
export interface RequestPermissionPayload {
  employeeForename: string;
  employeeSurname: string;
  permissionType: number;
  permissionDate: string;
}

// Body for POST /api/Permissions/ModifyPermission/{id} (ModifyPermissionCommand)
export interface ModifyPermissionPayload extends RequestPermissionPayload {
  id: number;
}
