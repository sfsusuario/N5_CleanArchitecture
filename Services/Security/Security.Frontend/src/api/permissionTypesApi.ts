import axios from "axios";
import type { CreatePermissionTypePayload, PermissionType } from "../types/permission";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5080";

const httpClient = axios.create({
  baseURL: `${API_BASE_URL}/api/PermissionTypes`,
});

export const permissionTypesApi = {
  getPermissionTypes: async (): Promise<PermissionType[]> => {
    const response = await httpClient.get<PermissionType[]>("");
    return response.data;
  },

  createPermissionType: async (payload: CreatePermissionTypePayload): Promise<PermissionType> => {
    const response = await httpClient.post<PermissionType>("", payload);
    return response.data;
  },
};
