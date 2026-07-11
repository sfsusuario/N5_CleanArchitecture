import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type {
  ModifyPermissionPayload,
  Permission,
  RequestPermissionPayload,
} from "../../types/permission";

export interface PermissionsState {
  items: Permission[];
  loading: boolean;
  submitting: boolean;
  error: string | null;
}

const initialState: PermissionsState = {
  items: [],
  loading: false,
  submitting: false,
  error: null,
};

const permissionsSlice = createSlice({
  name: "permissions",
  initialState,
  reducers: {
    // Dispatched by components; handled by permissionsSaga, which calls the API and
    // dispatches the matching *Succeeded/*Failed action below.
    fetchPermissionsRequested(state) {
      state.loading = true;
      state.error = null;
    },
    fetchPermissionsSucceeded(state, action: PayloadAction<Permission[]>) {
      state.loading = false;
      state.items = action.payload;
    },
    fetchPermissionsFailed(state, action: PayloadAction<string>) {
      state.loading = false;
      state.error = action.payload;
    },

    requestPermissionRequested(state, _action: PayloadAction<RequestPermissionPayload>) {
      state.submitting = true;
      state.error = null;
    },
    requestPermissionSucceeded(state) {
      state.submitting = false;
    },
    requestPermissionFailed(state, action: PayloadAction<string>) {
      state.submitting = false;
      state.error = action.payload;
    },

    modifyPermissionRequested(state, _action: PayloadAction<ModifyPermissionPayload>) {
      state.submitting = true;
      state.error = null;
    },
    modifyPermissionSucceeded(state) {
      state.submitting = false;
    },
    modifyPermissionFailed(state, action: PayloadAction<string>) {
      state.submitting = false;
      state.error = action.payload;
    },
  },
});

export const {
  fetchPermissionsRequested,
  fetchPermissionsSucceeded,
  fetchPermissionsFailed,
  requestPermissionRequested,
  requestPermissionSucceeded,
  requestPermissionFailed,
  modifyPermissionRequested,
  modifyPermissionSucceeded,
  modifyPermissionFailed,
} = permissionsSlice.actions;

export default permissionsSlice.reducer;
