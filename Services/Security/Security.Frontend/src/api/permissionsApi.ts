import axios from "axios";
import type {
  ModifyPermissionPayload,
  Permission,
  PermissionResponse,
  RequestPermissionPayload,
} from "../types/permission";

// Baked in at build time by Vite (see .env.example / Dockerfile ARG VITE_API_BASE_URL).
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";

const httpClient = axios.create({
  baseURL: `${API_BASE_URL}/api/Permissions`,
});

export const permissionsApi = {
  getPermissions: async (): Promise<Permission[]> => {
    const response = await httpClient.get<Permission[]>("/GetPermissions");
    return response.data;
  },

  requestPermission: async (payload: RequestPermissionPayload): Promise<PermissionResponse> => {
    const response = await httpClient.post<PermissionResponse>("/RequestPermission", payload);
    return response.data;
  },

  modifyPermission: async (payload: ModifyPermissionPayload): Promise<PermissionResponse> => {
    const response = await httpClient.post<PermissionResponse>(
      `/ModifyPermission/${payload.id}`,
      payload
    );
    return response.data;
  },
};
