import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { PermissionType } from "../../types/permission";

export interface PermissionTypesState {
  items: PermissionType[];
  loading: boolean;
  error: string | null;
}

const initialState: PermissionTypesState = {
  items: [],
  loading: false,
  error: null,
};

const permissionTypesSlice = createSlice({
  name: "permissionTypes",
  initialState,
  reducers: {
    fetchPermissionTypesRequested(state) {
      state.loading = true;
      state.error = null;
    },
    fetchPermissionTypesSucceeded(state, action: PayloadAction<PermissionType[]>) {
      state.loading = false;
      state.items = action.payload;
    },
    fetchPermissionTypesFailed(state, action: PayloadAction<string>) {
      state.loading = false;
      state.error = action.payload;
    },
  },
});

export const {
  fetchPermissionTypesRequested,
  fetchPermissionTypesSucceeded,
  fetchPermissionTypesFailed,
} = permissionTypesSlice.actions;

export default permissionTypesSlice.reducer;
