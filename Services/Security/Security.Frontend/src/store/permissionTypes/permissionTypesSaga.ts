import { call, put, takeEvery, all, fork } from "redux-saga/effects";
import type { PayloadAction } from "@reduxjs/toolkit";
import { permissionTypesApi } from "../../api/permissionTypesApi";
import type { CreatePermissionTypePayload, PermissionType } from "../../types/permission";
import {
  fetchPermissionTypesRequested,
  fetchPermissionTypesSucceeded,
  fetchPermissionTypesFailed,
  createPermissionTypeRequested,
  createPermissionTypeSucceeded,
  createPermissionTypeFailed,
} from "./permissionTypesSlice";

function errorMessage(error: unknown): string {
  if (error && typeof error === "object" && "message" in error) {
    return String((error as { message: unknown }).message);
  }
  return "Unexpected error";
}

export function* fetchPermissionTypesSaga() {
  try {
    const items: PermissionType[] = yield call(permissionTypesApi.getPermissionTypes);
    yield put(fetchPermissionTypesSucceeded(items));
  } catch (error) {
    yield put(fetchPermissionTypesFailed(errorMessage(error)));
  }
}

export function* createPermissionTypeSaga(action: PayloadAction<CreatePermissionTypePayload>) {
  try {
    yield call(permissionTypesApi.createPermissionType, action.payload);
    yield put(createPermissionTypeSucceeded());
    // Refresh the list so the new type shows up in the dropdown right away.
    yield put(fetchPermissionTypesRequested());
  } catch (error) {
    yield put(createPermissionTypeFailed(errorMessage(error)));
  }
}

function* watchFetchPermissionTypes() {
  yield takeEvery(fetchPermissionTypesRequested.type, fetchPermissionTypesSaga);
}

function* watchCreatePermissionType() {
  yield takeEvery(createPermissionTypeRequested.type, createPermissionTypeSaga);
}

export function* permissionTypesRootSaga() {
  yield all([fork(watchFetchPermissionTypes), fork(watchCreatePermissionType)]);
}
