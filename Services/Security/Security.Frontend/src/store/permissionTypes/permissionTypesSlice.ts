import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { CreatePermissionTypePayload, PermissionType } from "../../types/permission";

export interface PermissionTypesState {
  items: PermissionType[];
  loading: boolean;
  submitting: boolean;
  error: string | null;
}

const initialState: PermissionTypesState = {
  items: [],
  loading: false,
  submitting: false,
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

    createPermissionTypeRequested(state, _action: PayloadAction<CreatePermissionTypePayload>) {
      state.submitting = true;
      state.error = null;
    },
    createPermissionTypeSucceeded(state) {
      state.submitting = false;
    },
    createPermissionTypeFailed(state, action: PayloadAction<string>) {
      state.submitting = false;
      state.error = action.payload;
    },
  },
});

export const {
  fetchPermissionTypesRequested,
  fetchPermissionTypesSucceeded,
  fetchPermissionTypesFailed,
  createPermissionTypeRequested,
  createPermissionTypeSucceeded,
  createPermissionTypeFailed,
} = permissionTypesSlice.actions;

export default permissionTypesSlice.reducer;
